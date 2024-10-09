using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoTranscription
{
    internal class Word
    {
        internal string Text { get; set; }
        internal float Start { get; set; }
        internal float End { get; set; }
        internal int Length { get; set; }
    }
}
