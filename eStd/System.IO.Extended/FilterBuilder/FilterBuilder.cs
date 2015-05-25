using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Microsoft.Win32;

namespace Creek.IO.FilterBuilder
{
    /// <summary>
    /// A simple class allowing you to easily create dialogue filters.
    /// </summary>
    public class FilterBuilder
    {
        #region " Constructor "

        public FilterBuilder()
        {
            sFilters = new List<string>();
            sb = new StringBuilder();
        }

        #endregion

        #region " Private members "

        /// <summary>
        /// To hold the string filters internally.
        /// </summary>
        private List<string> sFilters;

        /// <summary>
        /// The string builder to return the filter string.
        /// </summary>
        private StringBuilder sb = new StringBuilder();

        #endregion

        #region Filters enum

        /// <summary>
        /// An enumeration of many common filter types.
        /// </summary>
        public enum Filters
        {
            /// <summary>
            /// All Files (*.*)
            /// </summary>
            AllFiles,

            /// <summary>
            /// Microsoft Word Documents (*.doc)
            /// </summary>
            WordDocuments,

            /// <summary>
            /// Microsoft Word Open XML Documents (*.docx)
            /// </summary>
            WordOpenXMLDocuments,

            /// <summary>
            /// Log Files (*.log)
            /// </summary>
            LogFiles,

            /// <summary>
            /// Mail Messages (*.msg)
            /// </summary>
            MailMessages,

            /// <summary>
            /// Pages Documents (*.pages)
            /// </summary>
            PagesDocuments,

            /// <summary>
            /// Rich Text Format Files (*.rtf)
            /// </summary>
            RichTextFiles,

            /// <summary>
            /// Plain Text Files (*.txt)
            /// </summary>
            TextFiles,

            /// <summary>
            /// WordPerfect Documents (*.wpd)
            /// </summary>
            WordPerfectDocuments,

            /// <summary>
            /// Microsoft Works Word Processor Documents (*.wps)
            /// </summary>
            WorksWordProcessorDocuments,

            /// <summary>
            /// Lotus 1-2-3 Spreadsheets (*.123)
            /// </summary>
            Lotus123Spreadsheets,

            /// <summary>
            /// Access 2007 Database Files (*.accdb)
            /// </summary>
            Access2007DatabaseFiles,

            /// <summary>
            /// Comma Separated Values Files (*.csv)
            /// </summary>
            CSV_Files,

            /// <summary>
            /// Data Files (*.dat)
            /// </summary>
            DataFiles,

            /// <summary>
            /// Database Files (*.db)
            /// </summary>
            DatabaseFiles,

            /// <summary>
            /// Dynamic Link Library Files (*.dll)
            /// </summary>
            DLL_Files,

            /// <summary>
            /// Microsoft Access Database Files (*.mdb)
            /// </summary>
            AccessDatabaseFiles,

            /// <summary>
            /// PowerPoint Slide Shows (*.pps)
            /// </summary>
            PowerPointSlideShows,

            /// <summary>
            /// PowerPoint Presentations (*.ppt)
            /// </summary>
            PowerPointPresentations,

            /// <summary>
            /// Microsoft PowerPoint Open XML Documents (*.pptx)
            /// </summary>
            PowerPointOpenXMLDocuments,

            /// <summary>
            /// OpenOffice.org Base Database Files (*.sdb)
            /// </summary>
            OpenOfficeBaseDatabaseFiles,

            /// <summary>
            /// Structured Query Language Data Files (*.sql)
            /// </summary>
            SQLDataFiles,

            /// <summary>
            /// vCard Files (*.vcf)
            /// </summary>
            vCardFiles,

            /// <summary>
            /// Universal Converter Conversion Files (*.ucv)
            /// </summary>
            UCConversionFiles,

            /// <summary>
            /// Microsoft Works Spreadsheets (*.wks)
            /// </summary>
            WorksSpreadsheets,

            /// <summary>
            /// Microsoft Excel Spreadsheets (*.xls)
            /// </summary>
            ExcelSpreadsheets,

            /// <summary>
            /// Microsoft Excel Open XML Documents (*.xlsx)
            /// </summary>
            ExcelOpenXMLDocuments,

            /// <summary>
            /// XML Files (*.xml)
            /// </summary>
            XML_Files,

            /// <summary>
            /// Bitmap Image Files (*.bmp)
            /// </summary>
            BMP_ImageFiles,

            /// <summary>
            /// Graphical Interchange Format Files (*.gif)
            /// </summary>
            GIF_ImageFiles,

            /// <summary>
            /// JPEG Image Files (*.jpg;*.jpeg)
            /// </summary>
            JPEG_ImageFiles,

            /// <summary>
            /// Portable Network Graphic Files (*.png)
            /// </summary>
            PNG_ImageFiles,

