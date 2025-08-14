using Application.Students.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Features.V1.Students;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[SwaggerTag("Students")]
public class StudentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public StudentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPut("{studentId:guid}/grade")]
    public async Task<ActionResult> UpdateGrade(
        [FromRoute] Guid studentId, 
        [FromBody] UpdateGradeRequest request)
    {
        var command = new UpdateStudentGradeCommand 
        { 
            StudentId = studentId, 
            NewGrade = request.Grade 
        };
        
        var result = await _mediator.Send(command);

        return result.IsSuccess 
            ? Ok(new { Message = "Grade updated successfully" }) 
            : BadRequest(new { Error = result.Error });
    }
}

public record UpdateGradeRequest
{
    public double Grade { get; init; }
}
