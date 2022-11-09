using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using Notes.BLL.AutoMapperProfiles;
using Notes.BLL.Services;
using Notes.DAL.Models;
using Notes.DAL.Repositories.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notes.BLL.Tests.Helpers;
using Notes.BLL.Services.NoteManagers;
using Notes.BLL.Services.NoteManagers.Models;
using Notes.BLL.Services.NoteManagers.Exceptions;

namespace Notes.BLL.Tests.ServicesTests
{
    public class TagManagerTests
    {
        [Fact]
        public void Should_AddTag()
        {
            // Arrange
            var users = new List<UserEntry>()
            { 
                new UserEntry() { Email = "test@mail.com", UserName = "userName" }, 
                new UserEntry() { Email = "test1@mail.com", UserName = "userName1" }, 
                new UserEntry() { Email = "test2@mail.com", UserName = "userName2" }, 
            };
            var tags = new List<TagEntry>()
            {
                new TagEntry() { Name = "tag13231", User = users[1], Id = 0 },
            };

            var unitOfWork = DIHelper.CreateUnitOfWork(tags, null);

            var mapper = DIHelper.InitializeMapper(typeof(NoteMappingProfile));

            var userManager = DIHelper.CreateUserManager(users);

            var userAccessor = DIHelper.CreateCurrentUserAccessor(new UserEntry() { UserName = "userName" });

            INoteManager noteManager = new NoteManager(unitOfWork, userManager, mapper, userAccessor);

            // Act

            noteManager.AddTagAsync(new TagCreateData() { Name = "testName" }).Wait();

            // Assert

            tags.Should().Contain(tag => tag.Name == "testName" && tag.User.UserName == "userName");
        }

        [Fact] 
        public void Should_DeleteTagById()
        {
            // Arrange
            var users = new List<UserEntry>()
            {
                new UserEntry() { Email = "test@mail.com", UserName = "userName" },
                new UserEntry() { Email = "test1@mail.com", UserName = "userName1" },
            };
            var tags = new List<TagEntry>()
            {
                new TagEntry() { Name = "testTag", User = users[0], Id = 4 },
                new TagEntry() { Name = "testTag1", User = users[0], Id = 5 },
                new TagEntry() { Name = "testTag2", User = users[1], Id = 6 },
            };

            var unitOfWork = DIHelper.CreateUnitOfWork(tags, null);

            var mapper = DIHelper.InitializeMapper(typeof(NoteMappingProfile));

            var userManager = DIHelper.CreateUserManager(users);

            var userAccessor = DIHelper.CreateCurrentUserAccessor(new UserEntry() { UserName = "userName" });

            var noteManager = new NoteManager(unitOfWork, userManager, mapper, userAccessor);

            // Act

            noteManager.DeleteTagById(4);

            // Assert

            tags.Should().NotContain(tag => tag.Name == "testTag" && tag.User.UserName == "userName");
        }

        [Fact]
        public void Should_ReturnAllTagsForUser()
        {
            // Arrange
            var users = new List<UserEntry>() 
            {
                new UserEntry() { Email = "test@mail.com", UserName = "userName" },
                new UserEntry() { Email = "test1@mail.com", UserName = "userName1" },
            };
            var tags = new List<TagEntry>() 
            {
                new TagEntry() { Name = "testTag", User = users[0], Id = 4 },
                new TagEntry() { Name = "testTag1", User = users[0], Id = 5 },
                new TagEntry() { Name = "testTag2", User = users[1], Id = 6 },
            };

            var unitOfWork = DIHelper.CreateUnitOfWork(tags, null);
            var userManager = DIHelper.CreateUserManager(users);
            var mapper = DIHelper.InitializeMapper(typeof(NoteMappingProfile));

            var userAccessor = DIHelper.CreateCurrentUserAccessor(new UserEntry() { UserName = "userName" });

            var noteManager = new NoteManager(unitOfWork, userManager, mapper, userAccessor);

            // Act

            var allTags = noteManager.GetAllTagsFor();

            // Assert

            allTags.Count().Should().Be(2);
        }

        [Fact]
        public void Should_ReturnTagById()
        {
            // Arrange
            var users = new List<UserEntry>()
            {
                new UserEntry() { Email = "test@mail.com", UserName = "userName" },
                new UserEntry() { Email = "test1@mail.com", UserName = "userName1" },
            };
            var tags = new List<TagEntry>()
            {
                new TagEntry() { Name = "testTag", User = users[0], Id = 4 },
                new TagEntry() { Name = "testTag1", User = users[0], Id = 5 },
                new TagEntry() { Name = "testTag2", User = users[1], Id = 6 },
            };

            var unitOfWork = DIHelper.CreateUnitOfWork(tags, null);
            var userManager = DIHelper.CreateUserManager(users);
            var mapper = DIHelper.InitializeMapper(typeof(NoteMappingProfile));

            var userAccessor = DIHelper.CreateCurrentUserAccessor(new UserEntry() { UserName = "userName" });

            INoteManager noteManager = new NoteManager(unitOfWork, userManager, mapper, userAccessor);

            // Act

            var tag = noteManager.GetTagById(5);

            // Assert

            tag.Name.Should().Be("testTag1");
        }

        
        [Fact]
        public void Should_ThrowException_When_UserNameIsWrong_AddTagAsync()
        {
            // Arrange
            var users = new List<UserEntry>()
            {
                new UserEntry() { Email = "test@mail.com", UserName = "userName" },
                new UserEntry() { Email = "test1@mail.com", UserName = "userName1" },
                new UserEntry() { Email = "test2@mail.com", UserName = "userName2" },
            };
            var tags = new List<TagEntry>()
            {
                new TagEntry() { Name = "tag13231", User = users[1], Id = 0 },
            };

            var unitOfWork = DIHelper.CreateUnitOfWork(tags, null);

            var mapper = DIHelper.InitializeMapper(typeof(NoteMappingProfile));

            var userManager = DIHelper.CreateUserManager(users);

            var userAccessor = DIHelper.CreateCurrentUserAccessor(new UserEntry() { UserName = "wrongUserName"});

            INoteManager noteManager = new NoteManager(unitOfWork, userManager, mapper, userAccessor);

            // Act
            Action act = () => 
            { 
                noteManager.AddTagAsync(new TagCreateData() { Name = "testName" }).Wait();
            };

            // Assert

            act.Should().Throw<NotFoundException> ().WithMessage("User with this name does not exist");
        }

