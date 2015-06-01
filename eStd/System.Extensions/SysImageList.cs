using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Extensions
{

    #region Public Enumerations

    /// <summary>
    /// Available system image list sizes
    /// </summary>
    public enum SysImageListSize : int
    {
        /// <summary>
        /// System Large Icon Size (typically 32x32)
        /// </summary>
        largeIcons = 0x0,

        /// <summary>
        /// System Small Icon Size (typically 16x16)
        /// </summary>
        smallIcons = 0x1,

        /// <summary>
        /// System Extra Large Icon Size (typically 48x48).
        /// Only available under XP; under other OS the
        /// Large Icon ImageList is returned.
        /// </summary>
        extraLargeIcons = 0x2
    }

    /// <summary>
    /// Flags controlling how the Image List item is 
    /// drawn
    /// </summary>
    [Flags]
    public enum ImageListDrawItemConstants : int
    {
        /// <summary>
        /// Draw item normally.
        /// </summary>
        ILD_NORMAL = 0x0,

        /// <summary>
        /// Draw item transparently.
        /// </summary>
        ILD_TRANSPARENT = 0x1,

        /// <summary>
        /// Draw item blended with 25% of the specified foreground colour
        /// or the Highlight colour if no foreground colour specified.
        /// </summary>
        ILD_BLEND25 = 0x2,

        /// <summary>
        /// Draw item blended with 50% of the specified foreground colour
        /// or the Highlight colour if no foreground colour specified.
        /// </summary>
        ILD_SELECTED = 0x4,

        /// <summary>
        /// Draw the icon's mask
        /// </summary>
        ILD_MASK = 0x10,

        /// <summary>
        /// Draw the icon image without using the mask
        /// </summary>
        ILD_IMAGE = 0x20,

        /// <summary>
        /// Draw the icon using the ROP specified.
        /// </summary>
        ILD_ROP = 0x40,

        /// <summary>
        /// Preserves the alpha channel in dest. XP only.
        /// </summary>
        ILD_PRESERVEALPHA = 0x1000,

        /// <summary>
        /// Scale the image to cx, cy instead of clipping it.  XP only.
        /// </summary>
        ILD_SCALE = 0x2000,

        /// <summary>
        /// Scale the image to the current DPI of the display. XP only.
        /// </summary>
        ILD_DPISCALE = 0x4000
    }

    /// <summary>
    /// Enumeration containing XP ImageList Draw State options
    /// </summary>
    [Flags]
    public enum ImageListDrawStateConstants : int
    {
        /// <summary>
        /// The image state is not modified. 
        /// </summary>
        ILS_NORMAL = (0x00000000),

        /// <summary>
        /// Adds a glow effect to the icon, which causes the icon to appear to glow 
        /// with a given color around the edges. (Note: does not appear to be
        /// implemented)
        /// </summary>
        ILS_GLOW = (0x00000001), // The color for the glow effect is passed to the IImageList::Draw method in the crEffect member of IMAGELISTDRAWPARAMS. 
        
        /// <summary>
        /// Adds a drop shadow effect to the icon. (Note: does not appear to be
        /// implemented)
        /// </summary>
        ILS_SHADOW = (0x00000002), // The color for the drop shadow effect is passed to the IImageList::Draw method in the crEffect member of IMAGELISTDRAWPARAMS. 
        
        /// <summary>
        /// Saturates the icon by increasing each color component 
        /// of the RGB triplet for each pixel in the icon. (Note: only ever appears
        /// to result in a completely unsaturated icon)
        /// </summary>
        ILS_SATURATE = (0x00000004), // The amount to increase is indicated by the frame member in the IMAGELISTDRAWPARAMS method. 
        
        /// <summary>
        /// Alpha blends the icon. Alpha blending controls the transparency 
        /// level of an icon, according to the value of its alpha channel. 
        /// (Note: does not appear to be implemented).
        /// </summary>
        ILS_ALPHA = (0x00000008) // The value of the alpha channel is indicated by the frame member in the IMAGELISTDRAWPARAMS method. The alpha channel can be from 0 to 255, with 0 being completely transparent, and 255 being completely opaque. 
    }

    /// <summary>
    /// Flags specifying the state of the icon to draw from the Shell
    /// </summary>
    [Flags]
    public enum ShellIconStateConstants
    {
        /// <summary>
        /// Get icon in normal state
        /// </summary>
        ShellIconStateNormal = 0,

        /// <summary>
        /// Put a link overlay on icon 
        /// </summary>
        ShellIconStateLinkOverlay = 0x8000,

        /// <summary>
        /// show icon in selected state 
        /// </summary>
        ShellIconStateSelected = 0x10000,

        /// <summary>
        /// get open icon 
        /// </summary>
        ShellIconStateOpen = 0x2,

        /// <summary>
        /// apply the appropriate overlays
        /// </summary>
        ShellIconAddOverlays = 0x000000020,
    }
    #endregion

    /// <summary>
    /// Allows for retrieval of large icons from files.
    /// </summary>
    public class SysImageList : IDisposable
    {
        #region Constants

        /// <summary>
        /// Maximum path.
        /// </summary>
        private const int MAXPATH = 260;
        private const int FILEATTRIBUTENORMAL = 0x80;
        private const int FILEATTRIBUTEDIRECTORY = 0x10;
        private const int FORMATMESSAGEALLOCATEBUFFER = 0x100;
        private const int FORMATMESSAGEARGUMENTARRAY = 0x2000;
        private const int FORMATMESSAGEFROMHMODULE = 0x800;
        private const int FORMATMESSAGEFROMSTRING = 0x400;
        private const int FORMATMESSAGEFROMSYSTEM = 0x1000;
        private const int FORMATMESSAGEIGNOREINSERTS = 0x200;
        private const int FORMATMESSAGEMAXWIDTHMASK = 0xFF;

        #endregion

        #region Member Variables

        private IntPtr himl = IntPtr.Zero;
        private IImageList imageList = null;
        private SysImageListSize size = SysImageListSize.smallIcons;
        private bool disposed = false;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the SysImageList class.
        /// </summary>
        public SysImageList()
        {
            this.Create();
        }

        /// <summary>
        /// Initializes a new instance of the SysImageList class.
        /// </summary>
        /// <param name="size">Size of System ImageList</param>
        public SysImageList(SysImageListSize size)
        {
            this.size = size;
            this.Create();
        }

        #endregion

        #region Destructor

        /// <summary>
        /// Finalizes an instance of the SysImageList class.
        /// </summary>
        ~SysImageList()
        {
            this.Dispose(false);
        }

        #endregion

        #region Private Enumerations

        [Flags]
        private enum SHGetFileInfoConstants : int
        {
            SHGFI_ICON = 0x100,                // get icon 
            SHGFI_DISPLAYNAME = 0x200,         // get display name 
            SHGFI_TYPENAME = 0x400,            // get type name 
            SHGFI_ATTRIBUTES = 0x800,          // get attributes 
            SHGFI_ICONLOCATION = 0x1000,       // get icon location 
            SHGFI_EXETYPE = 0x2000,            // return exe type 
            SHGFI_SYSICONINDEX = 0x4000,       // get system icon index 
            SHGFI_LINKOVERLAY = 0x8000,        // put a link overlay on icon 
            SHGFI_SELECTED = 0x10000,          // show icon in selected state 
            SHGFI_ATTR_SPECIFIED = 0x20000,    // get only specified attributes 
            SHGFI_LARGEICON = 0x0,             // get large icon 
            SHGFI_SMALLICON = 0x1,             // get small icon 
            SHGFI_OPENICON = 0x2,              // get open icon 
            SHGFI_SHELLICONSIZE = 0x4,         // get shell size icon
            SHGFI_USEFILEATTRIBUTES = 0x10,    // use passed dwFileAttribute 
            SHGFI_ADDOVERLAYS = 0x000000020,   // apply the appropriate overlays
            SHGFI_OVERLAYINDEX = 0x000000040   // Get the index of the overlay
        }

        #endregion

        #region Private Interfaces

        [ComImport()]
        [Guid("46EB5926-582E-4017-9FDF-E8998DAA0950")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IImageList
        {
            [PreserveSig]
            int Add(
                IntPtr maskedImage,
                IntPtr hbmMask,
                ref int pi);

            [PreserveSig]
            int ReplaceIcon(
                int i,
                IntPtr hicon,
                ref int pi);

            [PreserveSig]
            int SetOverlayImage(
                int image,
                int overlay);

            [PreserveSig]
            int Replace(
                int i,
                IntPtr maskedImage,
                IntPtr mask);

            [PreserveSig]
            int AddMasked(
                IntPtr maskedImage,
                int mask,
                ref int pi);

            [PreserveSig]
            int Draw(
                ref IMAGELISTDRAWPARAMS pimldp);

            [PreserveSig]
            int Remove(
                int i);

            [PreserveSig]
            int GetIcon(
                int i,
                int flags,
                ref IntPtr picon);

            [PreserveSig]
            int GetImageInfo(
                int i,
                ref IMAGEINFO imageInfo);

            [PreserveSig]
            int Copy(
                int destination,
                IImageList punkSrc,
                int source,
                int flags);

            [PreserveSig]
            int Merge(
                int i1,
                IImageList punk2,
                int i2,
                int dx,
                int dy,
                ref Guid riid,
                ref IntPtr ppv);

            [PreserveSig]
            int Clone(
                ref Guid riid,
                ref IntPtr ppv);

            [PreserveSig]
            int GetImageRect(
                int i,
                ref RECT prc);

            [PreserveSig]
            int GetIconSize(
                ref int cx,
                ref int cy);

            [PreserveSig]
            int SetIconSize(
                int cx,
                int cy);

            [PreserveSig]
            int GetImageCount(
                ref int pi);

            [PreserveSig]
            int SetImageCount(
                int newCount);

            [PreserveSig]
            int SetBkColor(
                int clrBk,
                ref int pclr);

            [PreserveSig]
            int GetBkColor(
                ref int pclr);

            [PreserveSig]
            int BeginDrag(
                int track,
                int hotspotX,
                int hotspotY);

            [PreserveSig]
            int EndDrag();

            [PreserveSig]
            int DragEnter(
                IntPtr lockHandle,
                int x,
                int y);

            [PreserveSig]
            int DragLeave(
                IntPtr lockHandle);

            [PreserveSig]
            int DragMove(
                int x,
                int y);

            [PreserveSig]
            int SetDragCursorImage(
                ref IImageList punk,
                int drag,
                int hotspotX,
                int hotspotY);

            [PreserveSig]
            int DragShowNolock(
                int show);

            [PreserveSig]
            int GetDragImage(
                ref POINT ppt,
                ref POINT pptHotspot,
                ref Guid riid,
                ref IntPtr ppv);

            [PreserveSig]
            int GetItemFlags(
                int i,
                ref int flags);

            [PreserveSig]
            int GetOverlayImage(
                int overlay,
                ref int index);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the hImageList handle
        /// </summary>
        public IntPtr Handle
        {
            get
            {
                return this.himl;
            }
        }

        /// <summary>
        /// Gets or sets the size of System Image List to retrieve.
        /// </summary>
        public SysImageListSize ImageListSize
        {
            get
            {
                return this.size;
            }

            set
            {
                this.size = value;

                this.Create();
            }
        }

        /// <summary>
        /// Gets the size of the Image List Icons.
        /// </summary>
        public System.Drawing.Size Size
        {
            get
            {
                int cx = 0;
                int cy = 0;

                if (this.imageList == null)
                {
                    ImageList_GetIconSize(this.himl, ref cx, ref cy);
                }
                else
                {
                    this.imageList.GetIconSize(ref cx, ref cy);
                }

                System.Drawing.Size sz = new System.Drawing.Size(cx, cy);

                return sz;
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Clears up any resources associated with the SystemImageList
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Clears up any resources associated with the SystemImageList
        /// when disposing is true.
        /// </summary>
        /// <param name="disposing">Whether the object is being disposed</param>
        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.imageList != null)
                    {
                        Marshal.ReleaseComObject(this.imageList);
                    }

                    this.imageList = null;
                }
            }

            this.disposed = true;
        }

        /// <summary>
        /// Returns a GDI+ copy of the icon from the ImageList
        /// at the specified index.
        /// </summary>
        /// <param name="index">The index to get the icon for</param>
        /// <returns>The specified icon</returns>
        public Icon Icon(int index)
        {
            Icon icon = null;

            IntPtr handleIcon = IntPtr.Zero;
            if (this.imageList == null)
            {
                handleIcon = ImageList_GetIcon(this.himl, index, (int)ImageListDrawItemConstants.ILD_TRANSPARENT);
            }
            else
            {
                this.imageList.GetIcon(index, (int)ImageListDrawItemConstants.ILD_TRANSPARENT, ref handleIcon);
            }

            if (handleIcon != IntPtr.Zero)
            {
                icon = System.Drawing.Icon.FromHandle(handleIcon);
            }

            return icon;
        }

        /// <summary>
        /// Return the index of the icon for the specified file, always using 
        /// the cached version where possible.
        /// </summary>
        /// <param name="fileName">Filename to get icon for</param>
        /// <returns>Index of the icon</returns>
        public int IconIndex(string fileName)
        {
            return this.IconIndex(fileName, false);
        }

        /// <summary>
        /// Returns the index of the icon for the specified file
        /// </summary>
        /// <param name="fileName">Filename to get icon for</param>
        /// <param name="forceLoadFromDisk">If True, then hit the disk to get the icon,
        /// otherwise only hit the disk if no cached icon is available.</param>
        /// <returns>Index of the icon</returns>
        public int IconIndex(string fileName, bool forceLoadFromDisk)
        {
            return this.IconIndex(fileName, forceLoadFromDisk, ShellIconStateConstants.ShellIconStateNormal);
        }

        /// <summary>
        /// Returns the index of the icon for the specified file
        /// </summary>
        /// <param name="fileName">Filename to get icon for</param>
        /// <param name="forceLoadFromDisk">If True, then hit the disk to get the icon,
        /// otherwise only hit the disk if no cached icon is available.</param>
        /// <param name="iconState">Flags specifying the state of the icon
        /// returned.</param>
        /// <returns>Index of the icon</returns>
        public int IconIndex(string fileName, bool forceLoadFromDisk, ShellIconStateConstants iconState)
        {
            SHGetFileInfoConstants flags = SHGetFileInfoConstants.SHGFI_SYSICONINDEX;

            int attr = 0;

            if (this.size == SysImageListSize.smallIcons)
            {
                flags |= SHGetFileInfoConstants.SHGFI_SMALLICON;
            }

            // We can choose whether to access the disk or not. If you don't
            // hit the disk, you may get the wrong icon if the icon is
            // not cached. Also only works for files.
            if (!forceLoadFromDisk)
            {
                flags |= SHGetFileInfoConstants.SHGFI_USEFILEATTRIBUTES;
                attr = FILEATTRIBUTENORMAL;
            }
            else
            {
                attr = 0;
            }

            // sFileSpec can be any file. You can specify a
            // file that does not exist and still get the
            // icon, for example sFileSpec = "C:\PANTS.DOC"
            SHFILEINFO shfi = new SHFILEINFO();

            uint shfiSize = (uint)Marshal.SizeOf(shfi.GetType());

            IntPtr retVal = SHGetFileInfo(fileName, attr, ref shfi, shfiSize, (uint)flags | (uint)iconState);

            if (retVal.Equals(IntPtr.Zero))
            {
                System.Diagnostics.Debug.Assert((!retVal.Equals(IntPtr.Zero)), "Failed to get icon index");

                return 0;
            }
            else
            {
                return shfi.Icon;
            }
        }

        /// <summary>
        /// Draws an image
        /// </summary>
        /// <param name="hdc">Device context to draw to</param>
        /// <param name="index">Index of image to draw</param>
        /// <param name="x">X Position to draw at</param>
        /// <param name="y">Y Position to draw at</param>
        public void DrawImage(IntPtr hdc, int index, int x, int y)
        {
            this.DrawImage(hdc, index, x, y, ImageListDrawItemConstants.ILD_TRANSPARENT);
        }

        /// <summary>
        /// Draws an image using the specified flags
        /// </summary>
        /// <param name="hdc">Device context to draw to</param>
        /// <param name="index">Index of image to draw</param>
        /// <param name="x">X Position to draw at</param>
        /// <param name="y">Y Position to draw at</param>
        /// <param name="flags">Drawing flags</param>
        public void DrawImage(IntPtr hdc, int index, int x, int y, ImageListDrawItemConstants flags)
        {
            if (this.imageList == null)
            {
                int ret = ImageList_Draw(
                    this.himl,
                    index,
                    hdc,
                    x,
                    y,
                    (int)flags);
            }
            else
            {
                IMAGELISTDRAWPARAMS pimldp = new IMAGELISTDRAWPARAMS();
                pimldp.HdcDst = hdc;
                pimldp.Size = Marshal.SizeOf(pimldp.GetType());
                pimldp.Index = index;
                pimldp.X = x;
                pimldp.Y = y;
                pimldp.ForegroundRgb = -1;
                pimldp.Style = (int)flags;
                this.imageList.Draw(ref pimldp);
            }
        }

        /// <summary>
        /// Draws an image using the specified flags and specifies
        /// the size to clip to (or to stretch to if ILD_SCALE
        /// is provided).
        /// </summary>
        /// <param name="hdc">Device context to draw to</param>
        /// <param name="index">Index of image to draw</param>
        /// <param name="x">X Position to draw at</param>
        /// <param name="y">Y Position to draw at</param>
        /// <param name="flags">Drawing flags</param>
        /// <param name="cx">Width to draw</param>
        /// <param name="cy">Height to draw</param>
        public void DrawImage(IntPtr hdc, int index, int x, int y, ImageListDrawItemConstants flags, int cx, int cy)
        {
            IMAGELISTDRAWPARAMS pimldp = new IMAGELISTDRAWPARAMS();
            pimldp.HdcDst = hdc;
            pimldp.Size = Marshal.SizeOf(pimldp.GetType());
            pimldp.Index = index;
            pimldp.X = x;
            pimldp.Y = y;
            pimldp.CX = cx;
            pimldp.CY = cy;
            pimldp.Style = (int)flags;

            if (this.imageList == null)
            {
                pimldp.Himl = this.himl;

                int ret = ImageList_DrawIndirect(ref pimldp);
            }
            else
            {
                this.imageList.Draw(ref pimldp);
            }
        }

        /// <summary>
        /// Draws an image using the specified flags and state on XP systems.
        /// </summary>
        /// <param name="hdc">Device context to draw to</param>
        /// <param name="index">Index of image to draw</param>
        /// <param name="x">X Position to draw at</param>
        /// <param name="y">Y Position to draw at</param>
        /// <param name="flags">Drawing flags</param>
        /// <param name="cx">Width to draw</param>
        /// <param name="cy">Height to draw</param>
        /// <param name="foreColor">Fore colour to blend with when using the ILD_SELECTED or ILD_BLEND25 flags</param>
        /// <param name="stateFlags">State flags</param>
        /// <param name="saturateColorOrAlpha">If stateFlags includes ILS_ALPHA, then the alpha component is applied to the icon. Otherwise if 
        /// ILS_SATURATE is included, then the (R,G,B) components are used to saturate the image.</param>
        /// <param name="glowOrShadowColor">If stateFlags include ILS_GLOW, then the colour to use for the glow effect.  Otherwise if stateFlags includes 
        /// ILS_SHADOW, then the colour to use for the shadow.</param>
        public void DrawImage(
            IntPtr hdc,
            int index,
            int x,
            int y,
            ImageListDrawItemConstants flags,
            int cx,
            int cy,
            System.Drawing.Color foreColor,
            ImageListDrawStateConstants stateFlags,
            System.Drawing.Color saturateColorOrAlpha,
            System.Drawing.Color glowOrShadowColor)
        {
            IMAGELISTDRAWPARAMS pimldp = new IMAGELISTDRAWPARAMS();
            pimldp.HdcDst = hdc;
            pimldp.Size = Marshal.SizeOf(pimldp.GetType());
            pimldp.Index = index;
            pimldp.X = x;
            pimldp.Y = y;
            pimldp.CX = cx;
            pimldp.CY = cy;
            pimldp.ForegroundRgb = Color.FromArgb(0, foreColor.R, foreColor.G, foreColor.B).ToArgb();
            pimldp.Style = (int)flags;
            pimldp.State = (int)stateFlags;
            if ((stateFlags & ImageListDrawStateConstants.ILS_ALPHA) == ImageListDrawStateConstants.ILS_ALPHA)
            {
                // Set the alpha
                pimldp.Frame = (int)saturateColorOrAlpha.A;
            }
            else if ((stateFlags & ImageListDrawStateConstants.ILS_SATURATE) == ImageListDrawStateConstants.ILS_SATURATE)
            {
                // discard alpha channel:
                saturateColorOrAlpha = Color.FromArgb(0, saturateColorOrAlpha.R, saturateColorOrAlpha.G, saturateColorOrAlpha.B);

                // set the saturate color
                pimldp.Frame = saturateColorOrAlpha.ToArgb();
            }

            glowOrShadowColor = Color.FromArgb(0, glowOrShadowColor.R, glowOrShadowColor.G, glowOrShadowColor.B);

            pimldp.Effect = glowOrShadowColor.ToArgb();

            if (this.imageList == null)
            {
                pimldp.Himl = this.himl;

                int ret = ImageList_DrawIndirect(ref pimldp);
            }
            else
            {
                this.imageList.Draw(ref pimldp);
            }
        }

        #endregion

        #region Private Methods

        [DllImport("user32.dll")]
        private static extern int DestroyIcon(IntPtr handleIcon);

        [DllImport("shell32")]
        private static extern IntPtr SHGetFileInfo(
            string pszPath,
            int fileAttributes,
            ref SHFILEINFO psfi,
            uint fileInfo,
            uint flags);

        [DllImport("kernel32")]
        private static extern int FormatMessage(
            int flags,
            IntPtr source,
            int messageId,
            int languageId,
            string buffer,
            uint size,
            int argumentsLong);

        [DllImport("kernel32")]
        private static extern int GetLastError();

        [DllImport("comctl32")]
        private static extern int ImageList_Draw(
            IntPtr himl,
            int i,
            IntPtr hdcDst,
            int x,
            int y,
            int style);

        [DllImport("comctl32")]
        private static extern int ImageList_DrawIndirect(
            ref IMAGELISTDRAWPARAMS pimldp);

        [DllImport("comctl32")]
        private static extern int ImageList_GetIconSize(
            IntPtr himl,
            ref int cx,
            ref int cy);

        [DllImport("comctl32")]
        private static extern IntPtr ImageList_GetIcon(
            IntPtr himl,
            int i,
            int flags);

        /// <summary>
        /// SHGetImageList is not exported correctly in XP.  See KB316931
        /// http://support.microsoft.com/default.aspx?scid=kb;EN-US;Q316931
        /// Apparently (and hopefully) ordinal 727 isn't going to change.
        /// </summary>
        /// <param name="imageList">Image list.</param>
        /// <param name="riid">Globally unique ID.</param>
        /// <param name="ppv">Image list.</param>
        /// <returns>Resulting integer.</returns>
        [DllImport("shell32.dll", EntryPoint = "#727")]
        private static extern int SHGetImageList(
            int imageList,
            ref Guid riid,
            ref IImageList ppv);

        [DllImport("shell32.dll", EntryPoint = "#727")]
        private static extern int SHGetImageListHandle(
            int imageList,
            ref Guid riid,
            ref IntPtr handle);

        /// <summary>
        /// Creates the SystemImageList
        /// </summary>
        private void Create()
        {
            // forget last image list if any:
            this.himl = IntPtr.Zero;

            if (SystemUtilities.IsXPOrAbove())
            {
                // Get the System IImageList object from the Shell:
                Guid iidImageList = new Guid("46EB5926-582E-4017-9FDF-E8998DAA0950");
                int ret = SHGetImageList(
                    (int)this.size,
                    ref iidImageList,
                    ref this.imageList);

                // the image list handle is the IUnknown pointer, but 
                // using Marshal.GetIUnknownForObject doesn't return
                // the right value.  It really doesn't hurt to make
                // a second call to get the handle:
                SHGetImageListHandle((int)this.size, ref iidImageList, ref this.himl);
            }
            else
            {
                // Prepare flags:
                SHGetFileInfoConstants flags = SHGetFileInfoConstants.SHGFI_USEFILEATTRIBUTES | SHGetFileInfoConstants.SHGFI_SYSICONINDEX;
                if (this.size == SysImageListSize.smallIcons)
                {
                    flags |= SHGetFileInfoConstants.SHGFI_SMALLICON;
                }

                // Get image list
                SHFILEINFO shfi = new SHFILEINFO();
                uint shfiSize = (uint)Marshal.SizeOf(shfi.GetType());

                // Call SHGetFileInfo to get the image list handle
                // using an arbitrary file:
                this.himl = SHGetFileInfo(".txt", FILEATTRIBUTENORMAL, ref shfi, shfiSize, (uint)flags);

                System.Diagnostics.Debug.Assert((this.himl != IntPtr.Zero), "Failed to create Image List");
            }
        }

        #endregion

        #region Private Structures

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct IMAGELISTDRAWPARAMS
        {
            public int Size;
            public IntPtr Himl;
            public int Index;
            public IntPtr HdcDst;
            public int X;
            public int Y;
            public int CX;
            public int CY;
            public int BitmapX;        // x offest from the upperleft of bitmap
            public int BitmapY;        // y offset from the upperleft of bitmap
            public int BackgroundRgb;
            public int ForegroundRgb;
            public int Style;
            public int Rop;
            public int State;
            public int Frame;
            public int Effect;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct IMAGEINFO
        {
            public IntPtr MaskedImage;
            public IntPtr Mask;
            public int Unused1;
            public int Unused2;
            public RECT RectangleImage;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public IntPtr HandleIcon;
            public int Icon;
            public int Attributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAXPATH)]
            public string DisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string TypeName;
        }

        #endregion
    }
}
