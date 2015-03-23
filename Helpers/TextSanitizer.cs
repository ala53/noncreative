using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace NonCreative.Helpers
{
    public class TextSanitizer
    {
        public static string MakeSafe(string unsanitized, bool whitelistHtml = true)
        {
            if (unsanitized == null) return String.Empty;
            if (whitelistHtml)
                return HtmlUtility.SanitizeHtml(unsanitized);
            else return HtmlUtility.StripHtml(unsanitized);
        }

        public static string MakeUrlSafe(string unsanitized)
        {
            return HttpUtility.UrlEncode(unsanitized.Replace(" ", "-"));
        }
        public static string Hypersanitize(string data, bool replaceSpaceWithDash = false, bool allowDot = false)
        {
            if (replaceSpaceWithDash)
                data = data.Replace(" ", "-");

            char[] allowed;
            if (replaceSpaceWithDash && allowDot)
                allowed = "abcdefghijklmnopqrstuvwxyz0123456789-.".ToCharArray();
            else if (replaceSpaceWithDash)
                allowed = "abcdefghijklmnopqrstuvwxyz0123456789-".ToCharArray();
            else
                allowed = "abcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
            var output = "";
            foreach (var p in data.ToLower().ToCharArray())
                foreach (var c in allowed)
                    if (p == c) //If it's in the whitelist
                    {
                        output += p;
                        break;
                    }

            return output;
        }
    }
}