            /// <summary>
            /// All Supported Image Files (*.bmp;*.gif;*.jpg;*.jpeg;*.png;*.tif;*.tiff)      
            /// </summary>
            AllImageFiles,

            /// <summary>
            /// Photoshop Documents (*.psd)
            /// </summary>
            PhotoshopDocuments,

            /// <summary>
            /// Paint Shop Pro Image Files (*.psp)
            /// </summary>
            PaintShopProImageFiles,

            /// <summary>
            /// Thumbnail Image Files (*.thm)
            /// </summary>
            ThumbnailImageFiles,

            /// <summary>
            /// Tagged Image Files (*.tif;*.tiff)
            /// </summary>
            TIFF_ImageFiles,

            /// <summary>
            /// Adobe Illustrator Files (*.ai)
            /// </summary>
            AdobeIllustratorFiles,

            /// <summary>
            /// Drawing Files (*.drw)
            /// </summary>
            DrawingFiles,

            /// <summary>
            /// Drawing Exchange Format Files (*.dxf)
            /// </summary>
            DrawingExchangeFormatFiles,

            /// <summary>
            /// Encapsulated PostScript Files (*.eps)
            /// </summary>
            EncapsulatedPostScriptFiles,

            /// <summary>
            /// PostScript Files (*.ps)
            /// </summary>
            PostScriptFiles,

            /// <summary>
            /// Scalable Vector Graphics Files (*.svg)
            /// </summary>
            SVG_Files,

            /// <summary>
            /// Rhino 3D Models (*.3dm)
            /// </summary>
            Rhino3DModels,

            /// <summary>
            /// AutoCAD Drawing Database Files (*.dwg)
            /// </summary>
            AutoCADDrawingDatabaseFiles,

            /// <summary>
            /// ArchiCAD Project Files (*.pln)
            /// </summary>
            ArchiCADProjectFiles,

            /// <summary>
            /// Adobe InDesign Files (*.indd)
            /// </summary>
            AdobeInDesignFiles,

            /// <summary>
            /// Portable Document Format Files (*.pdf)
            /// </summary>
            PDF_Files,

            /// <summary>
            /// Advanced Audio Coding Files (*.aac)
            /// </summary>
            AAC_Files,

            /// <summary>
            /// Audio Interchange File Format Files (*.aif)
            /// </summary>
            AIF_Files,

            /// <summary>
            /// Interchange File Format Files (*.iif)
            /// </summary>
            IIF_Files,

            /// <summary>
            /// Media Playlist Files (*.m3u)
            /// </summary>
            MediaPlaylistFiles,

            /// <summary>
            /// MIDI Files (*.mid;*.midi)
            /// </summary>
            MIDI_Files,

            /// <summary>
            /// MP3 Audio Files (*.mp3)
            /// </summary>
            MP3_AudioFiles,

            /// <summary>
            /// MPEG-2 Audio Files (*.mpa)
            /// </summary>
            MPEG2_AudioFiles,

            /// <summary>
            /// Real Audio Files (*.ra)
            /// </summary>
            RealAudioFiles,

            /// <summary>
            /// WAVE Audio Files (*.wav)
            /// </summary>
            WAVE_AudioFiles,

            /// <summary>
            /// Windows Media Audio Files (*.wma)
            /// </summary>
            WindowsMediaAudioFiles,

            /// <summary>
            /// 3GPP2 Multimedia Files (*.3g2)
            /// </summary>
            _3GPP2MultimediaFiles,

            /// <summary>
            /// 3GPP Multimedia Files (*.3gp)
            /// </summary>
            _3GPPMultimediaFiles,

            /// <summary>
            /// Audio Video Interleave Files (*.avi)
            /// </summary>
            AVI_Files,

            /// <summary>
            /// Flash Video Files (*.flv)
            /// </summary>
            FlashVideoFiles,

            /// <summary>
            /// Matroska Video Files (*.mkv)
            /// </summary>
            MatroskaVideoFiles,

            /// <summary>
            /// Apple QuickTime Movie Files (*.mov)
            /// </summary>
            AppleQuickTimeMoviesMov,

            /// <summary>
            /// MPEG-4 Video Files (*.mp4)
            /// </summary>
            MPEG4_VideoFiles,

            /// <summary>
            /// MPEG Video Files (*.mpg)
            /// </summary>
            MPEG_VideoFiles,

            /// <summary>
            /// Apple QuickTime Movie Files (*.qt)
            /// </summary>
            AppleQuickTimeMoviesQT,

            /// <summary>
            /// Real Media Files (*.rm)
            /// </summary>
            RealMediaFiles,

            /// <summary>
            /// Flash Movies (*.swf)
            /// </summary>
            FlashMovies,

