using System.Text;
using System.IO;

namespace System.Net.Smtp
{
	public class SmtpAttachment
	{
		public String FileName { get; set; }
		public String FileNameShort { get; set; }

		public SmtpAttachment()
		{
			FileName = "";
		}

		public SmtpAttachment(String file)
		{
			LoadFile(file);
		}

		public void LoadFile(String file)
		{
			FileName = file;
			FileNameShort = FileName.Substring(FileName.LastIndexOf('\\')+1);
		}

		public Int32 GetFileSize()
		{
			if (!File.Exists(FileName)) return 0;

			FileInfo fileInfo = new FileInfo(FileName);
			return (Int32)fileInfo.Length;
		}

		public String GetBase64()
		{
			if (!File.Exists(FileName)) return "";

			byte[] fileBytes = File.ReadAllBytes(FileName);
			String fileBase64 = Convert.ToBase64String(fileBytes);

			StringBuilder base64Wrapped = new StringBuilder();
			Int32 base64Length = fileBase64.Length;
			for (int i = 0; i < (base64Length); i+=76)
			{
				base64Wrapped.AppendLine(fileBase64.Substring(i, Math.Min(76, base64Length - i)));
			}

			return base64Wrapped.ToString();
		}

		public String GetMimeType()
		{
			String mimeType = "application/unknown";
			string extension = Path.GetExtension(FileName);

			if (extension == null) return mimeType;

			String fileExtension = extension.ToLower();
			Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(fileExtension);
			if (regKey != null && regKey.GetValue("Content Type") != null)
			{
				mimeType = regKey.GetValue("Content Type").ToString();
			}

			return mimeType;
		}

	}
}