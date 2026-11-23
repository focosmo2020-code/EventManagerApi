using System.ComponentModel.DataAnnotations;

namespace EventManagerApi.Models;

public class Attendee
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required, StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(100)]
    public string Email { get; set; } = string.Empty;

    public int EventId { get; set; }
    public Event? Event { get; set; }
}
