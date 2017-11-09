using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MessageBoard1.Models {
    public class Reply {
        public int Id { get; set; }
        public int MessageId { get; set; }
        public string AdminName { get; set; }
        public string Content { get; set; }
        public DateTime DateTime { get; set; }
    }
}