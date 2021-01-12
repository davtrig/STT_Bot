using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace STT_Internal.Models
{
    public class Payload
    {
        public string Word { get; set; }
        public List<Answer> ExpectedWords { get; set; }
        public int Distance { get; set; }
    }
}