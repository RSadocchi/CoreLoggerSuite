using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreLogger
{
    public class CoreLoggerConfiguration
    {
        #region ///BASIC
        public bool UseSQL => !string.IsNullOrWhiteSpace(SQL_ConnectionString);
        public bool UseSQLite => !string.IsNullOrWhiteSpace(SQLite_FullPath);
        public bool UseDailyLogFile => !string.IsNullOrWhiteSpace(File_FolderPath);

        /// <summary>
        /// Set this value if you want to use SQL Server for logging, the table Log_Master will be created automatically by migrations. 
        /// You can use booth SQL, SQLite and file at same time.
        /// </summary>
        public string SQL_ConnectionString { get; set; }

        /// <summary>
        /// The full path of the file .db3 (SQLite database format) to use for log. 
        /// You can use booth SQL, SQLite and file at same time.
        /// </summary>
        public string SQLite_FullPath { get; set; }

        /// <summary>
        /// The main folder to log in file .txt. 
        /// You can use booth SQL, SQLite and file at same time.
        /// </summary>
        public string File_FolderPath { get; set; }
        #endregion

        #region ///MIDDLEWARE
        /// <summary>
        /// FOR MIDDELWARE (required) - WARNING if you set to 0 (trace) all HttpContext request will be logged, all Middelware.Invoke()
        /// </summary>
        public LogLevel MinLevel { get; set; } = LogLevel.Warning;

        /// <summary>
        /// FOR MIDDELWARE (optional) - The relative or absolute url of the page to redirect if any error occour, all Response StatusCode errors will be redirected here
        /// </summary>
        public string ErrorPage { get; set; }

        /// <summary>
        /// FOR MIDDELWARE (optional) - The pages to redirect if any error occour, the key (int) represente the Response StatusCode, the value (string) the relative or absolute url to call
        /// </summary>
        public IEnumerable<KeyValuePair<int, string>> ErrorPages { get; set; }
        #endregion

        #region ///INSPECTOR
        /// <summary>
        /// FOR INSPECTOR (optional) - Title to show on header page
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// FOR INSPECTOR (optional) - Short description in header page
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// FOR INSPECTOR (optional) - Some contacts to show in the footer page
        /// </summary>
        public IEnumerable<Contact> Contacts { get; set; }
        ///// <summary>
        ///// FOR INSPECTOR (optional) - License info to show in footer page
        ///// </summary>
        //public License License { get; set; }
        /// <summary>
        /// FOR INSPECTOR (optional) - Environment allowed to inspect logs (Development, Production, Stage....), if null or empty all enviroments are allowed without authentication
        public IEnumerable<EnvConfiguration> Environments { get; set; }
        #endregion
    }
}
