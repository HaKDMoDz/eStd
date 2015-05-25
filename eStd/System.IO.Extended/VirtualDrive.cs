using System.IO;
using System.Runtime.InteropServices;

namespace Creek.IO
{
    /// <summary>
    /// Erstellung und L�schung nicht persistenter virtueller Laufwerke.
    /// Das Laufwerk muss nach jedem Neustart des Systems wiederhergestellt werden.
    /// Wird nicht unter dem Registryschl�ssel angezeigt : HKLM\System\MountedDevices.
    /// 
    /// VirtualDrive - � Konstantin Gross
    /// </summary>
    public class VirtualDrive
    {
        #region Win32
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool DefineDosDevice(
            int dwFlags,
            string lpDeviceName,
            string lpTargetPath
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern int GetDriveType(
            string lpRootPathName
            );
        
        private const int DDD_RAW_TARGET_PATH = 0x00000001;
        private const int DDD_REMOVE_DEFINITION = 0x00000002;
        private const int DDD_EXACT_MATCH_ON_REMOVE = 0x00000004;
        
        private const int DRIVE_UNKNOWN = 0;
        private const int DRIVE_NO_ROOT_DIR = 1;
        private const int DRIVE_FIXED = 3;        
        #endregion // Win32

        #region �ffentliche Methoden        
        
        #region Erstellen
        /// <summary>
        /// Erstellung eines nicht persistenten Laufwerks.
        /// </summary>
        /// <param name="driveChar">Laufwerksbuchstabe.</param>
        /// <param name="path">Pfad zu dem zu verkn�pfenden Ordner.</param>
        /// <returns>True/False beim Versuch das Laufwerk zu erstellen</returns>
        public static bool Create(char driveChar, string path)
        {
            return DDDOperation(driveChar, path, true);
        }
        #endregion // Erstellen

        #region L�schen
        /// <summary>
        /// L�schung eines nicht persistenten Laufwerks.
        /// </summary>
        /// <param name="driveChar">Laufwerksbuchstabe.</param>
        /// <param name="path">Pfad zu dem zu verkn�pfenden Ordner.</param>
        /// <returns>True/False beim Versuch das Laufwerk zu l�schen</returns>
        public static bool Delete(char driveChar, string path)
        {
            return DDDOperation(driveChar, path, false);
        }
        #endregion // L�schen

        #endregion // �ffentliche Methoden

        #region Private Methoden
        
        #region DDDOperationen
        private static bool DDDOperation(char driveChar, string path, bool create)
        {
            //G�ltiges Verzeichnis?
            if (!Directory.Exists(path))
                return false;

            string drive = string.Format("{0}:", driveChar.ToString().ToUpper());
            
            //Existiert das Volumen?
            int type = GetDriveType(string.Format("{0}{1}", drive, Path.DirectorySeparatorChar));

            //Hinwei�: Ein erstelltes virtuelles Laufwerk ist vom Typ DRIVE_FIXED
            if ((create && type != DRIVE_UNKNOWN && type != DRIVE_NO_ROOT_DIR) ||
                (!create && type != DRIVE_FIXED))
                return false;

            int flags = DDD_RAW_TARGET_PATH;

            if (!create) flags |= (DDD_REMOVE_DEFINITION | DDD_EXACT_MATCH_ON_REMOVE);
            return DefineDosDevice(
                flags,
                drive,
                string.Format("{0}??{0}{1}", Path.DirectorySeparatorChar, path)
                );
        }
        #endregion // DDDOperationen

        #endregion // Private Methoden
    }
}