using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication1.Database.Model;

namespace WpfApplication1.Database.Context
{
    class MyDatabaseContext : DbContext
    {
        public MyDatabaseContext()
            : base("MyDatabaseCC")
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
