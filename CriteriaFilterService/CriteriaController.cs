using Nancy;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CriteriaFilterService
{
    public class CriteriaFilterController : CriteriaFilterService.ICriteriaFilterController
    {
        IDatabase _database;

        public CriteriaFilterController(ConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public dynamic CreateCriteria(Criteria criteria)
        {
            if (criteria.CampaignId == null)
            {
                return HttpStatusCode.NotFound;
            }

            if (!_database.StringGet(criteria.CampaignId).HasValue)
            {
                if (_database.StringSet(criteria.CampaignId,  JsonConvert.SerializeObject(criteria)))
                    return HttpStatusCode.OK;
            }

            return HttpStatusCode.Conflict;            
        }

        public dynamic DeleteCriteria(string id)
        {
            if (id == null || !_database.KeyExists(id))
            {
                return HttpStatusCode.NotFound;
            }

            if (_database.KeyDelete(id))
                return HttpStatusCode.NoContent;

            return HttpStatusCode.NotFound;
        }

        public dynamic GetCriteria(string id)
        {           
            if(id == null || !_database.KeyExists(id))
            {
                return HttpStatusCode.NotFound;
            }

            return _database.StringGet(id);
            
            //return all; not sure how to do this with Redis yet
            return HttpStatusCode.OK;
        }

        public dynamic UpdateCriteria(Criteria criteria)
        {
            if (criteria.CampaignId == null || !_database.KeyExists(criteria.CampaignId))
            {
                return HttpStatusCode.NotFound;
            }

            string json = JsonConvert.SerializeObject(criteria);

            if (_database.StringSet(criteria.CampaignId, json, null, When.Exists))
                return json;
            
            return HttpStatusCode.NotFound;

        }
        
               
    }
}
