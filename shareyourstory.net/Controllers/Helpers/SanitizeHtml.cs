﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace shareyourstory.net.Controllers.Helpers
{
    public class SanitizeHtml
    {
        private static Regex _tags = new Regex("<[^>]*(>|$)", RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled);
        private static Regex _whitelist = new Regex(@"^</?(b(lockquote)?|code|d(d|t|l|el)|em|h(1|2|3)|i|kbd|li|ol|p(re)?|s(ub|up|trong|trike)?|ul)>$|^<(b|h)r\s?/?>$",
            RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
        private static Regex _whitelist_a = new Regex(@"^<a\shref=""(\#\d+|(https?|ftp)://[-a-z0-9+&@#/%?=~_|!:,.;\(\)]+)""(\stitle=""[^""<>]+"")?\s?>$|^</a>$",
            RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
        private static Regex _whitelist_img = new Regex(@"
                                                    ^<img\s
                                                    src=""https?://[-a-z0-9+&@#/%?=~_|!:,.;\(\)]+""
                                                    (\swidth=""\d{1,3}"")?
                                                    (\sheight=""\d{1,3}"")?
                                                    (\salt=""[^""<>]*"")?
                                                    (\stitle=""[^""<>]*"")?
                                                    \s?/?>$",
                                                    RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);


        /// <summary>
        /// sanitize any potentially dangerous tags from the provided raw HTML input using 
        /// a whitelist based approach, leaving the "safe" HTML tags
        /// CODESNIPPET:4100A61A-1711-4366-B0B0-144D1179A937
        /// </summary>
        public static string Sanitize(string html)
        {
            if (String.IsNullOrEmpty(html)) return html;

            string tagname;
            Match tag;

            // match every HTML tag in the input
            MatchCollection tags = _tags.Matches(html);
            for (int i = tags.Count - 1; i > -1; i--)
            {
                tag = tags[i];
                tagname = tag.Value.ToLowerInvariant();

                if (!(_whitelist.IsMatch(tagname) || _whitelist_a.IsMatch(tagname) || _whitelist_img.IsMatch(tagname)))
                {
                    html = html.Remove(tag.Index, tag.Length);
                    System.Diagnostics.Debug.WriteLine("tag sanitized: " + tagname);
                }
            }

            return html;
        }
        public static string ShortenAndStripHtml(string string_to_shorten, int string_length)
        {
            string str = string.IsNullOrEmpty(string_to_shorten)
                ? ""
                : System.Text.RegularExpressions.Regex.Replace(string_to_shorten, "<.+?>", string.Empty);
            if (str.Length > string_length)
                str = str.Substring(0, string_length - 3) + "...";

            return str;
        }

    }
}