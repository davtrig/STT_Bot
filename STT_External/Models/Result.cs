using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace STT_External.Models
{
    public class Result
    {
        public string MatchedWord { get; set; }

        public int Distance { get; set; }

        public int ResultId { get; set; }

        public DateTime DateTime { get; set; }
    }
}