using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace System.Extensions
{
    /// <summary>
    /// Helper Methods for Connecting SysImageList to Common Controls.
    /// </summary>
    public class SysImageListHelper
    {
        #region UnmanagedMethods

        private const int LVMFIRST = 0x1000;
        private const int LVMSETIMAGELIST = LVMFIRST + 3;

        private const int LVSILNORMAL = 0;
        private const int LVSILSMALL = 1;
        private const int LVSILSTATE = 2;

        private const int TVFIRST = 0x1100;
        private const int TVMSETIMAGELIST = TVFIRST + 9;

        private const int TVSILNORMAL = 0;
        private const int TVSILSTATE = 2;

        #endregion

        /// <summary>
        /// Associates a SysImageList with a ListView control
        /// </summary>
        /// <param name="listView">ListView control to associate ImageList with</param>
        /// <param name="sysImageList">System Image List to associate</param>
        /// <param name="forStateImages">Whether to add ImageList as StateImageList</param>
        public static void SetListViewImageList(ListView listView, SysImageList sysImageList, bool forStateImages)
        {
            IntPtr param = (IntPtr)LVSILNORMAL;

            if (sysImageList.ImageListSize == SysImageListSize.smallIcons)
            {
                param = (IntPtr)LVSILSMALL;
            }

            if (forStateImages)
            {
                param = (IntPtr)LVSILSTATE;
            }

            SendMessage(listView.Handle, LVMSETIMAGELIST, param, sysImageList.Handle);
        }

        /// <summary>
        /// Associates a SysImageList with a TreeView control
        /// </summary>
        /// <param name="treeView">TreeView control to associated ImageList with</param>
        /// <param name="sysImageList">System Image List to associate</param>
        /// <param name="forStateImages">Whether to add ImageList as StateImageList</param>
        public static void SetTreeViewImageList(TreeView treeView, SysImageList sysImageList, bool forStateImages)
        {
            IntPtr param = (IntPtr)TVSILNORMAL;

            if (forStateImages)
            {
                param = (IntPtr)TVSILSTATE;
            }

            SendMessage(treeView.Handle, TVMSETIMAGELIST, param, sysImageList.Handle);
        }

        [DllImport("user32", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(
            IntPtr messageHandle,
            int message,
            IntPtr param1,
            IntPtr param2);
    }
}
