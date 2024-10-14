using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Cursus.Data.DTO
{
    public class StepContentDTO
    {
        [JsonIgnore]
        public int Id { get; set; }
        public int StepId { get; set; }
        [JsonIgnore]
        public string ContentType { get; set; } = string.Empty;
        [JsonIgnore]
        public string ContentURL { get; set; } = string.Empty;
        [JsonIgnore]
        public DateTime DateCreated { get; set; }
        public string Description { get; set; } = string.Empty;

    }

    public class StepContentCreateDTO
    {

        public int StepId { get; set; }

        [JsonIgnore]
        public string ContentType { get; set; } = string.Empty;

        [JsonIgnore]
        public string ContentURL { get; set; } = string.Empty;
        [JsonIgnore]
        public DateTime DateCreated { get; set; }
        public string Description { get; set; } = string.Empty;

    }

}
