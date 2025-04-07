using AutoMapper;
using GameReviewSystem.Models;
using GameReviewSystem.DTOs;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace GameReviewSystem.Mapping
{
    public class AppMappingProfile : Profile
    {
        public AppMappingProfile()
        {
            // Map from the Entity -> DTO
            CreateMap<Game, GameDto>()
                // If you want to calculate the AverageRating from Reviews:
                .ForMember(dest => dest.AverageRating,
                    opt => opt.MapFrom(src =>
                        src.Reviews != null && src.Reviews.Count > 0
                            ? src.Reviews.Average(r => r.Rating)
                            : 0
                    ));

            // Map from the CreateGameDto -> Entity (for POST/PUT)
            CreateMap<CreateGameDto, Game>();

            // Similarly, you can do for Review <-> ReviewDto, etc.
            // CreateMap<Review, ReviewDto>();
            // CreateMap<CreateReviewDto, Review>();
        }
    }
}
