using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes.BLL.Services.NoteManagers.Exceptions
{
    public class UserAccessException : Exception
    {
        public UserAccessException() { }

        public UserAccessException(string message) : base(message) { }
    }
}
