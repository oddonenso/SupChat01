using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Tables
{
    public class Connection: DbContext
    {
        public Connection(DbContextOptions<Connection> options)
           : base(options)
        {
        }

        public DbSet<User> Userss { get; set; }
        public DbSet<Role> Roless { get; set; }
        public DbSet<Gender> Genderss { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server=localhost;Port=5432;User Id=postgres;Password=111;Database=SuperChat;");
        }
    }
}
