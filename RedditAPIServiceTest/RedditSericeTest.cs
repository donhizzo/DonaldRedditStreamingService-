using System.Threading.Tasks;
using Moq;
using Reddit.Controllers;
using Xunit;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Reddit;
using ExternalServices;
using ExternalServices.DataMemory;
using Microsoft.Extensions.Caching.Memory;

namespace RedditAPIServiceTest
{
    public class RedditSericeTest
    {
        [Fact]
        public async Task GetLatestPostAsync_ShouldReturnLatestPost()
        {
            // Arrange
            var mockClient = new Mock<RedditClient>("clientId", "clientSecret", "username", "password");
            var mockSubreddit = new Mock<Subreddit>(mockClient.Object, "TestSubreddit");
            var dispatch = new Mock<Dispatch>("clientId", "clientSecret", "username", "password");
            var memory = new Mock<IMemory>();
            var cache = new Mock<IMemoryCache>();        
            var TopPost = new Mock<ReditAPIService>(memory.Object,cache.Object);
            
            

            var expectedPost = new Post(dispatch.Object, "TestSubreddit")
            {
                Title = "Test Post",
                Author = "TestAuthor",
                UpVotes = 100,
                Permalink = "http://testurl.com"
            };

           TopPost.Setup(x => x.GetTopPostersAsync(new List<Post> { expectedPost }, "TestSubreddit"));
            

           
            var result = new List<Post>(TopPost.Object.GetHotPost(mockClient.Object, "TestSubreddit", 1));
            // Assert
            result.Should().NotBeNull();
            result.FirstOrDefault().Title.Should().Be("Test Post");
        }

        
    }
}

