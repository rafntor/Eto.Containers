
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
		PointF _init_drag_location;
		Point _init_scroll_posistion; 
		private void content_MouseDown(object sender, MouseEventArgs e)
		{
			e.Handled = e.Buttons == DragButton && e.Modifiers == DragModifier;

			if (e.Handled)
			{
				_init_scroll_posistion = ScrollPosition;
				_init_drag_location = e.Location;

				Cursor = Cursors.Move;
			}
		}
		private void content_MouseMove(object sender, MouseEventArgs e)
		{
			e.Handled = _init_drag_location != PointF.Empty;

			if (e.Handled)
			{
				var delta = e.Location - _init_drag_location;

				ScrollPosition = _init_scroll_posistion + (Point) (delta * Size / ScrollSize);
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
