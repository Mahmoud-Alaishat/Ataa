using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ataa.Models
{
    public class RequestForm
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [DataType(DataType.Time)]
        public DateTime StartTime { get; set; }

        [DataType(DataType.Time)]
        public DateTime EndTime { get; set; }
        public int NumberOfGrantee { get; set; }
        public String Service { get; set; }
        public String Location { get; set; }
        public String Feedback { get; set; }

        [HiddenInput]
        public String UserId { get; set; }
        [HiddenInput]
        public int StatusId { get; set; }
        public int IsCalculated { get; set; }

    }
}
