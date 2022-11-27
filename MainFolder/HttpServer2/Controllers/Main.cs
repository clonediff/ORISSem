using HttpServer2.Attributes;
using HttpServer2.Extensions;
using HttpServer2.Models;
using HttpServer2.ServerResponse;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HttpServer2.Controllers
{
    [ApiController("/")]
    public class Main
    {
        [HttpGET("/")]
        public async Task<IControllerResult> MainFooAsync()
        {
            var orm = MyORM.Instance;
            var posts = await orm.Select<Post>();
            //var posts = new List<Post>();
            var accounts = await orm.Select<Account>();
            var comments = await orm.Select<Comment>();
            var resultPosts = posts.GroupJoin(accounts,
                x => x.AuthorId,
                x => x.Id,
                (p, a) => (p, a: a.GetAuthorName(p.AuthorId)))
                .Select(x => new { AuthorName = x.a, Content = x.p.Content, Id = x.p.Id, Time = x.p.PostTime})
                .GroupJoin(comments,
                x => x.Id,
                com => com.PostId,
                (p, c) => (p, CommentsCount: c.Count()))
                .Select(x => new { x.p.AuthorName, x.p.Content, x.p.Id, x.p.Time, x.CommentsCount });
            return new View("main", resultPosts.ToList());
        }
    }
}