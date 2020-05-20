namespace CoreLogger
{
    public class License
    {
        /// <summary>
        /// A license expression (like MIT...)
        /// </summary>
        public string Expression { get; set; }
        /// <summary>
        /// The license owner name
        /// </summary>
        public string Owner { get; set; }
        /// <summary>
        /// Copyright information
        /// </summary>
        public string Copyright { get; set; }
        /// <summary>
        /// Link to external license page
        /// </summary>
        public string LicenseUrl { get; set; }
        /// <summary>
        /// Link to external owner page
        /// </summary>
        public string OwnerUrl { get; set; }
    }
}
