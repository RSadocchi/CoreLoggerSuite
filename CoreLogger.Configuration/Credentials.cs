using System;

namespace CoreLogger
{
    public class Credentials
    {
        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Timespan of authentication deadline, default 20 minutes
        /// </summary>
        public TimeSpan Expiring { get; set; } = TimeSpan.FromMinutes(20);
    }
}
