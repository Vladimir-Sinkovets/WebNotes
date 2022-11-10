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
using Notes.BLL.Services.NoteManagers;
using Notes.BLL.Services.NoteManagers.Models;
using Notes.BLL.Services.NoteManagers.Exceptions;

namespace Notes.BLL.Tests.ServicesTests
{
    public class NoteManagerTests
    {
        [Fact]
        public async void Should_AddNote()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);
            var currentUser = new UserEntry() { UserName = "userName" };

            INoteManager noteManager = InitializeNoteManager(users, tags, notes, currentUser);

            // Act
            await noteManager.CreateNewNoteAsync(new NoteCreateData() { Text = "new text", Title = "new title" });

            // Assert
            notes.Should().Contain(n => n.Text == "new text" && n.Title == "new title");
        }

        [Fact]
        public void Should_ReturnAllNotes()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(users, tags, notes, new UserEntry() { UserName = "userName" });

            // Act

            var allNotes = noteManager.GetAllNotes();

            // Assert

            allNotes.Should().HaveCount(2);
        }

        [Fact]
        public void Should_ReturnNoteById()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            var unitOfWork = MockHelper.SetupUnitOfWork(tags, notes);
            var mapper = MockHelper.InitializeMapper(typeof(NoteMappingProfile));
            var userManager = MockHelper.SetupUserManager(users);
            var userAccessor = MockHelper.SetupCurrentUserAccessor(new UserEntry() { UserName = "userName" });

            INoteManager noteManager = new NoteManager(unitOfWork, userManager, mapper, userAccessor);

            // Act

            var note = noteManager.GetNoteById(noteId: 2);

            // Assert

            note.Should().BeEquivalentTo(new Note()
            {
                Tags = new List<Tag>()
                {
                    new Tag() { Id = 4, Notes = new List<Note>(), Name = "testTag" },
                    new Tag() { Id = 5, Notes = new List<Note>(), Name = "testTag1" },
                },
                Id = 2,
                Text = "2",
                Title = "2" 
            });
        }

        [Fact]
        public async void Should_UpdateNote()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(users, tags, notes, new UserEntry() { UserName = "userName" });

            // Act
            await noteManager.UpdateNoteAsync(new NoteUpdateData() { Id = 1, Text = "updated text", Title = "1"});

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

            INoteManager noteManager = InitializeNoteManager(users, tags, notes, new UserEntry() { UserName = "userName" });

            // Act
            noteManager.AddTagToNote(noteId: 1, 4);

            // Assert
            notes.FirstOrDefault(n => n.Id == 1).Tags.Should().HaveCount(2);
        }

        [Fact]
        public void Should_ReturnNoteTagsById()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(users, tags, notes, new UserEntry() { UserName = "userName" });

            // Act
            var returnedNoteTags = noteManager.GetNoteTagsById(noteId: 2);

            // Assert
            returnedNoteTags.Should().HaveCount(2);
        }

        [Fact]
        public void Should_RemoveTagFromNote()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(users, tags, notes, new UserEntry() { UserName = "userName" });

            // Act

            noteManager.RemoveTagFromNote(noteId: 2, tagId: 4);

            // Assert

            notes.FirstOrDefault(n => n.Id == 2).Tags.Should().HaveCount(1);
        }


        [Fact]
        public void Should_ThrowException_When_IdIsWrong_GetNoteById()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(users, tags, notes, new UserEntry() { UserName = "userName" });

            // Act
            Action act = () => noteManager.GetNoteById(noteId: 2222);

            // Assert

            act.Should().Throw<NotFoundException>().WithMessage("There is no such note");
        }

        [Fact]
        public void Should_ThrowException_WhenNoteParameterIsNull_UpdateNoteAsync()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(users, tags, notes, new UserEntry() { UserName = "userName" });

            // Act
            Func<Task> act = async () => await noteManager.UpdateNoteAsync(null);

            // Assert
            act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public void Should_ThrowException_WhenNoteIdIsWrong_AddTagToNote()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(users, tags, notes, new UserEntry() { UserName = "userName" });

            // Act
            Action act = () => noteManager.AddTagToNote(noteId: 1111, tagId: 4);

            // Assert
            act.Should().Throw<NotFoundException>();
        }

        [Fact]
        public void Should_ThrowException_WhenTagIdIsWrong_AddTagToNote()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(users, tags, notes, new UserEntry() { UserName = "userName" });

            // Act
            Action act = () => noteManager.AddTagToNote(noteId: 1, tagId: 1234);

            // Assert

            act.Should().Throw<NotFoundException>();
        }

        [Fact]
        public void Should_ThrowException_WhenIdIsWrong_GetNoteTagsById()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(users, tags, notes, new UserEntry() { UserName = "userName" });

            // Act

            Action act = () => noteManager.GetNoteTagsById(noteId: 22312);

            // Assert

            act.Should().Throw<NotFoundException>();
        }

        [Fact]
        public void Should_ThrowException_WhenNoteIdIsWrong_RemoveTagFromNote()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(users, tags, notes, new UserEntry() { UserName = "userName" });

            // Act

            Action act = () => noteManager.RemoveTagFromNote(noteId: 22212, tagId: 4);

            // Assert

            act.Should().Throw<NotFoundException>();
        }

        [Fact]
        public void Should_ThrowException_WhenTagIdIsWrong_RemoveTagFromNote()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(users, tags, notes, new UserEntry() { UserName = "userName" });

            // Act

            Action act = () => noteManager.RemoveTagFromNote(noteId: 2, tagId: 4213123);

            // Assert

            act.Should().Throw<NotFoundException>();
        }


        private static INoteManager InitializeNoteManager(List<UserEntry> users, List<TagEntry> tags,
            List<NoteEntry> notes, UserEntry currentUser)
        {
            var unitOfWork = MockHelper.SetupUnitOfWork(tags, notes);
            var userManager = MockHelper.SetupUserManager(users);
            var mapper = MockHelper.InitializeMapper(typeof(NoteMappingProfile));
            var userAccessor = MockHelper.SetupCurrentUserAccessor(currentUser);

            INoteManager noteManager = new NoteManager(unitOfWork, userManager, mapper, userAccessor);

            return noteManager;
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




        //[Fact]
        //public void Should_ThrowException_WhenUserNameIsWrong_GetNoteTagsByIdForUser()
        //{
        //    // Arrange
        //    var users = CreateUserList();
        //    var tags = CreateTagList(users);
        //    var notes = CreateNoteList(users, tags);

        //    INoteManager noteManager = InitializeNoteManager(users, tags, notes, new UserEntry() { UserName = "userName" });

        //    // Act

        //    Action act = () => noteManager.GetNoteTagsById(2);

        //    // Assert

        //    act.Should().Throw<NotFoundException>();
        //}
        //[Fact]
        //public void Should_ThrowException_WhenUserNameIsWrong_AddNoteForUserAsync()
        //{
        //    // Arrange
        //    var users = CreateUserList();
        //    var tags = CreateTagList(users);
        //    var notes = CreateNoteList(users, tags);

        //    INoteManager noteManager = InitializeNoteManager(users, tags, notes, new UserEntry() { UserName = "wrongUserName" });

        //    // Act
        //    Action act = () => noteManager.CreateNewNoteAsync(
        //        new NoteCreateData() { Text = "new text", Title = "new title" }).Wait();

        //    // Assert

        //    act.Should().Throw<NotFoundException>().WithMessage("User with this name does not exist");
        //}
    }
}
