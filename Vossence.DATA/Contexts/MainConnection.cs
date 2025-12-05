using Vossence.DATA.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Contexts
{
    public class MainConnection : IdentityDbContext<AppUser, AppRole, string>
    {
        public MainConnection(DbContextOptions<MainConnection> options) : base(options)
        {

        }
    }
}
