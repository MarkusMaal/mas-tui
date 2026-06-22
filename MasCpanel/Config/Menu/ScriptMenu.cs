using System.Xml.Linq;

namespace MasCpanel.Config.Menu;

public class ScriptMenu
{
    private readonly List<MenuItem> _menuItems = [];

    public ScriptMenu(XDocument xml)
    {
        if (xml.Root == null) ThrowFormatException();
        foreach (var el in xml.Root!.Elements())
        {
            if (el!.Element("Title") == null) ThrowFormatException();
            if (el.Element("Script") == null) ThrowFormatException();
            var title = el.Element("Title")!.Value;
            var tooltip = el.Element("Tooltip")?.Value;
            var script = new Script
            {
                Wait = el.Element("Script")!.Attribute("Wait")!.Value == "True"
            };
            foreach (var shEl in el.Element("Script")!.Elements())
            {
                var platform = shEl.Attribute("Platform");
                var status = shEl.Attribute("Status");
                var detach =  shEl.Attribute("Detach");
                var directory =  shEl.Attribute("Directory");
                var shell = new Shell();
                if (platform != null) shell.Platform = platform.Value;
                if (status != null) shell.Status = status.Value;
                if (detach != null) shell.Detach = detach.Value == "True";
                if (directory != null) shell.Directory = directory.Value;
                shell.Command = shEl.Value;
                script.AddCommand(shell);
            }
            _menuItems.Add(new MenuItem() {Script = script, Title = title, Tooltip = tooltip});
        }
    }

    public MenuItem[] GetMenuItems()
    {
        return [.. _menuItems];
    }

    private static void ThrowFormatException()
    {
        throw new FormatException("XML document is not in the correct format");
    }
}