using System.Xml.Linq;
using MasCpanel.Config.Menu;
using MasTUICommon;
using MasTUICommon.Components;

namespace MasCpanel.Tabs;

public class Home : TabBase
{
    private readonly Menu _homeMenu;
    

    public Home()
    {
        ScriptMenu? scriptMenu = null;
        _homeMenu = new Menu
        {
            ActiveColor = new Color
            {
                BackgroundColor = 0x7,
                ForegroundColor = 0x0,
            },
            DefaultColor = new Color
            {
                BackgroundColor = 0x10,
                ForegroundColor = 0x7,
            },
            TextPadding = Console.WindowWidth  - 8,
        };
        var scriptMenuFile =Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mas", "ScriptMenu.xml");
        if (File.Exists(scriptMenuFile))
        {
            scriptMenu = new ScriptMenu(XDocument.Load(scriptMenuFile));
            foreach (var mi in scriptMenu.GetMenuItems()) _homeMenu.AddItem(mi.Title, (_, _) => { mi.Script.Run(); }, mi.Tooltip);
        }

        _homeMenu.MarginTop = 3;
        _homeMenu.MarginLeft = 1;
        if (scriptMenu == null) return;
        _homeMenu.TextPadding = scriptMenu.GetMenuItems().Max(sm => sm.Title.Length) + 6;
    }

    public override bool VerifileOk { get; init; } = File.Exists(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mas", "ScriptMenu.xml"));

    public override void ReceiveKey(object sender, ConsoleKey key)
    {
        if (sender is not MainScreen mainScreen) return;
        switch (key)
        {
            case ConsoleKey.DownArrow:
                _homeMenu.MoveDown();
                break;
            case ConsoleKey.UpArrow:
                _homeMenu.MoveUp();
                break;
            case ConsoleKey.Enter:
                _homeMenu.Execute();
                mainScreen.Cls();
                break;
        }
    }

    public override void Draw(object sender, EventArgs e)
    {
        _homeMenu.Draw();
    }
}