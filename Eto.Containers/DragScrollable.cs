
namespace Eto.Containers
{
	using Eto.Forms;
	using Eto.Drawing;
	//
	// Summary:
	//     Scrollable container adding mouse-dragging scroll-support
	public class DragScrollable : Scrollable
	{
		public Keys DragModifier { get; set; } = Keys.None;
		public MouseButtons DragButton { get; set; } = MouseButtons.Primary;
		public enum Mode { Content, Scrollbar }
		Mode DragMode { get; set; } = Mode.Content;
		new public Control Content 
		{
			get => base.Content;
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
		static readonly PointF _no_position = PointF.Empty - 1;
		PointF _scroll_position, _mouse_pos;
		private void content_MouseDown(object sender, MouseEventArgs e)
		{
			e.Handled = e.Buttons == DragButton && e.Modifiers == DragModifier;

			if (e.Handled)
			{
				_mouse_pos = Mouse.Position; // can't trust MouseEventArgs.Location, setting scrollposition fires bogus mousemoves(?)
				_scroll_position = ScrollPosition; // incremental tracking with extra PointF-member to not accumulate rounding errors

				Cursor = Cursors.Move;
			}
		}
		private void content_MouseMove(object sender, MouseEventArgs e)
		{
			e.Handled = _mouse_pos != _no_position;

			if (e.Handled)
			{
				var delta = Mouse.Position - _mouse_pos;

				_mouse_pos = Mouse.Position;

				if (DragMode == Mode.Scrollbar)
					_scroll_position += delta * ScrollSize / Size;
				else // normal mode ; drag content directly
					_scroll_position -= delta;

				// anti-windup when outside range
				_scroll_position = PointF.Max(PointF.Empty, _scroll_position);
				_scroll_position = PointF.Min(_scroll_position, (Point) ScrollSize - Size);

				ScrollPosition = (Point) _scroll_position;
			}
		}
		private void content_MouseUp(object sender, MouseEventArgs e)
		{
			e.Handled = _mouse_pos != _no_position;

			if (e.Handled)
			{
				_mouse_pos = _no_position;

				Cursor = Cursors.Default;
			}
		}
		#endregion

	}
}
