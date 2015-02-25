using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy.ModelBinding;
using CriteriaFilterService.Models;

namespace CriteriaFilterService
{
    public class Filter : Nancy.NancyModule
    {
        IFilterController controller;

        public Filter()
        {
            Get["/filters/{id?}"] = parameters =>
                {
                    var filter = this.Bind<FilterRequest>();
                    return controller.GetFilter(filter);
                };
        }
    }
}
