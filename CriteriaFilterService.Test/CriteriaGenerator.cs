using CriteriaFilterService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CriteriaFilterService.Test
{
    public static class CriteriaGenerator
    {
        public static Criteria GenerateCriteria()
        {
            Criteria criteria = new Criteria()
            {
                CampaignId = "12",
                CampaignUUID = "A02AECA1-C7DD-4FC5-ADDF-ED5486F77A09",
                Constraints = new Dictionary<string, ConstraintContainer>()
            };

            criteria.Constraints.Add("phone", new ConstraintContainer() { Inc = new List<Constraint>() { new Constraint("5000000000-6000000000") }, Exc = new List<Constraint>() { new Constraint("5555550000") } });
            criteria.Constraints.Add("age", new ConstraintContainer() { Inc = new List<Constraint>() { new Constraint("21-35"), new Constraint("40") }, Exc = new List<Constraint>() { new Constraint("26") } });
            criteria.Constraints.Add("zip", new ConstraintContainer() { Inc = new List<Constraint>() { new Constraint("12345-12549"), new Constraint("54313") }, Exc = new List<Constraint>() { new Constraint("12347-12349") } });

            return criteria;
        }

        public static List<Criteria> GenerateMultipleCriteria()
        {
            List<Criteria> criteriaList = new List<Criteria>();

            Criteria criteria = new Criteria()
            {
                CampaignId = "12",
                CampaignUUID = "A02AECA1-C7DD-4FC5-ADDF-ED5486F77A09",
                Constraints = new Dictionary<string, ConstraintContainer>()
            };

            criteria.Constraints.Add("phone", new ConstraintContainer() { Inc = new List<Constraint>() { new Constraint("5000000000-6000000000") }, Exc = new List<Constraint>() { new Constraint("5555550000") } });
            criteria.Constraints.Add("age", new ConstraintContainer() { Inc = new List<Constraint>() { new Constraint("21-35"), new Constraint("40") }, Exc = new List<Constraint>() { new Constraint("26") } });
            criteria.Constraints.Add("zip", new ConstraintContainer() { Inc = new List<Constraint>() { new Constraint("12345-12549"), new Constraint("54313") }, Exc = new List<Constraint>() { new Constraint("12347-12349") } });

            criteriaList.Add(criteria);

            criteria = new Criteria()
            {
                CampaignId = "13",
                CampaignUUID = "A02AECA1-C7DD-4FC5-ADDF-ED5486F77A08",
                Constraints = new Dictionary<string, ConstraintContainer>()
            };

            criteria.Constraints.Add("phone", new ConstraintContainer() { Inc = new List<Constraint>() { new Constraint("5000000000-6000000000") }, Exc = new List<Constraint>() { new Constraint("5555660000") } });
            criteria.Constraints.Add("age", new ConstraintContainer() { Inc = new List<Constraint>() { new Constraint("19-35"), new Constraint("40"), new Constraint("50-55") }, Exc = new List<Constraint>() { new Constraint("30") } });
            criteria.Constraints.Add("zip", new ConstraintContainer() { Inc = new List<Constraint>() { new Constraint("25512-25520"), new Constraint("90210") }, Exc = new List<Constraint>() { new Constraint("25514-25516") } });

            criteriaList.Add(criteria);

            criteria = new Criteria()
            {
                CampaignId = "25",
                CampaignUUID = "A02AECA1-C7DD-4FC5-ADDF-ED5476F77A08",
                Constraints = new Dictionary<string, ConstraintContainer>()
            };

            criteria.Constraints.Add("phone", new ConstraintContainer() { Inc = new List<Constraint>() { new Constraint("4000000000-4000000500") }, Exc = new List<Constraint>() { new Constraint("4000000200-4000000300") } });
            criteria.Constraints.Add("age", new ConstraintContainer() { Inc = new List<Constraint>() { new Constraint("40-45"), new Constraint("50-55") }, Exc = new List<Constraint>() { new Constraint("41") } });
            criteria.Constraints.Add("zip", new ConstraintContainer() { Inc = new List<Constraint>() { new Constraint("25512-25520"), new Constraint("90210") }, Exc = new List<Constraint>() { new Constraint("25514-25516") } });
            criteria.Constraints.Add("gender", new ConstraintContainer() { Inc = new List<Constraint>() { new Constraint("M")}, Exc = new List<Constraint>() { new Constraint("F")}});

            criteriaList.Add(criteria);

            return criteriaList;

        }
    }
}
