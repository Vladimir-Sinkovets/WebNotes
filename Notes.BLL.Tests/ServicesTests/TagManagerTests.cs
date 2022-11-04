using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using Notes.BLL.AutoMapperProfiles;
using Notes.BLL.Interfaces;
using Notes.BLL.Models;
using Notes.BLL.Services;
using Notes.DAL.Models;
using Notes.DAL.Repositories.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notes.BLL.Tests.Helpers;

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

            ITagManager tagManager = new TagManager(unitOfWork, mapper, userManager);

            // Act

            tagManager.AddTagAsync(new Tag() { Name = "testName" }, "userName").Wait();

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

            var tagManager = new TagManager(unitOfWork, mapper, userManager);

            // Act

            tagManager.DeleteTagById(4, "userName");

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

            var tagManager = new TagManager(unitOfWork, mapper, userManager);

            // Act

            var allTags = tagManager.GetAllTagsFor("userName");

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

            var tagManager = new TagManager(unitOfWork, mapper, userManager);

            // Act

            var tag = tagManager.GetTagById(5, "userName");

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

            ITagManager tagManager = new TagManager(unitOfWork, mapper, userManager);

            // Act
            Action act = () => 
            { 
                tagManager.AddTagAsync(new Tag() { Name = "testName" }, "wrongUserName").Wait();
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

            ITagManager tagManager = new TagManager(unitOfWork, mapper, userManager);

            // Act
            Func<Task> act = async () =>
            {
                await tagManager.AddTagAsync(new Tag() { Name = "tag13231" }, "userName");
            };

            // Assert

            act.Should().ThrowAsync<ArgumentException>().WithMessage("Wrong user name");
        }


        [Fact]
        public void Should_ThrowException_When_TagIdIsWrong_DeleteTagById()
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

            var tagManager = new TagManager(unitOfWork, mapper, userManager);
            
            // Act
            Action act = () =>
            {
                tagManager.DeleteTagById(7, "userName");
            };

            // Assert

            act.Should().Throw<NotFoundException>().WithMessage("This tag does not exist");
        }

        [Fact]
        public void Should_ThrowException_When_UserHaveNoAccessToTag_DeleteTagById()
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

            var tagManager = new TagManager(unitOfWork, mapper, userManager);

            // Act
            Action act = () =>
            {
                tagManager.DeleteTagById(4, "userName1");
            };

            // Assert

            act.Should().Throw<UserAccessException>().WithMessage("This user does not have access to this tag");
        }

        [Fact]
        public void Should_ThrowException_When_UserNameDoesNotExist_DeleteTagById()
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

            var tagManager = new TagManager(unitOfWork, mapper, userManager);

            // Act
            Action act = () =>
            {
                tagManager.DeleteTagById(4, "userName3333333");
            };

            // Assert

            act.Should()
               .Throw<NotFoundException>()
               .WithMessage("User with this name does not exist");
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

            var tagManager = new TagManager(unitOfWork, mapper, userManager);

            // Act
            Action act = () =>
            {
                var allTags = tagManager.GetAllTagsFor("userName21312");
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

            var tagManager = new TagManager(unitOfWork, mapper, userManager);

            // Act
            Action act = () =>
            {
                tagManager.GetTagById(4, "userName3333333");
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

            var tagManager = new TagManager(unitOfWork, mapper, userManager);

            // Act
            Action act = () =>
            {
                tagManager.GetTagById(7, "userName");
            };

            // Assert

            act.Should().Throw<NotFoundException>().WithMessage("This tag does not exist");
        }
    }
}