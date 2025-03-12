using System.ComponentModel.DataAnnotations;

namespace UserAuthenticationService.ApiContract.Requests
{
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; }
    }
} 