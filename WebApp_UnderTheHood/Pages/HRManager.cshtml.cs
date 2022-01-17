using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp_UnderTheHood.Pages
{
    [Authorize(Policy = "ManagerOfHumanResource")]
    public class HRManager : PageModel
    {
        private readonly ILogger<HRManager> _logger;

        public HRManager(ILogger<HRManager> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}