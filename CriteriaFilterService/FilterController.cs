using CriteriaFilterService.Models;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CriteriaFilterService
{
    public class FilterController : IFilterController
    {

        RedisHelper _redis;
        
        public FilterController(RedisHelper redis)
        {
            _redis = redis;
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
                enumerable = _redis.GetAllCriteriaId();
            }
            
            foreach (var campaign in enumerable)
            {
                string match = GetMatchedCampaign(filter.User, campaign.ToString(), enumerable is IEnumerable<RedisValue>);
                
                if (match != null)
                    campaignsMatched.Add(match);
            }

            return campaignsMatched;
        }

        private string GetMatchedCampaign(User user, string id, bool byCriteria = false)
        {
            string json; 
            if(byCriteria)
            {
                json = _redis.GetCriteriaById(id);
            }
            else
            {
                json = _redis.GetCriteriaByCampaignId(id); 
            }

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
