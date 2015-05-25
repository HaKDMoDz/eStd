namespace Creek.IO.FilterBuilder
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
        public static string ReturnFilterAsString(Creek.IO.FilterBuilder.FilterBuilder.Filters filter)
        {

            //Return the correct filter string for the filter items
            switch (filter)
            {

                case Creek.IO.FilterBuilder.FilterBuilder.Filters.WordDocuments:

                    return "Microsoft Word Documents (*.doc)|*.doc";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.WordOpenXMLDocuments:

                    return "Microsoft Word Open XML Documents (*.docx)|*.docx";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.LogFiles:

                    return "Log Files (*.log)|*.log";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.MailMessages:

                    return "Mail Messages (*.msg)|*.msg";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.PagesDocuments:

                    return "Pages Documents (*.pages)|*.pages";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.RichTextFiles:

                    return "Rich Text Files (*.rtf)|*.rtf";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.TextFiles:

                    return "Plain Text Files (*.txt)|*.txt";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.WordPerfectDocuments:

                    return "WordPerfect Documents (*.wpd)|*.wpd";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.WorksWordProcessorDocuments:

                    return "Microsoft Works Word Processor Documents (*.wps)|*.wps";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.Lotus123Spreadsheets:

                    return "Lotus 1-2-3 Spreadsheets (*.123)|*.123";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.Access2007DatabaseFiles:

                    return "Access 2007 Database Files (*.accdb)|*.accdb";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.CSV_Files:

                    return "Comma Separated Values Files (*.csv)|*.csv";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.DataFiles:

                    return "Data Files (*.dat)|*.dat";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.DatabaseFiles:

                    return "Database Files (*.db)|*.db";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.DLL_Files:

                    return "Dynamic Link Library Files (*.dll)|*.dll";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.AccessDatabaseFiles:

                    return "Microsoft Access Database Files (*.mdb)|*.mdb";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.PowerPointSlideShows:

                    return "PowerPoint Slide Shows (*.pps)|*.pps";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.PowerPointPresentations:

                    return "PowerPoint Presentations (*.ppt)|*.ppt";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.PowerPointOpenXMLDocuments:

                    return "Microsoft PowerPoint Open XML Documents (*.pptx)|*.pptx";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.OpenOfficeBaseDatabaseFiles:

                    return "OpenOffice.org Base Database Files (*.sdb)|*.sdb";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.SQLDataFiles:

                    return "SQL Data Files (*.sql)|*.sql";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.vCardFiles:

                    return "vCard Files (*.vcf)|*.vcf";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.UCConversionFiles:

                    return "Universal Converter Conversion Files (*.ucv)|*.ucv";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.WorksSpreadsheets:

                    return "Microsoft Works Spreadsheets (*.wks)|*.wks";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.ExcelSpreadsheets:

                    return "Microsoft Excel Spreadsheets (*.xls)|*.xls";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.ExcelOpenXMLDocuments:

                    return "Microsoft Excel Open XML Documents (*.xlsx)|*.xlsx";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.XML_Files:

                    return "XML Files (*.xml)|*.xml";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.BMP_ImageFiles:

                    return "Bitmap Image Files (*.bmp)|*.bmp";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.GIF_ImageFiles:

                    return "Graphical Interchange Format Files (*.gif)|*.gif";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.JPEG_ImageFiles:

                    return "JPEG Image Files (*.jpg,*.jpeg)|*.jpg;*.jpeg";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.PNG_ImageFiles:

                    return "Portable Network Graphic Files (*.png)|*.png";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.AllImageFiles:

                    return "All Supported Image Files|*.bmp;*.gif;*.jpg" + ";*.jpeg;*.png;*.tif;*.tiff";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.PhotoshopDocuments:

                    return "Photoshop Documents (*.psd)|*.psd";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.PaintShopProImageFiles:

                    return "Paint Shop Pro Image Files (*.psp)|*.psp";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.ThumbnailImageFiles:

                    return "Thumbnail Image Files (*.thm)|*.thm";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.TIFF_ImageFiles:

                    return "Tagged Image Files (*.tif,*.tiff)|*.tif;*.tiff";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.AdobeIllustratorFiles:

                    return "Adobe Illustrator Files (*.ai)|*.ai";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.DrawingFiles:

                    return "Drawing Files (*.drw)|*.drw";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.DrawingExchangeFormatFiles:

                    return "Drawing Exchange Format Files (*.dxf)|*.dxf";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.EncapsulatedPostScriptFiles:

                    return "Encapsulated PostScript Files (*.eps)|*.eps";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.PostScriptFiles:

                    return "PostScript Files (*.ps)|*.ps";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.SVG_Files:

                    return "Scalable Vector Graphics Files (*.svg)|*.svg";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.Rhino3DModels:

                    return "Rhino 3D Models (*.3dm)|*.3dm";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.AutoCADDrawingDatabaseFiles:

                    return "AutoCAD Drawing Database Files (*.dwg)|*.dwg";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.ArchiCADProjectFiles:

                    return "ArchiCAD Project Files (*.pln)|*.pln";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.AdobeInDesignFiles:

                    return "Adobe InDesign Files (*.indd)|*.indd";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.PDF_Files:

                    return "Portable Document Format Files (*.pdf)|*.pdf";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.AAC_Files:

                    return "Advanced Audio Coding Files (*.aac)|*.aac";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.AIF_Files:

                    return "Audio Interchange File Format Files (*.aif)|*.aif";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.IIF_Files:

                    return "Interchange File Format Files (*.iif)|*.iif";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.MediaPlaylistFiles:

                    return "Media Playlist Files (*.m3u)|*.m3u";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.MIDI_Files:

                    return "MIDI Files (*.mid,*.midi)|*.mid;*.midi";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.MP3_AudioFiles:

                    return "MP3 Audio Files (*.mp3)|*.mp3";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.MPEG2_AudioFiles:

                    return "MPEG-2 Audio Files (*.mpa)|*.mpa";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.RealAudioFiles:

                    return "Real Audio Files (*.ra)|*.ra";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.WAVE_AudioFiles:

                    return "WAVE Audio Files (*.wav)|*.wav";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.WindowsMediaAudioFiles:

                    return "Windows Media Audio Files (*.wma)|*.wma";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters._3GPP2MultimediaFiles:

                    return "3GPP2 Multimedia Files (*.3g2)|*.3g2";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters._3GPPMultimediaFiles:

                    return "3GPP Multimedia Files (*.3gp)|*.3gp";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.AVI_Files:

                    return "Audio Video Interleave Files (*.avi)|*.avi";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.FlashVideoFiles:

                    return "Flash Video Files (*.flv)|*.flv";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.MatroskaVideoFiles:

                    return "Matroska Video Files (*.mkv)|*.mkv";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.AppleQuickTimeMoviesMov:

                    return "Apple QuickTime Movie Files (*.mov)|*.mov";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.MPEG4_VideoFiles:

                    return "MPEG-4 Video Files (*.mp4)|*.mp4";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.MPEG_VideoFiles:

                    return "MPEG Video Files (*.mpg)|*.mpg";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.AppleQuickTimeMoviesQT:

                    return "Apple QuickTime Movie Files (*.qt)|*.qt";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.RealMediaFiles:

                    return "Real Media Files (*.rm)|*.rm";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.FlashMovies:

                    return "Flash Movies (*.swf)|*.swf";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.DVDVideoObjectFiles:

                    return "DVD Video Object Files (*.vob)|*.vob";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.WindowsMediaVideoFiles:

                    return "Windows Media Video Files (*.wmv)|*.wmv";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.ActiveServerPages:

                    return "Active Server Pages (*.asp)|*.asp";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.CascadingStyleSheets:

                    return "Cascading Style Sheets (*.css)|*.css";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.HTML_Files:

                    return "HTML Files (*.htm,*.html)|*.htm;*.html";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.JavaScriptFiles:

                    return "JavaScript Files (*.js)|*.js";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.JavaServerPages:

                    return "Java Server Pages (*.jsp)|*.jsp";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.PHP_Files:

                    return "Hypertext Preprocessor Files (*.php)|*.php";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.RichSiteSummaryFiles:

                    return "Rich Site Summary Files (*.rss)|*.rss";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.XHTML_Files:

                    return "XHTML Files (*.xhtml)|*.xhtml";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.WindowsFontFiles:

                    return "Windows Font Files (*.fnt)|*.fnt";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.GenericFontFiles:

                    return "Generic Font Files (*.fon)|*.fon";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.OpenTypeFonts:

                    return "OpenType Fonts (*.otf)|*.otf";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.TrueTypeFonts:

                    return "TrueType Fonts (*.ttf)|*.ttf";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.ExcelAddInFiles:

                    return "Excel Add-In Files (*.xll)|*.xll";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.WindowsCabinetFiles:

                    return "Windows Cabinet Files (*.cab)|*.cab";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.WindowsControlPanel:

                    return "Windows Control Panel (*.cpl)|*.cpl";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.WindowsCursors:

                    return "Windows Cursors (*.cur)|*.cur";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.WindowsMemoryDumps:

                    return "Windows Memory Dumps (*.dmp)|*.dmp";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.DeviceDrivers:

                    return "Device Drivers (*.drv)|*.drv";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.SecurityKeys:

                    return "Security Keys (*.key)|*.key";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.FileShortcuts:

                    return "File Shortcuts (*.lnk)|*.lnk";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.WindowsSystemFiles:

                    return "Windows System Files (*.sys)|*.sys";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.ConfigurationFiles:

                    return "Configuration Files (*.cfg)|*.cfg";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.INI_Files:

                    return "Windows Initialization Files (*.ini)|*.ini";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.OutlookProfileFiles:

                    return "Outlook Profile Files (*.prf)|*.prf";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.MacOSXApplications:

                    return "Mac OS X Applications (*.app)|*.app";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.DOSBatchFiles:

                    return "DOS Batch Files (*.bat)|*.bat";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.CGI_Files:

                    return "Common Gateway Interface Scripts (*.cgi)|*.cgi";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.DOSCommandFiles:

                    return "DOS Command Files (*.com)|*.com";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.WindowsExecutableFiles:

                    return "Windows Executable File (*.exe)|*.exe";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.WindowsScripts:

                    return "Windows Scripts (*.ws)|*.ws";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters._7ZipCompressedFiles:

                    return "7-Zip Compressed Files (*.7z)|*.7z";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.DebianSoftwarePackages:

                    return "Debian Software Packages (*.deb)|*.deb";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.GnuZippedFile:

                    return "Gnu Zipped Files (*.gz)|*.gz";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.MacOSXInstallerPackages:

                    return "Mac OS X Installer Packages (*.pkg)|*.pkg";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.WinRARCompressedArchives:

                    return "WinRAR Compressed Archives (*.rar)|*.rar";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.SelfExtractingArchives:

                    return "Self-Extractingd Archives (*.sea)|*.sea";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.StuffitArchives:

                    return "Stuffit Archives (*.sit)|*.sit";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.StuffitXArchives:

                    return "Stuffit X Archives (*.sitx)|*.sitx";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.ZippedFiles:

                    return "Zipped Files (*.zip)|*.zip";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.ExtendedZipFiles:

                    return "Extended Zip Files (*.zipx)|*.zipx";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.BinHex4EncodedFiles:

                    return "BinHex 4.0 Encoded Files (*.hqx)|*.hqx";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.MultiPurposeInternetMailMessages:

                    return "Multi-Purpose Internet Mail Messages (*.mim)|*.mim";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.UuencodedFiles:

                    return "Uuencoded Files (*.uue)|*.uue";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.C_CPlusPlus_SourceCodeFiles:

                    return "C/C++ Source Code Files (*.c)|*.c";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.CPlusPlus_SourceCodeFiles:

                    return "C++ Source Code Files (*.cpp)|*.cpp";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.Java_SourceCodeFiles:

                    return "Java Source Code Files (*.java)|*.java";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.PerlScripts:

                    return "Perl Scripts (*.pl)|*.pl";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.VB_SourceCodeFiles:

                    return "VB Source Code Files (*.vb)|*.vb";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.VisualStudioSolutionFiles:

                    return "Visual Studio Solution Files (*.sln)|*.sln";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.CSharp_SourceCodeFiles:

                    return "C# Source Code Files (*.cs)|*.cs";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.BackupFiles_BAK:

                    return "Backup Files (*.bak)|*.bak";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.BackupFiles_BUP:

                    return "Backup Files (*.bup)|*.bup";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.NortonGhostBackupFiles:

                    return "Norton Ghost Backup Files (*.gho)|*.gho";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.OriginalFiles:

                    return "Original Files (*.ori)|*.ori";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.TemporaryFiles:

                    return "Temporary Files (*.tmp)|*.tmp";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.DiscImageFiles:

                    return "Disc Image Files (*.iso)|*.iso";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.ToastDiscImages:

                    return "Toast Disc Images (*.toast)|*.toast";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.Virtual_CDs:

                    return "Virtual CDs (*.vcd)|*.vcd";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.WindowsInstallerPackages:

                    return "Windows Installer Packages (*.msi)|*.msi";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.PartiallyDownloadedFiles:

                    return "Partially Downloaded Files (*.part)|*.part";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.BitTorrentFiles:

                    return "BitTorrent Files (*.torrent)|*.torrent";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.YahooMessengerDataFiles:

                    return "Yahoo! Messenger Data Files (*.yps)|*.yps";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.AllFiles:

                    return "All Files (*.*)|*.*";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.WindowsIcons:

                    return "Windows Icons (*.ico)|*.ico";
                case Creek.IO.FilterBuilder.FilterBuilder.Filters.EXIF_ImageFiles:

                    return "Exchangeable Image Format Files (*.exif)|*.exif";
                default:

                    return null;
            }

        }

        #endregion

    }
}