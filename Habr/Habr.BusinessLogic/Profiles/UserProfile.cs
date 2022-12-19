using AutoMapper;
using Habr.Common.DTOs;
using Habr.Common.DTOs.V2;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, AuthorDTO>()
                .ForMember(
                    author => author.Email,
                    cfgExpression => cfgExpression.MapFrom(user => user.Email))
                .ForMember(
                    author => author.Name,
                    cfgExpression => cfgExpression.MapFrom(user => user.Name));

            CreateMap<User, UserDTO>()
                .ForMember(
                    user => user.Id,
                    cfgExpression => cfgExpression.MapFrom(user => user.Id))
                .ForMember(
                    user => user.Email,
                    cfgExpression => cfgExpression.MapFrom(user => user.Email))
                .ForMember(
                    user => user.Tokens,
                    cfgExpression => cfgExpression.Ignore());
        }
    }
}