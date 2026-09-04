using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Common;
using SchoolManagement.Application.DTOs;
using SchoolManagement.Application.Interfaces;

namespace SchoolManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GradesController : ControllerBase
    {
        private readonly IGradeService _service;

        public GradesController(IGradeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<GradeDtos>>> GetAll(
            [FromQuery] PaginationQuery query, CancellationToken cancellationToken)
            => Ok(await _service.GetPagedAsync(query, cancellationToken));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GradeDtos>> GetById(
            int id, CancellationToken cancellationToken)
            => Ok(await _service.GetByIdAsync(id, cancellationToken));

        [HttpPost]
        [Authorize(Policy = "CanManageGrades")]
        public async Task<ActionResult<GradeDtos>> Create(
            [FromBody] CreateGradeDto dto, CancellationToken cancellationToken)
        {
            var created = await _service.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "CanManageGrades")]
        public async Task<ActionResult<GradeDtos>> Update(
            int id, [FromBody] UpdateGradeDto dto, CancellationToken cancellationToken)
            => Ok(await _service.UpdateAsync(id, dto, cancellationToken));

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "CanManageGrades")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
