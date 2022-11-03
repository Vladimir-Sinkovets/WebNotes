using Notes.BLL.Interfaces;
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
using Notes.BLL.Models;

namespace Notes.BLL.Tests.ServicesTests
{
    public class AccountInfoManagerTests
    {
        [Fact]
        public void Should_ReturnAccountInfo()
        {
            // Arrange
            var users = new List<UserEntry>() { new UserEntry() { UserName = "userName", Email = "userEmail@mail.com" } };

            var notesManagerMock = new Mock<INotesManager>();
            
            notesManagerMock.Setup(x => x.GetAllNotesFor(It.IsAny<string>()))
                .Returns(new List<Note>() { new Note(), new Note(), new Note(), new Note(), new Note(), new Note(), new Note(), });

            INotesManager notesManager = notesManagerMock.Object;

            var userManager = DIHelper.CreateUserManager(users);

            IAccountInfoManager manager = new AccountInfoManager(userManager, notesManager);

            // Act

            var model = manager.GetAccountInfo("userName");

            // Assert

            model.NotesCount.Should().Be(7);
            model.Email.Should().Be("userEmail@mail.com");
        }

        [Fact]
        public void Should_ThrowException_WhenUserNameIsWrong_GetAccountInfo()
        {
            // Arrange
            var users = new List<UserEntry>() { new UserEntry() { UserName = "userName", Email = "userEmail@mail.com" } };

            var notesManagerMock = new Mock<INotesManager>();

            notesManagerMock.Setup(x => x.GetAllNotesFor(It.IsAny<string>()))
                .Returns(new List<Note>() { new Note(), new Note() });

            INotesManager notesManager = notesManagerMock.Object;

            var userManager = DIHelper.CreateUserManager(users);

            IAccountInfoManager manager = new AccountInfoManager(userManager, notesManager);

            // Act

            Action act = () => manager.GetAccountInfo("userName_sadmaslmd,;asmdlk");

            // Assert

            act.Should().Throw<NotFoundException>().WithMessage("User with this name does not exist");
        }
    }
}
