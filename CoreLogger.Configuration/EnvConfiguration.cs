namespace CoreLogger
{
    public class EnvConfiguration
    {
        /// <summary>
        /// FOR INSPECTOR (optional) - The allowed environment name (Development, Production, Stage ...)
        /// </summary>
        public string EnvironmentName { get; set; }
        /// <summary>
        /// Explicit deny access to inspector in this environment, default is false (allowed)
        /// </summary>
        public bool DenyEnvironmet { get; set; } = false;
        /// <summary>
        /// FOR INSPECTOR (optional) - Optional credentials for the page, if null or empty no authentication is needed
        /// </summary>
        public Credentials UseAuthentication { get; set; }
    }
}
