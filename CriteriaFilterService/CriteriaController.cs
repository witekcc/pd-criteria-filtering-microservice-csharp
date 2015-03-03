using Nancy;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CriteriaFilterService.Models;

namespace CriteriaFilterService
{
    public class CriteriaFilterController : CriteriaFilterService.ICriteriaFilterController
    {
        RedisHelper _redis;

        public CriteriaFilterController(RedisHelper redis)
        {
            _redis = redis;
        }

        public dynamic CreateCriteria(Criteria criteria)
        {
            if (criteria == null || (criteria.CampaignId == null && criteria.CampaignUUID == null))
            {
                return HttpStatusCode.NotFound;
            }

            if (_redis.CriteriaExists(criteria))
                return HttpStatusCode.Conflict;

            if (_redis.SaveCriteria(criteria))
                return HttpStatusCode.OK;

            return HttpStatusCode.BadRequest;        
        }

        public dynamic DeleteCriteria(string id)
        {
            if (!_redis.CriteriaExists(id))
            {
                return HttpStatusCode.NotFound;
            }

            if (_redis.DeleteCriteria(id))
                return HttpStatusCode.NoContent;

            return HttpStatusCode.NotFound;
        }

        public dynamic GetCriteria(string id)
        {
            if (!_redis.CriteriaExists(id))
            {
                return HttpStatusCode.NotFound;
            }

            return _redis.GetCriteriaByCampaignId(id);
        }

        public dynamic UpdateCriteria(Criteria criteria)
        {
            if (criteria == null || criteria.CampaignId == null || !_redis.CriteriaExists(criteria.CampaignId))
            {
                return HttpStatusCode.NotFound;
            }

            if (_redis.UpdateCriteria(criteria))
                return JsonConvert.SerializeObject(criteria);
            
            return HttpStatusCode.NotFound;

        }
        
               
    }
}
