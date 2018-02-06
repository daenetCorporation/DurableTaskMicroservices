using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetTweetsMention
{
    public class GetTweetTaskOutput
    {
        public bool IsTweetAvailable { get; set; }

        public string LatestTweetId { get; set; }
    }
}
