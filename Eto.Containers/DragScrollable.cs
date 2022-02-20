
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
		PointF _init_drag_location, _scroll_position;
		private void content_MouseDown(object sender, MouseEventArgs e)
		{
			e.Handled = e.Buttons == DragButton && e.Modifiers == DragModifier;

			if (e.Handled)
			{
				_init_drag_location = e.Location;
				_scroll_position = ScrollPosition;

				Cursor = Cursors.Move;
			}
		}
		private void content_MouseMove(object sender, MouseEventArgs e)
		{
			e.Handled = _init_drag_location != PointF.Empty;

			if (e.Handled)
			{
				var delta = e.Location - _init_drag_location;

				_init_drag_location = e.Location;

				// incremental tracking with extra PointF-member to not accumulate rounding errors
				_scroll_position += delta * Size / ScrollSize;

				// avoid spin up when outside range
				_scroll_position = PointF.Max(PointF.Empty, _scroll_position);
				_scroll_position = PointF.Min(_scroll_position, (Point) ScrollSize - Size);

				ScrollPosition = (Point) _scroll_position;
			}
		}
		private void content_MouseUp(object sender, MouseEventArgs e)
		{
			e.Handled = _init_drag_location != PointF.Empty;

			if (e.Handled)
			{
				_init_drag_location= PointF.Empty;

				Cursor = Cursors.Default;
			}
		}
		#endregion

	}
}
