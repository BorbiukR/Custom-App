using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }     
        public string Email { get; set; }      
        public string Password { get; set; }
        public bool IsEmailVerified { get; set; }
        public Guid ActivationCode { get; set; }
        public string ResetPasswordCode { get; set; }
    }
}