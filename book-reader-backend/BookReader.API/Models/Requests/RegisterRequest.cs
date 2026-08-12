using System.ComponentModel.DataAnnotations;

namespace BookReader.API.Models.Requests
{
    public record RegisterRequest(
    [Required]
    [EmailAddress]
    string Email,

    [Required]
    [MinLength(8)]
    string Password);
}
