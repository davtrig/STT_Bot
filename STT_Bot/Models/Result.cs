using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STT_Bot.Models
{
    class Result
    {
        public string MatchedWord { get; set; }

        public int Distance { get; set; }

        public int ResultId { get; set; }

        public DateTime DateTime { get; set; }
    }
}
