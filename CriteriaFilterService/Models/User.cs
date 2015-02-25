using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CriteriaFilterService.Models
{
    [Serializable]
    public class User
    {
        [DataMember(Name = "phone")]
        public string Phone { get; set; }
        [DataMember(Name = "age")]
        public string Age { get; set; }
        [DataMember(Name = "gender")]
        public string Gender { get; set; }
        [DataMember(Name = "zip")]
        public string Zip { get; set; }
        [DataMember(Name = "state")]
        public string State { get; set; }
    }
}
