using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CriteriaFilterService.Models
{
    [DataContract]
    public struct Constraint
    {
        private string _value;
                        
        public Constraint(string value)
        {
            _value = value;
        }
        
        [IgnoreDataMember]
        public bool IsRange 
        {
            get
            {
                return _value.Contains('-');
            }
        }

        [IgnoreDataMember]
        public string StartRange
        {
            get
            {
                if (_value.Contains('-'))
                    return _value.Split('-')[0];
                else
                    return null;
            }
        }

        [IgnoreDataMember]
        public string EndRange
        {
            get
            {
                if (_value.Contains('-'))
                    return _value.Split('-')[1];
                else
                    return null;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is string)
                return this.ToString() == obj as string;

            return base.Equals(obj);
        }
        
        public override string ToString()
        {
            return _value;
        }

        public static implicit operator Constraint(string value)
        {
            return new Constraint(value);
        }

        public static implicit operator string(Constraint constraint)
        {
            return constraint._value;
        }
              

    }
}
