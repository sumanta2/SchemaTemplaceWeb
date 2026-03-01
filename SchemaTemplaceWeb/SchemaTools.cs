using ModelContextProtocol.Server;
using System.ComponentModel;
using SchemaTemplateLib.Interfaces;
using System.Text.Json;

namespace SchemaTemplaceWeb;

[McpServerToolType]
public class SchemaTools
{
    private readonly IExposeMethods _exposeMethods;

    public SchemaTools(IExposeMethods exposeMethods)
    {
        _exposeMethods = exposeMethods;
    }

    [McpServerTool]
    [Description("Search for stored procedures by name or substring")]
    public string SearchProcedures(
        [Description("The search term to filter procedure names")] string term)
    {
        var procedures = _exposeMethods.SearchProcedures(term ?? string.Empty);
        return JsonSerializer.Serialize(procedures);
    }

    [McpServerTool]
    [Description("Get the list of parameters required for a specific stored procedure")]
    public string GetProcedureParams(
        [Description("The exact name of the stored procedure")] string procedureName)
    {
        var parameters = _exposeMethods.GetProcedureParams(procedureName ?? string.Empty);
        return JsonSerializer.Serialize(parameters);
    }

    [McpServerTool]
    [Description("Execute a stored procedure, save the result as CSV to the outputs folder, and return as base64 string")]
    public async Task<string> ExecuteProcedure(
        [Description("The exact name of the stored procedure")] string procedureName,
        [Description("Key-value pairs of parameters")] Dictionary<string, string> parameters)
    {
        var (stream, fileName) = _exposeMethods.GenerateExcelTemplate(procedureName ?? string.Empty, parameters ?? new Dictionary<string, string>());
        
        if (stream != null)
        {
            var outputsFolder = Path.Combine(Directory.GetCurrentDirectory(), "outputs");
            Directory.CreateDirectory(outputsFolder);

            // Change the file extension to .csv
            var csvFileName = System.IO.Path.GetFileNameWithoutExtension(fileName) + ".csv";
            var filePath = System.IO.Path.Combine(outputsFolder, csvFileName);

            using var workbook = new ClosedXML.Excel.XLWorkbook(stream);
            var worksheet = workbook.Worksheets.First();
            var lastRowUsed = worksheet.LastRowUsed();

            if (lastRowUsed == null)
            {
                return JsonSerializer.Serialize(new { message = "No data found or execution failed." });
            }

            int lastRow = lastRowUsed.RowNumber();
            int lastColumn = worksheet.LastColumnUsed().ColumnNumber();

            var csvBuilder = new System.Text.StringBuilder();

            // Row 1 is group header in Excel, Row 2 is column header, Row 3+ is data.
            // We read from row 2 onwards to create the CSV.
            for (int r = 2; r <= lastRow; r++)
            {
                var rowValues = new List<string>();
                for (int c = 1; c <= lastColumn; c++)
                {
                    var cellValue = worksheet.Cell(r, c).GetValue<string>() ?? string.Empty;
                    // Escape quotes and enclose in double quotes
                    rowValues.Add($"\"{cellValue.Replace("\"", "\"\"")}\"");
                }
                csvBuilder.AppendLine(string.Join(",", rowValues));
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csvBuilder.ToString());
            await System.IO.File.WriteAllBytesAsync(filePath, bytes);

            string base64 = Convert.ToBase64String(bytes);
            return JsonSerializer.Serialize(new { 
                fileName = csvFileName, 
                filePath,
                fileContent = base64, 
                message = "CSV file generated and saved successfully to outputs folder" 
            });
        }
        else
        {
            return JsonSerializer.Serialize(new { message = "No data found or execution failed." });
        }
    }
}
