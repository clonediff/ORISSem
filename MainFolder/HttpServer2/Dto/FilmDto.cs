using HttpServer2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer2.Dto
{
    public class FilmDto
    {
        public string Name { get; set; }
        public string OriginalName { get; set; }
        public int Year { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
        public FilmStuff Director { get; set; }
        public List<FilmStuff> ScreenWriters { get; init; }
        public string ScreenWritersString => string.Join(", ", ScreenWriters);
        public List<FilmStuff> Producers { get; init; }
        public string ProducersString => string.Join(", ", Producers);
        public List<FilmStuff> Actors { get; init; }
        public string ActorsString => string.Join(", ", Actors);
    }
}
