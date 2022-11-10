using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using Notes.BLL.Services.CurrentUserAccessor;
using Notes.DAL.Models;
using Notes.Web.Services;

namespace Notes.Web.Tests.Services
{
    public class CurrentUserAccessorTests
    {
        [Fact]
        public void Should_ReturnCurrentUser()
        {
            // Arrange
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            httpContextAccessorMock.Setup(x => x.HttpContext.User.Identity.Name)
                .Returns("userName");

            var users = new List<UserEntry>() { new UserEntry() { UserName = "userName" } };

            var userManagerMock = new Mock<UserManager<UserEntry>>(Mock.Of<IUserStore<UserEntry>>(), null, null, null, null, null, null, null, null);

            userManagerMock.Setup(x => x.Users)
                .Returns(users.AsQueryable());

            ICurrentUserAccessor userAccessor = new CurrentUserAccessor(httpContextAccessorMock.Object, userManagerMock.Object);

            // Act
            var returnedUser = userAccessor.Current;

            // Assert
            returnedUser.Should().BeEquivalentTo(users.First());
        }
    }
}