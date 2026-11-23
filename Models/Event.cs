using System.ComponentModel.DataAnnotations;

namespace EventManagerApi.Models;

public class Event
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required, DataType(DataType.DateTime)]
    public DateTime Date { get; set; }

    [Required, StringLength(100)]
    public string Location { get; set; } = string.Empty;

    public List<Attendee> Attendees { get; set; } = new List<Attendee>();
}
