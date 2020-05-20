using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace CoreLogger.Inspector.Areas.corelogger.Pages
{
    public class IndexModel : PageModel
    {
        readonly IInspectorService _service;
        public readonly CoreLoggerConfiguration Options;
        
        public IndexModel(
            IInspectorService inspectorService,
            IOptions<CoreLoggerConfiguration> options)
        {
            _service = inspectorService ?? throw new ArgumentNullException($"Injection fail for {nameof(IInspectorService)} in CoreLogger.Inspector");
            Options = options.Value ?? throw new ArgumentNullException($"Injection fail for {nameof(CoreLoggerConfiguration)} in CoreLogger.Inspector");
        }

        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Password { get; set; }
        [TempData]
        public string Message { get; set; }
        public bool DenyEnvironment { get; set; }

        public IActionResult OnGet()
        {
            if (!_service.EnvAllowed())
            {
                DenyEnvironment = true;
                return Page();
            }
            if (!_service.IsLogged()) return Page();
            return RedirectToPage("./Recap");
        }

        public async Task<IActionResult> OnPostAsync(string username, string password)
        {
            var result = _service.Login(username, password);
            if (!result.success)
            {
                Message = result.message;
                return Page();
            }
            return RedirectToPage("./Recap");
        }
    }
}
