namespace CGui.Gui.Primitives
{
	using System;

	/// <summary>
	/// Defines the <see cref="Scrollable" /> abstract class.
	/// </summary>
	public abstract class Scrollable : GuiElement, IScrollable
	{
		private string _scrollBarChar = "█";

		private string _scrollBarCharInactive = "▒";

		private string _scrollBarCharUp = "▲";

		private string _scrollBarCharDown = "▼";

		private int _scrollStep = 1;

		private int _scrollPageStep = 10;

		private ConsoleColor _scrollBarColor = ConsoleWrapper.Instance.ForegroundColor;
		public ConsoleColor ScrollBarColor
		{
			get => _scrollBarColor; set => _scrollBarColor = value;
		}

		/// <summary>
		/// Gets or sets the current position of scrollbar
		/// </summary>
		public int Offset { get; set; }

		/// <summary>
		/// Gets or sets number of lines to scroll when using pgup/pgdn
		/// </summary>
		public int ScrollPageStep { get => _scrollPageStep; set => _scrollPageStep = value; }

		/// <summary>
		/// Gets the total items to scroll
		/// </summary>
		public abstract int TotalItems { get; }

		/// <summary>
		/// Gets or sets a value indicating whether to show scrollbar
		/// </summary>
		public bool ShowScrollBar { get; set; }
		/// <summary>
		/// Gets or sets character to show as current scrollbar position
		/// </summary>
		public string ScrollBarChar { get => _scrollBarChar; set => _scrollBarChar = value; }
		/// <summary>
		/// Gets or sets character to show as inactive scrollbar position
		/// </summary>
		public string ScrollBarCharInactive { get => _scrollBarCharInactive; set => _scrollBarCharInactive = value; }
		/// <summary>
		/// Gets or sets character to show as scroll up character
		/// </summary>
		public string ScrollBarCharUp { get => _scrollBarCharUp; set => _scrollBarCharUp = value; }
		/// <summary>
		/// Gets or sets character to show as scroll down character
		/// </summary>
		public string ScrollBarCharDown { get => _scrollBarCharDown; set => _scrollBarCharDown = value; }
		/// <summary>
		/// Number of lines to scroll
		/// </summary>
		public int ScrollStep { get => _scrollStep; set => _scrollStep = value; }

		/// <summary>
		/// Returns formatted display text for item including scrollbar
		/// </summary>
		/// <param name="Index">The zero-based <see cref="int"/>index of item</param>
		/// <param name="DisplayText">The <see cref="string"/> unformatted display text</param>
		/// <returns>The <see cref="string"/></returns>
		protected string GetDisplayText(int Index, string DisplayText)
		{
			switch (this.TextAlignment)
			{
				case TextAlignment.Left:
					DisplayText = DisplayText.PadRight(this.Width - 1 - (BorderWidth * 2), this.PadChar);
					break;

				case TextAlignment.Right:
					DisplayText = DisplayText.PadLeft(this.Width - 1 - (BorderWidth * 2), this.PadChar);
					break;

				case TextAlignment.Center:
					DisplayText = DisplayText.PadBoth(this.Width - 1 - (BorderWidth * 2), this.PadChar);
					break;
			}

			if (DisplayText.VisibleLength() > this.Width - 1)
			{
				if (DisplayText.VisibleLength() > 4)
				{
					DisplayText = DisplayText.Substring(0, this.Width - 4 - (BorderWidth * 2)) + "...";
				}
				else
				{
					DisplayText = DisplayText.Substring(0, this.Width - (BorderWidth * 2));
				}
			}

			var visibleHeight = Height - (BorderWidth * 2);
			if (ShowScrollBar && visibleHeight > 3 && visibleHeight < TotalItems)
			{
				var ratio = ((double)visibleHeight-1.5) / TotalItems; //This fine-tuned constant ensures that the bottom scroll position is correct. Do Not Touch.
				int size = Math.Max((int)Math.Ceiling(((double)visibleHeight) * ratio) - 2, 1);
				var top = Math.Ceiling(Offset * ratio);

				bool first = (Index - Offset == 0);
				bool last = Index - Offset == visibleHeight - 1;

				bool show = Index - Offset > top
					&& Index - Offset <= size + top;

				if (first)
				{
					DisplayText = DisplayText + ScrollBarColor.GetColorCode(true) + _scrollBarCharUp + ConsoleWrapper.ColorReset;
				}
				else if (last)
				{
					DisplayText = DisplayText + ScrollBarColor.GetColorCode(true) + _scrollBarCharDown + ConsoleWrapper.ColorReset;
				}
				else if (show)
				{
					DisplayText = DisplayText + ScrollBarColor.GetColorCode(true) + _scrollBarChar + ConsoleWrapper.ColorReset;
				}
				else
				{
					DisplayText = DisplayText + ScrollBarColor.GetColorCode(true) + _scrollBarCharInactive + ConsoleWrapper.ColorReset;
				}
			}

			return DisplayText;
		}

		public virtual void ScrollUp()
		{
			ScrollUp(_scrollStep);
		}
		public virtual void ScrollDown()
		{
			ScrollDown(_scrollStep);
		}
		public abstract void ScrollUp(int Step);
		public abstract void ScrollDown(int Step);
	}
}