            /// <summary>
            /// DVD Video Object Files (*.vob)
            /// </summary>
            DVDVideoObjectFiles,

            /// <summary>
            /// Windows Media Video Files (*.wmv)
            /// </summary>
            WindowsMediaVideoFiles,

            /// <summary>
            /// Active Server Pages (*.asp)
            /// </summary>
            ActiveServerPages,

            /// <summary>
            /// Cascading Style Sheets (*.css)
            /// </summary>
            CascadingStyleSheets,

            /// <summary>
            /// Hypertext Markup Language Files (*.htm;*.html)
            /// </summary>
            HTML_Files,

            /// <summary>
            /// JavaScript Files (*.js)
            /// </summary>
            JavaScriptFiles,

            /// <summary>
            /// Java Server Pages (*.jsp)
            /// </summary>
            JavaServerPages,

            /// <summary>
            /// Hypertext Preprocessor Files (*.php)
            /// </summary>
            PHP_Files,

            /// <summary>
            /// Rich Site Summary Files (*.rss)
            /// </summary>
            RichSiteSummaryFiles,

            /// <summary>
            /// Extensible Hypertext Markup Language Files (*.xhtml)
            /// </summary>
            XHTML_Files,

            /// <summary>
            /// Windows Font Files (*.fnt)
            /// </summary>
            WindowsFontFiles,

            /// <summary>
            /// Generic Font Files (*.fon)
            /// </summary>
            GenericFontFiles,

            /// <summary>
            /// OpenType Fonts (*.otf)
            /// </summary>
            OpenTypeFonts,

            /// <summary>
            /// TrueType Fonts (*.ttf)
            /// </summary>
            TrueTypeFonts,

            /// <summary>
            /// Excel Add-In Files (*.xll)
            /// </summary>
            ExcelAddInFiles,

            /// <summary>
            /// Windows Cabinet Files (*.cab)
            /// </summary>
            WindowsCabinetFiles,

            /// <summary>
            /// Windows Control Panel (*.cpl)
            /// </summary>
            WindowsControlPanel,

            /// <summary>
            /// Windows Cursors (*.cur)
            /// </summary>
            WindowsCursors,

            /// <summary>
            /// Windows Memory Dumps (*.dmp)
            /// </summary>
            WindowsMemoryDumps,

            /// <summary>
            /// Device Drivers (*.drv)
            /// </summary>
            DeviceDrivers,

            /// <summary>
            /// Security Keys (*.key)
            /// </summary>
            SecurityKeys,

            /// <summary>
            /// File Shortcuts (*.lnk)
            /// </summary>
            FileShortcuts,

            /// <summary>
            /// Windows System Files (*.sys)
            /// </summary>
            WindowsSystemFiles,

            /// <summary>
            /// Configuration Files (*.cfg)
            /// </summary>
            ConfigurationFiles,

            /// <summary>
            /// Windows Initialization Files (*.ini)
            /// </summary>
            INI_Files,

            /// <summary>
            /// Outlook Profile Files (*.prf)
            /// </summary>
            OutlookProfileFiles,

            /// <summary>
            /// Mac OS X Applications (*.app)
            /// </summary>
            MacOSXApplications,

            /// <summary>
            /// DOS Batch Files (*.bat)
            /// </summary>
            DOSBatchFiles,

            /// <summary>
            /// Common Gateway Interface Scripts (*.cgi)
            /// </summary>
            CGI_Files,

            /// <summary>
            /// DOS Command Files (*.com)
            /// </summary>
            DOSCommandFiles,

            /// <summary>
            /// Windows Executable File (*.exe)
            /// </summary>
            WindowsExecutableFiles,

            /// <summary>
            /// Windows Scripts (*.ws)
            /// </summary>
            WindowsScripts,

            /// <summary>
            /// 7-Zip Compressed Files (*.7z)
            /// </summary>
            _7ZipCompressedFiles,

            /// <summary>
            /// Debian Software Packages (*.deb)
            /// </summary>
            DebianSoftwarePackages,

            /// <summary>
            /// Gnu Zipped Files (*.gz)
            /// </summary>
            GnuZippedFile,

            /// <summary>
            /// Mac OS X Installer Packages (*.pkg)
            /// </summary>
            MacOSXInstallerPackages,

            /// <summary>
            /// WinRAR Compressed Archives (*.rar)
            /// </summary>
            WinRARCompressedArchives,

            /// <summary>
            /// Self-Extractingd Archives (*.sea)
            /// </summary>
            SelfExtractingArchives,

            /// <summary>
            /// Stuffit Archives (*.sit)
            /// </summary>
            StuffitArchives,

            /// <summary>
            /// Stuffit X Archives (*.sitx)
            /// </summary>
            StuffitXArchives,

