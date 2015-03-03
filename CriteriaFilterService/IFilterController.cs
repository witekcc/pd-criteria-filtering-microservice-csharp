using CriteriaFilterService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CriteriaFilterService
{
    public interface IFilterController
    {
        List<string> GetFilteredCampaigns(FilterRequest filter);
    }
}
