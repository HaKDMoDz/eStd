namespace System.IO.FilterBuilder
{
    /// <summary>
    /// Helper methods.
    /// </summary>
    static internal class Helpers
    {

        #region " Public methods "

        /// <summary>
        /// Returns the specified filter as a filter string.
        /// </summary>
        /// <param name="filter">
        /// The filter to return.
        /// </param>
        /// <returns>
        /// The string representation of the filter.
        /// </returns>
        public static string ReturnFilterAsString(System.IO.FilterBuilder.FilterBuilder.Filters filter)
        {

            //Return the correct filter string for the filter items
            switch (filter)
            {

                case System.IO.FilterBuilder.FilterBuilder.Filters.WordDocuments:

                    return "Microsoft Word Documents (*.doc)|*.doc";
                case System.IO.FilterBuilder.FilterBuilder.Filters.WordOpenXMLDocuments:

                    return "Microsoft Word Open XML Documents (*.docx)|*.docx";
                case System.IO.FilterBuilder.FilterBuilder.Filters.LogFiles:

                    return "Log Files (*.log)|*.log";
                case System.IO.FilterBuilder.FilterBuilder.Filters.MailMessages:

                    return "Mail Messages (*.msg)|*.msg";
                case System.IO.FilterBuilder.FilterBuilder.Filters.PagesDocuments:

                    return "Pages Documents (*.pages)|*.pages";
                case System.IO.FilterBuilder.FilterBuilder.Filters.RichTextFiles:

                    return "Rich Text Files (*.rtf)|*.rtf";
                case System.IO.FilterBuilder.FilterBuilder.Filters.TextFiles:

                    return "Plain Text Files (*.txt)|*.txt";
                case System.IO.FilterBuilder.FilterBuilder.Filters.WordPerfectDocuments:

                    return "WordPerfect Documents (*.wpd)|*.wpd";
                case System.IO.FilterBuilder.FilterBuilder.Filters.WorksWordProcessorDocuments:

                    return "Microsoft Works Word Processor Documents (*.wps)|*.wps";
                case System.IO.FilterBuilder.FilterBuilder.Filters.Lotus123Spreadsheets:

                    return "Lotus 1-2-3 Spreadsheets (*.123)|*.123";
                case System.IO.FilterBuilder.FilterBuilder.Filters.Access2007DatabaseFiles:

                    return "Access 2007 Database Files (*.accdb)|*.accdb";
                case System.IO.FilterBuilder.FilterBuilder.Filters.CSV_Files:

                    return "Comma Separated Values Files (*.csv)|*.csv";
                case System.IO.FilterBuilder.FilterBuilder.Filters.DataFiles:

                    return "Data Files (*.dat)|*.dat";
                case System.IO.FilterBuilder.FilterBuilder.Filters.DatabaseFiles:

                    return "Database Files (*.db)|*.db";
                case System.IO.FilterBuilder.FilterBuilder.Filters.DLL_Files:

                    return "Dynamic Link Library Files (*.dll)|*.dll";
                case System.IO.FilterBuilder.FilterBuilder.Filters.AccessDatabaseFiles:

                    return "Microsoft Access Database Files (*.mdb)|*.mdb";
                case System.IO.FilterBuilder.FilterBuilder.Filters.PowerPointSlideShows:

                    return "PowerPoint Slide Shows (*.pps)|*.pps";
                case System.IO.FilterBuilder.FilterBuilder.Filters.PowerPointPresentations:

                    return "PowerPoint Presentations (*.ppt)|*.ppt";
                case System.IO.FilterBuilder.FilterBuilder.Filters.PowerPointOpenXMLDocuments:

                    return "Microsoft PowerPoint Open XML Documents (*.pptx)|*.pptx";
                case System.IO.FilterBuilder.FilterBuilder.Filters.OpenOfficeBaseDatabaseFiles:

                    return "OpenOffice.org Base Database Files (*.sdb)|*.sdb";
                case System.IO.FilterBuilder.FilterBuilder.Filters.SQLDataFiles:

                    return "SQL Data Files (*.sql)|*.sql";
                case System.IO.FilterBuilder.FilterBuilder.Filters.vCardFiles:

                    return "vCard Files (*.vcf)|*.vcf";
                case System.IO.FilterBuilder.FilterBuilder.Filters.UCConversionFiles:

                    return "Universal Converter Conversion Files (*.ucv)|*.ucv";
                case System.IO.FilterBuilder.FilterBuilder.Filters.WorksSpreadsheets:

                    return "Microsoft Works Spreadsheets (*.wks)|*.wks";
                case System.IO.FilterBuilder.FilterBuilder.Filters.ExcelSpreadsheets:

                    return "Microsoft Excel Spreadsheets (*.xls)|*.xls";
                case System.IO.FilterBuilder.FilterBuilder.Filters.ExcelOpenXMLDocuments:

                    return "Microsoft Excel Open XML Documents (*.xlsx)|*.xlsx";
                case System.IO.FilterBuilder.FilterBuilder.Filters.XML_Files:

                    return "XML Files (*.xml)|*.xml";
                case System.IO.FilterBuilder.FilterBuilder.Filters.BMP_ImageFiles:

                    return "Bitmap Image Files (*.bmp)|*.bmp";
                case System.IO.FilterBuilder.FilterBuilder.Filters.GIF_ImageFiles:

                    return "Graphical Interchange Format Files (*.gif)|*.gif";
                case System.IO.FilterBuilder.FilterBuilder.Filters.JPEG_ImageFiles:

                    return "JPEG Image Files (*.jpg,*.jpeg)|*.jpg;*.jpeg";
                case System.IO.FilterBuilder.FilterBuilder.Filters.PNG_ImageFiles:

                    return "Portable Network Graphic Files (*.png)|*.png";
                case System.IO.FilterBuilder.FilterBuilder.Filters.AllImageFiles:

                    return "All Supported Image Files|*.bmp;*.gif;*.jpg" + ";*.jpeg;*.png;*.tif;*.tiff";
                case System.IO.FilterBuilder.FilterBuilder.Filters.PhotoshopDocuments:

                    return "Photoshop Documents (*.psd)|*.psd";
                case System.IO.FilterBuilder.FilterBuilder.Filters.PaintShopProImageFiles:

                    return "Paint Shop Pro Image Files (*.psp)|*.psp";
                case System.IO.FilterBuilder.FilterBuilder.Filters.ThumbnailImageFiles:

                    return "Thumbnail Image Files (*.thm)|*.thm";
                case System.IO.FilterBuilder.FilterBuilder.Filters.TIFF_ImageFiles:

                    return "Tagged Image Files (*.tif,*.tiff)|*.tif;*.tiff";
                case System.IO.FilterBuilder.FilterBuilder.Filters.AdobeIllustratorFiles:

                    return "Adobe Illustrator Files (*.ai)|*.ai";
                case System.IO.FilterBuilder.FilterBuilder.Filters.DrawingFiles:

                    return "Drawing Files (*.drw)|*.drw";
                case System.IO.FilterBuilder.FilterBuilder.Filters.DrawingExchangeFormatFiles:

                    return "Drawing Exchange Format Files (*.dxf)|*.dxf";
                case System.IO.FilterBuilder.FilterBuilder.Filters.EncapsulatedPostScriptFiles:

                    return "Encapsulated PostScript Files (*.eps)|*.eps";
                case System.IO.FilterBuilder.FilterBuilder.Filters.PostScriptFiles:

                    return "PostScript Files (*.ps)|*.ps";
                case System.IO.FilterBuilder.FilterBuilder.Filters.SVG_Files:

                    return "Scalable Vector Graphics Files (*.svg)|*.svg";
                case System.IO.FilterBuilder.FilterBuilder.Filters.Rhino3DModels:

                    return "Rhino 3D Models (*.3dm)|*.3dm";
                case System.IO.FilterBuilder.FilterBuilder.Filters.AutoCADDrawingDatabaseFiles:

                    return "AutoCAD Drawing Database Files (*.dwg)|*.dwg";
                case System.IO.FilterBuilder.FilterBuilder.Filters.ArchiCADProjectFiles:

                    return "ArchiCAD Project Files (*.pln)|*.pln";
                case System.IO.FilterBuilder.FilterBuilder.Filters.AdobeInDesignFiles:

                    return "Adobe InDesign Files (*.indd)|*.indd";
                case System.IO.FilterBuilder.FilterBuilder.Filters.PDF_Files:

                    return "Portable Document Format Files (*.pdf)|*.pdf";
                case System.IO.FilterBuilder.FilterBuilder.Filters.AAC_Files:

                    return "Advanced Audio Coding Files (*.aac)|*.aac";
                case System.IO.FilterBuilder.FilterBuilder.Filters.AIF_Files:

                    return "Audio Interchange File Format Files (*.aif)|*.aif";
                case System.IO.FilterBuilder.FilterBuilder.Filters.IIF_Files:

                    return "Interchange File Format Files (*.iif)|*.iif";
                case System.IO.FilterBuilder.FilterBuilder.Filters.MediaPlaylistFiles:

                    return "Media Playlist Files (*.m3u)|*.m3u";
                case System.IO.FilterBuilder.FilterBuilder.Filters.MIDI_Files:

                    return "MIDI Files (*.mid,*.midi)|*.mid;*.midi";
                case System.IO.FilterBuilder.FilterBuilder.Filters.MP3_AudioFiles:

                    return "MP3 Audio Files (*.mp3)|*.mp3";
                case System.IO.FilterBuilder.FilterBuilder.Filters.MPEG2_AudioFiles:

                    return "MPEG-2 Audio Files (*.mpa)|*.mpa";
                case System.IO.FilterBuilder.FilterBuilder.Filters.RealAudioFiles:

                    return "Real Audio Files (*.ra)|*.ra";
                case System.IO.FilterBuilder.FilterBuilder.Filters.WAVE_AudioFiles:

                    return "WAVE Audio Files (*.wav)|*.wav";
                case System.IO.FilterBuilder.FilterBuilder.Filters.WindowsMediaAudioFiles:

                    return "Windows Media Audio Files (*.wma)|*.wma";
                case System.IO.FilterBuilder.FilterBuilder.Filters._3GPP2MultimediaFiles:

                    return "3GPP2 Multimedia Files (*.3g2)|*.3g2";
                case System.IO.FilterBuilder.FilterBuilder.Filters._3GPPMultimediaFiles:

                    return "3GPP Multimedia Files (*.3gp)|*.3gp";
                case System.IO.FilterBuilder.FilterBuilder.Filters.AVI_Files:

                    return "Audio Video Interleave Files (*.avi)|*.avi";
                case System.IO.FilterBuilder.FilterBuilder.Filters.FlashVideoFiles:

                    return "Flash Video Files (*.flv)|*.flv";
                case System.IO.FilterBuilder.FilterBuilder.Filters.MatroskaVideoFiles:

                    return "Matroska Video Files (*.mkv)|*.mkv";
                case System.IO.FilterBuilder.FilterBuilder.Filters.AppleQuickTimeMoviesMov:

                    return "Apple QuickTime Movie Files (*.mov)|*.mov";
                case System.IO.FilterBuilder.FilterBuilder.Filters.MPEG4_VideoFiles:

                    return "MPEG-4 Video Files (*.mp4)|*.mp4";
                case System.IO.FilterBuilder.FilterBuilder.Filters.MPEG_VideoFiles:

                    return "MPEG Video Files (*.mpg)|*.mpg";
                case System.IO.FilterBuilder.FilterBuilder.Filters.AppleQuickTimeMoviesQT:

                    return "Apple QuickTime Movie Files (*.qt)|*.qt";
                case System.IO.FilterBuilder.FilterBuilder.Filters.RealMediaFiles:

                    return "Real Media Files (*.rm)|*.rm";
                case System.IO.FilterBuilder.FilterBuilder.Filters.FlashMovies:

                    return "Flash Movies (*.swf)|*.swf";
                case System.IO.FilterBuilder.FilterBuilder.Filters.DVDVideoObjectFiles:

                    return "DVD Video Object Files (*.vob)|*.vob";
                case System.IO.FilterBuilder.FilterBuilder.Filters.WindowsMediaVideoFiles:

                    return "Windows Media Video Files (*.wmv)|*.wmv";
                case System.IO.FilterBuilder.FilterBuilder.Filters.ActiveServerPages:

                    return "Active Server Pages (*.asp)|*.asp";
                case System.IO.FilterBuilder.FilterBuilder.Filters.CascadingStyleSheets:

                    return "Cascading Style Sheets (*.css)|*.css";
                case System.IO.FilterBuilder.FilterBuilder.Filters.HTML_Files:

                    return "HTML Files (*.htm,*.html)|*.htm;*.html";
                case System.IO.FilterBuilder.FilterBuilder.Filters.JavaScriptFiles:

                    return "JavaScript Files (*.js)|*.js";
                case System.IO.FilterBuilder.FilterBuilder.Filters.JavaServerPages:

                    return "Java Server Pages (*.jsp)|*.jsp";
                case System.IO.FilterBuilder.FilterBuilder.Filters.PHP_Files:

                    return "Hypertext Preprocessor Files (*.php)|*.php";
                case System.IO.FilterBuilder.FilterBuilder.Filters.RichSiteSummaryFiles:

                    return "Rich Site Summary Files (*.rss)|*.rss";
                case System.IO.FilterBuilder.FilterBuilder.Filters.XHTML_Files:

                    return "XHTML Files (*.xhtml)|*.xhtml";
                case System.IO.FilterBuilder.FilterBuilder.Filters.WindowsFontFiles:

                    return "Windows Font Files (*.fnt)|*.fnt";
                case System.IO.FilterBuilder.FilterBuilder.Filters.GenericFontFiles:

                    return "Generic Font Files (*.fon)|*.fon";
                case System.IO.FilterBuilder.FilterBuilder.Filters.OpenTypeFonts:

                    return "OpenType Fonts (*.otf)|*.otf";
                case System.IO.FilterBuilder.FilterBuilder.Filters.TrueTypeFonts:

                    return "TrueType Fonts (*.ttf)|*.ttf";
                case System.IO.FilterBuilder.FilterBuilder.Filters.ExcelAddInFiles:

                    return "Excel Add-In Files (*.xll)|*.xll";
                case System.IO.FilterBuilder.FilterBuilder.Filters.WindowsCabinetFiles:

                    return "Windows Cabinet Files (*.cab)|*.cab";
                case System.IO.FilterBuilder.FilterBuilder.Filters.WindowsControlPanel:

                    return "Windows Control Panel (*.cpl)|*.cpl";
                case System.IO.FilterBuilder.FilterBuilder.Filters.WindowsCursors:

                    return "Windows Cursors (*.cur)|*.cur";
                case System.IO.FilterBuilder.FilterBuilder.Filters.WindowsMemoryDumps:

                    return "Windows Memory Dumps (*.dmp)|*.dmp";
                case System.IO.FilterBuilder.FilterBuilder.Filters.DeviceDrivers:

                    return "Device Drivers (*.drv)|*.drv";
                case System.IO.FilterBuilder.FilterBuilder.Filters.SecurityKeys:

                    return "Security Keys (*.key)|*.key";
                case System.IO.FilterBuilder.FilterBuilder.Filters.FileShortcuts:

                    return "File Shortcuts (*.lnk)|*.lnk";
                case System.IO.FilterBuilder.FilterBuilder.Filters.WindowsSystemFiles:

                    return "Windows System Files (*.sys)|*.sys";
                case System.IO.FilterBuilder.FilterBuilder.Filters.ConfigurationFiles:

                    return "Configuration Files (*.cfg)|*.cfg";
                case System.IO.FilterBuilder.FilterBuilder.Filters.INI_Files:

                    return "Windows Initialization Files (*.ini)|*.ini";
                case System.IO.FilterBuilder.FilterBuilder.Filters.OutlookProfileFiles:

                    return "Outlook Profile Files (*.prf)|*.prf";
                case System.IO.FilterBuilder.FilterBuilder.Filters.MacOSXApplications:

                    return "Mac OS X Applications (*.app)|*.app";
                case System.IO.FilterBuilder.FilterBuilder.Filters.DOSBatchFiles:

                    return "DOS Batch Files (*.bat)|*.bat";
                case System.IO.FilterBuilder.FilterBuilder.Filters.CGI_Files:

                    return "Common Gateway Interface Scripts (*.cgi)|*.cgi";
                case System.IO.FilterBuilder.FilterBuilder.Filters.DOSCommandFiles:

                    return "DOS Command Files (*.com)|*.com";
                case System.IO.FilterBuilder.FilterBuilder.Filters.WindowsExecutableFiles:

                    return "Windows Executable File (*.exe)|*.exe";
                case System.IO.FilterBuilder.FilterBuilder.Filters.WindowsScripts:

                    return "Windows Scripts (*.ws)|*.ws";
                case System.IO.FilterBuilder.FilterBuilder.Filters._7ZipCompressedFiles:

                    return "7-Zip Compressed Files (*.7z)|*.7z";
                case System.IO.FilterBuilder.FilterBuilder.Filters.DebianSoftwarePackages:

                    return "Debian Software Packages (*.deb)|*.deb";
                case System.IO.FilterBuilder.FilterBuilder.Filters.GnuZippedFile:

                    return "Gnu Zipped Files (*.gz)|*.gz";
                case System.IO.FilterBuilder.FilterBuilder.Filters.MacOSXInstallerPackages:

                    return "Mac OS X Installer Packages (*.pkg)|*.pkg";
                case System.IO.FilterBuilder.FilterBuilder.Filters.WinRARCompressedArchives:

                    return "WinRAR Compressed Archives (*.rar)|*.rar";
                case System.IO.FilterBuilder.FilterBuilder.Filters.SelfExtractingArchives:

                    return "Self-Extractingd Archives (*.sea)|*.sea";
                case System.IO.FilterBuilder.FilterBuilder.Filters.StuffitArchives:

                    return "Stuffit Archives (*.sit)|*.sit";
                case System.IO.FilterBuilder.FilterBuilder.Filters.StuffitXArchives:

                    return "Stuffit X Archives (*.sitx)|*.sitx";
                case System.IO.FilterBuilder.FilterBuilder.Filters.ZippedFiles:

                    return "Zipped Files (*.zip)|*.zip";
                case System.IO.FilterBuilder.FilterBuilder.Filters.ExtendedZipFiles:

                    return "Extended Zip Files (*.zipx)|*.zipx";
                case System.IO.FilterBuilder.FilterBuilder.Filters.BinHex4EncodedFiles:

                    return "BinHex 4.0 Encoded Files (*.hqx)|*.hqx";
                case System.IO.FilterBuilder.FilterBuilder.Filters.MultiPurposeInternetMailMessages:

                    return "Multi-Purpose Internet Mail Messages (*.mim)|*.mim";
                case System.IO.FilterBuilder.FilterBuilder.Filters.UuencodedFiles:

                    return "Uuencoded Files (*.uue)|*.uue";
                case System.IO.FilterBuilder.FilterBuilder.Filters.C_CPlusPlus_SourceCodeFiles:

                    return "C/C++ Source Code Files (*.c)|*.c";
                case System.IO.FilterBuilder.FilterBuilder.Filters.CPlusPlus_SourceCodeFiles:

                    return "C++ Source Code Files (*.cpp)|*.cpp";
                case System.IO.FilterBuilder.FilterBuilder.Filters.Java_SourceCodeFiles:

                    return "Java Source Code Files (*.java)|*.java";
                case System.IO.FilterBuilder.FilterBuilder.Filters.PerlScripts:

                    return "Perl Scripts (*.pl)|*.pl";
                case System.IO.FilterBuilder.FilterBuilder.Filters.VB_SourceCodeFiles:

                    return "VB Source Code Files (*.vb)|*.vb";
                case System.IO.FilterBuilder.FilterBuilder.Filters.VisualStudioSolutionFiles:

                    return "Visual Studio Solution Files (*.sln)|*.sln";
                case System.IO.FilterBuilder.FilterBuilder.Filters.CSharp_SourceCodeFiles:

                    return "C# Source Code Files (*.cs)|*.cs";
                case System.IO.FilterBuilder.FilterBuilder.Filters.BackupFiles_BAK:

                    return "Backup Files (*.bak)|*.bak";
                case System.IO.FilterBuilder.FilterBuilder.Filters.BackupFiles_BUP:

                    return "Backup Files (*.bup)|*.bup";
                case System.IO.FilterBuilder.FilterBuilder.Filters.NortonGhostBackupFiles:

                    return "Norton Ghost Backup Files (*.gho)|*.gho";
                case System.IO.FilterBuilder.FilterBuilder.Filters.OriginalFiles:

                    return "Original Files (*.ori)|*.ori";
                case System.IO.FilterBuilder.FilterBuilder.Filters.TemporaryFiles:

                    return "Temporary Files (*.tmp)|*.tmp";
                case System.IO.FilterBuilder.FilterBuilder.Filters.DiscImageFiles:

                    return "Disc Image Files (*.iso)|*.iso";
                case System.IO.FilterBuilder.FilterBuilder.Filters.ToastDiscImages:

                    return "Toast Disc Images (*.toast)|*.toast";
                case System.IO.FilterBuilder.FilterBuilder.Filters.Virtual_CDs:

                    return "Virtual CDs (*.vcd)|*.vcd";
                case System.IO.FilterBuilder.FilterBuilder.Filters.WindowsInstallerPackages:

                    return "Windows Installer Packages (*.msi)|*.msi";
                case System.IO.FilterBuilder.FilterBuilder.Filters.PartiallyDownloadedFiles:

                    return "Partially Downloaded Files (*.part)|*.part";
                case System.IO.FilterBuilder.FilterBuilder.Filters.BitTorrentFiles:

                    return "BitTorrent Files (*.torrent)|*.torrent";
                case System.IO.FilterBuilder.FilterBuilder.Filters.YahooMessengerDataFiles:

                    return "Yahoo! Messenger Data Files (*.yps)|*.yps";
                case System.IO.FilterBuilder.FilterBuilder.Filters.AllFiles:

                    return "All Files (*.*)|*.*";
                case System.IO.FilterBuilder.FilterBuilder.Filters.WindowsIcons:

                    return "Windows Icons (*.ico)|*.ico";
                case System.IO.FilterBuilder.FilterBuilder.Filters.EXIF_ImageFiles:

                    return "Exchangeable Image Format Files (*.exif)|*.exif";
                default:

                    return null;
            }

        }

        #endregion

    }
}