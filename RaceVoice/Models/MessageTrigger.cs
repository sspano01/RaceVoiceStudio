using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceVoice
{
    public class MessageTrigger
    {
        public string Phrase { get; set; }
        public int Repeat { get; set; }

        public MessageTrigger()
        {
            Phrase = "BLANK";
            Repeat = 0;
        }
    }


}
