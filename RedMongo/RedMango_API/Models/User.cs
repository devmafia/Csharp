using System;
using System.ComponentModel.DataAnnotations;

namespace RedMango_API.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
    }

    public enum Role
    {
        User = 0,
        Admin = 1 
    }
}

