using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace Flags.Models
{
    public class Country
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "abreviation")]
        public string Abreviation { get; set; }

        [JsonProperty(PropertyName = "names")]
        public string[] Names { get; set; }
    }
}
