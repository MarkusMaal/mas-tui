namespace MasTUICommon;

public class Edition
{
    /// <summary>
    /// Edition name (e.g. Pro, Premium, Basic+)
    /// </summary>
    public string EditionName { get; set; }
    
    /// <summary>
    /// Version number (e.g. 10.4)
    /// </summary>
    public string Version { get; set; }
    
    /// <summary>
    /// Build number - first letter(s) represent(s) the initial(s) for the edition name, next few numbers represent major version number and remaining numbers represent minor revisions. The last lowercase letter represents device type (a = physical desktop computer, b = virtual computer, c = tablet)
    /// </summary>
    public string BuildNo { get; set; }
    
    /// <summary>
    /// Boolean representing if a system integrity check has been run during the deployment process, stored in edition.txt as either "Yes" or "No"
    /// </summary>
    public bool Tested { get; set; }
    
    /// <summary>
    /// The user who initially started the deployment process for this computer
    /// </summary>
    public string Username { get; set; }
    
    /// <summary>
    /// System language during the deployment process
    /// </summary>
    public string Language { get; set; }
    
    /// <summary>
    /// Operating system kernel version during the initial deployment process
    /// </summary>
    public string WinVer { get; set; }
    
    /// <summary>
    /// List of optional features, stored in edition.txt with dashes (-) used as separators
    /// </summary>
    public List<string>? Features { get; set; }
    
    /// <summary>
    /// Insecure PIN code for this computer, for legacy compatibility
    /// </summary>
    public string Pin { get; set; }
    
    /// <summary>
    /// Name for the current version
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Verifile 1.0 hash
    /// </summary>
    public string Hash { get; set; }

    /// <summary>
    /// Root directory for Markus' stuff deployment
    /// </summary>
    private static string MasRoot => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.mas";

    public Edition()
    {
        var lines =  File.ReadAllLines(Path.Combine(MasRoot, "edition.txt"));
        if (lines[0] != "[Edition_info]") throw new FormatException("The edition file does not start with [Edition_info]");
        EditionName = lines[1];
        Version = lines[2];
        BuildNo = lines[3];
        Tested = lines[4] == "Yes";
        Username = lines[5];
        Language = lines[6];
        WinVer = lines[7];
        Features = lines[8].Split('-').ToList();
        Pin = lines[9];
        Name = lines[10];
        Hash = lines[11];
    }
}