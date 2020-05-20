using CoreLogger.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace CoreLogger.Contexts
{
    internal class ContextFile : IDisposable
    {
        readonly string _baseFolder;
        readonly string _traceFolder;
        readonly string _infoFolder;
        readonly string _warningFolder;
        readonly string _errorFolder;

        bool _disposed = false;
        SafeHandle _handle = new SafeFileHandle(IntPtr.Zero, true);

        public ContextFile(string folder)
        {
            _baseFolder = folder ?? throw new ArgumentNullException("Base folder cannot be null");
            if (!Directory.Exists(_baseFolder)) Directory.CreateDirectory(_baseFolder);

            _traceFolder = Path.Combine(_baseFolder, LogLevel.Trace.ToString());
            _infoFolder = Path.Combine(_baseFolder, LogLevel.Information.ToString());
            _warningFolder = Path.Combine(_baseFolder, LogLevel.Warning.ToString());
            _errorFolder = Path.Combine(_baseFolder, LogLevel.Error.ToString());

            if (!Directory.Exists(_traceFolder)) Directory.CreateDirectory(_traceFolder);
            if (!Directory.Exists(_infoFolder)) Directory.CreateDirectory(_infoFolder);
            if (!Directory.Exists(_warningFolder)) Directory.CreateDirectory(_warningFolder);
            if (!Directory.Exists(_errorFolder)) Directory.CreateDirectory(_errorFolder);
        }

        public Task Log(Log_Master data)
        {
            string file = string.Empty;
            switch ((LogLevel)data.LevelID)
            {
                case LogLevel.Error:
                    file = Path.Combine(_errorFolder, $"{DateTime.Today:yyyy-MM-dd}_{LogLevel.Error}.txt");
                    break;
                case LogLevel.Warning:
                    file = Path.Combine(_warningFolder, $"{DateTime.Today:yyyy-MM-dd}_{LogLevel.Warning}.txt");
                    break;
                case LogLevel.Information:
                    file = Path.Combine(_infoFolder, $"{DateTime.Today:yyyy-MM-dd}_{LogLevel.Information}.txt");
                    break;
                default:
                case LogLevel.Trace:
                    file = Path.Combine(_traceFolder, $"{DateTime.Today:yyyy-MM-dd}_{LogLevel.Trace}.txt");
                    break;
            }

            var lines = new List<string>();
            lines.Add($"[START][{nameof(Log_Master.ID)}:{data.DateTime.Ticks}]");
            lines.Add($"[{nameof(Log_Master.DateTime)}:{data.DateTime:yyyy-MM-dd HH:mm:ss}][{((LogLevel)data.LevelID)}][{nameof(Log_Master.CallerMemberName)}:{data.CallerMemberName}][{nameof(Log_Master.CallerMemberLineNumber)}:{data.CallerMemberLineNumber}]");
            lines.Add($"[{nameof(Log_Master.Message)}:{data.Message}]");
            if (!string.IsNullOrWhiteSpace(data.FullData)) lines.Add($"[{nameof(Log_Master.FullData)}:{data.FullData}]");
            lines.Add("[END]");

            for (int i = 0; i < 30; i++)
            {
                try
                {
                    File.AppendAllLines(file, lines);
                    break;
                }
                catch
                {
                    Thread.Sleep(100);
                }
            }
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Log_Master>> GetList(int? level = null, DateTime? from = null, DateTime? to = null)
        {
            var fileNames = new List<string>();

            if (!level.HasValue || level.Value == (int)LogLevel.Error)
                fileNames.AddRange(Directory
                    .GetFiles(_errorFolder)
                    ?.Where(t => !string.IsNullOrEmpty(t) && Path.GetExtension(t) == ".txt" && t.Contains($"_{LogLevel.Error}")));
            if (!level.HasValue || level.Value == (int)LogLevel.Warning)
                fileNames.AddRange(Directory
                    .GetFiles(_warningFolder)
                    ?.Where(t => !string.IsNullOrEmpty(t) && Path.GetExtension(t) == ".txt" && t.Contains($"_{LogLevel.Warning}")));
            if (!level.HasValue || level.Value == (int)LogLevel.Information)
                fileNames.AddRange(Directory
                    .GetFiles(_infoFolder)
                    ?.Where(t => !string.IsNullOrEmpty(t) && Path.GetExtension(t) == ".txt" && t.Contains($"_{LogLevel.Information}")));
            if (!level.HasValue || level.Value == (int)LogLevel.Trace)
                fileNames.AddRange(Directory
                    .GetFiles(_traceFolder)
                    ?.Where(t => !string.IsNullOrEmpty(t) && Path.GetExtension(t) == ".txt" && t.Contains($"_{LogLevel.Trace}")));

            if (from.HasValue)
                fileNames = fileNames
                    ?.Where(t => DateTime.TryParse(Path.GetFileNameWithoutExtension(t).Split('_').FirstOrDefault(), out DateTime dateTime) ?
                        from.Value <= dateTime : false)
                    ?.ToList();
            if (to.HasValue)
                fileNames = fileNames
                    ?.Where(t => DateTime.TryParse(Path.GetFileNameWithoutExtension(t).Split('_').FirstOrDefault(), out DateTime dateTime) ?
                        to.Value.AddDays(1).Date > dateTime : false)
                    ?.ToList();

            var fileContents = new List<string>();

            foreach (var f in fileNames)
                fileContents.AddRange(File.ReadAllLines(f)?.Where(t => !string.IsNullOrWhiteSpace(t)));

            var entities = new List<Log_Master>();
            Log_Master entity = null;
            bool isFullData = false;
            foreach (var elm in fileContents)
            {
                if (elm.StartsWith("[START]"))
                {
                    isFullData = false;
                    entity = new Log_Master();
                }
                else if (elm.StartsWith($"[{nameof(Log_Master.DateTime)}"))
                {
                    isFullData = false;
                    var datas = elm.Split(new string[] { "][" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var data in datas)
                    {
                        if (data.StartsWith($"[{nameof(Log_Master.DateTime)}"))
                        {
                            var dres = DateTime.TryParse(data.Replace($"[{nameof(Log_Master.DateTime)}:", "").Trim(), out DateTime d);
                            if (dres) entity.DateTime = d;
                        }
                        else if (data.StartsWith($"{LogLevel.Error}")) entity.LevelID = (int)LogLevel.Error;
                        else if (data.StartsWith($"{LogLevel.Warning}")) entity.LevelID = (int)LogLevel.Warning;
                        else if (data.StartsWith($"{LogLevel.Information}")) entity.LevelID = (int)LogLevel.Information;
                        else if (data.StartsWith($"{LogLevel.Trace}")) entity.LevelID = (int)LogLevel.Trace;
                        else if (data.StartsWith($"{nameof(Log_Master.CallerMemberName)}"))
                            entity.CallerMemberName = data.Replace($"[{nameof(Log_Master.CallerMemberName)}:", "").Trim();
                        else if (data.StartsWith($"{nameof(Log_Master.CallerMemberLineNumber)}"))
                        {
                            var dres = int.TryParse(data.Replace($"[{nameof(Log_Master.CallerMemberName)}:", "").Trim(), out int d);
                            if (dres) entity.CallerMemberLineNumber = d;
                        }
                    }
                }
                else if (elm.StartsWith($"[{nameof(Log_Master.Message)}"))
                {
                    isFullData = false;
                    var data = elm.Replace($"[{nameof(Log_Master.Message)}:", "").Replace("]", "");
                    entity.Message = data?.Trim() ?? null;
                }
                else if (!elm.StartsWith("[END]") && (elm.StartsWith($"[{nameof(Log_Master.FullData)}") || isFullData))
                {
                    isFullData = true;
                    var data = elm.Replace($"[{nameof(Log_Master.FullData)}:", "").Replace("]", "");
                    entity.FullData ??= "";
                    entity.FullData += data ?? null;
                }
                else if (elm.StartsWith("[END]"))
                {
                    isFullData = false;
                    entities.Add(entity);
                    entity = null;
                }
            }
            return Task.FromResult(entities.AsEnumerable());
        }

        public Task<Log_Master> Get(LogLevel level, long id)
        {
            Log_Master entity = null;
            return Task.FromResult(entity);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing) _handle.Dispose();
            _disposed = true;
        }
    }
}
