using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class NotificationEntity
    {
        public int Id { get; set; }
        public int Period { get; set; }
        public int ThrashHold { get; set; }
        public string LogLevels { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
