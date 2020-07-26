using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SCIndexer.Record.Filter
{
    class RegexFileRecordFilter : IFileRecordFilter
    {

        private readonly Regex regex;

        public RegexFileRecordFilter()
        {
            
        }

        public RegexFileRecordFilter(string regex)
        {
            this.regex = new Regex(regex);
        }

        public bool Filter(string filePath, FileRecordReason reason)
        {
            return this.regex == null || this.regex.Match(filePath).Success;
        }
    }
}
