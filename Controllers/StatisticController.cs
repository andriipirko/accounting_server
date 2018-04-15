using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Models.Statistic;

namespace WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Statistic")]
    public class StatisticController : Controller
    {
        [HttpGet("/api/Statistic/Accounting/General", Name = "GetGeneralAccountingStatistic", Order = 1)]
        public List<GeneralStatisticDto> GetGeneralAccountingStatistic(int storageId, string firstDate, string lastDate)
        {
            DbContext _context = HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;
            return _context.GetGeneralAccountingStatistic(storageId, firstDate,
                DateTime.ParseExact(lastDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture));
        }

        [HttpGet("/api/Statistic/Accounting/Comparision", Name = "GetComparisionAccountingStatistic", Order = 1)]
        public List<ItemComparisionDto> GetComparisionAccountingStatistic(int storageId, int productId, string firstDate, string lastDate)
        {
            DbContext _context = HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;
            return _context.GetStatisticAccountingComparisionItem(DateTime.ParseExact(firstDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
                                                        DateTime.ParseExact(lastDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
                                                        storageId,
                                                        productId);
        }

        [HttpGet("/api/Statistic/Orders/General", Name = "GetGeneralOrdersStatistic", Order = 1)]
        public List<GeneralStatisticDto> GetGeneralOrdersStatistic(int storageId, string firstDate, string lastDate)
        {
            DbContext _context = HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;
            return _context.GetGeneralOrdersStatistic(storageId, firstDate, 
                DateTime.ParseExact(lastDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture));
        }

        [HttpGet("/api/Statistic/Orders/Comparision", Name = "GetComparisionOrdersStatistic", Order = 1)]
        public List<ItemComparisionDto> GetComparisionOrdersStatistic(int storageId, int productId, string firstDate, string lastDate)
        {
            DbContext _context = HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;
            return _context.GetStatisticOrdersComparisionItem(DateTime.ParseExact(firstDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
                                                        DateTime.ParseExact(lastDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
                                                        storageId,
                                                        productId);
        }
    }
}