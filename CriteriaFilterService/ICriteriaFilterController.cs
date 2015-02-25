using System;
namespace CriteriaFilterService
{
    public interface ICriteriaFilterController
    {
        dynamic CreateCriteria(Models.Criteria criteria);
        dynamic DeleteCriteria(string id);
        dynamic GetCriteria(string id);
        dynamic UpdateCriteria(Models.Criteria criteria);
    }
}
