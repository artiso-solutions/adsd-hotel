namespace artiso.AdsdHotel.Red.Api.Configuration
{
    internal record RabbitMqConfig(string Host, string User, string Password)
    {
        public override string ToString()
        {
            return $"host={Host};username={User};password={Password}";
        }
    }
}