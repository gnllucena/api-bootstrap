using System;
using System.Collections.Generic;

namespace API.Domain.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Document { get; set; }
        public DateTime Birthdate { get; set; }
        public string Country { get; set; }
        public Profile Profile { get; set; }
        public bool Active { get; set; }
    }
}
