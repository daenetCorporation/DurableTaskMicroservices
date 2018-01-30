using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.Microservice.Common.Test
{
    public class GetTweetTaskOutput
    {
        public bool IsTweetAvailable { get; set; }

        public string LatestTweetId { get; set; }
    }
}
