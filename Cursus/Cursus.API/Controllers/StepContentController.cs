﻿using Cursus.Common.Helper;
using Cursus.Data.DTO;
using Cursus.RepositoryContract.Interfaces;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.IO;

namespace Cursus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StepContentController : ControllerBase
    {
        private readonly IStepContentService _stepContentService;
        private readonly IAzureBlobStorageService _azureBlobStorageService; // Inject Blob Storage Service
        private readonly APIResponse _response;

        public StepContentController(IStepContentService stepContentService, IAzureBlobStorageService azureBlobStorageService, APIResponse aPIResponse)
        {
            _stepContentService = stepContentService;
            _azureBlobStorageService = azureBlobStorageService; // Initialize Blob Storage Service
            _response = aPIResponse;
        }

        [HttpPost("upload")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UploadFile([FromForm] StepContentDTO stepContentDTO, IFormFile file)
        {
            try
            {
                // check file
                if (file == null || file.Length == 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages.Add("No file uploaded.");
                    return BadRequest(_response);
                }

                // up file lên Azure Blob Storage
                var blobUrl = await _azureBlobStorageService.UploadFileAsync(file);

                // save DTO
                stepContentDTO.ContentURL = blobUrl;
                stepContentDTO.ContentType = Path.GetExtension(file.FileName); // Lấy đuôi file

                // save
                var createdStepContentDTO = await _stepContentService.CreateStepContent(stepContentDTO);

                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.Created;
                _response.Result = createdStepContentDTO;
                return CreatedAtAction("GetStepContentById", new { id = createdStepContentDTO.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages.Add($"Internal Server Error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStepContentById(int id)
        {
            var stepContentDTO = await _stepContentService.GetStepContentByIdAsync(id);

            if (stepContentDTO == null)
            {
                return NotFound(new { message = "StepContent not found." });
            }

            return Ok(stepContentDTO);
        }

    }
}