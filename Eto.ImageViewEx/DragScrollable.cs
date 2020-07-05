using System;
using System.IO;

using Eto.Drawing;

namespace Eto.Forms
{
	//
	// Summary:
	//     Scrollable container adding mouse-dragging support
	public class DragScrollable : Scrollable
	{
		public MouseButtons DragButton { get; set; }

		new public Control Content 
		{
			get { return base.Content; }
			set 
			{ 
				if (base.Content != null)
				{
					base.Content.MouseDown -= content_MouseDown;
					base.Content.MouseMove -= content_MouseMove;
					base.Content.MouseUp -= content_MouseUp;
				}
				base.Content = value;
				if (base.Content != null)
				{
					base.Content.MouseDown += content_MouseDown;
					base.Content.MouseMove += content_MouseMove;
					base.Content.MouseUp += content_MouseUp;
				}
			}
		}

		#region mouse_scroll
		PointF _scroll_init_pos;
		private void content_MouseDown(object sender, MouseEventArgs e)
		{
			e.Handled = e.Buttons == DragButton && e.Modifiers == Keys.None;

			if (e.Handled)
			{
				_scroll_init_pos = ScrollPosition - e.Location;

				Cursor = Cursors.Move;
			}
		}
		private void content_MouseMove(object sender, MouseEventArgs e)
		{
			e.Handled = _scroll_init_pos != PointF.Empty;

			if (e.Handled)
			{
				var factor = 0.9f; // scroll speed adjustment

				var delta = e.Location - _scroll_init_pos;
				var move = _scroll_init_pos + delta * factor;

				ScrollPosition = (Point) (_scroll_init_pos + move);
			}
		}
		private void content_MouseUp(object sender, MouseEventArgs e)
		{
			e.Handled = _scroll_init_pos != PointF.Empty;

			if (e.Handled)
			{
				_scroll_init_pos= PointF.Empty;

				Cursor = Cursors.Default;
			}
		}
		#endregion

	}
}
