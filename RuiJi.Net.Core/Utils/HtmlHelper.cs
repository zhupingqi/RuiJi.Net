using System.Text.RegularExpressions;

namespace RuiJi.Net.Core.Utils
{
    public class HtmlHelper
    {
        public static string ClearTag(string input)
        {
            input = Regex.Replace(input, "<script.*?>.*?</script>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "<style.*?>.*?</style>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "<iframe.*?>.*?</iframe>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "< type=\"text/javascript\">.*?</script>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"<div>\s*</div>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "<input.*?>", "", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "<input.*?/>", "", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "<textarea.*?>.*?</textarea>", "<textarea></textarea>", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "<!--.*?-->", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "<form.*?>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "</form>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            //input = Regex.Replace(input, "<font.*?>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            //input = Regex.Replace(input, "</font>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "<select.*?>.*?</select>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);

            return input.Trim();
        }

        public static string ClearTag(string input,string[] tags)
        {
            foreach (var tag in tags)
            {
                input = Regex.Replace(input, "<"+ tag +".*?>.*?</" + tag + ">", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            }

            return input.Trim();
        }
    }
}
