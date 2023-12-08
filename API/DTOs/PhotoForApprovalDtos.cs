using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class PhotoForApprovalDtos
    {

        public int Id { get; set; }
        public string Url { get; set; }
        public string Username { get; set; }
        public bool IsApproved { get; set; }

    }
}