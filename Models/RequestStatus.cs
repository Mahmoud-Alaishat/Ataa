using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ataa.Models
{
    public class RequestStatus
    {
        public RequestForm reqForm { get; set; }
        public Status status { get; set; }
    }
}
