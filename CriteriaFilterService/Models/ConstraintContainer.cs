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
    public class ConstraintContainer : ISerializable
    {

        [IgnoreDataMember]
        public List<Constraint> Inc { get; set; }

        [IgnoreDataMember]
        public List<Constraint> Exc { get; set; }

        public ConstraintContainer() { }

        public ConstraintContainer(SerializationInfo info, StreamingContext context)
        {
            Inc = (List<Constraint>)info.GetValue("inc", typeof(List<Constraint>));
            Exc = (List<Constraint>)info.GetValue("exc", typeof(List<Constraint>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //This gives us 2 lists of strings based on the Constraints
            info.AddValue("inc", Inc.ConvertAll(i => i.ToString()));
            info.AddValue("exc", Exc.ConvertAll(i => i.ToString()));
        }
    }
}
