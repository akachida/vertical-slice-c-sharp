using AutoMapper;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Helpers;
using SharedKernel.Domain;

namespace Application.Home.Query;

public sealed record GetUserInfoResponse
{
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
}

public sealed record GetUserInfoQuery : IRequest<Result<GetUserInfoResponse>>
{
    public Guid UserId { get; init; }
}

public class GetUserInfoQueryHandler : IRequestHandler<GetUserInfoQuery, Result<GetUserInfoResponse>>
{
    private readonly ApplicationContext _context;
    private readonly IMapper _mapper;

    public GetUserInfoQueryHandler(ApplicationContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<GetUserInfoResponse>> Handle(
        GetUserInfoQuery request,
        CancellationToken token)
    {
        if (request.UserId.IsDefault())
            return Result.Failure<GetUserInfoResponse>("UserId cannot be empty");

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == request.UserId, token)
            .ConfigureAwait(false);

        if (user is null || user.IsDefault())
            return Result.Failure<GetUserInfoResponse>("User not found");

        var response = _mapper.Map<GetUserInfoResponse>(user);
        return Result.Success(response);
    }
}
