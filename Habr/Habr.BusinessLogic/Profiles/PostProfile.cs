using AutoMapper;
using Habr.Common.DTOs;
using Habr.Common.DTOs.V2;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Profiles
{
    public class PostProfile : Profile
    {
        public PostProfile()
        {
            CreateMap<Post, DraftPostDTO>()
                .ForMember(
                    draftPost => draftPost.Title,
                    cfgExpression => cfgExpression.MapFrom(post => post.Title))
                .ForMember(
                    draftPost => draftPost.CreatedAt,
                    cfgExpression => cfgExpression.MapFrom(post => post.CreatedAt))
                .ForMember(
                    draftPost => draftPost.UpdatedAt,
                    cfgExpression => cfgExpression.MapFrom(post => post.UpdatedAt));

            CreateMap<Post, PublishedPostDetailsDTO>()
                .ForMember(
                    publishedPostDetails => publishedPostDetails.Title,
                    cfgExpression => cfgExpression.MapFrom(post => post.Title))
                .ForMember(
                    publishedPostDetails => publishedPostDetails.Text,
                    cfgExpression => cfgExpression.MapFrom(post => post.Text))
                .ForMember(
                    publishedPostDetails => publishedPostDetails.AuthorEmail,
                    cfgExpression => cfgExpression.MapFrom(post => post.User.Email))
                .ForMember(
                    publishedPostDetails => publishedPostDetails.PublishedAt,
                    cfgExpression => cfgExpression.MapFrom(post => post.PublishedAt))
                .ForMember(
                    publishedPostDetails => publishedPostDetails.Comments,
                    cfgExpression => cfgExpression.Ignore());


            CreateMap<Post, PublishedPostDTO>()
                .ForMember(
                    publishedPost => publishedPost.Title,
                    cfgExpression => cfgExpression.MapFrom(post => post.Title))
                .ForMember(
                    publishedPost => publishedPost.AuthorEmail,
                    cfgExpression => cfgExpression.MapFrom(post => post.User.Email))
                .ForMember(
                    publishedPost => publishedPost.PublishedAt,
                    cfgExpression => cfgExpression.MapFrom(post => post.PublishedAt!.Value));

            CreateMap<Post, PublishedPostDTOV2>()
                .ForMember(
                    publishedPost => publishedPost.Title,
                    cfgExpression => cfgExpression.MapFrom(post => post.Title))
                .ForMember(
                    publishedPost => publishedPost.PublishedAt,
                    cfgExpression => cfgExpression.MapFrom(post => post.PublishedAt!.Value))
                .ForMember(
                    publishedPost => publishedPost.Author,
                    cfgExpression => cfgExpression.MapFrom(post => post.User));
        }
    }
}