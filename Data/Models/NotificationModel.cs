using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class NotificationModel
    {
        public int Id { get; set; }
        public int Period { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ThrashHold { get; set; }
        public string LogLevels { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
