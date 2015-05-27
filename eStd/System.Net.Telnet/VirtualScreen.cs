// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED!
// YOU MAY USE THIS CODE: HOWEVER THIS GRANTS NO FUTURE RIGHTS.
// see http://telnetcsharp.codeplex.com/ for further details and license information
namespace System.Net.Telnet
{
    using System;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
	/// Implements a simple "screen". This is used by telnet.
	/// <p>
	/// The x (rows) and y (columns) values may have an offset. If the offset
	/// is 0/0 the left upper corner is [0,0], or 0-based. With an offset of 1/1
	/// the left upper corner is [1,1], or 1-based.
	/// </p>
	/// </summary>
	/// <remarks>
	/// The class is not thread safe (e.g. search in buffer and modification
	/// of buffer must not happen. It is duty of the calling class to guarantee this.
	/// </remarks>
	public class VirtualScreen : IDisposable
	{
		/// <summary>
		/// ASCII code for Space
		/// </summary>
		public const byte Space = 32;

		// Width

		// External cursor values allowing an offset and thus
		// 0-based or 1-based coordinates
		private readonly int _offsetx;
		private readonly int _offsety;
		private bool _changedScreen;
		private int _cursorx0;
		private int _cursory0;
		private string _screenString;
		private string _screenStringLower;
		private int _visibleAreaY0Bottom;
		private int _visibleAreaY0Top;
		private byte[,] _vs;

