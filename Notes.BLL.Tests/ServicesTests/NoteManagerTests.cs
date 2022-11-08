using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notes.BLL.AutoMapperProfiles;
using Notes.BLL.Services;
using Notes.BLL.Tests.Helpers;
using Notes.DAL.Models;
using FluentAssertions;
using static System.Net.Mime.MediaTypeNames;
using Notes.BLL.Exceptions;
using Notes.BLL.Services.NoteManagers;
using Notes.BLL.Services.NoteManagers.Models;

namespace Notes.BLL.Tests.ServicesTests
{
    public class NoteManagerTests
    {
        [Fact]
        public void Should_AddNote()
        {
            // Arrange
            List<UserEntry> users = CreateUserList();
            List<TagEntry> tags = CreateTagList(users);
            List<NoteEntry> notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(users, tags, notes);

            // Act

            noteManager.AddNoteForUserAsync(new Note() { Text = "new text", Title = "new title" }, "userName").Wait();

            // Assert

            notes.Should().Contain(n => n.Text == "new text" && n.Title == "new title");
        }

        private static INoteManager InitializeNoteManager(List<UserEntry> users, List<TagEntry> tags, List<NoteEntry> notes)
        {
            var unitOfWork = DIHelper.CreateUnitOfWork(tags, notes);

            var mapper = DIHelper.InitializeMapper(typeof(NoteMappingProfile));

            var userManager = DIHelper.CreateUserManager(users);

            INoteManager noteManager = new NoteManager(unitOfWork, userManager, mapper);
            return noteManager;
        }

        [Fact]
        public void Should_ReturnAllNotesForUser()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(users, tags, notes);

            // Act

            var allNotes = noteManager.GetAllNotesForUser("userName");

            // Assert

            allNotes.Should().HaveCount(2);
        }

        [Fact]
        public void Should_ReturnNoteByIdAndUserName()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            var unitOfWork = DIHelper.CreateUnitOfWork(tags, notes);

            var mapper = DIHelper.InitializeMapper(typeof(NoteMappingProfile));

            var userManager = DIHelper.CreateUserManager(users);

            INoteManager noteManager = new NoteManager(unitOfWork, userManager, mapper);

            // Act

            var note = noteManager.GetNoteByIdForUser(2, "userName");

            // Assert

