﻿using System;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Text;
using Creek.I18N.Gettext.Loaders;

namespace Creek.I18N.Gettext
{
	/// <summary>
	/// Represents a Gettext catalog instance.
	/// Loads translations from gettext *.mo files.
	/// </summary>
	public class Catalog : BaseCatalog
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Catalog"/> class with no translations and with current CultureInfo.
		/// </summary>
		public Catalog()
			: base(CultureInfo.CurrentUICulture)
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Catalog"/> class with no translations and with given CultureInfo.
		/// </summary>
		/// <param name="cultureInfo">Culture info</param>
		public Catalog(CultureInfo cultureInfo)
			: base(cultureInfo)
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Catalog"/> class with current CultureInfo
		/// and loads MO translations from given stream.
		/// </summary>
		/// <param name="moStream">Stream that contain binary data in the MO file format</param>
		public Catalog(Stream moStream)
			: this()
		{
			try
			{
				this.Load(moStream);
			}
			catch (FileNotFoundException exception)
			{
				Trace.WriteLine(String.Format("Translation file loading fail: {0}", exception.Message), "Lib.Gettext");
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Catalog"/> class with given CultureInfo
		/// and loads MO translations from given stream.
		/// </summary>
		/// <param name="moStream">Stream that contain binary data in the MO file format</param>
		/// <param name="cultureInfo">Culture info</param>
		public Catalog(Stream moStream, CultureInfo cultureInfo)
			: this(cultureInfo)
		{
			try
			{
				this.Load(moStream);
			}
			catch (FileNotFoundException exception)
			{
				Trace.WriteLine(String.Format("Translation file loading fail: {0}", exception.Message), "Lib.Gettext");
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Catalog"/> class with current CultureInfo
		/// and loads translations from MO file that can be found by given parameters.
		/// </summary>
		/// <param name="domain">Catalog domain name</param>
		/// <param name="localeDir">Directory that contains gettext localization files</param>
		public Catalog(string domain, string localeDir)
			: this()
		{
			try
			{
				this.Load(CultureInfo, domain, localeDir);
			}
			catch (FileNotFoundException exception)
			{
				Trace.WriteLine(String.Format("Translation file loading fail: {0}", exception.Message), "Lib.Gettext");
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Catalog"/> class with given CultureInfo
		/// and loads translations from MO file that can be found by given parameters.
		/// </summary>
		/// <param name="domain">Catalog domain name</param>
		/// <param name="localeDir">Directory that contains gettext localization files</param>
		/// <param name="cultureInfo">Culture info</param>
		public Catalog(string domain, string localeDir, CultureInfo cultureInfo)
			: this(cultureInfo)
		{
			try
			{
				this.Load(CultureInfo, domain, localeDir);
			}
			catch (FileNotFoundException exception)
			{
				Trace.WriteLine(String.Format("Translation file loading fail: {0}", exception.Message), "Lib.Gettext");
			}
		}

		/// <summary>
		/// Load translations from MO file that can be found by given parameters.
		/// </summary>
		/// <param name="cultureInfo">Culture info</param>
		/// <param name="domain">Catalog domain name</param>
		/// <param name="localeDir">Directory that contains gettext localization files</param>
		public void Load(CultureInfo cultureInfo, string domain, string localeDir)
		{
			var path =_FindTranslationFile(cultureInfo, domain, localeDir);
			if (path == null)
			{
				throw new FileNotFoundException(String.Format("Can not find MO file name in locale directory \"{0}\".", localeDir));
			}

			this.Load(path);
		}

		/// <summary>
		/// Load translations from MO file that can be found by given path.
		/// </summary>
		/// <param name="path">Path to *.mo file</param>
		public void Load(string path)
		{
			Trace.WriteLine(String.Format("Loading translations from file \"{0}\"...", path), "Lib.Gettext");
			using (var stream = File.OpenRead(path))
			{
				this.Load(stream);
			}
		}

		/// <summary>
		/// Load translations from given MO file stream using given encoding (UTF-8 encoding by default).
		/// </summary>
		/// <param name="moStream">Stream that contain binary data in the MO file format</param>
		/// <param name="encoding">File encoding (UTF-8 by default)</param>
		public void Load(Stream moStream, Encoding encoding = null)
		{
			var parser = new MoFileParser();
			if (encoding != null)
			{
				parser.Encoding = encoding;
			}

			var translations = parser.GetTranslations(moStream);

			this.Translations = translations;
		}

		private string _FindTranslationFile(CultureInfo cultureInfo, string domain, string localeDir)
		{
			var posibleFiles = new [] {
				this._GetFileName(localeDir, domain, cultureInfo.Name),
				this._GetFileName(localeDir, domain, cultureInfo.TwoLetterISOLanguageName)
			};

			foreach (var posibleFilePath in posibleFiles)
			{
				if (File.Exists(posibleFilePath))
				{
					return posibleFilePath;
				}
			}

			return null;
		}

		private string _GetFileName(string localeDir, string domain, string locale)
		{
			var relativePath =
				locale.Replace('-', '_') + Path.DirectorySeparatorChar +
				"LC_MESSAGES" + Path.DirectorySeparatorChar +
				domain + ".mo";

			return Path.Combine(localeDir, relativePath);
		}
	}
}