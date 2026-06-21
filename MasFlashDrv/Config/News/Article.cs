using MasTUICommon;
using System.Text;

namespace MasFlashDrv.Config.News
{
    internal class Article
    {
        public required string Title { get; set; }
        public required string Content { get; set; }
        public DateTime LastModified { get; set; }

        public int GetLastSkip(int constraint)
        {
            return Utils.WrapLines(ToString(), constraint).Split('\n').Length - Console.WindowHeight + 1;
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine("~-F" + Title);
            sb.AppendLine("~-8" + LastModified.ToString("g") + "~--");
            sb.AppendLine();
            sb.AppendLine(Content);
            return sb.ToString();
        }
    }
}
