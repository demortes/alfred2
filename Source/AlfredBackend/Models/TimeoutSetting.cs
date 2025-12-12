namespace AlfredBackend.Models
{
    public class TimeoutSetting
    {
        public int Id { get; set; } // For EF Core owned entities
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ValueSeconds { get; set; }
    }
}
