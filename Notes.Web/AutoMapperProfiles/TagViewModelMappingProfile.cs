using AutoMapper;
using Notes.BLL.Services.NoteManagers.Models;
using Notes.Web.Models;

namespace Notes.Web.AutoMapperProfiles
{
    public class TagViewModelMappingProfile : Profile
    {
        public TagViewModelMappingProfile()
        {
            CreateMap<TagViewModel, Tag>();
            CreateMap<Tag, TagViewModel>()
                .ForMember(dest => dest.NotesCount, opt => opt.MapFrom(src => src.Notes.Count));
        }
    }
}
