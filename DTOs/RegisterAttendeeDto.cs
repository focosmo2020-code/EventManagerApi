using System.ComponentModel.DataAnnotations;

namespace EventManagerApi.DTOs;

public class RegisterAttendeeDto
{
    [Required, StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(100)]
    public string Email { get; set; } = string.Empty;
}
