using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Common;
using SchoolManagement.Application.DTOs;
using SchoolManagement.Application.Interfaces;

namespace SchoolManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeachersController : ControllerBase
    {
        private readonly ITeacherService _service;

        public TeachersController(ITeacherService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<TeacherDtos>>> GetAll(
            [FromQuery] PaginationQuery query, CancellationToken cancellationToken)
            => Ok(await _service.GetPagedAsync(query, cancellationToken));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TeacherDtos>> GetById(
            int id, CancellationToken cancellationToken)
            => Ok(await _service.GetByIdAsync(id, cancellationToken));

        [HttpPost]
        public async Task<ActionResult<TeacherDtos>> Create(
            [FromBody] CreateTeacherDto dto, CancellationToken cancellationToken)
        {
            var created = await _service.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TeacherDtos>> Update(
            int id, [FromBody] UpdateTeacherDto dto, CancellationToken cancellationToken)
            => Ok(await _service.UpdateAsync(id, dto, cancellationToken));

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
