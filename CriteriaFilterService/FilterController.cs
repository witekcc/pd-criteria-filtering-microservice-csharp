using CriteriaFilterService.Models;
using Newtonsoft.Json;
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

        public FilterController(IDatabase database)
        {
            _database = database;
            //_server = server;
        }

        public List<string> GetFilter(FilterRequest filter)
        {
            List<string> campaignsMatched = new List<string>();

            if (filter.CampaignIds != null)
            {
                foreach (var campaign in filter.CampaignIds)
                {
                    string json = _database.StringGet(campaign);
                    if (json != null)
                    {
                        var criteria = JsonConvert.DeserializeObject<Criteria>(json);

                        if(CriteriaHelper.MeetsCriteria(filter.User, criteria))
                        {
                            campaignsMatched.Add(campaign);
                        }
                    }
                }
            }

            return campaignsMatched;
        }




    }
}
