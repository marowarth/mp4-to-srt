using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoTranscription
{
    internal class VoskResult
    {
        internal List<Word> VoskResultToWords(string resultJson)
        {
            var json = JObject.Parse(resultJson);
            var items = json["result"].Select(item => new Word
            {
                Text = item["word"].ToString(),
                Start = (float)item["start"],
                End = (float)item["end"],
                Length= item["word"].ToString().Length,
            }).ToList();
            return items;
        }
    }
}
