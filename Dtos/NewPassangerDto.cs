using System.ComponentModel.DataAnnotations;

namespace angular_asp.Dtos
{
    public record NewPassangerDto(
        [Required][EmailAddress][StringLength(100, MinimumLength = 3)] string Email,
        [Required][MinLength(2)][MaxLength(35)] string FirstName,
        [Required][MinLength(2)][MaxLength(35)] string LastName,
        [Required][Range(0, 130 )] byte Age
     );
}
