using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalServices.DataTransferObjects
{
    public class RedditComments
    {
        public string Author { get; set; }
        public string Body { get; set; }
        public int UpVotes { get; set; }
        public string Permalink { get; set; }
    }
}
