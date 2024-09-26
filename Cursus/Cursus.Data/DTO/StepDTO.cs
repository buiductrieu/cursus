
using System.ComponentModel.DataAnnotations;

namespace Cursus.Data.DTO
{
	public class StepDTO
	{
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
