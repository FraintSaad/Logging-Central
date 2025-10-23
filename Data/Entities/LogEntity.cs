using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models
{
    [Table("SerilogEvents")]
    public class LogEntity
    {
        public int Id { get; set; }
        public string? Message { get; set; }
        public string? Level { get; set; }
        public string? Exception { get; set; }
        public string? Environment { get; set; }
        [Key]
        public DateTimeOffset Timestamp { get; set; }
        [NotMapped]
        public DateTimeOffset LocalTimestamp { get; set; }
    }
}
