using Notes.BLL.AutoMapperProfiles;
using Notes.BLL.Tests.Helpers;
using Notes.DAL.Models;
using FluentAssertions;
using Notes.BLL.Services.NoteManagers;
using Notes.BLL.Services.NoteManagers.Models;
using Notes.BLL.Services.NoteManagers.Exceptions;
using Notes.BLL.Services.NoteManagers.Enums;

namespace Notes.BLL.Tests.ServicesTests
{
    public class NoteManagerTests
    {
        #region CreateNewNoteAsync
        [Fact]
        public async void Should_CreateNewNoteAsync()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);
            var currentUser = users[0];

            INoteManager noteManager = InitializeNoteManager(tags, notes, currentUser);

            // Act
            await noteManager.CreateNewNoteAsync(new NoteCreateData() { Text = "new text", Title = "new title" });

            // Assert
            notes.Should().Contain(n => n.Text == "new text" && n.Title == "new title");
        }
        #endregion

        #region GetAllNotes
        [Fact]
        public void Should_ReturnAllNotes_GetAllNotes()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(tags, notes, users[0]);

            // Act

            var allNotes = noteManager.GetAllNotes();

            // Assert

            allNotes.Should().HaveCount(2);
        }
        #endregion

        #region GetNoteTagsById
        [Fact]
        public void Should_ThrowException_WhenNoteWithThisIdDoesNotExist_GetNoteTagsById()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(tags, notes, users[0]);

            // Act
            Action act = () => noteManager.GetNoteTagsById(noteId: 22312);

            // Assert
            act.Should().Throw<NotFoundException>();
        }

        [Fact]
        public void Should_ThrowException_WhenUserHaveNoAccessToNoteWithThisId_GetNoteTagsById()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(tags, notes, users[0]);

            // Act
            Action act = () => noteManager.GetNoteTagsById(noteId: 4);

            // Assert
            act.Should().Throw<NotFoundException>();
        }
        
        [Fact]
        public void Should_ReturnNoteTagsById_GetNoteTagsById()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(tags, notes, users[0]);

            // Act
            var returnedNoteTags = noteManager.GetNoteTagsById(noteId: 2);

            // Assert
            returnedNoteTags.Should().HaveCount(2);
        }
        #endregion

        #region GetTagById
        [Fact]
        public void Should_ReturnTagById()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(tags, notes, users[0]);

            // Act
            var tag = noteManager.GetTagById(tagId: 5);

            // Assert
            tag.Name.Should().Be("testTag1");
        }
        #endregion

        #region GetAllByFilter
        [Fact]
        public void Should_ReturnNotesByFilterWithTitle()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = new List<NoteEntry>()
            {
                new NoteEntry() { Tags = new List<TagEntry>() { tags[0], }, Id = 1, Text = "1", Title = "1", User = users[0], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[0], }, Id = 2, Text = "1", Title = "title", User = users[0], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[0], }, Id = 3, Text = "1", Title = "1", User = users[0], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[0], tags[1] }, Id = 4, Text = "2", Title = "title", User = users[0], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2], }, Id = 5, Text = "3", Title = "title", User = users[1], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2], }, Id = 6, Text = "3", Title = "3", User = users[1], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2], }, Id = 7, Text = "3", Title = "3", User = users[1], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2], }, Id = 8, Text = "4", Title = "title", User = users[1], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2], }, Id = 9, Text = "3", Title = "3", User = users[1], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2], }, Id = 10, Text = "3", Title = "3", User = users[1], IsImportant = false,},
            };

            INoteManager noteManager = InitializeNoteManager(tags, notes, users[0]);

            // Act
            var filteredNotes = noteManager.GetAllByFilter(new SearchFilter()
            {
                Title = "title",
                Importance = ImportanceFilterUsing.None,
                MaxLength = 0,
                MinLength = 0,
                Tags = new List<string>(),
                Text = null,
            });

            // Assert
            filteredNotes.Should().HaveCount(2);
        }

        [Fact]
        public void Should_ReturnNotesByFilterWithText()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = new List<NoteEntry>()
            {
                new NoteEntry() { Tags = new List<TagEntry>() { tags[0],         }, Id = 1, Text = "text", Title = "1", User = users[0], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[0],         }, Id = 2, Text = "text", Title = "title", User = users[0], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[0],         }, Id = 3, Text = "1", Title = "1", User = users[0], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[0], tags[1] }, Id = 4, Text = "text", Title = "title", User = users[0], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2],         }, Id = 5, Text = "3", Title = "title", User = users[1], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2],         }, Id = 6, Text = "3", Title = "3", User = users[1], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2],         }, Id = 7, Text = "text", Title = "3", User = users[1], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2],         }, Id = 8, Text = "4", Title = "title", User = users[1], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2],         }, Id = 9, Text = "3", Title = "3", User = users[1], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2],         }, Id =10, Text = "text", Title = "3", User = users[1], IsImportant = false,},
            };

            INoteManager noteManager = InitializeNoteManager(tags, notes, users[0]);

            // Act
            var filteredNotes = noteManager.GetAllByFilter(new SearchFilter()
            {
                Title = null,
                Importance = ImportanceFilterUsing.None,
                UseMaxLength = false,
                UseMinLength = false,
                MaxLength = 0,
                MinLength = 0,
                Tags = new List<string>(),
                Text = "text",
            });

            // Assert
            filteredNotes.Should().HaveCount(3);
        }

        [Fact]
        public void Should_ReturnNotesByFilterWithTags()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = new List<NoteEntry>()
            {
                new NoteEntry() { Tags = new List<TagEntry>() { tags[0],         }, Id = 1, Text = "text", Title = "1", User = users[0], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[0], tags[1] }, Id = 2, Text = "text", Title = "title", User = users[0], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[0], tags[1] }, Id = 3, Text = "1", Title = "1", User = users[0], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[0], tags[1] }, Id = 4, Text = "text", Title = "title", User = users[0], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2],         }, Id = 5, Text = "3", Title = "title", User = users[1], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2],         }, Id = 6, Text = "3", Title = "3", User = users[1], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2],         }, Id = 7, Text = "text", Title = "3", User = users[1], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2],         }, Id = 8, Text = "4", Title = "title", User = users[1], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2],         }, Id = 9, Text = "3", Title = "3", User = users[1], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2],         }, Id =10, Text = "text", Title = "3", User = users[1], IsImportant = false,},
            };

            INoteManager noteManager = InitializeNoteManager(tags, notes, users[0]);

            // Act
            var filteredNotes = noteManager.GetAllByFilter(new SearchFilter()
            {
                Title = null,
                Text = null,
                Importance = ImportanceFilterUsing.None,
                Tags = new List<string>() { "testTag", "testTag1" },
                UseMaxLength = false,
                UseMinLength = false,
                MaxLength = 0,
                MinLength = 0,
            });

            // Assert
            filteredNotes.Should().HaveCount(3);
        }

        [Fact]
        public void Should_ReturnNotesByFilterWithMinAndMaxLength()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = new List<NoteEntry>()
            {
                new NoteEntry() { Tags = new List<TagEntry>() { tags[0],         }, Id = 1, Text = "text", Title = "1", User = users[0], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[0], tags[1] }, Id = 2, Text = "text", Title = "title", User = users[0], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[0], tags[1] }, Id = 3, Text = "text", Title = "1", User = users[0], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[0], tags[1] }, Id = 4, Text = "text", Title = "title", User = users[0], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2],         }, Id = 5, Text = "1", Title = "title", User = users[1], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2],         }, Id = 6, Text = "22", Title = "3", User = users[1], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2],         }, Id = 7, Text = "333", Title = "3", User = users[1], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2],         }, Id = 8, Text = "4444", Title = "title", User = users[1], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2],         }, Id = 9, Text = "55555", Title = "3", User = users[1], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2],         }, Id =10, Text = "666666", Title = "3", User = users[1], IsImportant = false,},
            };

            INoteManager noteManager = InitializeNoteManager( tags, notes, users[1]);

            // Act
            var filteredNotes = noteManager.GetAllByFilter(new SearchFilter()
            {
                Title = null,
                Text = null,
                Importance = ImportanceFilterUsing.None,
                Tags = new List<string>(),
                UseMaxLength = true,
                UseMinLength = true,
                MinLength = 2,
                MaxLength = 4,
            });

            // Assert
            filteredNotes.Should().HaveCount(3);
        }

        [Fact]
        public void Should_ReturnNotesByFilterWithImportance()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = new List<NoteEntry>()
            {
                new NoteEntry() { Tags = new List<TagEntry>() { tags[0],         }, Id = 1, Text = "text", Title = "1", User = users[0],     IsImportant = true, },
                new NoteEntry() { Tags = new List<TagEntry>() { tags[0], tags[1] }, Id = 2, Text = "text", Title = "title", User = users[0], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[0], tags[1] }, Id = 3, Text = "text", Title = "1", User = users[0],     IsImportant = true, },
                new NoteEntry() { Tags = new List<TagEntry>() { tags[0], tags[1] }, Id = 4, Text = "text", Title = "title", User = users[0], IsImportant = true, },
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2],         }, Id = 5, Text = "1", Title = "title", User = users[1],    IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2],         }, Id = 6, Text = "22", Title = "3", User = users[1],       IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2],         }, Id = 7, Text = "333", Title = "3", User = users[1],      IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2],         }, Id = 8, Text = "4444", Title = "title", User = users[1], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2],         }, Id = 9, Text = "55555", Title = "3", User = users[1],    IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2],         }, Id =10, Text = "666666", Title = "3", User = users[1],   IsImportant = false,},
            };

            INoteManager noteManager = InitializeNoteManager(tags, notes, users[0]);

            // Act
            var filteredNotes = noteManager.GetAllByFilter(new SearchFilter()
            {
                Title = null,
                Text = null,
                Importance = ImportanceFilterUsing.Important,
                Tags = new List<string>(),
                UseMaxLength = false,
                UseMinLength = false,
                MinLength = 0,
                MaxLength = 0,
            });

            // Assert
            filteredNotes.Should().HaveCount(3);
        }

        [Fact]
        public void Should_ReturnNotesByFilter()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = new List<NoteEntry>()
            {
                new NoteEntry() { Tags = new List<TagEntry>() { tags[0],         }, Id = 1, Text = "text1", Title = "1", User = users[0],     IsImportant = true, },
                new NoteEntry() { Tags = new List<TagEntry>() { tags[0], tags[1] }, Id = 2, Text = "text2", Title = "title", User = users[0], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[0], tags[1] }, Id = 3, Text = "text3", Title = "1", User = users[0],     IsImportant = true, },
                new NoteEntry() { Tags = new List<TagEntry>() { tags[0], tags[1] }, Id = 4, Text = "text42123123", Title = "title", User = users[0], IsImportant = false, },
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2],         }, Id = 5, Text = "1", Title = "title", User = users[1],    IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2],         }, Id = 6, Text = "22", Title = "3", User = users[1],       IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2],         }, Id = 7, Text = "333", Title = "3", User = users[1],      IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2],         }, Id = 8, Text = "4444", Title = "title", User = users[1], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2],         }, Id = 9, Text = "55555", Title = "3", User = users[1],    IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2],         }, Id =10, Text = "666666", Title = "3", User = users[1],   IsImportant = false,},
            };

            INoteManager noteManager = InitializeNoteManager(tags, notes, users[0]);

            // Act
            var filteredNotes = noteManager.GetAllByFilter(new SearchFilter()
            {
                Title = null,
                Text = "text",
                Importance = ImportanceFilterUsing.Unimportant,
                Tags = new List<string>() { "testTag" },
                UseMaxLength = true,
                UseMinLength = true,
                MinLength = 4,
                MaxLength = 8,
            });

            // Assert
            filteredNotes.Should().HaveCount(1);
        }
        #endregion

        #region GetTagById
        [Fact]
        public void Should_ThrowException_When_TagDoesNotExist_GetTagById()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(tags, notes, users[0]);

            // Act
            Action act = () => noteManager.GetTagById(tagId: 77777);

            // Assert
            act.Should().Throw<NotFoundException>().WithMessage("This tag does not exist");
        }
        #endregion

        #region GetAllTags
        [Fact]
        public void Should_ReturnAllTags()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(tags, notes, users[0]);

            // Act
            var allTags = noteManager.GetAllTags();

            // Assert
            allTags.Should().HaveCount(2);
        }
        #endregion

        #region GetNoteById
        [Fact]
        public void Should_ReturnNoteById_GetNoteById()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(tags, notes, users[0]);

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
                Title = "2",
            });
        }

        [Fact]
        public void Should_ThrowException_WhenNoteWithThisIdDoesNotExist_GetNoteById()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(tags, notes, users[0]);

            // Act
            Action act = () => noteManager.GetNoteById(noteId: 2222);

            // Assert
            act.Should().Throw<NotFoundException>().WithMessage("This note does not exist");
        }

        [Fact]
        public void Should_ThrowException_WhenUserHaveNoAccessToNoteWithThisId_GetNoteById()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(tags, notes, users[0]);

            // Act
            Action act = () => noteManager.GetNoteById(noteId: 3);

            // Assert
            act.Should().Throw<NotFoundException>();
        }
        #endregion

        #region AddTagToNote

        [Fact]
        public void Should_AddTagToNote()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(tags, notes, users[0]);

            // Act
            noteManager.AddTagToNote(noteId: 1, tagId: 4);

            // Assert
            _ = notes.FirstOrDefault(n => n.Id == 1).Tags.Should().HaveCount(2);
        }

        [Fact]
        public void Should_ThrowException_WhenUserHaveNoAccessToNoteWithThisId_AddTagToNote()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(tags, notes, users[0]);

            // Act
            Action act = () => noteManager.AddTagToNote(noteId: 3, tagId: 4);

            // Assert
            act.Should().Throw<NotFoundException>();
        }

        [Fact]
        public void Should_ThrowException_WhenUserHaveNoAccessToTagWithThisId_AddTagToNote()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(tags, notes, users[0]);

            // Act
            Action act = () => noteManager.AddTagToNote(noteId: 3, tagId: 6);

            // Assert
            act.Should().Throw<NotFoundException>();
        }

        [Fact]
        public void Should_ThrowException_WhenNoteWithThisIdDoesNotExist_AddTagToNote()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(tags, notes, users[0]);

            // Act
            Action act = () => noteManager.AddTagToNote(noteId: 213123, tagId: 4);

            // Assert
            act.Should().Throw<NotFoundException>();
        }

        [Fact]
        public void Should_ThrowException_WhenTagWithThisIdDoesNotExist_AddTagToNote()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(tags, notes, users[0]);

            // Act
            Action act = () => noteManager.AddTagToNote(noteId: 2, tagId: 213123);

            // Assert
            act.Should().Throw<NotFoundException>();
        }
        #endregion

        #region AddTagAsync
        [Fact]
        public async void Should_AddTagAsync()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(tags, notes, users[0]);

            // Act
            await noteManager.AddTagAsync(new TagCreateData() { Name = "testName" });

            // Assert
            tags.Should().Contain(tag => tag.Name == "testName" && tag.User.UserName == "userName");
        }

        [Fact]
        public void Should_ThrowException_When_TagAlreadyExists_AddTagAsync()
        {
            // Arrange
            List<UserEntry> users = CreateUserList();
            var tags = new List<TagEntry>()
            {
                new TagEntry() { Name = "tag13231", User = users[1], Id = 0 },
            };

            var unitOfWork = MockHelper.SetupUnitOfWork(tags, null);
            var mapper = MockHelper.InitializeMapper(typeof(NoteMappingProfile));
            var userAccessor = MockHelper.SetupCurrentUserAccessor(users[0]);

            INoteManager noteManager = new NoteManager(unitOfWork, mapper, userAccessor);

            // Act
            Func<Task> act = () => noteManager.AddTagAsync(new TagCreateData() { Name = "tag13231" });

            // Assert
            act.Should().ThrowAsync<ExistedTagNameException>().WithMessage("Cannot add tag with already existing name");
        }
        #endregion

        #region RemoveTagFromNote
        [Fact]
        public void Should_RemoveTagFromNote()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(tags, notes, users[0]);

            // Act
            noteManager.RemoveTagFromNote(noteId: 2, tagId: 4);

            // Assert
            _ = notes.FirstOrDefault(n => n.Id == 2).Tags.Should().HaveCount(1);
        }

        [Fact]
        public void Should_ThrowException_WhenNoteWithThisIdDoesNotExist_RemoveTagFromNote()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(tags, notes, users[0]);

            // Act
            Action act = () => noteManager.RemoveTagFromNote(noteId: 22212, tagId: 4);

            // Assert
            act.Should().Throw<NotFoundException>();
        }

        [Fact]
        public void Should_ThrowException_WhenTagWithThisIdDoesNotExist_RemoveTagFromNote()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(tags, notes, users[0]);

            // Act
            Action act = () => noteManager.RemoveTagFromNote(noteId: 1, tagId: 44444);

            // Assert
            act.Should().Throw<NotFoundException>();
        }
        #endregion

        #region UpdateNoteAsync

        [Fact]
        public async void Should_ThrowException_WhenUserHaveNoAccessToNoteWithThisId_UpdateNoteAsync()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(tags, notes, users[0]);

            // Act
            Func<Task> act = async () => await noteManager.UpdateNoteAsync(new NoteUpdateData() { Id = 3, Tags = new List<Tag>(), Text = "", Title = "" });

            // Assert
            await act.Should().ThrowAsync<UserAccessException>();
        }
        
        [Fact]
        public async void Should_UpdateNoteAsync()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(tags, notes, users[0]);

            // Act
            await noteManager.UpdateNoteAsync(new NoteUpdateData() { Id = 1, Text = "updated text", Title = "1" });

            // Assert
            var note = notes.FirstOrDefault(n => n.Id == 1);

            note.Title.Should().Be("1");
            note.Text.Should().Be("updated text");
            note.Id.Should().Be(1);
        }
        #endregion

        #region SetNoteImportance
        [Fact]
        public async void Should_SetNoteImportance()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);
            
            INoteManager noteManager = InitializeNoteManager(tags, notes, users[0]);

            // Act
            await noteManager.SetNoteImportanceAsync(1, true);

            // Assert
            _ = notes.FirstOrDefault(n => n.Id == 1).IsImportant.Should().Be(true);
        }

        [Fact]
        public async void Should_ThrowException_When_UserHaveNoAccessToNote_SetNoteImportance()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(tags, notes, users[0]);

            // Act
            Func<Task> act = async () => await noteManager.SetNoteImportanceAsync(4, true);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async void Should_ThrowException_When_NoteDoesNotExist_SetNoteImportance()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(tags, notes, users[0]);

            // Act
            Func<Task> act = async () => await noteManager.SetNoteImportanceAsync(4444, true);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
        #endregion

        #region DeleteTagById
        [Fact]
        public void Should_DeleteTagById()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(tags, notes, users[0]);

            // Act
            noteManager.DeleteTagById(tagId: 4);

            // Assert
            tags.Should().NotContain(tag => tag.Name == "testTag" && tag.User.UserName == "userName");
        }

        [Fact]
        public void Should_ThrowException_When_TagDoesNotExist_DeleteTagById()
        {
            // Arrange
            var users = CreateUserList();
            var tags = CreateTagList(users);
            var notes = CreateNoteList(users, tags);

            INoteManager noteManager = InitializeNoteManager(tags, notes, users[0]);

            // Act
            Action act = () => noteManager.DeleteTagById(tagId: 9874);

            // Assert
            act.Should().Throw<NotFoundException>().WithMessage("This tag does not exist");
        }
        #endregion

        #region initialization methods
        private static INoteManager InitializeNoteManager(List<TagEntry> tags, List<NoteEntry> notes, UserEntry currentUser)
        {
            var unitOfWork = MockHelper.SetupUnitOfWork(tags, notes);
            var mapper = MockHelper.InitializeMapper(typeof(NoteMappingProfile));
            var userAccessor = MockHelper.SetupCurrentUserAccessor(currentUser);

            INoteManager noteManager = new NoteManager(unitOfWork, mapper, userAccessor);

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
                new NoteEntry() { Tags = new List<TagEntry>() { tags[0], }, Id = 1, Text = "1", Title = "1", User = users[0], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[0], tags[1] }, Id = 2, Text = "2", Title = "2", User = users[0], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2], }, Id = 3, Text = "3", Title = "3", User = users[1], IsImportant = false,},
                new NoteEntry() { Tags = new List<TagEntry>() { tags[2], }, Id = 4, Text = "4", Title = "4", User = users[1], IsImportant = false,},
            };
        }
        #endregion
    }
}