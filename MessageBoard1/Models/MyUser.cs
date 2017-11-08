using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MessageBoard1.Models {
    public class MyUser {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string PhoneNum { get; set; }
        public DateTime SignDate { get; set; }
        public int NewReply { get; set; }
    }
}