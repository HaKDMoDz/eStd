﻿using System;
using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.Win32;

//----------------------------------------------'
//--------------- SystemFileType ---------------'
//----------------------------------------------'

namespace Creek.IO.FilterBuilder
{
    /// <summary>
    /// A class providing methods for dealing with system files.
    /// </summary>
    internal class SystemFileType
    {
        /// <summary>
        /// Used for containing file types and their icons information.
        /// </summary>
        public static Hashtable iconsInfo;

        /// <summary>
        /// The current size of the icons.
        /// </summary>
        public static FilterBuilder.ImageSize currentSize;

        [DllImport("shell32.dll", EntryPoint = "ExtractIconA", CharSet = CharSet.Ansi, SetLastError = true,
            ExactSpelling = true)]

        private static extern IntPtr ExtractIcon(int hInst, string lpszExeFileName, int nIconIndex);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern uint ExtractIconEx(string szFileName, int nIconIndex, IntPtr[] phiconLarge,
                                                 IntPtr[] phiconSmall, uint nIcons);

        [DllImport("user32.dll", EntryPoint = "DestroyIcon", SetLastError = true)]
        private static extern int DestroyIcon(IntPtr hIcon);

        #region " Core Methods "

        /// <summary>
        /// Gets registered file types and their associated icon in the system.
        /// </summary>
        /// <returns>Returns a hash table which contains the file extension as keys, the icon file and param as values.</returns>
        public static Hashtable GetFileTypeAndIcon()
        {
            try
            {
                // Create a registry key object to represent the HKEY_CLASSES_ROOT registry section
                RegistryKey rkRoot = Registry.ClassesRoot;

                //Gets all sub keys' names.
                string[] keyNames = rkRoot.GetSubKeyNames();
                var iconsInfo = new Hashtable();

                //Find the file icon.
                foreach (string keyName in keyNames)
                {
                    if (String.IsNullOrEmpty(keyName))
                    {
                        continue;
                    }
                    int indexOfPoint = keyName.IndexOf(".");

                    //If this key is not a file exttension(eg, .zip), skip it.
                    if (indexOfPoint != 0)
                    {
                        continue;
                    }

                    RegistryKey rkFileType = rkRoot.OpenSubKey(keyName);
                    if (rkFileType == null)
                    {
                        continue;
                    }

                    //Gets the default value of this key that contains the information of file type.
                    object defaultValue = rkFileType.GetValue("");
                    if (defaultValue == null)
                    {
                        continue;
                    }

                    //Go to the key that specifies the default icon associates with this file type.
                    string defaultIcon = defaultValue + "\\DefaultIcon";
                    RegistryKey rkFileIcon = rkRoot.OpenSubKey(defaultIcon);
                    if (rkFileIcon != null)
                    {
                        //Get the file contains the icon and the index of the icon in that file.
                        object value = rkFileIcon.GetValue("");
                        if (value != null)
                        {
                            //Clear all unecessary " sign in the string to avoid error.
                            string fileParam = value.ToString().Replace("\"", "");
                            iconsInfo.Add(keyName, fileParam);
                        }
                        rkFileIcon.Close();
                    }
                    rkFileType.Close();
                }
                rkRoot.Close();
                return iconsInfo;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// Extract the icon from file.
        /// </summary>
        /// <param name="fileAndParam">The params string, 
        /// such as ex: "C:\\Program Files\\NetMeeting\\conf.exe,1".</param>
        /// <returns>This method always returns the large size of the icon (may be 32x32 px).</returns>
        public static Icon ExtractIconFromFile(string fileAndParam)
        {
            try
            {
                EmbeddedIconInfo embeddedIcon = GetEmbeddedIconInfo(fileAndParam);

                //Gets the handle of the icon.
                IntPtr lIcon = ExtractIcon(0, embeddedIcon.FileName, embeddedIcon.IconIndex);

                //Gets the real icon.
                return Icon.FromHandle(lIcon);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// Extract the icon from file.
        /// </summary>
        /// <param name="fileAndParam">The params string, 
        /// such as ex: "C:\\Program Files\\NetMeeting\\conf.exe,1".</param>
        /// <param name="isLarge">
        /// Determines the returned icon is a large (may be 32x32 px) 
        /// or small icon (16x16 px).</param>
        public static Icon ExtractIconFromFile(string fileAndParam, bool isLarge)
        {
            uint readIconCount = 0;
            var hDummy = new IntPtr[1] {IntPtr.Zero};
            var hIconEx = new IntPtr[1] {IntPtr.Zero};

            try
            {
                EmbeddedIconInfo embeddedIcon = GetEmbeddedIconInfo(fileAndParam);

                if (isLarge)
                {
                    readIconCount = ExtractIconEx(embeddedIcon.FileName, 0, hIconEx, hDummy, 1);
                }
                else
                {
                    readIconCount = ExtractIconEx(embeddedIcon.FileName, 0, hDummy, hIconEx, 1);
                }

                if (readIconCount > 0 && hIconEx[0] != IntPtr.Zero)
                {
                    // Get first icon.
                    var extractedIcon = (Icon) Icon.FromHandle(hIconEx[0]).Clone();

                    return extractedIcon;
                }
                else
                {
                    // No icon read
                    return null;
                }
            }
            catch (Exception exc)
            {
                // Extract icon error.
                throw new ApplicationException("Could not extract icon", exc);
            }
            finally
            {
                // Release resources.
                foreach (IntPtr ptr in hIconEx)
                {
                    if (ptr != IntPtr.Zero)
                    {
                        DestroyIcon(ptr);
                    }
                }

                foreach (IntPtr ptr in hDummy)
                {
                    if (ptr != IntPtr.Zero)
                    {
                        DestroyIcon(ptr);
                    }
                }
            }
        }

        #endregion

        #region " Utility Methods "

        /// <summary>
        /// Parses the parameters string to the structure of EmbeddedIconInfo.
        /// </summary>
        /// <param name="fileAndParam">The params string, 
        /// such as ex: "C:\\Program Files\\NetMeeting\\conf.exe,1".</param>
        /// <returns></returns>
        protected static EmbeddedIconInfo GetEmbeddedIconInfo(string fileAndParam)
        {
            var embeddedIcon = new EmbeddedIconInfo();

            if (String.IsNullOrEmpty(fileAndParam))
            {
                return embeddedIcon;
            }

            //Use to store the file contains icon.
            string fileName = String.Empty;

            //The index of the icon in the file.
            int iconIndex = 0;
            string iconIndexString = String.Empty;

            int commaIndex = fileAndParam.IndexOf(",");
            //if fileAndParam is some thing likes that: "C:\\Program Files\\NetMeeting\\conf.exe,1".
            if (commaIndex > 0)
            {
                fileName = fileAndParam.Substring(0, commaIndex);
                iconIndexString = fileAndParam.Substring(commaIndex + 1);
            }
            else
            {
                fileName = fileAndParam;
            }

            if (!String.IsNullOrEmpty(iconIndexString))
            {
                //Get the index of icon.
                iconIndex = int.Parse(iconIndexString);
                if (iconIndex < 0)
                {
                    iconIndex = 0;
                    //To avoid the invalid index.
                }
            }

            embeddedIcon.FileName = fileName;
            embeddedIcon.IconIndex = iconIndex;

            return embeddedIcon;
        }

        /// <summary>
        /// Returns the icon associates with a specific file type.
        /// </summary>
        /// <param name="fileType">The type of file (or file extension).</param>
        public static Icon ReturnIcon(string fileType)
        {
            try
            {
                string fileAndParam = (iconsInfo[fileType]).ToString();

                if (String.IsNullOrEmpty(fileAndParam))
                {
                    return null;
                }

                Icon icon = null;

                bool isLarge = currentSize != FilterBuilder.ImageSize.Small;

                //Get the icon
                icon = ExtractIconFromFile(fileAndParam, isLarge);

                //The icon cannot be zero.
                if (icon != null)
                {
                    //Return the icon
                    return icon;
                }
                else
                {
                    //Return nothing
                    return null;
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        #endregion

        #region Nested type: EmbeddedIconInfo

        /// <summary>
        /// Structure that encapsulates basic information of icon embedded in a file.
        /// </summary>
        public struct EmbeddedIconInfo
        {
            public string FileName;
            public int IconIndex;
        }

        #endregion
    }
}