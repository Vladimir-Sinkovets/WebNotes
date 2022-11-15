using AutoMapper;
using Notes.BLL.Services.NoteManagers.Models;
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
            CreateMap<Note, ListNoteItemViewModel>();
            CreateMap<ListNoteItemViewModel, Note>();

            CreateMap<Note, NoteCreateViewModel>();
            CreateMap<NoteCreateViewModel, Note>();

            CreateMap<NoteCreateViewModel, NoteCreateData>();

            CreateMap<NoteUpdateData, EditNoteViewModel>();
            CreateMap<EditNoteViewModel, NoteUpdateData>();

            CreateMap<Note, EditNoteViewModel>();

        }
    }
}
