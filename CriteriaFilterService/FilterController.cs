using CriteriaFilterService.Models;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections;
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

        public FilterController(ConnectionMultiplexer connection)
        {
            _database = connection.GetDatabase();
            _server = connection.GetServer("localhost", 6379);
        }

        public List<string> GetFilteredCampaigns(FilterRequest filter)
        {
            List<string> campaignsMatched = new List<string>();

            IEnumerable enumerable = null;
    
            if (filter.CampaignIds != null)
            {
                enumerable = filter.CampaignIds;
            }
            else
            {
                enumerable = _server.Keys();
            }
            
            foreach (var campaign in enumerable)
            {
                string match = GetMatchedCampaign(filter.User, campaign.ToString());

                if (match != null)
                    campaignsMatched.Add(match);
            }

            return campaignsMatched;
        }

        private string GetMatchedCampaign(User user, string campaign)
        {
            string json = _database.StringGet(campaign);
            if (json != null)
            {
                var criteria = JsonConvert.DeserializeObject<Criteria>(json);

                if (CriteriaHelper.MeetsCriteria(user, criteria))
                {
                    return criteria.CampaignId;
                }
            }
            
            return null;
        }

    }
}
