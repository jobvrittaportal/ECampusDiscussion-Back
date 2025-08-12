using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ECampusDiscussion.Models;

namespace ECampusDiscussion.EntityData
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }
        public DbSet<ECampusDiscussion.Models.Page> Page { get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<Role_Mast> Role_Mast { get; set; }
        public DbSet<User_Roles> User_Roles { get; set; }
        public DbSet<User_Details> User_Details { get; set; }
        public DbSet<Permission_Table> Permission_Table { get; set; }
    }
}
