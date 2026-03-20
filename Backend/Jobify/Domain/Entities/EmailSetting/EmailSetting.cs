using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.EmailSetting
{
    public class EmailSetting
    {
        public string SmtpHost { get; set; } = "smtp.gmail.com";  // Gmail SMTP server
        public int SmtpPort { get; set; } = 587;                  // TLS port
        public string SenderEmail { get; set; } = "mohamed.reda3332.2005@gmail.com";     // Your Gmail address
        public string SenderPassword { get; set; } = "xven gksk cbtu noti\r\n";         // App password

    }
}