            /// <summary>
            /// Zipped Files (*.zip)
            /// </summary>
            ZippedFiles,

            /// <summary>
            /// Extended Zip Files (*.zipx)
            /// </summary>
            ExtendedZipFiles,

            /// <summary>
            /// BinHex 4.0 Encoded Files (*.hqx)
            /// </summary>
            BinHex4EncodedFiles,

            /// <summary>
            /// Multi-Purpose Internet Mail Messages (*.mim)
            /// </summary>
            MultiPurposeInternetMailMessages,

            /// <summary>
            /// Uuencoded Files (*.uue)
            /// </summary>
            UuencodedFiles,

            /// <summary>
            /// C/C++ Source Code Files (*.c)
            /// </summary>
            C_CPlusPlus_SourceCodeFiles,

            /// <summary>
            /// C++ Source Code Files (*.cpp)
            /// </summary>
            CPlusPlus_SourceCodeFiles,

            /// <summary>
            /// Java Source Code Files (*.java)
            /// </summary>
            Java_SourceCodeFiles,

            /// <summary>
            /// Perl Scripts (*.pl)
            /// </summary>
            PerlScripts,

            /// <summary>
            ///  VB Source Code Files (*.vb)
            /// </summary>
            VB_SourceCodeFiles,

            /// <summary>
            /// Visual Studio Solution Files (*.sln)
            /// </summary>
            VisualStudioSolutionFiles,

            /// <summary>
            /// C# Source Code Files (*.cs)
            /// </summary>
            CSharp_SourceCodeFiles,

            /// <summary>
            /// Backup Files (*.bak)
            /// </summary>
            BackupFiles_BAK,

            /// <summary>
            /// Backup Files (*.bup)
            /// </summary>
            BackupFiles_BUP,

            /// <summary>
            /// Norton Ghost Backup Files (*.gho)
            /// </summary>
            NortonGhostBackupFiles,

            /// <summary>
            /// Original Files (*.ori)
            /// </summary>
            OriginalFiles,

            /// <summary>
            /// Temporary Files (*.tmp)
            /// </summary>
            TemporaryFiles,

            /// <summary>
            /// Disc Image Files (*.iso)
            /// </summary>
            DiscImageFiles,

            /// <summary>
            /// Toast Disc Images (*.toast)
            /// </summary>
            ToastDiscImages,

            /// <summary>
            /// Virtual CDs (*.vcd)
            /// </summary>
            Virtual_CDs,

            /// <summary>
            /// Windows Installer Packages (*.msi)
            /// </summary>
            WindowsInstallerPackages,

            /// <summary>
            /// Partially Downloaded Files (*.part)
            /// </summary>
            PartiallyDownloadedFiles,

            /// <summary>
            /// BitTorrent Files (*.torrent)
            /// </summary>
            BitTorrentFiles,

            /// <summary>
            /// Yahoo! Messenger Data Files (*.yps)
            /// </summary>
            YahooMessengerDataFiles,

            /// <summary>
            /// Windows Icons (*.ico)
            /// </summary>
            WindowsIcons,

            /// <summary>
            /// Exchangeable Image Format Files (*.exif)
            /// </summary>
            EXIF_ImageFiles
        }

        #endregion

        #region ImageSize enum

        /// <summary>
        /// An enumeration of the valid system icon sizes.
        /// </summary>
        public enum ImageSize
        {
            /// <summary>
            /// View image in 16x16 px.
            /// </summary>
            Small,

            /// <summary>
            /// View image in 32x32 px.
            /// </summary>
            Large
        }

        #endregion

        #region " Public properties "

        /// <summary>
        /// Gets/Sets the current icon size of the system extension icons to return.
        /// </summary>
        public ImageSize FileTypeIconSize
        {
            get { return SystemFileType.currentSize; }
            set { SystemFileType.currentSize = value; }
        }

        /// <summary>
        /// Returns how many filters are contained at present.
        /// </summary>
        public int Count
        {
            get { return sFilters.Count; }
        }

        #endregion

        #region " Public methods "

        /// <summary>
        /// Add a filter to the current filter string.
        /// </summary>
        /// <param name="filter">
        /// The filter to add.
        /// </param>
        public void Add(Filters filter)
        {
            //Add filter to the list
            sFilters.Add(Helpers.ReturnFilterAsString(filter));
        }

        /// <summary>
        /// Add filters to the current filter string.
        /// </summary>
        /// <param name="filters">
        /// The filters to add.
        /// </param>
        public void Add(Filters[] filters)
        {
            //Add filter to the list
            foreach (Filters item in filters)
            {
                sFilters.Add(Helpers.ReturnFilterAsString(item));
            }
        }

