using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class SentNotificationEntity
    {
        public int Id { get; set; }

        public int LogId { get; set; }
        public int NotificationId { get; set; }
        public DateTime SentAt { get; set; }
    }
}
