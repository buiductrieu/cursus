﻿using Cursus.Common.Helper;
using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.Repository.Repository;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateCourse(int id, [FromBody] CourseDTO courseDto)
        {
            if (id != courseDto.Id)
            {
                return BadRequest(new APIResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = { "Course ID mismatch." }
                });
            }

            try
            {
                var updatedCourse = await _courseService.UpdateCourseWithSteps(courseDto);
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = updatedCourse;
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

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> DeleteCourse(int id)
        {
            try
            {
                bool result = await _courseService.DeleteCourse(id);

                if (!result)
                {
                    return NotFound(new APIResponse
                    {
                        IsSuccess = false,
                        StatusCode = HttpStatusCode.NotFound,
                        ErrorMessages = { "Course not found." }
                    });
                }

                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.NoContent;
                return NoContent();
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add(e.Message);
                return BadRequest(_response);
            }
        }
        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetAllCourses([FromQuery] string? searchTerm,
        [FromQuery] string? sortColumn,
        [FromQuery] string? sortOrder,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
        {
            try
            {

                var result = await _courseService.GetCoursesAsync(
                    searchTerm: searchTerm,
                    sortColumn: sortColumn,
                    sortOrder: sortOrder,
                    page: page,
                    pageSize: pageSize
                );
                if (result.Items.Any())
                {

                    _response.IsSuccess = true;
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.Result = result;
                    return Ok(_response);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages.Add("No courses found");
                    return BadRequest(_response);
                }

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages.Add($"An error occurred: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        [HttpGet("courses/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetCoursesByUserId(string userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {

                var courses = await _courseService.GetRegisteredCoursesByUserIdAsync(userId, page, pageSize);

                if (courses != null && courses.Items.Any())
                {
                    _response.IsSuccess = true;
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.Result = courses;
                    return Ok(_response);
                }

                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages.Add("No courses found for the specified user");
                return NotFound(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages.Add($"An error occurred: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

    }
}