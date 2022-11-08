using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notes.BLL.Services.AccountInfoManagers.Models;

namespace Notes.BLL.Services.AccountInfoManagers
{
    public interface IAccountInfoManager
    {
        AccountInfo GetAccountInfo(string UserName);
    }
}
