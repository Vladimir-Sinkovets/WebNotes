using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes.DAL.Models
{
    public class UserEntry : IdentityUser
    {
        public string Nickname { get; set; }
    }
}
