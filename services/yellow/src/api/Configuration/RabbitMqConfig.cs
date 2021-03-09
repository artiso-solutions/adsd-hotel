namespace artiso.AdsdHotel.Yellow.Api.Configuration
{
    internal class RabbitMqConfig
    {
        public string Host { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        public override string ToString()
        {
            return $"host={Host};username={User};password={Password}";
        }
    }
}