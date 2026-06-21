namespace MasFlashDrv.Config.Drives
{
    internal class FlashDriveFinder
    {
        public List<Edition> Drives { get; set; } = [];

        public FlashDriveFinder()
        {
            while (Drives.Count == 0)
            {
                foreach (var di in DriveInfo.GetDrives())
                {
                    if (Edition.IsDriveCompatible(di.RootDirectory.FullName))
                    {
                        Drives.Add(new Edition(di.RootDirectory.FullName));
                    }
                }
                Program.L.StatusText = "Mälupulkade otsimine";

                if (Drives.Count == 0)
                {
                    Thread.Sleep(5000);
                }
            }
        }
    }
}
