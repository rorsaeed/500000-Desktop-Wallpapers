using Microsoft.Win32;

using System;
using System.IO;
using System.Runtime.InteropServices;

public class WallpaperChanger
{
    private const int SPI_SETDESKWALLPAPER = 0x0014;
    private const int SPIF_UPDATEINIFILE = 0x01;
    private const int SPIF_SENDCHANGE = 0x02;

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr ILCreateFromPath([In, MarshalAs(UnmanagedType.LPWStr)] string pszPath);
    public static void SetWallpaper(string path)
    {
        if (!System.IO.File.Exists(path))
        {
            Console.WriteLine("The specified wallpaper file does not exist.");
            return;
        }

        SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
    }
    public static void SetWallpaperSlideshow(string folderPath, int intervalInSeconds)
    {
        //string slideshowPath = $"{folderPath}\\*.*";

        //// Combine the folder path and interval to form the parameter for setting slideshow
        //string parameter = $"{slideshowPath},{intervalInSeconds}";

        //// Set the wallpaper slideshow
        //SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, parameter, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);


        // Replace this path with the folder containing your images


        // Set the wallpaper slideshow
        // Set slideshow options
        // Set the desktop wallpaper slideshow
        //RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Wallpapers", true);
        //key.SetValue("SlideshowDirectory", folderPath);
        //key.SetValue("Interval", "1800000"); // 30 minutes interval
        //key.SetValue("Shuffle", "1"); // Enable shuffling

        //// Set wallpaper style to slideshow
        //SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, "", SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
        SetSlideshow(folderPath);

    }
    public const int SetDesktopWallpaper = 20;
    public const int UpdateIniFile = 0x01;
    public const int SendWinIniChange = 0x02;
    public static void SetSlideshow(string path)
    {
        RegistryKey keyz = Registry.CurrentUser.OpenSubKey("Control Panel\\Personalization\\Desktop Slideshow", true);
        //enable shuffle
        keyz.SetValue(@"LastTickHigh", 0);
        keyz.SetValue(@"LastTickLow", 0);
        //set to 10 minutes shuffle slideshow
        keyz.SetValue(@"Interval", 600000);
        keyz.SetValue(@"Shuffle", 1);
        keyz.Close();

        keyz = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", true);
        //"Fit" style
        keyz.SetValue(@"WallpaperStyle", "10");
        keyz.SetValue(@"TileWallpaper", "0");
        keyz.Close();
        SystemParametersInfo(SetDesktopWallpaper, 0, path, UpdateIniFile | SendWinIniChange);
        //Thread.Sleep(20000);
    }
    //public static void Main(string[] args)
    //{
    //    // Replace "wallpaper.jpg" with the path to your desired wallpaper image
    //    string wallpaperPath = @"C:\Path\To\Your\Wallpaper.jpg";
    //    SetWallpaper(wallpaperPath);
    //    Console.WriteLine("Wallpaper changed successfully.");
    //}
}