
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Cursus.Data.DTO
{
	public class StepDTO
	{
        [JsonIgnore]
        public int Id { get; set; }
		public int CourseId { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public int Order { get; set; }
		[Required]
		public string Description { get; set; }
		public DateTime DateCreated { get; set; }
	}
}
