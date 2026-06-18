namespace MasTUICommon.Components;

public class FilePicker
{
    public delegate void FileOkHandler(object sender, FilePickerEventArgs e);
    public event FileOkHandler? FileOk;
    
    public delegate void FileCancelHandler(object sender, FilePickerEventArgs e);
    public event FileCancelHandler? FileCancel;
    
    public delegate void FileChangeHandler(object sender, FilePickerEventArgs e);
    public event FileChangeHandler? FileChange;
    
    private string StartDirectory { get; } = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    
    public string Directory { get; set; }
    
    public string? FileName { get; set; }

    private bool Break { get; set; }

    private int Skip { get; set; }

    private int SelectedIndex { get; set; }

    private bool InvertDirection { get; set; } = true;
    
    private readonly int _maxHeight = Console.WindowHeight - 5;

    private readonly string _ansiBg;
    private readonly string _ansiFg;

    public FilePicker(string ansiBg = "\e[100m", string ansiFg = "\e[97m", string? startDirectory = null)
    {
        if (startDirectory != null)
        {
            StartDirectory = startDirectory;
        }
        Directory = StartDirectory;
        _ansiBg = ansiBg;
        _ansiFg = ansiFg;
    }

    public void Show()
    {
        Cls();
        while (!Break)
        {
            Draw();
            Controller();
        }
    }

    private void Cls()
    {
        Console.Clear();
        const string exitHint = "Q/Esc ";
        ColorConsole.Write("~--" + _ansiBg + _ansiFg + ("Failisirvija").PadBoth(Console.WindowWidth - 2) + " ~--");
        Console.CursorLeft -= exitHint.Length + 1;
        ColorConsole.WriteLine($"~--{_ansiBg}\e[91m{exitHint}~--");
        Console.WriteLine($"Asukoht: {Directory}".PadBoth(Console.WindowWidth - 2));
    }

    private void Draw()
    {
        Console.SetCursorPosition(0, 2);
        var fsInfo = new DirectoryInfo(Directory)
            .GetFileSystemInfos("*.*", SearchOption.TopDirectoryOnly)
            .OrderBy(fI => fI is not DirectoryInfo)
            .ThenBy(fI => fI.Name)
            .Where(fI => (fI.Attributes & FileAttributes.Hidden) == 0).ToArray();
        
        if (fsInfo.Length == 0) return;

        FileName = Path.Join(fsInfo[Math.Min(SelectedIndex, fsInfo.Length - 1)].FullName);
        
        var maxWidth = fsInfo.Max(fI => fI.Name.Length + (fI is DirectoryInfo ? 1 : 0));
        if (Skip > fsInfo.Length - _maxHeight)
        {
            Skip = fsInfo.Length - _maxHeight;
            SelectedIndex = Skip;
        }
        var s = fsInfo
            .Skip(Skip); 
        var fileSystemInfos = s as FileSystemInfo[] ?? s.ToArray();
        foreach (var (i, f) in fileSystemInfos.Take(Math.Min(_maxHeight, fileSystemInfos.ToArray().Length)).Index())
        {
            var currentIndex = Skip < 0 ? i : Skip + i;
            var c = currentIndex == SelectedIndex ? "70" : "--";
            var arrows = currentIndex == SelectedIndex ? "<>" : "  ";
            var slash = f is DirectoryInfo ? "/" : "";
            var fName = $"{f.Name}{slash}".PadRight(maxWidth);
            ColorConsole.WriteLine($"~{c} {arrows[0]} {fName} {arrows[1]} ~--");
        }
        Console.SetCursorPosition(0, 0);
    }

    private void CheckIndex()
    {
        var fsInfo = new DirectoryInfo(Directory)
            .GetFileSystemInfos("*.*", SearchOption.TopDirectoryOnly)
            .OrderBy(fI => fI is not DirectoryInfo)
            .ThenBy(fI => fI.Name)
            .Where(fI => (fI.Attributes & FileAttributes.Hidden) == 0).ToArray();
        if (SelectedIndex < 0) SelectedIndex = 0;
        else if (SelectedIndex >= fsInfo.Length) SelectedIndex = fsInfo.Length - 1;

        if (Skip + _maxHeight < SelectedIndex && Skip > SelectedIndex) return;
        if (Skip + _maxHeight - 1 >= SelectedIndex) Skip--;
        if (Skip + 1 <= SelectedIndex) Skip++;
        FileChange?.Invoke(this, new FilePickerEventArgs(fsInfo[SelectedIndex].FullName));
    }
    
    private void Controller()
    {
        var keyInfo = Console.ReadKey(true);
        if (((keyInfo.Modifiers & ConsoleModifiers.Control) != 0))
        {
            OnFileCancel();
            return;
        }
        var k = keyInfo.Key;
        switch (k)
        {
            case ConsoleKey.UpArrow:
                SelectedIndex--;
                InvertDirection = true;
                CheckIndex();
                break;
            case ConsoleKey.DownArrow:
                SelectedIndex++;
                InvertDirection = false;
                CheckIndex();
                break;
            case ConsoleKey.Backspace:
            case ConsoleKey.LeftArrow:
                SelectedIndex = 0;
                Skip = 0;
                Directory = System.IO.Directory.GetParent(Directory)?.FullName ?? Directory;
                Cls();
                break;
            case ConsoleKey.Escape:
            case ConsoleKey.Q:
                OnFileCancel();
                break;
            case ConsoleKey.Enter:
            case ConsoleKey.RightArrow:
                if (File.Exists(FileName))
                {
                    OnFileOk();
                }
                else if (System.IO.Directory.Exists(FileName))
                {
                    SelectedIndex = 0;
                    Skip = 0;
                    // check if the directory is accessible before switching to it
                    try { _ = new DirectoryInfo(FileName).GetFiles(); }
                    catch (UnauthorizedAccessException) { break; }
                    Directory = FileName;
                    Cls();
                }
                break;
            case ConsoleKey.PageUp:
                if (Skip > 0)
                {
                    Skip -= _maxHeight;
                }

                if (Skip < 0) Skip = 0;
                SelectedIndex = Skip;
                break;
            case ConsoleKey.PageDown:
                Skip += _maxHeight;
                SelectedIndex = Skip;
                break;
        }
    }
    
    private void OnFileOk()
    {
        Break = true;
        FileOk?.Invoke(this, new FilePickerEventArgs(FileName));
    }
    
    private void OnFileCancel()
    {
        Break = true;
        FileCancel?.Invoke(this, new FilePickerEventArgs(FileName));
    }
}

public class FilePickerEventArgs(string? fileName) : EventArgs
{
    public string? FileName { get; } = fileName;
}