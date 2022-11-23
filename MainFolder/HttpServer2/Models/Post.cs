using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer2.Models
{
    [Table("Posts")]
    public class Post : IModel
    {
        public int Id { get; set; }
        public DateTime PostTime { get; set; }
        public int? AuthorId { get; set; }
        public string Content { get; set; }
    }
}