		/// <summary>
		/// Constructor (offset 0/0)
		/// </summary>
		/// <param name="width">Screen's width</param>
		/// <param name="height">Screen's height</param>
		public VirtualScreen(int width, int height) : this(width, height, 0, 0)
		{
			// nothing here
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="width">Screen's width</param>
		/// <param name="height">Screen's height</param>
		/// <param name="xOffset">Screen coordinates are 0,1 .. based</param>
		/// <param name="yOffset">Screen coordinates are 0,1 .. based</param>
		public VirtualScreen(int width, int height, int xOffset, int yOffset)
		{
			this._offsetx = xOffset;
			this._offsety = yOffset;
			this._vs = new byte[width,height];
			this.CleanScreen();
			this._changedScreen = false; // reset becuase constructor
			this._visibleAreaY0Top = 0;
			this._visibleAreaY0Bottom = height - 1;
			this.CursorReset();
		}

		/// <summary>
		/// window size 
		/// </summary>
		public int Width { get { return this._vs == null ? 0 : this._vs.GetLength(0); } } // Width

		/// <summary>
		/// Window height
		/// </summary>
		public int Height
		{
			get
			{
				if (this._vs == null) return 0;
				return this._vs.GetLength(1);
			}
		}

		/// <summary>
		/// Cursor position with offset considered
		/// </summary>
		public int CursorX
		{
			get
			{
				// 2004-09-01 fixed to plus based on mail of Steve
				return this.CursorX0 + this._offsetx;
			}
			set { this.CursorX0 = value - this._offsetx; }
		} // X

		/// <summary>
		/// Cursor position with offset considered
		/// </summary>
		public int CursorY
		{
			get
			{
				// 2004-09-01 fixed to plus based on mail of Steve
				return this.CursorY0 + this._offsety;
			}
			set { this.CursorY0 = value - this._offsety; }
		} // Y

		/// <summary>
		/// X Offset 
		/// </summary>
		public int CursorXLeft { get { return this._offsetx; } }

		/// <summary>
		/// X max value 
		/// </summary>
		public int CursorXRight { get { return this.Width - 1 + this._offsetx; } }
		/// <summary>
		/// Y max value 
		/// </summary>
		public int CursorYMax { get { return this.Height - 1 + this._offsety; } }
		/// <summary>
		/// Y max value 
		/// </summary>
		public int CursorYMin { get { return this._offsety; } }

		/// <summary>
		/// 0-based coordinates for cursor, internally used.
		/// </summary>
		private int CursorX0
		{
			get { return this._cursorx0; }
			set
			{
				if (value <= 0)
					this._cursorx0 = 0;
				else if (value >= this.Width)
					this._cursorx0 = this.Width - 1;
				else
					this._cursorx0 = value;
			}
		}
		/// <summary>
		/// 0-based coordinates for cursor, internally used
		/// </summary>
		private int CursorY0 { get { return this._cursory0; } set { this._cursory0 = value <= 0 ? 0 : value; } }

		// screen array [x,y]

		/// <summary>
		/// Changed screen buffer ?
		/// </summary>
		public bool ChangedScreen { get { return this._changedScreen; } }

		#region IDisposable Members
		/// <summary>
		/// Clean everything up
		/// </summary>
		public void Dispose()
		{
			this._vs = null; // break link to array
			this._screenString = null;
			this._screenStringLower = null;
		}
		#endregion

		/// <summary>
		/// Reset the cursor to upper left corner
		/// </summary>
		public void CursorReset()
		{
			this.CursorY0 = 0;
			this.CursorX0 = 0;
		}

		/// <summary>
		/// Move the cursor to the beginning of the next line
		/// </summary>
		public void CursorNextLine()
		{
			this.Write("\n\r");
		}

		/// <summary>
		/// Set the cursor (offset coordinates)
		/// </summary>
		/// <param name="x">X Position (lines) with offset considered</param>
		/// <param name="y">Y Position (columns) with offset considered</param>
		/// <remarks>
		/// Use the method MoveCursorTo(x,y) when upscrolling should
		/// be supported
		/// </remarks>
		public void CursorPosition(int x, int y)
		{
			this.CursorX = x;
			this.CursorY = y;
		}

		/// <summary>
		/// Clean the screen and reset cursor.
		/// </summary>
		/// <remarks>
		/// Changes the output-flag and scrolledUp attribute!
		/// </remarks>
		public void CleanScreen()
		{
			int lx = this._vs.GetLength(0);
			int ly = this._vs.GetLength(1);
			for (int y = 0; y < ly; y++)
			{
				for (int x = 0; x < lx; x++)
				{
					this._vs[x, y] = Space;
				}
			}
			this.CursorReset(); // cursor back to beginning
			this._changedScreen = true;
			this._visibleAreaY0Top = 0;
			this._visibleAreaY0Bottom = this.Height - 1;
		}

		// cleanScreen

		/// <summary>
		/// Cleans a screen area, all values are
		/// considering any offset
		/// </summary>
		/// <remarks>
		/// - Changes the output-flag!
		/// - Visible area is considered
		/// </remarks>
		/// <param name="xstart">upper left corner (included)</param>
		/// <param name="ystart">upper left corner (included)</param>
		/// <param name="xend">lower right corner (included)</param>
		/// <param name="yend">lower right corner (included)</param>
		public void CleanScreen(int xstart, int ystart, int xend, int yend)
		{
			if (this._vs == null || xend <= xstart || yend <= ystart || xstart < this._offsetx || xend < this._offsetx || ystart < this._offsety || yend < this._offsety)
				return; // nothing to do

			int x0Start = xstart - this._offsetx;
			int y0Start = ystart - this._offsety - this._visibleAreaY0Top;
			if (y0Start < 0) y0Start = 0; // only visible area
			int x0End = xend - this._offsetx;
			int y0End = yend - this._offsety - this._visibleAreaY0Top;
			if (y0End < 0) return; // nothing to do

			int lx = this._vs.GetLength(0);
			int ly = this._vs.GetLength(1);

			if (x0End >= lx) x0End = lx - 1;
			if (y0End >= ly) y0End = ly - 1;

			for (int y = y0Start; y <= y0End; y++)
			{
				for (int x = x0Start; x <= x0End; x++)
				{
					this._vs[x, y] = Space;
				}
			}
			this._changedScreen = true;
		}

		/// <summary>
		/// Clean the current line. Changes the output-flag! Visible area is considered.
		/// </summary>
		/// <param name="xStart">X with offset considered</param>
		/// <param name="xEnd">X with offset considered</param>
		public void CleanLine(int xStart, int xEnd)
		{
			int x0S = xStart - this._offsetx;
			int x0E = xEnd - this._offsetx;

			if (xStart < xEnd) return;
			if (x0S < 0) x0S = 0;
			if (x0E >= this.Width) x0E = this.Width - 1;

			int y = this._cursory0 - this._visibleAreaY0Top;
			if (this._vs == null || y < 0 || y > this._vs.GetLength(1)) return;

			for (int x = x0S; x <= x0E; x++)
			{
				this._vs[x, y] = Space;
			}
			this._changedScreen = true;
		}

		/// <summary>
		/// Clean screen including the cursor position.
		/// Changes the output-flag! The visible area is considered.
		/// </summary>
		public void CleanToCursor()
		{
			int y = this.CursorY - 1; // line before
			if (y >= this._offsety)
				this.CleanScreen(this.CursorXLeft, this._offsety, this.CursorXRight, y);
			this.CleanLine(this.CursorXLeft, this.CursorX);
			this._changedScreen = true;
		}

		/// <summary>
		/// Clean screen including the cursor position.
		/// Changes the output-flag! The Visible area is considered.
		/// </summary>
		public void CleanFromCursor()
		{
			int y = this.CursorY; // line before FIX: changed from this.CursorY + 1; T.Neumann 160211 (2)
			if (y <= this._visibleAreaY0Bottom + this._offsety)
				this.CleanScreen(this.CursorXLeft, y, this.CursorXRight, this._visibleAreaY0Bottom + this._offsety);
			this.CleanLine(this.CursorX, this.CursorXRight);
			this._changedScreen = true;
		}

		/// <summary>
		/// Scrolls up about n lines.Changes the output-flag!
		/// </summary>
		/// <param name="lines"></param>
		/// TODO: Do we have to change the coordinates offset?
		/// TODO: Is line 5 after 2 lines scrolling now line 3 or still 5?
		/// <returns>number of lines scrolled</returns>
		public int ScrollUp(int lines)
		{
			// scrolls up about n lines
			if (lines < 1) return 0;

			int lx = this._vs.GetLength(0);
			int ly = this._vs.GetLength(1);

			if (lines >= ly)
			{
				// we need to save the visible are info
				int vat = this._visibleAreaY0Top;
				int vab = this._visibleAreaY0Bottom;
				this.CleanScreen();
				this._visibleAreaY0Top = vat;
				this._visibleAreaY0Bottom = vab;
			}
			else
			{
				for (int y = lines; y < ly; y++)
				{
					int yTo = y - lines;
					for (int x = 0; x < lx; x++)
					{
						this._vs[x, yTo] = this._vs[x, y];
					}
				} // for copy over
				// delete the rest
				this.CleanScreen(this._offsetx, ly - lines, lx + this._offsetx, ly - 1 + this._offsety);
			}
			this._changedScreen = true;
			return lines;
		}

		/// <summary>
		/// Write a byte to the screen, and set new cursor position.
		/// Changes the output-flag!.
		/// </summary>
		/// <param name="writeByte">Output byte</param>
		/// <returns>True if byte has been written</returns>
		public bool WriteByte(byte writeByte)
		{
			return this.WriteByte(writeByte, true);
		}

		/// <summary>
		/// Write a byte to the screen, and set new cursor position. Changes the output-flag!
		/// </summary>
		/// <param name="writeBytes">Output bytes</param>
		/// <returns>True if byte has been written</returns>
		public bool WriteByte(byte[] writeBytes)
		{
			if (writeBytes == null || writeBytes.Length < 1) return false;
			// ReSharper disable LoopCanBeConvertedToQuery
			foreach (byte t in writeBytes)
			{
				if (!this.WriteByte(t, true)) return false;
			}
			return true;
			// ReSharper restore LoopCanBeConvertedToQuery
		}

		/// <summary>
		/// Write a byte to the screen.
		/// </summary>
		/// <remarks>
		/// Changes the output-flag!
		/// </remarks>
		/// <param name="writeByte">Output byte</param>
		/// <param name="moveCursor">Move the cursor or not</param>
		/// <returns>True if byte has been written</returns>
		public bool WriteByte(byte writeByte, bool moveCursor)
		{
			if (this._vs == null) return false;
			switch (writeByte)
			{
				case 10:
					// NL
					this.CursorY0++;
					break;
				case 13:
					// CR
					this.CursorX0 = 0;
					break;
				default:
					int y = this.CursorY0;
					if (this._visibleAreaY0Top > 0)
						y -= this._visibleAreaY0Top;
					if (y >= 0)
					{
						try
						{
							this._vs[this.CursorX0, y] = writeByte;
						}
							// ReSharper disable EmptyGeneralCatchClause
						catch
						{
							// boundary problems should never occur, however
						}
						// ReSharper restore EmptyGeneralCatchClause
					}
					if (moveCursor) this.MoveCursor(1);
					break;
			}
			this._changedScreen = true;
			return true;
		}

		/// <summary>
		/// Write a string to the screen, and set new cursor position. Changes the output-flag!
		/// </summary>
		/// <param name="s">Output string</param>
		/// <returns>True if byte has been written</returns>
		public bool WriteLine(String s)
		{
			return s != null && this.Write(s + "\n\r");
		}

		/// <summary>
		/// Write a string to the screen, and set new cursor position.
		/// </summary>
		/// <remarks>
		/// Changes the output-flag!
		/// </remarks>
		/// <param name="s">Output string</param>
		/// <returns>True if string has been written</returns>
		public bool Write(string s)
		{
			return s != null && this.WriteByte(Encoding.ASCII.GetBytes(s));
		}

		/// <summary>
		/// Write a char to the screen, and set new cursor position. Changes the output-flag!
		/// </summary>
		/// <param name="c">Output char</param>
		/// <returns>True if char has been written</returns>
		public bool Write(char c)
		{
			return this.Write(new string(c, 1));
		}

		/// <summary>
		/// Move cursor +/- positions forward. Scrolls up if necessary.
		/// </summary>
		/// <param name="positions">Positions to move (+ forward / - backwards)</param>
		public void MoveCursor(int positions)
		{
			if (positions == 0)
				return;
			int dy = positions/this.Width;
			int dx = positions - (dy*this.Width); // remaining x

			// change dx / dy if necessary
			if (dx >= 0)
			{
				// move forward
				if ((this.CursorX0 + dx) >= this.Width)
				{
					dy++;
					dx = dx - this.Width;
				}
			}
			else
			{
				// move backward (dx is NEGATIVE)
				if (this.CursorX0 + dx < 0)
				{
					dy --; // one line up
					dx = dx - this.Width;
				}
			}

			// new values:
			// do we have to scroll, line wraping for x is guaranteed
			int ny = this.CursorY0 + dy;
			int nx = this.CursorX0 + dx;
			if (ny > this._visibleAreaY0Bottom)
			{
				int sUp = ny - this._visibleAreaY0Bottom;
				this.ScrollUp(sUp);
				this._visibleAreaY0Bottom += sUp;
				this._visibleAreaY0Top = this._visibleAreaY0Bottom - this.Height - 1;
			}
			this.CursorY0 = ny;
			this.CursorX0 = nx; // since we use the PROPERTY exceeding values are cut
		}

		/// <summary>
		/// Move the cursor n rows down (+) or up(-). 
		/// </summary>
		/// <param name="lines">Number of rows up(-) or down(+)</param>
		public void MoveCursorVertical(int lines)
		{
			this.MoveCursor(lines*this.Width);
		}

		/// <summary>
		/// Move cursor to a position considering scrolling up / lines breaks.
		/// Changes the scrolledUp attribute!
		/// </summary>
		/// <param name="xPos">X Position considering offset</param>
		/// <param name="yPos">Y Position considering offset</param>
		/// <returns>true if cursor could be moved</returns>
		/// <remarks>
		/// Just to set a cursor position the attributes <see cref="CursorX"/> / <see cref="CursorY"/>
		/// could be used. This here features scrolling.
		/// </remarks>
		public bool MoveCursorTo(int xPos, int yPos)
		{
			int x0 = xPos - this._offsetx;
			int y0 = yPos - this._offsety;

			// check
			if (x0 < 0 || y0 < 0)
				return false;

			// determine extra lines because of 
			// X-Pos too high
			int dy = x0/this.Width;
			if (dy > 0)
			{
				y0 += dy;
				x0 = x0 - (dy*this.Width);
			}

			// do we have to scroll?
			if (y0 > this._visibleAreaY0Bottom)
			{
				int sUp = y0 - this._visibleAreaY0Bottom;
				this.ScrollUp(sUp);
				this._visibleAreaY0Bottom = y0 + sUp;
				this._visibleAreaY0Top = this._visibleAreaY0Bottom - this.Height - 1;
			}

			// set values
			this.CursorX0 = x0;
			this.CursorY0 = y0;
			return true;
		}

		/// <summary>
		/// Get a line as string.
		/// </summary>
		/// <param name="yPosition"></param>
		/// <returns></returns>
		public string GetLine(int yPosition)
		{
			int y0 = yPosition - this._offsety;
			if (this._vs == null || y0 >= this.Height || this.Width < 1) return null;
			var la = new byte[this.Width];
			for (int x = 0; x < this.Width; x++)
			{
				la[x] = this._vs[x, y0];
			}
			return Encoding.ASCII.GetString(la, 0, la.Length);
		}

		/// <summary>
		/// Class info
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this.GetType().FullName + " " + this.Width + " | " + this.Height + " | changed " + this._changedScreen;
		}

