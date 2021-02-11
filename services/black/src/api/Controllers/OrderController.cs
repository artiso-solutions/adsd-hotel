using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using artiso.AdsdHotel.Black.Api.DatabaseModel;
using artiso.AdsdHotel.Black.Commands;
using artiso.AdsdHotel.Infrastructure.DataStorage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace artiso.AdsdHotel.Black.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IDataStoreClient _dataStoreClient;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IDataStoreClient dataStoreClient, ILogger<OrderController> logger)
        {
            _dataStoreClient = dataStoreClient;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<OrderIdRespone>> Get(string? firstName, string? lastName, string? eMail)
        {
            List<Expression<Func<GuestInformationRecord, bool>>> filters = new();
            if (!string.IsNullOrEmpty(firstName))
            {
                Expression<Func<GuestInformationRecord, bool>> fEx = r => r.GuestInformation.FirstName.ToLowerInvariant().Contains(firstName);
                filters.Add(fEx);
            }
            if (!string.IsNullOrEmpty(lastName))
            {
                Expression<Func<GuestInformationRecord, bool>> fEx = r => r.GuestInformation.LastName.ToLowerInvariant().Contains(lastName);
                filters.Add(fEx);
            }
            if (!string.IsNullOrEmpty(eMail))
            {
                Expression<Func<GuestInformationRecord, bool>> fEx = r => r.GuestInformation.EMail.ToLowerInvariant().Contains(eMail);
                filters.Add(fEx);
            }
            
            var result = await _dataStoreClient.GetAllAsync(ExpressionCombinationOperator.And, filters.ToArray());
            var ids = result.Select(r => r.OrderId).ToList();
            return new OrderIdRespone(ids);
        }
    }
}