        [Fact]
        public void Should_ThrowException_When_TagAlreadyExists_AddTagAsync()
        {
            // Arrange
            var users = new List<UserEntry>()
            {
                new UserEntry() { Email = "test@mail.com", UserName = "userName" },
                new UserEntry() { Email = "test1@mail.com", UserName = "userName1" },
                new UserEntry() { Email = "test2@mail.com", UserName = "userName2" },
            };
            var tags = new List<TagEntry>()
            {
                new TagEntry() { Name = "tag13231", User = users[1], Id = 0 },
            };

            var unitOfWork = DIHelper.CreateUnitOfWork(tags, null);

            var mapper = DIHelper.InitializeMapper(typeof(NoteMappingProfile));

            var userManager = DIHelper.CreateUserManager(users);

            var userAccessor = DIHelper.CreateCurrentUserAccessor(new UserEntry() { UserName = "userName" });

            INoteManager noteManager = new NoteManager(unitOfWork, userManager, mapper, userAccessor);

            // Act
            Action act = () =>
            {
                noteManager.AddTagAsync(new TagCreateData() { Name = "tag13231" }).Wait();
            };

            // Assert

            act.Should().Throw<ExistedTagNameException>().WithMessage("Cannot add tag with already existing name");
        }

        [Fact]
        public void Should_ThrowException_When_UserNameIsWrong_GetAllTagsFor()
        {
            // Arrange
            var users = new List<UserEntry>()
            {
                new UserEntry() { Email = "test@mail.com", UserName = "userName" },
                new UserEntry() { Email = "test1@mail.com", UserName = "userName1" },
            };
            var tags = new List<TagEntry>()
            {
                new TagEntry() { Name = "testTag", User = users[0], Id = 4 },
                new TagEntry() { Name = "testTag1", User = users[0], Id = 5 },
                new TagEntry() { Name = "testTag2", User = users[1], Id = 6 },
            };

            var unitOfWork = DIHelper.CreateUnitOfWork(tags, null);

            var mapper = DIHelper.InitializeMapper(typeof(NoteMappingProfile));

            var userManager = DIHelper.CreateUserManager(users);

            var userAccessor = DIHelper.CreateCurrentUserAccessor(new UserEntry() { UserName = "userName21312" });

            var noteManager = new NoteManager(unitOfWork, userManager, mapper, userAccessor);

            // Act
            Action act = () =>
            {
                var allTags = noteManager.GetAllTagsFor();
            };

            // Assert

            act.Should()
               .ThrowExactly<NotFoundException>()
               .WithMessage("User with this name does not exist");
        }

        [Fact]
        public void Should_ThrowException_When_UserNameDoesNotExist_GetTagById()
        {
            // Arrange
            var users = new List<UserEntry>()
            {
                new UserEntry() { Email = "test@mail.com", UserName = "userName" },
                new UserEntry() { Email = "test1@mail.com", UserName = "userName1" },
            };
            var tags = new List<TagEntry>()
            {
                new TagEntry() { Name = "testTag", User = users[0], Id = 4 },
                new TagEntry() { Name = "testTag1", User = users[0], Id = 5 },
                new TagEntry() { Name = "testTag2", User = users[1], Id = 6 },
            };

            var unitOfWork = DIHelper.CreateUnitOfWork(tags, null);

            var mapper = DIHelper.InitializeMapper(typeof(NoteMappingProfile));

            var userManager = DIHelper.CreateUserManager(users);

            var userAccessor = DIHelper.CreateCurrentUserAccessor(new UserEntry() { UserName = "userName" });

            var noteManager = new NoteManager(unitOfWork, userManager, mapper, userAccessor);

            // Act
            Action act = () =>
            {
                noteManager.GetTagById(4);
            };

            // Assert

            act.Should()
               .Throw<NotFoundException>()
               .WithMessage("User with this name does not exist");
        }

        [Fact]
        public void Should_ThrowException_When_TagIdIsWrong_GetTagById()
        {

            // Arrange
            var users = new List<UserEntry>()
            {
                new UserEntry() { Email = "test@mail.com", UserName = "userName" },
                new UserEntry() { Email = "test1@mail.com", UserName = "userName1" },
            };
            var tags = new List<TagEntry>()
            {
                new TagEntry() { Name = "testTag", User = users[0], Id = 4 },
                new TagEntry() { Name = "testTag1", User = users[0], Id = 5 },
                new TagEntry() { Name = "testTag2", User = users[1], Id = 6 },
            };

            var unitOfWork = DIHelper.CreateUnitOfWork(tags, null);

            var mapper = DIHelper.InitializeMapper(typeof(NoteMappingProfile));

            var userManager = DIHelper.CreateUserManager(users);

            var userAccessor = DIHelper.CreateCurrentUserAccessor(new UserEntry() { UserName = "userName" });

            var noteManager = new NoteManager(unitOfWork, userManager, mapper, userAccessor);

            // Act
            Action act = () =>
            {
                noteManager.GetTagById(7);
            };

            // Assert

            act.Should().Throw<NotFoundException>().WithMessage("This tag does not exist");
        }
    }
}