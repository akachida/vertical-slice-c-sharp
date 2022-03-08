using AutoMapper;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Helpers;

namespace Application.Home.Query;

public sealed record GetUserInfoResponse
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
}

public sealed record GetUserInfoQuery : IRequest<GetUserInfoResponse>
{
    public Guid UserId { get; init; }
}

public class GetUserInfoQueryHandler : IRequestHandler<GetUserInfoQuery, GetUserInfoResponse>
{
    private readonly ApplicationContext _context;
    private readonly IMapper _mapper;

    public GetUserInfoQueryHandler(ApplicationContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GetUserInfoResponse> Handle(
        GetUserInfoQuery request,
        CancellationToken token)
    {
        if (request.UserId.IsDefault())
            throw new Exception("UserId should not be null or empty");

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.UserId, token)
            .ConfigureAwait(false);

        if (user is null || user.IsDefault())
            throw new Exception("UserId not found on the system"); // or could be a domain validator

        return _mapper.Map<GetUserInfoResponse>(user);
    }
}
