using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ataa.Models
{
    public class User: IdentityUser
    {
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public int JobId { get; set; }
        public int ClockCoin { get; set; }


    }
}
