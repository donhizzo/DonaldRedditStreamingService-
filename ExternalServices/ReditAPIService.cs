using System.Diagnostics;
using Reddit;
using Reddit.AuthTokenRetriever;
using ExternalServices.DataTransferObjects;
using ExternalServices.DataMemory;
using Microsoft.Extensions.Caching.Memory;
using Reddit.Controllers;
using Newtonsoft.Json.Linq;
using System;

namespace ExternalServices
{
    public class ReditAPIService :IReditAPIService
    {
        private  readonly IMemory _memory;
        private  readonly IMemoryCache _memoryCache;

        public  ReditAPIService(IMemory memory, IMemoryCache memoryCache)
        {
            _memory = memory;
            _memoryCache = memoryCache;

        }

        [Obsolete]
        public void  Run(string ClientId, string ClientSecret, string SubredditName,string refereshToken, string accesstoken)
        {
            
            //var secrets = Authenticate(true, ClientId, ClientSecret).Result;

            var client = new RedditClient(ClientId, refereshToken, ClientSecret, accesstoken);

            _ = Task.Run(() => GetPostCommentsAsync(client, SubredditName));

            var result =Task.Run(() => GetHotPost(client, SubredditName, limit: 100));
            Task.WaitAll(result);
           
            _ = Task.Run(() => GetTopPostByVotesAsync(result.Result,SubredditName));
            _ = Task.Run(() => GetTopPostersAsync(result.Result, SubredditName));
            

        }
        [Obsolete]
        public async Task<Secrets> Authenticate(bool tryCache, string ClientId, string ClientSecret)
        {
            try
            {
                if (tryCache)
                {
                    return getkey(ClientId, ClientId, ClientSecret);
                }

                AuthTokenRetrieverLib authTokenRetrieverLib = new(ClientId, ClientSecret, 8080);

                authTokenRetrieverLib.AwaitCallback();
                Process.Start(new ProcessStartInfo(authTokenRetrieverLib.AuthURL()) { UseShellExecute = true });

                await Task.Delay(TimeSpan.FromSeconds(30));

                // Console.ReadKey();

                authTokenRetrieverLib.StopListening();
                Secrets secrets = new Secrets
                {
                    access_token = authTokenRetrieverLib.AccessToken,
                    refresh_token = authTokenRetrieverLib.RefreshToken,
                };

                return secrets;
            }
            catch (Exception  e)
            {

                Console.WriteLine("error occured: " + e.Message);
                return null;
            }

        }

        public  void  GetTopPostByVotesAsync(List<Reddit.Controllers.Post> posts, string subredditName)
        {
            try
            {
                var topPost = posts.OrderByDescending(p => p.UpVotes).FirstOrDefault();

                ReddisUpvotes upvote = new ReddisUpvotes
                {
                    Title = topPost.Title,
                    Author = topPost.Author,
                    UpVotes = topPost.UpVotes,
                    Permalink = topPost.Permalink,
                };
                Console.WriteLine($"=========== {DateTime.Now} Top post in /r/{subredditName} with most votes:======");
                Console.WriteLine($"Title: {upvote.Title}");
                Console.WriteLine($"Author: {upvote.Author}");
                Console.WriteLine($"Upvotes: {upvote.UpVotes}");
                Console.WriteLine($"URL: {upvote.Permalink}");
                Console.WriteLine($"============================");
                Console.WriteLine($"");

                _memory.AddUpVote(upvote);
            }
            catch (Exception e)
            {

                Console.WriteLine("error occured: " +  e.Message);
            }            
           
        }

        public List<Post> GetHotPost(RedditClient redditClient, string subredditName, int limit)
        {
            
            try
            {
                var subreddit = redditClient.Subreddit(subredditName);

                return subreddit.Posts.GetHot(limit: 100);
            }
            catch (Exception e)
            {
                
                Console.WriteLine("error occured: " + e.Message);
                return null;
            }
            
        }

        public void GetTopPostersAsync(List<Reddit.Controllers.Post> posts, string subredditName)
        {
            try
            {
                var userPostCounts = new Dictionary<string, int>();
                foreach (var post in posts)
                {
                    if (userPostCounts.ContainsKey(post.Author))
                    {
                        userPostCounts[post.Author]++;
                    }
                    else
                    {
                        userPostCounts[post.Author] = 1;
                    }
                }


                var sortedUserPostCounts = userPostCounts.OrderByDescending(kvp => kvp.Value).Take(10).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                TopUser user = new TopUser
                {
                    User = sortedUserPostCounts.FirstOrDefault().Key,
                    Post = sortedUserPostCounts.FirstOrDefault().Value
                };

                Console.WriteLine($"=========== {DateTime.Now} Top Users in /r/{subredditName} with the most post:======");
                Console.WriteLine($"User: {user.User}, Posts: {user.Post}");
                Console.WriteLine($"============================");
                Console.WriteLine($"");
                _memory.AddUser(user);
            }
            catch (Exception e)
            {

                Console.WriteLine("error occured: " + e.Message);
            }
            
                        
        }

        private Reddit.Controllers.Post GetLatestPostAsync(RedditClient redditClient, string subredditName)
        {
            
            var subreddit = redditClient.Subreddit(subredditName);            
            var posts = subreddit.Posts.GetNew(limit: 1);            
            var latestPost = posts.FirstOrDefault();
            return latestPost;
        }

        public  void GetPostCommentsAsync(RedditClient redditClient, string subredditName)
        {
            try
            {
                var post = GetLatestPostAsync(redditClient, subredditName);
                var comments = post.Comments.GetNew().FirstOrDefault();

                if (comments !=null)
                {
                    RedditComments comment = new RedditComments
                    {
                        Body = comments.Body,
                        Author = comments.Author,
                        UpVotes = comments.UpVotes,
                        Permalink = comments.Permalink,
                    };

                    if (!_memory.CheckCommentExist(comment.Body))
                    {
                        _memory.AddComment(comment);
                        Console.WriteLine($"\nComments for the latest post in /r/{subredditName}:");
                        Console.WriteLine($"=========== {DateTime.Now} Most Recent Post in /r/{subredditName}:======");
                        Console.WriteLine($"\nAuthor: {comment.Author}");
                        Console.WriteLine($"Comment: {comment.Body}");
                        Console.WriteLine($"Upvotes: {comment.UpVotes}");
                        Console.WriteLine($"Permalink: {comment.Permalink}");
                        Console.WriteLine($"============================");
                        Console.WriteLine($"");
                    }
                    else
                    {
                        Console.WriteLine($"=========== {DateTime.Now} Most Recent Post in /r/{subredditName}:======");
                        Console.WriteLine($"There are no new Post");
                        Console.WriteLine($"============================");
                        Console.WriteLine($"");
                    }
                }
                else
                {
                    Console.WriteLine($"=========== {DateTime.Now} Most Recent Post in /r/{subredditName}:======");
                    Console.WriteLine($"No Post available");
                    Console.WriteLine($"============================");
                    Console.WriteLine($"");
                }
                
            }
            catch (Exception e)
            {

                Console.WriteLine("error occured: " + e.Message);
            }
            
            
        }

        [Obsolete]
        private Secrets getkey (string CacheKey, string clientID, string clientSecrets)
        {
            Secrets secrets = new Secrets();
            if (!_memoryCache.TryGetValue(CacheKey, out Secrets keys))
            {
                secrets =  Authenticate(false,clientID,clientSecrets).Result;
               

                // Set cache options
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(50)
                };

                // Save data in cache
                _memoryCache.Set(CacheKey, keys, cacheOptions);
            }

            return secrets;
        }
        
    }
}
