using Notes.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes.BLL.Services.CurrentUserAccessor
{
    public interface ICurrentUserAccessor
    {
        UserEntry Current { get; }
    }
}