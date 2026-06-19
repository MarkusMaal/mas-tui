using CodeHollow.FeedReader;
using System.Text.RegularExpressions;

namespace MasFlashDrv.Config.News
{
    internal class Feed
    {
        public List<Article> Articles { get; set; } = [];

        public async Task Read()
        {
            Program.L.StatusText = "Uudiste allalaadimine";
            var feed = await FeedReader.ReadAsync("https://feeds.feedburner.com/markuseasjad_ee");
            foreach (var item in feed.Items)
            {
                Articles.Add(new Article
                {
                    Title = item.Title,
                    Content = RemoveHtmlTags(item.Content
                    .Replace("</p>", "\n\n")
                    .Replace("<p>", "")
                    .Replace("<b>", "**")
                    .Replace("</b>", "**")
                    .Replace("<li>", "\n* ")
                    .Replace("<pre>", "`")
                    .Replace("</pre>", "`")
                    .Replace("<br>", "\n")
                    .Replace("<br/>", "\n")
                    .Replace("</td>", "\n")
                    ),
                    LastModified = item.PublishingDate ?? new DateTime(),
                });
            }
        }

        public string GetArticle(int idx)
        {
            return Articles[idx].ToString();
        }

        public static string RemoveHtmlTags(string input)
        {
            return Regex.Replace(input, "<.*?>", string.Empty);
        }
    }
}
