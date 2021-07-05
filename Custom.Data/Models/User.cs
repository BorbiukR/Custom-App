using System;

namespace Web.Models
{
    public class User
    {
        public int Id { get; set; }     
        public string Email { get; set; }      
        public string Password { get; set; }
        public bool IsEmailVerified { get; set; }
        public Guid ActivationCode { get; set; }
        public string ResetPasswordCode { get; set; }
    }
}