using AutoMapper;
using Notes.BLL.Services.NoteManagers.Models;
using Notes.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes.BLL.AutoMapperProfiles
{
    public class NoteMappingProfile : Profile
    {
        public NoteMappingProfile()
        {
            CreateMap<Note, NoteEntry>();
            CreateMap<NoteEntry, Note>();

            CreateMap<NoteCreateData, NoteEntry>();
            CreateMap<NoteEntry, NoteCreateData>();

            CreateMap<NoteUpdateData, NoteEntry>();
            CreateMap<NoteEntry, NoteUpdateData>();

            CreateMap<Tag, TagEntry>();
            CreateMap<TagEntry, Tag>();

            CreateMap<TagCreateData, TagEntry>();
            CreateMap<TagEntry, TagCreateData>();
        }
    }
}
