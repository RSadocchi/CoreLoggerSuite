using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLogger.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CoreLogger.Inspector.Areas.corelogger.Pages
{
    public class DetailModel : PageModel
    {
        readonly IInspectorService _service;
        public readonly CoreLoggerConfiguration Options;

        public DetailModel(IInspectorService inspectorService)
        {
            _service = inspectorService ?? throw new ArgumentNullException($"Injection fail for {nameof(IInspectorService)} in CoreLogger.Inspector");
            Options = _service.Options;
        }

        [TempData]
        public int Detail_Source { get; set; }
        [TempData]
        public int? Detail_Level { get; set; }
        [TempData]
        public string Detail_ID { get; set; }

        public Log_Master Detail_Log { get; set; }

        public void OnGet()
        {
            try
            {
                if (!_service.EnvAllowed() || !_service.IsLogged()) GoToIndex();
                else Detail_Log = _service.Get((LogSource)Detail_Source, Detail_ID, Detail_Level).Result;
            }
            catch (Exception e)
            {
                var item = new Log_Master()
                {
                    Message = e.Message,
                    DateTime = DateTime.Now,
                    LevelID = (int)LogLevel.Error,
                    CallerMemberName = "DetailModel.OnGet",
                    FullData = e.ToString()
                };
                _service.LogError(item, e);
            }
        }

        private IActionResult GoToIndex() => RedirectToPage("./Index");
    }
}
