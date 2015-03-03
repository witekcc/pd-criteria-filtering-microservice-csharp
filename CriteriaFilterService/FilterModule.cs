using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy.ModelBinding;
using CriteriaFilterService.Models;
using Nancy;
using System.Dynamic;
using Nancy.Responses.Negotiation;

namespace CriteriaFilterService
{
    public class FilterModule : Nancy.NancyModule
    {
        IFilterController _controller;

        public FilterModule(FilterController controller)
        {
            _controller = controller;

            Get["/filters/{id?}"] = parameters =>
                {
                    
                    FilterRequest filter = null;

                    if (parameters.id != null)
                    {
                        filter = new FilterRequest() { CampaignIds = new List<string>(), User = this.Bind<User>() };
                        filter.CampaignIds.Add(parameters.id);
                    }
                    else
                    {
                        filter = this.Bind<FilterRequest>();
                    }                                        

                    return Negotiate
                        .WithModel(_controller.GetFilteredCampaigns(filter))
                        .WithStatusCode(HttpStatusCode.OK);
                };
        }
    }
}
