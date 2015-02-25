using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CriteriaFilterService.Models;

namespace CriteriaFilterService
{
    static class CriteriaHelper
    {
        public static bool MeetsCriteria(User user, Criteria criteria)
        {
            List<string> criteriaInclusion = new List<string>();
            List<string> criteriaExclusion = new List<string>();

            //here join the list of constraints in the criteria with the properties on a user.
            var query = from keyValue in criteria.Constraints
                        join property in user.GetType().GetProperties() on keyValue.Key.ToLower() equals property.Name.ToLower()
                        where property.GetValue(user) != null
                        select new { value = (string)property.GetValue(user), constraintName = property.Name.ToLower(), constraint = keyValue.Value };

            //for every match we had, we check to see if it's excluded or included
            foreach (var result in query)
            {
                if (MeetsCritera(result.constraint.Exc, result.value, result.constraintName))
                {
                    criteriaExclusion.Add(result.constraintName);
                }

                if (MeetsCritera(result.constraint.Inc, result.value, result.constraintName))
                {
                    criteriaInclusion.Add(result.constraintName);
                }
            }

            if (criteriaExclusion.Count > 0 || criteriaInclusion.Count == 0)
                return false;

            return true;
        }

        private static bool MeetsCritera(List<Constraint> constraints, string value, string constraintName)
        {
            return constraints.Where(c => (c.ToString() == value ||
                    (c.IsRange && (String.Compare(c.StartRange, value) <= 0 && String.Compare(c.EndRange, value) >= 0)))).Count() > 0;
        }
    }
}
