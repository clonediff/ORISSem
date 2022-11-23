using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer2.Models
{
    public enum Role : byte
    {
        Actor,
        Director,
        Screenwriter,
        Producer
    }

    [Table("StuffToFilm")]
    public class StuffToFilm : IModel
    {
        public int Id { get; set; }
        public int StuffId { get; set; }
        public int FilmId { get; set; }
        public Role Role { get; set; }
    }
}
