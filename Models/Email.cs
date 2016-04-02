using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMLtoHTML.Models
{
    public class Email
    {
        public Int32 id { get; set; }
        public DateTime time { get; set; }
        public string fileName { get; set; }
        public string fromAddress { get; set; }
        public string toAddress { get; set; }
        public string emailSubject { get; set; }
        public string content { get; set; }
    }
}