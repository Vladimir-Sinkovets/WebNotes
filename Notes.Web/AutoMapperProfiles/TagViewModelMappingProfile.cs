using AutoMapper;
using Notes.BLL.Services.NoteManagers.Models;
using Notes.Web.Models.Note;

namespace Notes.Web.AutoMapperProfiles
{
    public class TagViewModelMappingProfile : Profile
    {
        public TagViewModelMappingProfile()
        {
            CreateMap<TagItemViewModel, Tag>();
            CreateMap<Tag, TagItemViewModel>()
                .ForMember(dest => dest.NotesCount, opt => opt.MapFrom(src => src.Notes.Count));

            CreateMap<Tag, string>()
                .ConstructUsing((src, contex) => src.Name);
        }
    }
}
