using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class UserDtos
    {
        public string UserName { get; set; }
        public string token { get; set; }

        public string PhotoUrl { get; set; }
    }
}