using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MessageBoard1.Models {
    public class Message {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsPublic { get; set; }
        public DateTime DateTime { get; set; }
        public int ReplyNum { get; set; }
        public int NewReply { get; set; }
        public List<Reply> Replies { get; set; }    //保存该留言的所有回复
    }
}