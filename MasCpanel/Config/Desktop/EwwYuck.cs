namespace MasCpanel.Config.Desktop;

// ~/.config/eww/eww.yuck
public class EwwYuck
{
    public List<DesktopEntry> Entries { get; set; } = [];
    
    public void LoadConfig()
    {
        var cfgPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "eww",
            "eww.yuck");
        if (!File.Exists(cfgPath))
        {
            throw new FileNotFoundException("Eww configuration file does not exist");
        }
        TextReader tr = File.OpenText(cfgPath);
        var cfg = tr.ReadToEnd();
        tr.Close();
        tr.Dispose();
        foreach (var container in cfg.Split("(defwidget container []")[1]
                     .Split("(box :class \"container\" :orientation \"h\"").Skip(1))
        {
            foreach (var icon in container.Split("(box :class \"icon\" :style \"background-image: url(\'").Skip(1))
            {
                var image = icon.Split('\'')[0];
                var tooltip = icon.Split("button :tooltip \"")[1].Split('\"')[0];
                var scriptArr = icon.Split(":onclick \"")[1].Split("\"")[0].Split(".sh ");
                var script = string.Join(".sh ", scriptArr.Skip(1));
                Entries.Add(new DesktopEntry
                {
                    Image = image,
                    Tooltip = tooltip,
                    Executable = script
                });
            }
        }
    }
}