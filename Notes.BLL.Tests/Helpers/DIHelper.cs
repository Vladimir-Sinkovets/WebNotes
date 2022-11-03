using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using Notes.BLL.AutoMapperProfiles;
using Notes.DAL.Models;
using Notes.DAL.Repositories.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Notes.BLL.Tests.Helpers
{
    internal static class DIHelper
    {
        public static IMapper InitializeMapper(params Type[] types)
        {
            IEnumerable<Profile> profiles = types.Select(t => (Profile)Activator.CreateInstance(t));

            var profile = new NoteMappingProfile();

            var conf = new MapperConfiguration(cfg => cfg.AddProfiles(profiles));

            var mapper = new Mapper(conf);

            return mapper;
        } // remove to another class

        public static UserManager<TUser> CreateUserManager<TUser>(List<TUser> users) where TUser : IdentityUser
        {
            var userManagerMock = new Mock<UserManager<TUser>>(Mock.Of<IUserStore<TUser>>(), null, null, null, null, null, null, null, null);

            userManagerMock.Setup(x => x.CreateAsync(It.IsAny<TUser>()))
                .Returns<TUser>(u =>
                {
                    users.Add(u);
                    return Task.FromResult(IdentityResult.Success);
                });

            userManagerMock.Setup(x => x.Users)
                .Returns(users.AsQueryable());

            userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .Returns<string>(n => {
                    return Task.FromResult(users.FirstOrDefault(u => u.UserName == n));
                });


            return userManagerMock.Object;
        }

        public static IUnitOfWork CreateUnitOfWork(List<TagEntry> tags, List<NoteEntry> notes)
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(unit => unit.Tags.Create(It.IsAny<TagEntry>()))
                .Callback((TagEntry t) => tags.Add(t));

            unitOfWorkMock.Setup(unit => unit.Tags.GetAll())
                .Returns(() => tags.AsQueryable());

            unitOfWorkMock.Setup(unit => unit.Tags.DeleteById(It.IsAny<int>()))
                .Callback<int>(id => tags.Remove(tags.FirstOrDefault(t => t.Id == id)));


            unitOfWorkMock.Setup(unit => unit.Notes.Create(It.IsAny<NoteEntry>()))
                .Callback((NoteEntry n) => notes.Add(n));

            unitOfWorkMock.Setup(unit => unit.Notes.GetAll())
                .Returns(() => notes.AsQueryable());

            unitOfWorkMock.Setup(unit => unit.Notes.DeleteById(It.IsAny<int>()))
                .Callback<int>(id => notes.Remove(notes.FirstOrDefault(n => n.Id == id)));

            unitOfWorkMock.Setup(unit => unit.Notes.Update(It.IsAny<NoteEntry>()))
                .Callback<NoteEntry>(noteEntry => 
                {
                    notes.Remove(notes.FirstOrDefault(n => n.Id == noteEntry.Id));
                    notes.Add(noteEntry);
                });


            var unitOfWork = unitOfWorkMock.Object;
            return unitOfWork;
        }
    }
}
