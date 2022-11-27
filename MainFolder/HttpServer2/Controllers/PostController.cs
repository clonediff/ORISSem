using HttpServer2.Attributes;
using HttpServer2.Extensions;
using HttpServer2.Models;
using HttpServer2.ServerInfrastructure.CookiesAndSessions;
using HttpServer2.ServerInfrastructure.ServerResponse;
using HttpServer2.ServerInfrstructure.CookiesAndSessions;
using HttpServer2.ServerResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer2.Controllers
{
    [ApiController("post")]
    public class PostController
    {
        [HttpPOST("addpost")]
        public async Task<IControllerResult> AddPost(string content,
            [FromCookie(typeof(SessionId), nameof(SessionId.Id))] Guid sessionId)
        {
            if (string.IsNullOrEmpty(content.Trim()))
                return new BadRequest();
            var sessionManager = SessionManager.Inst;
            int? authorId = null;
            if (sessionId != default)
            {
                var session = await sessionManager.GetSessionInfo(sessionId);
                authorId = session.AccountId;
            }
            var postTime = DateTime.Now;
            await MyORM.Instance.Insert(new Post { AuthorId = authorId, Content = content, PostTime = postTime });
            var posts = await MyORM.Instance.Select<Post>();
            var post = posts
                .FirstOrDefault(x => x.AuthorId == authorId && 
                x.Content == content && 
                (x.PostTime - postTime).Abs() < TimeSpan.FromSeconds(1));
            var name = (await MyORM.Instance.Select<Account>()).GetAuthorName(post?.AuthorId);
            var commentsCount = (await MyORM.Instance.Select<Comment>()).Where(x => x.PostId == post.Id).Count();
            return new View("postTemplate", 
                new { 
                    Id = post.Id, 
                    AuthorName = name, 
                    Time = post.PostTime, 
                    Content = post.Content, 
                    CommentsCount = commentsCount
                });
        }

        [HttpGET("/{postId}")]
        public async Task<IControllerResult> GetPostAsync(int postId)
        {
            var orm = MyORM.Instance;
            var posts = await orm.Select<Post>();
            var accounts = await orm.Select<Account>();
            var comments = await orm.Select<Comment>();

            var post = posts.FirstOrDefault(x => x.Id == postId);
            if (post is null)
                return new NotFound();
            var authorName = accounts.GetAuthorName(postId);
            var preparedComments = comments
                .Where(x => x.PostId == postId)
                .GroupJoin(accounts,
                x => x.AuthorId,
                x => x.Id,
                (c, a) => (c, Name: a.GetAuthorName(c.AuthorId)))
                .Select(x => new { AuthorName = x.Name, CommentInfo = x.c });

            return new View("postViewer.html", 
                new { Post = post, 
                    AuthorName = authorName, 
                    CommentsCount = preparedComments.Count(), 
                    Comments = preparedComments });
        }

        [HttpPOST("/{postId}/addcomment")]
        public async Task<IControllerResult> AddCommentAsync(int postId, string content,
            [FromCookie(typeof(SessionId), nameof(SessionId.Id))] Guid sessionId)
        {
            if (string.IsNullOrEmpty(content.Trim()))
                return new BadRequest();
            var sessionManager = SessionManager.Inst;
            int? authorId = null;
            if (sessionId != default)
            {
                var session = await sessionManager.GetSessionInfo(sessionId);
                authorId = session.AccountId;
            }
            var posts = await MyORM.Instance.Select<Post>();
            var post = posts.FirstOrDefault(x => x.Id == postId);
            var postTime = DateTime.Now;
            await MyORM.Instance.Insert(new Comment { AuthorId = authorId, Content = content, CommentTime = postTime, PostId = postId });
            var comments = await MyORM.Instance.Select<Comment>();
            var comment = comments
                .FirstOrDefault(x => x.AuthorId == authorId &&
                x.PostId == postId &&
                x.Content == content &&
                (x.CommentTime - postTime).Abs() < TimeSpan.FromSeconds(1));
            var name = (await MyORM.Instance.Select<Account>()).GetAuthorName(comment?.AuthorId);
            var commentsCount = (await MyORM.Instance.Select<Comment>()).Where(x => x.PostId == post.Id).Count();
            return new View("commentTemplate",
                new
                {
                    AuthorName = name,
                    CommentTime = comment.CommentTime,
                    Content = content
                });
        }
    }
}
