using System.Diagnostics;
using Reddit;
using Reddit.AuthTokenRetriever;
using Newtonsoft.Json;
using ExternalServices.DataTransferObjects;
using ExternalServices.DataMemory;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace ExternalServices
{
    public interface IReditAPIService
    {
         Task<Secrets> Authenticate(bool tryCache, string ClientId, string ClientSecret);
        List<Reddit.Controllers.Post> GetHotPost(RedditClient redditClient, string subredditName, int limit);
        void GetTopPostByVotesAsync(List<Reddit.Controllers.Post> posts, string subredditName);
        void GetTopPostersAsync(List<Reddit.Controllers.Post> posts, string subredditName);
        void GetPostCommentsAsync(RedditClient redditClient, string subredditName);
        void Run(string ClientId, string ClientSecret, string SubredditName, string refereshToken, string accesstoken);
       



    }
}
