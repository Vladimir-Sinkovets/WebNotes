﻿using AutoMapper;
using Notes.BLL.Services.NoteManagers.Enums;
using Notes.BLL.Services.NoteManagers.Models;
using Notes.Web.Enums;
using Notes.Web.Models.Note;
using System.Collections.Generic;

namespace Notes.Web.AutoMapperProfiles
{
    public class SearchFilterMappingProfile : Profile
    {
        public SearchFilterMappingProfile()
        {
            CreateMap<SearchFilterViewModel, SearchFilter>()
                .ConvertUsing((src, dest) =>
                {
                    return new SearchFilter
                    {
                        Importance = src.Importance switch
                        {
                            Importance.Important => ImportanceFilterUsing.Important,
                            Importance.Unimportant => ImportanceFilterUsing.Unimportant,
                            Importance.None => ImportanceFilterUsing.None,
                        },
                        Text = src.Text,
                        Title = src.Title,
                        MaxLength = src.MaxLength,
                        MinLength = src.MinLength,
                        UseLength = src.MaxLength == 0 && src.MinLength == 0 ? false : true,
                        Tags = src.Tags ?? new List<string>(),
                    };
                });
        }
    }
}
