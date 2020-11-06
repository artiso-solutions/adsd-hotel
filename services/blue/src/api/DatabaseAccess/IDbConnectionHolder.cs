using System;
using System.Data;

namespace artiso.AdsdHotel.Blue.Api
{
    internal interface IDbConnectionHolder : IDbConnection, IAsyncDisposable
    {
        bool HasTransaction { get; }
    }
}
