using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreLogger.Models
{
    [Serializable]
    public partial class Log_Master
    {
        [JsonProperty(nameof(ID))]
        [Key]
        public int ID { get; set; }

        [JsonProperty(nameof(CallerMemberName))]
        public string CallerMemberName { get; set; }

        [JsonProperty(nameof(CallerMemberLineNumber))]
        public int CallerMemberLineNumber { get; set; }

        [JsonProperty(nameof(DateTime))]
        [DataType(dataType: DataType.DateTime)]
        public DateTime DateTime { get; set; }

        [JsonProperty(nameof(LevelID))]
        public int LevelID { get; set; }

        [JsonProperty(nameof(Message))]
        public string Message { get; set; }

        [JsonProperty(nameof(FullData))]
        public string FullData { get; set; }
    }
}
