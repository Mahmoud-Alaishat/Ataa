using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ataa.Models
{
    [Keyless]
    public class RequestUser
    {
        public RequestForm ReqForm { get; set; }
        public User user { get; set; }

    }
}
