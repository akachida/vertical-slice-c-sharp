using Application.Home.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Features.V1.Home;

[ApiController]
[ApiVersion("1.0")]
[Route("v{v:apiVersion}/home")]
[SwaggerTag("Home")]
public class HomeController : ControllerBase
{
    private readonly IMediator _mediator;

    public HomeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Route("{userId:guid}")]
    public async Task<ActionResult<GetUserInfoResponse>> GetUserInfo([FromRoute] Guid userId)
    {
        var query = new GetUserInfoQuery { UserId = userId };
        var result = await _mediator.Send(query);

        return result.IsSuccess 
            ? Ok(result.Value) 
            : BadRequest(new { Error = result.Error });
    }
}
