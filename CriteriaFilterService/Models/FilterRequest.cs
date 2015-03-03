using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CriteriaFilterService.Models
{
    [Serializable]
    public class FilterRequest
    {
        [DataMember(Name = "user")]
        public User User { get; set; }
        [DataMember(Name = "campaigns")]
        public List<string> CampaignIds { get; set; }
    }
}
