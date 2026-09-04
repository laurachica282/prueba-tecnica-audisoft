using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Common;
using SchoolManagement.Application.DTOs;
using SchoolManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace SchoolManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _service;

        public StudentsController(IStudentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<StudentDtos>>> GetAll(
        [FromQuery] PaginationQuery query, CancellationToken cancellationToken)
        => Ok(await _service.GetPagedAsync(query, cancellationToken));

        [HttpGet("lookup")]
        public async Task<ActionResult<IReadOnlyList<StudentDtos>>> GetLookup(
            CancellationToken cancellationToken)
            => Ok(await _service.GetLookupAsync(cancellationToken));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<StudentDtos>> GetById(
            int id, CancellationToken cancellationToken)
            => Ok(await _service.GetByIdAsync(id, cancellationToken));

        [HttpPost]
        [Authorize(Policy = "CanManagePeople")]
        public async Task<ActionResult<StudentDtos>> Create(
            [FromBody] CreateStudentDto dto, CancellationToken cancellationToken)
        {
            var created = await _service.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "CanManagePeople")]
        public async Task<ActionResult<StudentDtos>> Update(
            int id, [FromBody] UpdateStudentDto dto, CancellationToken cancellationToken)
            => Ok(await _service.UpdateAsync(id, dto, cancellationToken));

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "CanManagePeople")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
