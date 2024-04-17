using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Enable.Presentation.EventSourcing.EventStreaming.Features.EventStreaming.Entities;

[PrimaryKey(nameof(SequenceNumber))]
[Index(nameof(SequenceNumber), IsUnique = true)]
[Table("EventOutbox", Schema = "dbo")]
public class EventOutbox
{
    [Key]
    public long SequenceNumber { get; set; }

    [MaxLength(128)]
    [Required]
    public required string Name { get; set; }

    [MaxLength(int.MaxValue)]
    [Required]
    public required string Payload { get; set; }

    public required DateTimeOffset EnqueuedDateTime { get; set; } = DateTimeOffset.UtcNow;
}