            note.Should().BeEquivalentTo(new Note()
            {
                Tags = new List<Tag>()
                {
                    new Tag() { Id = 4, Notes = new List<Note>(), Name = "testTag" },
                    new Tag() { Id = 5, Notes = new List<Note>(), Name = "testTag1" },
                },
            Id = 2, Text = "2", Title = "2" });
        }

        [Fact]
        public void Should_UpdateNote()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(users, tags, notes);


            // Act

            noteManager.UpdateAsync(new Note() { Id = 1, Text = "updated text", Title = "1" }).Wait();

            // Assert
            var note = notes.FirstOrDefault(n => n.Id == 1);

            note.Title.Should().Be("1");
            note.Text.Should().Be("updated text");
            note.Id.Should().Be(1);
        }

        [Fact]
        public void Should_AddTagToNote()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(users, tags, notes);

            // Act
            noteManager.AddTagToNoteForUser(1, 4, "userName");

            // Assert
            notes.FirstOrDefault(n => n.Id == 1).Tags.Should().HaveCount(2);
        }

        [Fact]
        public void Should_ReturnNoteTagsByIdAndUserName()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(users, tags, notes);

            // Act
            var noteTags = noteManager.GetNoteTagsByIdForUser(2, "userName");

            // Assert
            noteTags.Should().HaveCount(2);
        }

        [Fact]
        public void Should_RemoveTagFromNote()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(users, tags, notes);

            // Act

            noteManager.RemoveTagFromNoteForUser(2, 4, "userName");

            // Assert

            notes[1].Tags.Should().HaveCount(1);
        }


        [Fact]
        public void Should_ThrowException_WhenUserNameIsWrong_AddNoteForUserAsync()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(users, tags, notes);

            // Act
            Action act = () => noteManager.AddNoteForUserAsync(new Note() { Text = "new text", Title = "new title" }, "wrongUserName").Wait();

            // Assert

            act.Should().Throw<NotFoundException>().WithMessage("User with this name does not exist");
        }

        [Fact]
        public void Should_ThrowException_When_IdIsWrong_GetNoteByIdForUser()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(users, tags, notes);

            // Act
            Action act = () => noteManager.GetNoteByIdForUser(2222, "userName");

            // Assert

            act.Should().Throw<NotFoundException>().WithMessage("There is no such note");
        }

        [Fact]
        public void Should_ThrowException_WhenNoteParameterIsNull_UpdateAsync()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(users, tags, notes);

            // Act

            Action act = () => noteManager.UpdateAsync(null).Wait();

            // Assert

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Should_ThrowException_WhenNoteIdIsWrong_AddTagToNoteForUser()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(users, tags, notes);

            // Act
            Action act = () => noteManager.AddTagToNoteForUser(1111, 4, "userName");

            // Assert

            act.Should().Throw<NotFoundException>();
        }

        [Fact]
        public void Should_ThrowException_WhenTagIdIsWrong_AddTagToNoteForUser()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(users, tags, notes);

            // Act
            Action act = () => noteManager.AddTagToNoteForUser(1, 1234, "userName");

            // Assert

            act.Should().Throw<NotFoundException>();
        }

        [Fact]
        public void Should_ThrowException_WhenIdIsWrong_GetNoteTagsByIdForUser()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(users, tags, notes);

            // Act

            Action act = () => noteManager.GetNoteTagsByIdForUser(22312, "userName");

            // Assert

            act.Should().Throw<NotFoundException>();
        }

        [Fact]
        public void Should_ThrowException_WhenUserNameIsWrong_GetNoteTagsByIdForUser()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(users, tags, notes);

            // Act

            Action act = () => noteManager.GetNoteTagsByIdForUser(2, "wrongUserName");

            // Assert

            act.Should().Throw<NotFoundException>();
        }

        [Fact]
        public void Should_ThrowException_WhenNoteIdIsWrong_RemoveTagFromNoteForUser()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(users, tags, notes);

            // Act

            Action act = () => noteManager.RemoveTagFromNoteForUser(22212, 4, "userName");

            // Assert

            act.Should().Throw<NotFoundException>();
        }

        [Fact]
        public void Should_ThrowException_WhenTagIdIsWrong_RemoveTagFromNoteForUser()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(users, tags, notes);

            // Act

            Action act = () => noteManager.RemoveTagFromNoteForUser(2, 4213123, "userName");

            // Assert

            act.Should().Throw<NotFoundException>();
        }


        private static List<UserEntry> CreateUserList()
        {
            return new List<UserEntry>()
            {
                new UserEntry() { Email = "test@mail.com", UserName = "userName" },
                new UserEntry() { Email = "test1@mail.com", UserName = "userName1" },
            };
        }

        private static List<TagEntry> CreateTagList(List<UserEntry> users)
        {
            return new List<TagEntry>()
            {
                new TagEntry() { Name = "testTag", User = users[0], Id = 4 },
                new TagEntry() { Name = "testTag1", User = users[0], Id = 5 },
                new TagEntry() { Name = "testTag2", User = users[1], Id = 6 },
            };
        }

        private static List<NoteEntry> CreateNoteList(List<UserEntry> users, List<TagEntry> tags)
        {
            return new List<NoteEntry>()
            {
                new NoteEntry() { Tags = new List<TagEntry>() { tags[0], }, Id = 1, Text = "1", Title = "1", User = users[0]},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[0], tags[1] }, Id = 2, Text = "2", Title = "2", User = users[0]},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2], }, Id = 3, Text = "3", Title = "3", User = users[1]},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2], }, Id = 4, Text = "4", Title = "4", User = users[1]},
            };
        }
    }
}
