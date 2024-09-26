using AutoMapper;
using Cursus.Common.Helper;
using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Cursus.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CourseController : ControllerBase
	{
		private readonly ICourseService _courseService;
		private readonly APIResponse _response;

		public CourseController(ICourseService courseService, APIResponse response)
		{
			_courseService = courseService;
			_response = response;
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<APIResponse>> CreateCourse([FromBody] CourseDTO courseDto)
		{
			try
			{
				var createdCourse = await _courseService.CreateCourseWithSteps(courseDto);

				_response.IsSuccess = true;
				_response.StatusCode = HttpStatusCode.OK;
				_response.Result = createdCourse;
				return Ok(_response);
			}
			catch (Exception e)
			{
				_response.IsSuccess = false;
				_response.StatusCode = HttpStatusCode.BadRequest;
				_response.ErrorMessages.Add(e.Message);
				return BadRequest(_response);
			}
		}

	}
}
