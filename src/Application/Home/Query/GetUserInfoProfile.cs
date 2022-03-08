using AutoMapper;
using Domain.Users;

namespace Application.Home.Query;

public class GetUserInfoProfile : Profile
{
    public GetUserInfoProfile()
    {
        CreateMap<User, GetUserInfoResponse>();
    }
}
