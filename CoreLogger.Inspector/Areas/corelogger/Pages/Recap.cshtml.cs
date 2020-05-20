using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLogger.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CoreLogger.Inspector.Areas.corelogger.Pages
{
    public class RecapModel : PageModel
    {
        public readonly CoreLoggerConfiguration Options;
        readonly IInspectorService _service;

        public RecapModel(IInspectorService inspectorService)
        {
            _service = inspectorService ?? throw new ArgumentNullException($"Injection fail for {nameof(IInspectorService)} in CoreLogger.Inspector");
            Options = _service.Options;
            _loadDatasources();
        }

        public List<(int Value, string Text)> Sources { get; private set; }
        public List<(int? Value, string Text)> Levels { get; private set; }

        private void _loadDatasources()
        {
            Sources = new List<(int Value, string Text)>();
            if (_service.Options.UseDailyLogFile) Sources.Add(((int)LogSource.File, $"{LogSource.File}"));
            if (_service.Options.UseDailyLogFile) Sources.Add(((int)LogSource.SQLite, $"{LogSource.SQLite}"));
            if (_service.Options.UseDailyLogFile) Sources.Add(((int)LogSource.SQL, $"{LogSource.SQL}"));

            Levels = new List<(int? Value, string Text)>();
            Levels.Add((null, $"All"));
            Levels.Add(((int)LogLevel.Trace, $"{LogLevel.Trace}"));
            Levels.Add(((int)LogLevel.Information, $"{LogLevel.Information}"));
            Levels.Add(((int)LogLevel.Warning, $"{LogLevel.Warning}"));
            Levels.Add(((int)LogLevel.Error, $"{LogLevel.Error}"));
        }

        [TempData]
        [BindProperty]
        public int Source { get; set; }
        [TempData]
        [BindProperty]
        public int? Level { get; set; }
        [TempData]
        [BindProperty]
        public DateTime? From { get; set; }
        [TempData]
        [BindProperty]
        public DateTime? To { get; set; }

        public List<Log_Master> Logs { get; private set; }

        public void OnGet()
        {
            if (!_service.EnvAllowed() || !_service.IsLogged()) GoToIndex();
            else
                Logs = _service.GetList((LogSource)Source, Level, From, To).Result
                    ?.OrderByDescending(t => t.DateTime)
                    ?.ToList();
        }

        private IActionResult GoToIndex() => RedirectToPage("./Index");

        public async Task<IActionResult> OnPostAsync(int source, int? level, DateTime? from, DateTime? to)
        {
            Logs = (await _service.GetList((LogSource)source, level, from, to))
                ?.OrderByDescending(t => t.DateTime)
                ?.ToList();
            Source = source;
            Level = level;
            From = from;
            To = to;
            return RedirectToPage("./Recap");
        }

        public IActionResult OnPostDetail(int source, int level, string id)
        {
            TempData["Detail_Source"] = source;
            TempData["Detail_Level"] = level;
            TempData["Detail_ID"] = id;
            return RedirectToPage("./Detail");
        }
    }
}
