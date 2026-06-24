using CodeHollow.FeedReader;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace MasFlashDrv.Config.News
{
    internal partial class Feed
    {
        public List<Article> Articles { get; set; } = [];

        public void Read(string xmlFile)
        {
            Program.L.StatusText = "Uudiste laadimine";
            var feed = FeedReader.ReadFromFile(xmlFile);
            ParseNews(feed);
        }

        public async Task Read()
        {
            Program.L.StatusText = "Uudiste allalaadimine";
            try
            {
                var feed = await FeedReader.ReadAsync("https://feeds.feedburner.com/markuseasjad_ee");
                ParseNews(feed);
                var feedStr = XDocument.Parse(feed.OriginalDocument).ToString();
                File.WriteAllText(Path.Join(Path.GetTempPath(), "mas_flashdrv_feed.xml"), feedStr);
            }
            catch (HttpRequestException)
            {

            }
            catch (IOException)
            {
                
            }
        }

        private void ParseNews(CodeHollow.FeedReader.Feed feed)
        {
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
                    .Replace("</ul>", "\n\n")
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
            return NoHtmlTags().Replace(input, string.Empty);
        }

        [GeneratedRegex("<.*?>")]
        private static partial Regex NoHtmlTags();
    }
}
