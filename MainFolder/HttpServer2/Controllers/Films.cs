using HttpServer2.Attributes;
using HttpServer2.Dto;
using HttpServer2.Models;
using HttpServer2.ServerInfrastructure.ServerResponse;
using HttpServer2.ServerResponse;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer2.Controllers
{
    [ApiController("/films")]
    public class Films
    {
        [HttpGET("/")]
        public async Task<IControllerResult> GetAllFilmsAsync()
        {
            var films = await MyORM.Instance.Select<Film>();
            return new View("films", films);
        }

        [HttpGET("/{id}")]
        public async Task<IControllerResult> GetFilmInfoAsync(int id)
        {
            var films = await MyORM.Instance.Select<Film>();
            var film = films.Where(x => x.Id == id);
            if (!film.Any())
                return new NotFound();
            var filmStuff = MyORM.Instance.Select<FilmStuff>();
            var stuff = (await MyORM.Instance.Select<StuffToFilm>()).Where(x => x.FilmId == id)
                .Join(await filmStuff,
                x => x.StuffId,
                x => x.Id,
                (r, s) => new { Role = r.Role, Stuff = s })
                .ToLookup(x => x.Role, x => x.Stuff);
            var curFilm = film.First();
            var filmDto = new FilmDto
            {
                Name = curFilm.Name,
                OriginalName = curFilm.OriginalName,
                Year = curFilm.Year,
                Path = curFilm.Path,
                Description = curFilm.Description,
                Director = stuff[Role.Director].FirstOrDefault()!,
                Actors = stuff[Role.Actor].ToList(),
                Producers = stuff[Role.Producer].ToList(),
                ScreenWriters = stuff[Role.Screenwriter].ToList()
            };
            return new View("filmInfo", filmDto);
        }
    }
}
