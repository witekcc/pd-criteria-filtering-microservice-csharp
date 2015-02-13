using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CriteriaFilterService
{
    [Serializable]
    public class Criteria
    {
        public string CampaignId { get; set; }
        public string CampaignUUID { get; set; }
        public dynamic Constraints { get; set; }

    }
}
