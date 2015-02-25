using CriteriaFilterService.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CriteriaFilterService
{
    public class FilterController : IFilterController
    {
        IDatabase _database;
        IServer _server;

        FilterController(IDatabase database, IServer server)
        {
            _database = database;
            _server = server;
        }

        public dynamic GetFilter(FilterRequest filter)
        {

            if (filter.CampaignId != null)
            {
                //foreach id get data and filter
                //results = _database.StringGet(filter.CampaignId);
            }
            //else get EVERYTHING

            throw new NotImplementedException();
        }




    }
}