        /// <summary>
        /// Adds a system file type to the current filter
        /// string (gets description from computer).
        /// </summary>
        /// <param name="extension">
        /// The system file extension (e.g. "mp3").
        /// </param>
        /// <param name="plural">
        /// Optional. Specifies whether to try and make the system
        /// description plural ("Files" instead of "File") for 
        /// consistency with other filters. Default is true.
        /// </param>
        public void Add(string extension, bool plural = true)
        {
            //Make sure extension starts with a dot
            extension = extension.StartsWith(".") == false ? "." + extension : extension;

            //Try to get the system file type desription
            string description = "";
            try
            {
                description = GetSystemFileTypeDescription(extension);
            }
            catch (Exception)
            {
                description = "";
            }


            //Return is description is nothing
            if (string.IsNullOrEmpty(description))
            {
                return;
            }

            //Check if needs to be plural
            if (plural)
            {
                //Add an 's' to the end if it doesn't have one
                description = description.EndsWith("s") == false ? description + "s" : description;
            }

            //Get the raw extension (without spaces, dots and stars)
            string ext = extension.Trim(' ', '.', '*');

            //Make the filter string by combining the description and extension
            //properly
            string s = (description.Trim() + " (*." + ext + ")|*." + ext);

            //Add filter to the list
            sFilters.Add(s);
        }

        /// <summary>
        /// Add a custom filter to the current filter string.
        /// </summary>
        /// <param name="description">
        /// The custom description (e.g. "Foo Files").
        /// </param>
        /// <param name="extension">
        /// The custom file extension (e.g. "foo").
        /// </param>
        public void Add(string description, string extension)
        {
            //Check if description or extension are empty, if so return
            if (string.IsNullOrEmpty(description) || string.IsNullOrEmpty(extension))
            {
                return;
            }

            //Get the raw extension (without spaces, dots and stars)
            string ext = extension.Trim(' ', '.', '*');

            //Make the filter string by combining the description and extension
            //properly
            string s = (description.Trim() + " (*." + ext + ")|*." + ext);

            //Add filter to the list
            sFilters.Add(s);
        }

        /// <summary>
        /// Add a specified group.
        /// </summary>
        /// <param name="group">
        /// A filter group.
        /// </param>
        public void Add(FilterGroup @group)
        {
            //Get the filter string for the group.
            sFilters.Add(@group.GetFilterString());
        }

        /// <summary>
        /// Remove a filter from the current filter string.
        /// </summary>
        /// <param name="filter">
        /// The filter to remove.
        /// </param>
        public void Remove(Filters filter)
        {
            //Check if list contains the filter, if so remove it
            if (Contains(filter))
            {
                sFilters.Remove(Helpers.ReturnFilterAsString(filter));
            }
        }

        /// <summary>
        /// Removes a system file type from the current filter
        /// string.
        /// </summary>
        /// <param name="extension">
        /// The system file extension (e.g. "mp3").
        /// </param>
        public void Remove(string extension)
        {
            //Make sure extension starts with a dot
            extension = extension.StartsWith(".") == false ? "." + extension : extension;

            //Try to get the system file type desription
            string description = "";
            try
            {
                description = GetSystemFileTypeDescription(extension);
            }
            catch (Exception)
            {
                description = "";
            }

            //Return is description is nothing
            if (string.IsNullOrEmpty(description))
            {
                return;
            }

            //Add an s to the end (e.g. Files)
            string description1 = description.EndsWith("s") == false ? description + "s" : description;

            //Take off an s from the end (e.g File)
            string description2 = description.EndsWith("s") == false
                                      ? description
                                      : description.Substring(0, description.Length - 1);

            //Get the raw extension (without spaces, dots and stars)
            string ext = extension.Trim(' ', '.', '*');

            //Make the filter string by combining the description and extension
            //properly
            string s = (description1.Trim() + " (*." + ext + ")|*." + ext);

            string s2 = (description1.Trim() + " (*." + ext + ")|*." + ext);

            //Remove filter (plural)
            if (sFilters.Contains(s))
            {
                sFilters.Remove(s);
                return;
            }

            //Remove filter
            if (sFilters.Contains(s2))
            {
                sFilters.Remove(s2);
                return;
            }
        }

        /// <summary>
        /// Remove a custom filter from the current filter string.
        /// </summary>
        /// <param name="description">
        /// The custom description (e.g. "Foo Files").
        /// </param>
        /// <param name="extension">
        /// The custom file extension (e.g. "foo").
        /// </param>
        public void Remove(string description, string extension)
        {
            //Check if description or extension are empty, if so return
            if (string.IsNullOrEmpty(description) || string.IsNullOrEmpty(extension))
            {
                return;
            }

            //Get the raw extension (without spaces, dots and stars)
            string ext = extension.Trim(' ', '.', '*');

            //Make the filter string by combining the description and extension
            //properly
            string s = (description.Trim() + " (*." + ext + ")|*." + ext);

            //Check if list contains the filter, if so remove it
            if (Contains(description.Trim(), ext))
            {
                sFilters.Remove(s);
            }
        }

