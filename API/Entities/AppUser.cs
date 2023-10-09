using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;

namespace API.Entities
{
    public class AppUser
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserName { get; set; }


        public DateOnly DateOfBirth { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public List<Photo> Photos { get; set; } = new();
        public string Gender { get; set; }
        public byte[] passwordHash { get; set; }
        public byte[] passwordSalt { get; set; }

        public List<UserLike> LikedByUsers { get; set; }

        public List<UserLike> LikedUsers { get; set; }

        public List<Messages> MessagesSent { get; set; }

        public List<Messages> MessagesReceived { get; set; }


    }


}