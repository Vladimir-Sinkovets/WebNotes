using AutoMapper;
using Notes.BLL.Models;
using Notes.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notes.Web.AutoMapperProfiles
{
    public class NoteViewModelMappingProfile : Profile
    {
        public NoteViewModelMappingProfile()
        {
            CreateMap<Note, NoteViewModel>();
            CreateMap<NoteViewModel, Note>();

            CreateMap<Note, NoteCreateViewModel>();
            CreateMap<NoteCreateViewModel, Note>();
        }
    }
}