		/// <summary>
		/// Return the values as string
		/// </summary>
		/// <returns>Screen buffer as string including NLs (newlines)</returns>
		public string Hardcopy()
		{
			return this.Hardcopy(false);
		}

		/// <summary>
		/// Return the values as string
		/// </summary>
		/// <param name="lowercase">true return as lower case</param>
		/// <returns>Screen buffer as string including NLs (newlines)</returns>
		public string Hardcopy(bool lowercase)
		{
			if (this._vs == null) return null;
			if (this._changedScreen || this._screenString == null)
			{
				int cap = this.Width*this.Height;
				var sb = new StringBuilder(cap);
				for (int y = 0; y < this.Height; y++)
				{
					if (y > 0)
						sb.Append('\n');
					sb.Append(this.GetLine(y + this._offsety));
				} // for
				this._screenString = sb.ToString();
				this._changedScreen = false; // reset the flag
				if (!lowercase) return this._screenString;
				this._screenStringLower = this._screenString.ToLower();
				return this._screenStringLower;
			}
			// return from cache
			if (lowercase) return this._screenStringLower ?? (this._screenStringLower = this._screenString.ToLower());
			return this._screenString; // from cache
		}

		/// <summary>
		/// Find a string on the screen.
		/// </summary>
		/// <param name="findString">String to find</param>
		/// <param name="caseSensitive">true for case sensitive search</param>
		/// <returns>string found</returns>
		public string FindOnScreen(string findString, bool caseSensitive)
		{
			if (this._vs == null || findString == null || findString.Length < 1)
				return null;
			try
			{
				string screen = (caseSensitive) ? this.Hardcopy() : this.Hardcopy(true);
				int index = (caseSensitive) ? screen.IndexOf(findString) : screen.IndexOf(findString.ToLower());
				if (index < 0) return null;
				return caseSensitive ? findString : this.Hardcopy().Substring(index, findString.Length);
			}
			catch
			{
				// Null pointer etc.
				return null;
			}
		}

