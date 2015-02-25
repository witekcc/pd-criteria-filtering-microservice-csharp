using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CriteriaFilterService.Models
{
    [Serializable]
    public class Criteria
    {
        [DataMember(Name = "campaignId")]
        public string CampaignId { get; set; }
        [DataMember(Name = "campaignUUID")]
        public string CampaignUUID { get; set; }
        [DataMember(Name = "constraints")]
        public Dictionary<string, ConstraintContainer> Constraints { get; set; }

    }
}
