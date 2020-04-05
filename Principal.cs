using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32;

namespace Kenose
{
    public static class Principal
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni); //método WINAPI para cambiar fondo pantalla
        const int SPI_SETDESKWALLPAPER = 0x0014;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;
        private static void CambiarFondo() //payload 1 -> cambiar fondo de pantalla
        {
            string temp = Path.GetTempPath() + "fondo.jpg";
            Kenose.Properties.Resources.fondo.Save(temp); //guardar el fondo en una localización temporal
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true); //stretch
            key.SetValue(@"WallpaperStyle", 2.ToString());
            key.SetValue(@"TileWallpaper", 0.ToString());
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, temp, SPIF_SENDWININICHANGE | SPIF_UPDATEINIFILE); //aplicar cambio de fondo
        }

        public static void Main(string[] args)
        {
            CambiarFondo();
        }
    }
}
