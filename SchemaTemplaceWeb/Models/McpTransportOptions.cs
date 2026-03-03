namespace SchemaTemplaceWeb.Models
{
    public class McpTransportOptions
    {
        public bool EnableStdio { get; set; }
        public bool EnableHttp { get; set; }
        public bool Stateless { get; set; }
        public string Path { get; set; } = "/mcp";
    }
}
