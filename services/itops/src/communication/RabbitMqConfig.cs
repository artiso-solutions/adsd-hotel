using System.Text;

namespace artiso.AdsdHotel.ITOps.Communication
{
    #nullable disable

    public class RabbitMqConfig
    {
        public string Host { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        public string AsConnectionString()
        {
            var sb = new StringBuilder();
            sb.Append($"host={Host}");

            if (!string.IsNullOrWhiteSpace(User))
                sb.Append($";username={User}");

            if (!string.IsNullOrWhiteSpace(Password))
                sb.Append($";password={Password}");

            var cs = sb.ToString();
            return cs;
        }

        public override string ToString() => AsConnectionString();
    }
}