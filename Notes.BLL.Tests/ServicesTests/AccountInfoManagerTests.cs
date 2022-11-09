using Notes.BLL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notes.BLL.Tests.Helpers;
using Notes.DAL.Models;
using Moq;
using FluentAssertions;
using Notes.BLL.Services.NoteManagers;
using Notes.BLL.Services.NoteManagers.Models;
using Notes.BLL.Services.AccountInfoManagers;
using Notes.BLL.Services.NoteManagers.Exceptions;
using Notes.BLL.Services.CurrentUserAccessor.Exceptions;

namespace Notes.BLL.Tests.ServicesTests
{
    public class AccountInfoManagerTests
    {
        [Fact]
        public void Should_ReturnAccountInfo()
        {
            // Arrange
            var users = new List<UserEntry>() { new UserEntry() { UserName = "userName", Email = "userEmail@mail.com" } };

            var notesManagerMock = new Mock<INoteManager>();
            
            notesManagerMock.Setup(x => x.GetAllNotes())
                .Returns(new List<Note>() { new Note(), new Note(), new Note(), new Note(), new Note(), new Note(), new Note(), });

            INoteManager notesManager = notesManagerMock.Object;

            var userAccessor = DIHelper.CreateCurrentUserAccessor(users[0]);

            var userManager = DIHelper.CreateUserManager(users);

            IAccountInfoManager manager = new AccountInfoManager(userManager, notesManager, userAccessor);

            // Act

            var model = manager.GetAccountInfo();

            // Assert

            model.NotesCount.Should().Be(7);
            model.Email.Should().Be("userEmail@mail.com");
        }

        [Fact]
        public void Should_ThrowException_WhenUserNameIsWrong_GetAccountInfo()
        {
            // Arrange
            var users = new List<UserEntry>() { new UserEntry() { UserName = "userName", Email = "userEmail@mail.com" } };

            var notesManagerMock = new Mock<INoteManager>();

            notesManagerMock.Setup(x => x.GetAllNotes())
                .Returns(new List<Note>() { new Note(), new Note() });

            INoteManager notesManager = notesManagerMock.Object;

            var userManager = DIHelper.CreateUserManager(users);

            var userAccessor = DIHelper.CreateCurrentUserAccessor(new UserEntry() { UserName = "userName_sadmaslmd,;asmdlk" });

            IAccountInfoManager manager = new AccountInfoManager(userManager, notesManager, userAccessor);

            // Act

            Action act = () => manager.GetAccountInfo();

            // Assert

            act.Should().Throw<NotAuthorizedException>();
        }
    }
}
