using ExternalServices.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalServices.DataMemory
{
    public class InMemory : IMemory
    {
        public InMemory()
        {
            Users = new List<TopUser>();
            comments = new List<RedditComments>();
            upvotes = new List<ReddisUpvotes>();
        }

        public List<TopUser> Users { get; set; }
        public List<RedditComments> comments { get; set; }
        public List<ReddisUpvotes> upvotes { get; set; }


        public void AddUser(TopUser user)
        {
            Users.Add(user);
        }
        public void AddComment(RedditComments comment)
        {
            comments.Add(comment);
        }
        public void AddUpVote(ReddisUpvotes upvote)
        {
            upvotes.Add(upvote);
        }

        public bool CheckCommentExist(string comment)
        {
            bool result = false;
            if (comments.Any())
            {
               var Storedcomment =  comments.Where(x => x.Body.Contains(comment)).FirstOrDefault();
               result = Storedcomment != null ? true : false;
            }
            return false;
        }
    }
}
