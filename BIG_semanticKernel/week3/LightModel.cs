using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace week3
{
    public class LightModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("is_on")]
        public bool? IsOn { get; set; }

        [JsonPropertyName("brightness")]
        public byte? Brightness { get; set; }

        [JsonPropertyName("hex")]
        public string? Hex { get; set; }
    }


}
