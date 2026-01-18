using SchemaTemplateLib.DataModel;

namespace SchemaTemplaceWeb.Models
{
    public class ProcedureInputViewModel
    {
        public string ProcedureName { get; set; }
        public List<ProcedureParam> Parameters { get; set; } 
    }
}
