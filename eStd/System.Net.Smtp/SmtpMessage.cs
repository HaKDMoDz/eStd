using System.Collections.Generic;
using System.Text;

namespace System.Net.Smtp
{
	public class SmtpMessage
	{
		private List<SmtpAttachment> _attachments;
		private List<SmtpHeader> _headers;

		public enum MessageType
		{
			None,
			TextOnly,
			HtmlOnly,
			TextAndHtml
		}

		public IEnumerable<SmtpHeader> Headers
		{
			get { return _headers; }
			set { _headers = value as List<SmtpHeader>; }
		}
		public String BodyText { get; set; }
		public String BodyHtml { get; set; }
		public String Subject { get; set; }
		public String From { get; set; }
		public String To { get; set; }
		public String ReplyTo { get; set; }
		public String ReadReceiptTo { get; set; }
		public MessageType Type { get; set; }
		public IEnumerable<SmtpAttachment> Attachments
		{
			get { return _attachments; }
			set { _attachments = value as List<SmtpAttachment>; }
		}
		
		public SmtpMessage()
		{
			_headers = new List<SmtpHeader>();
			_attachments = new List<SmtpAttachment>();
			BodyText = "";
			BodyHtml = "";
			Subject = "";
			From = "";
			To = "";
			ReplyTo = "";
			ReadReceiptTo = "";
			Type = MessageType.None;
		}

		public String GenerateMessage()
		{
			StringBuilder message = new StringBuilder();
			String bound = "";
			String boundMixed = "";

			if (Type == MessageType.None)
			{
				throw new SmtpException("Type == MessageType.NONE");
			}

			message.AppendLine("To: " + To);
			message.AppendLine("From: " + From);
			message.AppendLine("Subject: " + Subject);
			message.AppendLine("Reply-To: " + (ReplyTo != "" ? ReplyTo : From));
			if (ReadReceiptTo != "") message.AppendLine("Disposition-Notification-To: " + ReadReceiptTo);

			foreach (SmtpHeader h in Headers)
			{
				message.AppendLine(h.Name + ": " + h.Value);
			}

			message.AppendLine("MIME-Version: 1.0");

			if (_attachments.Count > 0)
			{
				bound = GenerateBound();
				boundMixed = GenerateBound();
				message.AppendLine("Content-type: multipart/mixed; boundary=\"" + boundMixed + "\"");
			}
			else switch (Type)
			{
				case MessageType.TextOnly:
					message.AppendLine("Content-type: text/plain");
					break;
				case MessageType.HtmlOnly:
					message.AppendLine("Content-type: text/html");
					break;
				case MessageType.TextAndHtml:
					bound = GenerateBound();
					message.AppendLine("Content-type: multipart/alternative; boundary=\"" + bound + "\"");
					break;
			}

			message.AppendLine();

			if (_attachments.Count > 0)
			{
				// open up bound mixed
				message.AppendLine("--" + boundMixed);
				message.AppendLine("Content-type: multipart/alternative; boundary=\"" + bound + "\"");
				message.AppendLine();

				message.AppendLine("--" + bound);

				if (Type == MessageType.TextOnly)
				{
					message.AppendLine("Content-type: text/plain");
					message.AppendLine();
					message.AppendLine(BodyText);
				}
				else if (Type == MessageType.HtmlOnly)
				{
					message.AppendLine("Content-type: text/html");
					message.AppendLine();
					message.AppendLine(BodyHtml);
				}
				else if (Type == MessageType.TextAndHtml)
				{
					message.AppendLine("Content-type: text/plain");
					message.AppendLine();
					message.AppendLine(BodyText);
					message.AppendLine();

					message.AppendLine("--" + bound);
					message.AppendLine("Content-type: text/html");
					message.AppendLine();
					message.AppendLine(BodyHtml);
				}

				message.AppendLine();
				message.AppendLine("--" + bound + "--");
				message.AppendLine();

				foreach (SmtpAttachment i in Attachments)
				{
					message.AppendLine("--" + boundMixed);
					message.AppendLine("Content-Type: " + i.GetMimeType() + "; name=\"" + i.FileNameShort + "\"");
					message.AppendLine("Content-Transfer-Encoding: base64");
					message.AppendLine("Content-Disposition: attachment; filename=\"" + i.FileNameShort + "\"");
					message.AppendLine();
					message.AppendLine(i.GetBase64());
					message.AppendLine();
				}

				message.AppendLine("--" + boundMixed + "--");
			}
			else
			{
				if (Type == MessageType.TextOnly)
				{
					message.AppendLine(BodyText);
				}
				else if (Type == MessageType.HtmlOnly)
				{
					message.AppendLine(BodyHtml);
				}
				else if (Type == MessageType.TextAndHtml)
				{
					message.AppendLine("--" + bound);
					message.AppendLine("Content-type: text/plain");
					message.AppendLine();
					message.AppendLine(BodyText);

					message.AppendLine("--" + bound);
					message.AppendLine("Content-type: text/html");
					message.AppendLine();
					message.AppendLine(BodyHtml);

					message.AppendLine("--" + bound + "--");
				}
			}

			return message.ToString();
		}

		private static String GenerateBound()
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringHash = new StringBuilder();
			Random random = new Random((int)DateTime.UtcNow.Ticks);
			for (int x = 0; x < 32; x++)
			{
				stringBuilder.Append(random.Next());
			}
			byte[] bs = Encoding.UTF8.GetBytes(stringBuilder.ToString());
			System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
			md5.Initialize();
			byte[] hash = md5.ComputeHash(bs, 0, bs.Length);
			for (int i = 0; i < hash.Length; i++) stringHash.Append(hash[i].ToString("x2"));

			return stringHash.ToString();
		}
	}
}