		// FindOnScreen

		/// <summary>
		/// Find a regular expression on the screen.
		/// </summary>
		/// <param name="regExp">Regular expression to find</param>
		/// <returns>string found</returns>
		public string FindRegExOnScreen(string regExp)
		{
			return this.FindRegExOnScreen(regExp, false);
		}

		/// <summary>
		/// Find a regular expression on the screen.
		/// </summary>
		/// <param name="regExp">Regular expression to find</param>
		/// <param name="caseSensitive">true for case sensitive search</param>
		/// <returns>string found</returns>
		public string FindRegExOnScreen(string regExp, bool caseSensitive)
		{
			if (this._vs == null || regExp == null || regExp.Length < 1)
				return null;
			Regex r = caseSensitive ? new Regex(regExp) : new Regex(regExp, RegexOptions.IgnoreCase);
			Match m = r.Match(this.Hardcopy()); // Remark: hardcopy uses a cache !
			return m.Success ? m.Value : null;
		}

		/// <summary>
		/// Find a regular expression on the screen
		/// </summary>
		/// <param name="regExp">Regular expression to find</param>
		/// <returns>Mathc object or null</returns>
		public Match FindRegExOnScreen(Regex regExp)
		{
			if (this._vs == null || regExp == null) return null;
			Match m = regExp.Match(this.Hardcopy()); // Remark: hardcopy uses a cache !
			return m.Success ? m : null;
		}
	} // class
}