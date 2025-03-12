using System.ComponentModel.DataAnnotations;

namespace UserAuthenticationService.ApiContract.Requests
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
} 