using System.Collections.Generic;

namespace ZebuPairOrder.Models.Request
{
    public class GetScripModel
    {
        public string symbol { get; set; }
        public List<string> exchange { get; set; }

    }
}
