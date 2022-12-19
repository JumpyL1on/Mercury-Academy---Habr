using AutoMapper;
using Habr.Common.DTOs;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Profiles
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<Comment, PreliminaryCommentDTO>()
                .ForMember(
                    preliminaryComment => preliminaryComment.Id,
                    cfgExpression => cfgExpression.MapFrom(comment => comment.Id))
                .ForMember(
                    preliminaryComment => preliminaryComment.Text,
                    cfgExpression => cfgExpression.MapFrom(comment => comment.Text))
                .ForMember(
                    preliminaryComment => preliminaryComment.CreatedAt,
                    cfgExpression => cfgExpression.MapFrom(comment => comment.CreatedAt))
                .ForMember(
                    preliminaryComment => preliminaryComment.AuthorEmail,
                    cfgExpression => cfgExpression.MapFrom(comment => comment.User.Email))
                .ForMember(
                    preliminaryComment => preliminaryComment.ParentId,
                    cfgExpression => cfgExpression.MapFrom(comment => comment.ParentId));
        }
    }
}