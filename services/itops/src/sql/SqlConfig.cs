namespace artiso.AdsdHotel.ITOps.Sql
{
    public record SqlConfig(
        string Host,
        int Port,
        string Database,
        string Username,
        string? Password);
}