        /// <summary>
        /// Remove a filters from the current filter string.
        /// </summary>
        /// <param name="filter">
        /// The filters to remove.
        /// </param>
        public void Remove(Filters[] filter)
        {
            //Check if list contains the filter, if so remove it
            foreach (Filters item in filter)
            {
                if (Contains(item))
                {
                    sFilters.Remove(Helpers.ReturnFilterAsString(item));
                }
            }
        }

        /// <summary>
        /// Clears the entire filter list.
        /// </summary>
        public void Clear()
        {
            //Clear the filters
            sFilters.Clear();
            sFilters = new List<string>();
            sb = new StringBuilder();
        }

        /// <summary>
        /// Checks if the specified filter is contained within the filter string.
        /// </summary>
        /// <param name="filter">
        /// The filter to check for.
        /// </param>
        /// <returns>
        /// Boolean as to whether the filter was found in the collection.
        /// </returns>
        public bool Contains(Filters filter)
        {
            //Check the filter and return boolean
            return sFilters.Contains(Helpers.ReturnFilterAsString(filter));
        }

        /// <summary>
        /// Checks if the specified custom filter is contained within the filter string.
        /// </summary>
        /// <param name="description">
        /// The custom description (e.g. "Foo Files").
        /// </param>
        /// <param name="extension">
        /// The custom file extension (e.g. "foo").
        /// </param>
        /// <returns>
        /// Boolean as to whether the filter was found in the collection.
        /// </returns>
        public bool Contains(string description, string extension)
        {
            //Check if description or extension are empty, if so return
            if (string.IsNullOrEmpty(description) || string.IsNullOrEmpty(extension))
            {
                return false;
            }

            //Get the raw extension (without spaces, dots and stars)
            string ext = extension.Trim(' ', '.', '*');

            //Make the filter string by combining the description and extension
            //properly
            string s = (description.Trim() + " (*." + ext + ")|*." + ext);

            //Check the filter and return boolean
            return sFilters.Contains(s);
        }

