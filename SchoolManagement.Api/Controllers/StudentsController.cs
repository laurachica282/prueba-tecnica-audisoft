using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Common;
using SchoolManagement.Application.DTOs;
using SchoolManagement.Application.Interfaces;

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

        [HttpGet("{id:int}")]
        public async Task<ActionResult<StudentDtos>> GetById(
            int id, CancellationToken cancellationToken)
            => Ok(await _service.GetByIdAsync(id, cancellationToken));

        [HttpPost]
        public async Task<ActionResult<StudentDtos>> Create(
            [FromBody] CreateStudentDto dto, CancellationToken cancellationToken)
        {
            var created = await _service.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<StudentDtos>> Update(
            int id, [FromBody] UpdateStudentDto dto, CancellationToken cancellationToken)
            => Ok(await _service.UpdateAsync(id, dto, cancellationToken));

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
