using Ataa.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ataa.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
       
        public DbSet<User> User { get; set; }
        public DbSet<RequestForm> RequestForm { get; set; }

        public DbSet<Status> Status { get; set; }

        public DbSet<Job> Job { get; set; }
        public DbSet<Attendancelist> Attendancelist { get; set; }
        public DbSet<Section> Section { get; set; }


    }
}
