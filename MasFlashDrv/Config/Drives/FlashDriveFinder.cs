namespace MasFlashDrv.Config.Drives
{
    internal class FlashDriveFinder
    {
        public List<Edition> Drives { get; set; } = [];

        public FlashDriveFinder()
        {
            Program.L.StatusText = "Mälupulkade otsimine";
            while (Drives.Count == 0)
            {
                foreach (var di in DriveInfo.GetDrives())
                {
                    if (Edition.IsDriveCompatible(di.RootDirectory.FullName))
                    {
                        Drives.Add(new Edition(di.RootDirectory.FullName));
                    }
                }
            }
        }
    }
}
