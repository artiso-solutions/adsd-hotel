using System.Threading.Tasks;

namespace artiso.AdsdHotel.Blue.Api
{
    internal interface IDbConnectionFactory
    {
        Task<IDbConnectionHolder> CreateAsync();
    }
}
