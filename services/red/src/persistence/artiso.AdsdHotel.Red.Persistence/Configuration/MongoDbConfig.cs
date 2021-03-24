namespace artiso.AdsdHotel.Red.Persistence.Configuration
{
    public class MongoDbConfig
    {
        public string? Host { get; set; }
        public string? User { get; set; }
        public int Port { get; set; }
        public string? Password { get; set; }
        public string? Database { get; set; }
        public string? Scheme { get; set; }
    }
}
