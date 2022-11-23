using HttpServer2.Attributes;
using HttpServer2.Models;
using HttpServer2.ServerResponse;
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
            var accounts = await orm.Select<Account>();
            var resultPosts = posts.Join(accounts,
                x => x.AuthorId,
                x => x.Id,
                (p, a) => new { AuthorName = a.Login, Content = p.Content, Id = p.Id });
            return new View("main", resultPosts.ToList());
        }
    }
}

/*
1)
2) 
9)

*/