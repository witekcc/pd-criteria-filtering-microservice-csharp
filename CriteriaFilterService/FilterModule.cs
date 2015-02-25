using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy.ModelBinding;
using CriteriaFilterService.Models;
using Nancy;

namespace CriteriaFilterService
{
    public class FilterModule : Nancy.NancyModule
    {
        IFilterController _controller;

        public FilterModule()
        {
            _controller = new FilterController(StackExchange.Redis.ConnectionMultiplexer.Connect("localhost").GetDatabase()); ;

            Get["/filters/{id?}"] = parameters =>
                {
                    var user = this.Bind<User>();
                    
                    FilterRequest filter = new FilterRequest() { CampaignIds = new List<string>(), User = user };
                    filter.CampaignIds.Add(parameters.id);
                    
                    List<string> results = _controller.GetFilter(filter);

                    return Response.AsJson(results);                                      
               
                };
        }
    }
}
