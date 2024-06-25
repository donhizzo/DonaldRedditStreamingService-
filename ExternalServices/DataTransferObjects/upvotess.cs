using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalServices.DataTransferObjects
{
    public class ReddisUpvotes
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public int UpVotes { get; set; }
        public string Permalink { get; set; }
       
    }
}
