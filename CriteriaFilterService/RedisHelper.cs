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
    //This is a simple implementation which provides similar functionality as Redis Ohm, but customized for this service and it's models. 
    public class RedisHelper
    {
        private static string _connectionString = ConfigurationManager.AppSettings["redisConnectionString"];
        private static ConfigurationOptions _options;

        private static ConnectionMultiplexer _multiplexer;

        private const string _campaignIdHashesKey = "Criteria:uniques:campaignid";
        private const string _uuIdHashesKey = "Criteria:uniques:uuid";

        static RedisHelper()
        {
            _options = ConfigurationOptions.Parse(_connectionString);
            _options.ClientName = "Criteria Filter Service";

            _multiplexer = ConnectionMultiplexer.Connect(_options);
        }

        private IDatabase GetDatabase()
        {
            return _multiplexer.GetDatabase();
        }

        private IServer GetServer()
        {
            return _multiplexer.GetServer(_options.EndPoints[0]);
        }

        public bool SaveCriteria(Criteria criteria)
        {
            long id = GetDatabase().StringIncrement("Criteria:Id");

            return UpdateCriteria(criteria, id.ToString());

        }

        private bool UpdateCriteria(Criteria criteria, string id)
        {
            GetDatabase().SetAdd("Criteria:All", id);

            if (criteria.CampaignId != null)
                GetDatabase().HashSet(_campaignIdHashesKey, new HashEntry[] { new HashEntry(criteria.CampaignId, id) });

            if (criteria.CampaignUUID != null)
                GetDatabase().HashSet(_uuIdHashesKey, new HashEntry[] { new HashEntry(criteria.CampaignUUID, id) });

            return GetDatabase().StringSet("Criteria:" + id, JsonConvert.SerializeObject(criteria));
                
        }

        public bool CriteriaExists(Criteria criteria)
        {
            return GetDatabase().HashExists(_campaignIdHashesKey, criteria.CampaignId) || 
                GetDatabase().HashExists(_uuIdHashesKey, criteria.CampaignId);               
        }

        public bool CriteriaExists(string id)
        {
            if (id == null)
                return false;

            return GetDatabase().HashExists(_campaignIdHashesKey, id) ||
                GetDatabase().HashExists(_uuIdHashesKey, id);
        }

        public RedisValue GetCriteriaByCampaignId(string id)
        {
            return GetDatabase().StringGet("Criteria:" + GetIdOfCriteria(id));
        }

        public RedisValue GetCriteriaById(string id)
        {
            return GetDatabase().StringGet("Criteria:" + id);
        }

        public bool DeleteCriteria(string id)
        {
            Criteria criteria = JsonConvert.DeserializeObject<Criteria>(GetCriteriaByCampaignId(id));
            string criteriaId = GetIdOfCriteria(id);

            GetDatabase().HashDelete(_campaignIdHashesKey, criteria.CampaignId);
            GetDatabase().HashDelete(_uuIdHashesKey, criteria.CampaignUUID);

            GetDatabase().SetRemove("Criteria:All", criteriaId);
            return GetDatabase().KeyDelete("Criteria:" + criteriaId);

        }

        private string GetIdOfCriteria(string id)
        {
            RedisValue criteriaId = GetDatabase().HashGet(_campaignIdHashesKey, id);

            if (criteriaId.IsNullOrEmpty)
                criteriaId = GetDatabase().HashGet(_uuIdHashesKey, id);

            return criteriaId;
        }

        private string GetIdOfCriteria(Criteria criteria)
        {
            RedisValue criteriaId = GetDatabase().HashGet(_campaignIdHashesKey, criteria.CampaignId);

            if (criteriaId.IsNullOrEmpty)
                criteriaId = GetDatabase().HashGet(_uuIdHashesKey, criteria.CampaignUUID);

            return criteriaId;
        }

        public bool UpdateCriteria(Criteria criteria)
        {
            string id = GetIdOfCriteria(criteria);

            return UpdateCriteria(criteria, id);
        }

        public IEnumerable GetAllCriteriaId()
        {
            return GetDatabase().SetScan("Criteria:All");
        }
    }
}
