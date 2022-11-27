using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer2.Models
{
    public enum FilmType : byte
    {
        Movie,
        Series
    }

    public class Film : IModel
    {
        public int Id { get; set; }
        public FilmType Type { get; set; }
        public string Name { get; set;  }
        public string OriginalName { get; set; }
        public int Year { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
    }
}