        /// <summary>
        /// Checks if the specified system file type is contained within the filter string.
        /// </summary>
        /// <param name="extension">
        /// The system file extension (e.g. "mp3").
        /// </param>
        public bool Contains(string extension)
        {
            //Make sure extension starts with a dot
            extension = extension.StartsWith(".") == false ? "." + extension : extension;

            //Try to get the system file type desription
            string description = "";
            try
            {
                description = GetSystemFileTypeDescription(extension);
            }
            catch (Exception)
            {
                description = "";
            }

            //Return is description is nothing
            if (string.IsNullOrEmpty(description))
            {
                return false;
            }

            //Add an s to the end (e.g. Files)
            string description1 = description.EndsWith("s") == false ? description + "s" : description;

            //Take off an s from the end (e.g File)

            //Get the raw extension (without spaces, dots and stars)
            string ext = extension.Trim(' ', '.', '*');

            //Make the filter string by combining the description and extension
            //properly
            string s = (description1.Trim() + " (*." + ext + ")|*." + ext);

            string s2 = (description1.Trim() + " (*." + ext + ")|*." + ext);

            //Remove filter (plural)
            if (sFilters.Contains(s))
            {
                return true;
            }

            //Remove filter
            if (sFilters.Contains(s2))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the entire filter string.
        /// </summary>
        /// <returns>The completed filter string.</returns>
        public override string ToString()
        {
            sb = new StringBuilder();

            //Build the string builder

            foreach (string item in sFilters)
            {
                //Trim off unneeded white space and "|" characters 
                //and add to the stringbuilder
                sb.Append((item.Trim(' ', '|') + "|"));
                //Add a "|" to separate the filters
            }

            //Return the underlying string builder's contents, trimming excess "|".
            return sb.ToString().Trim(' ', '|');
        }

        #endregion

        #region " Extras "

        /// <summary>
        /// Returns the relevant file extension for the filter.
        /// </summary>
        /// <param name="filter">
        /// The filter to get the extension for.
        /// </param>
        /// <param name="withDot">
        /// Optional. Specifies whether to include the dot with the extension.
        /// </param>
        /// <returns>The file extension e.g. ".txt"</returns>
        public string GetFileExtension(Filters filter, bool withDot = true)
        {
            //Get the filter
            string s = Helpers.ReturnFilterAsString(filter);

            //Get the extension
            string[] sArray = s.Split('|');
            //Split filter with "|"

            //Make sure there are two parts
            if (sArray.Length < 2)
            {
                return null;
            }

            //The second part is the file extension
            s = sArray[1];

            //Return the extension with or without a dot and trim stars
            return withDot ? s.Trim('*') : s.Trim('.', '*');
        }

        /// <summary>
        /// Returns the file extension part of the filter string.
        /// </summary>
        /// <param name="filter">
        /// The filter string.
        /// </param>
        /// <param name="withDot">
        /// Optional. Specifies whether to include the dot with the extension.
        /// </param>
        /// <returns>The file extension e.g. ".txt"</returns>
        public string GetFileExtension(string filter, bool withDot = true)
        {
            //Get the filter
            string s = filter;

            //Get the extension
            string[] sArray = s.Split('|');
            //Split filter with "|"

            //Make sure there are two parts
            if (sArray.Length < 2)
            {
                return null;
            }

            //The second part is the file extension
            s = sArray[1];

            //Return the extension with or without a dot and trim stars
            return withDot ? s.Trim('*') : s.Trim('.', '*');
        }

        /// <summary>
        /// Returns the relevant file description for the filter.
        /// </summary>
        /// <param name="filter">
        /// The filter to get the extension for.
        /// </param>
        /// <returns>The file description e.g. "Text Files"</returns>
        public string GetFileDescription(Filters filter)
        {
            //Get the filter
            string s = Helpers.ReturnFilterAsString(filter);

            //Get the extension
            string[] s2 = {" (*."};
            string[] sArray = s.Split(s2, StringSplitOptions.None);
            //Split filter with " (*."

            //Make sure there are two parts
            if (sArray.Length < 2)
            {
                return null;
            }

            //Return the extension with or without a dot and trim stars
            return sArray[0];
        }

        /// <summary>
        /// Returns the file description part of the filter string.
        /// </summary>
        /// <param name="filter">
        /// The filter to get the description.
        /// </param>
        /// <returns>The file description e.g. "Text Files"</returns>
        public string GetFileDescription(string filter)
        {
            //Get the filter
            string s = filter;

            //Get the extension
            string[] s2 = {" (*."};
            string[] sArray = s.Split(s2, StringSplitOptions.None);
            //Split filter with " (*."

            //Make sure there are two parts
            if (sArray.Length < 2)
            {
                return null;
            }

            //Return the extension with or without a dot and trim stars
            return sArray[0];
        }

        /// <summary>
        /// Returns the filter index position of the filter.
        /// </summary>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <returns>The filter index to use.</returns>
        public int GetFilterIndex(Filters filter)
        {
            //Return the index of the filter + 1 (1 based)
            return (sFilters.IndexOf(Helpers.ReturnFilterAsString(filter)) > -1)
                       ? (sFilters.IndexOf(Helpers.ReturnFilterAsString(filter)) + 1)
                       : 1;
        }

        /// <summary>
        /// Returns the filter index position of the filter string.
        /// </summary>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <returns>The filter index to use.</returns>
        public int GetFilterIndex(string filter)
        {
            //Return the index of the filter + 1 (1 based)
            return (sFilters.IndexOf(filter) > -1) ? (sFilters.IndexOf(filter) + 1) : 1;
        }

        /// <summary>
        /// Returns the first string of the first item.
        /// </summary>
        public string GetFirstItem()
        {
            //Return the first item of the list
            return sFilters.Count > 0 ? sFilters[0] : string.Empty;
        }

        /// <summary>
        /// Returns the first string of the last item.
        /// </summary>
        public string GetLastItem()
        {
            //Return the first item of the list
            return sFilters.Count > 0 ? sFilters[sFilters.Count - 1] : string.Empty;
        }

        #endregion

        #region " System File Types "

        /// <summary>
        /// Returns all the system registered file type extensions.
        /// </summary>
        /// <param name="withDot">
        /// Optional. Specifies whether to add a dot before the extension (.ext).
        /// Default is true.
        /// </param>s
        public List<string> GetSystemFileTypes(bool withDot = true)
        {
            var s = new List<string>();

            //Get the icons info
            SystemFileType.iconsInfo = SystemFileType.GetFileTypeAndIcon();

            //Loads file types into list
            foreach (object objString in SystemFileType.iconsInfo.Keys)
            {
                //Check for dot at start
                s.Add((string) (withDot ? objString : ((string) objString).TrimStart('.')));
            }

            //Return the list
            return s;
        }

        /// <summary>
        /// Gets the associated system icon image for the filter.
        /// </summary>
        /// <param name="filter">
        /// The filter to get the icon for.
        /// </param>
        public Icon GetSystemFileTypeIcon(Filters filter)
        {
            List<string> l = GetSystemFileTypes();

            //Get the icon
            try
            {
                string ext = GetFileExtension(filter);
                //Get file extension
                Icon icon = SystemFileType.ReturnIcon(ext.StartsWith(".") == false ? "." + ext : ext);

                return icon;
            }
            catch (Exception)
            {
                return null;
                //Return nothing
            }
        }

        /// <summary>
        /// Gets the associated system icon image for the extension.
        /// </summary>
        /// <param name="ext">
        /// The file extension.
        /// </param>
        public Icon GetSystemFileTypeIcon(string ext)
        {
            //Get the icon
            try
            {
                Icon icon = SystemFileType.ReturnIcon(ext.StartsWith(".") == false ? "." + ext : ext);

                return icon;
            }
            catch (Exception)
            {
                return null;
                //Return nothing
            }
        }

        /// <summary>
        /// Gets the file type description associated with the particular file type.
        /// </summary>
        /// <param name="filter">
        /// The filter to get the description for.
        /// </param>
        /// <returns>The description (if one exists).</returns>
        public string GetSystemFileTypeDescription(Filters filter)
        {
            //Return value
            string fileType = "";

            //Get the extension
            string ext = GetFileExtension(filter);

            //Check that the extension starts with a dot
            ext = ext.StartsWith(".") == false ? "." + ext : ext;

            //Search all keys under HKEY_CLASSES_ROOT
            foreach (string subKey in Registry.ClassesRoot.GetSubKeyNames())
            {
                if (string.IsNullOrEmpty(subKey))
                {
                    continue;
                }

                if (subKey == ext)
                {
                    //File extension found. Get Default Value.
                    string defaultValue = "";
                    try
                    {
                        defaultValue = Registry.ClassesRoot.OpenSubKey(subKey).GetValue("").ToString();
                    }
                    catch (Exception)
                    {
                        return fileType;
                    }

                    if (defaultValue.Length == 0)
                    {
                        //No File Type specified
                        break; // TODO: might not be correct. Was : Exit For
                    }

                    if (fileType.Length == 0)
                    {
                        //Get Initial File Type and search for the full File Type Description
                        fileType = defaultValue;
                        ext = fileType;
                    }
                    else
                    {
                        //File Type Description found
                        if (defaultValue.Length > 0)
                        {
                            fileType = defaultValue;
                        }

                        break; // TODO: might not be correct. Was : Exit For
                    }
                }
            }

            return fileType;
        }

        /// <summary>
        /// Gets the file type description associated with the particular file type.
        /// </summary>
        /// <param name="ext">
        /// The file extension.
        /// </param>
        /// <returns>The description (if one exists).</returns>
        public string GetSystemFileTypeDescription(string ext)
        {
            //Return value
            string fileType = "";

            //Check that the extension starts with a dot
            ext = ext.StartsWith(".") == false ? "." + ext : ext;

            //Search all keys under HKEY_CLASSES_ROOT
            foreach (string subKey in Registry.ClassesRoot.GetSubKeyNames())
            {
                if (string.IsNullOrEmpty(subKey))
                {
                    continue;
                }

                if (subKey == ext)
                {
                    //File extension found. Get Default Value.
                    string defaultValue = "";
                    try
                    {
                        defaultValue = Registry.ClassesRoot.OpenSubKey(subKey).GetValue("").ToString();
                    }
                    catch (Exception)
                    {
                        return fileType;
                    }

                    if (defaultValue.Length == 0)
                    {
                        //No File Type specified
                        break; // TODO: might not be correct. Was : Exit For
                    }

                    if (fileType.Length == 0)
                    {
                        //Get Initial File Type and search for the full File Type Description
                        fileType = defaultValue;
                        ext = fileType;
                    }
                    else
                    {
                        //File Type Description found
                        if (defaultValue.Length > 0)
                        {
                            fileType = defaultValue;
                        }

                        break; // TODO: might not be correct. Was : Exit For
                    }
                }
            }

            return fileType;
        }

        /// <summary>
        /// Returns an object that should be the info of the system file type
        /// specified. If not contained on the computer, then the function returns
        /// nothing.
        /// </summary>
        /// <param name="filter">
        /// The filter to get the info for.
        /// </param>
        public object GetSystemFileTypeInfo(Filters filter)
        {
            //Get file extension
            string ext = GetFileExtension(filter);

            //Ensure that the extension starts with a dot.
            ext = ext.StartsWith(".") == false ? "." + ext : ext;

            //Get the info
            return SystemFileType.iconsInfo[ext];
        }

        /// <summary>
        /// Returns an object that should be the info of the system file type
        /// specified. If not contained on the computer, then the function returns
        /// nothing.
        /// </summary>
        /// <param name="ext">
        /// The file extension.
        /// </param>
        public object GetSystemFileTypeInfo(string ext)
        {
            //Ensure that the extension starts with a dot.
            ext = ext.StartsWith(".") == false ? "." + ext : ext;

            //Get the info
            return SystemFileType.iconsInfo[ext];
        }

        #endregion
    }
}