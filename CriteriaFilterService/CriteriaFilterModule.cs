using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy.ModelBinding;

namespace CriteriaFilterService
{
    public class CriteriaFilterModule : Nancy.NancyModule
    {
        private ICriteriaFilterController _CriteraController = new CriteriaFilterController(StackExchange.Redis.ConnectionMultiplexer.Connect("localhost").GetDatabase());

        public CriteriaFilterModule()
        {           
            Post["/criteria"] = parameters =>
            {
                var criteria = this.Bind<Criteria>();
                return _CriteraController.CreateCriteria(criteria);
            };

            Get["/criteria/{id?}"] = parameters => _CriteraController.GetCriteria(parameters.id);
            Put["/criteria/{id}"] = parameters =>
            {
                var criteria = this.Bind<Criteria>();
                return _CriteraController.UpdateCriteria(criteria);
            }; 
            Delete["/criteria/{id}"] = parameters => _CriteraController.DeleteCriteria(parameters.id);

        }
    }
}
