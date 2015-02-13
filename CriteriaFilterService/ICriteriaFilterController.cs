using System;
namespace CriteriaFilterService
{
    public interface ICriteriaFilterController
    {
        dynamic CreateCriteria(Criteria criteria);
        dynamic DeleteCriteria(string id);
        dynamic GetCriteria(string id);
        dynamic UpdateCriteria(Criteria criteria);
    }
}
