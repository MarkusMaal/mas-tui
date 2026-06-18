namespace MasCpanel.Config.Menu;

// <Script> tags in ScriptMenu.xml
public class Script
{
    public bool Wait { get; set; } = false;
    private Shell[]? Commands { get; set; }

    public void AddCommand(Shell command)
    {
        Commands ??= [];
        var cmds = new Shell[Commands.Length + 1];
        Array.Copy(Commands, cmds, Commands.Length);
        cmds[Commands.Length] = command;
        Commands = cmds;
    }

    public void Run()
    {
        if (Commands == null) return;
        if (Wait)
        {
            Program.L.StatusText = "Skripti jooksutamine";
            new Thread(Program.SpinLoader).Start();
        }
        foreach (var cmd in Commands)
        {
            if (Wait) cmd.Run();
            else new Thread(cmd.Run).Start();
        }
        if (Wait) Program.L.StatusText = "";
    }
}