using ExternalServices.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalServices.DataMemory
{
    public interface IMemory
    {
        void AddUser(TopUser user);

        void AddComment(RedditComments comment);

        void AddUpVote(ReddisUpvotes upvote);
        bool CheckCommentExist(string comment);


    }
}
