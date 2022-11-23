using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer2.Models
{
    public class Comment : IModel
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public DateTime CommentTime { get; set; }
        public int? AuthorId { get; set; }
        public string Content { get; set; }
    }
}
