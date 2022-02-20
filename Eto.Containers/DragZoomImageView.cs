
namespace Eto.Containers
{
	using System;
	using Eto.Forms;
	using Eto.Drawing;
	//
	// Summary:
	//     Panel with Zoom & Pan support
	public class DragZoomImageView : Drawable
	{
		readonly IMatrix _transform = Matrix.Create();

		public Keys DragModifier { get; set; } = Keys.None;
		public MouseButtons DragButton { get; set; } = MouseButtons.Primary;
		public Image? Image { get; set; }
		new public Control? Content
		{
			get => base.Content;
			set 
			{
				if (value is ImageView iv)
					Image = iv.Image;
				else if (value is null)
					Image = null;
				else
					throw new ArgumentException("Use Image Property ; not Content");
			}
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			if (Image != null)
			{
				e.Graphics.MultiplyTransform(_transform);
				e.Graphics.DrawImage(Image, e.ClipRectangle);
			}
		}

		static SizeF One = SizeF.Empty + 1;
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			var scale_siz = One + e.Delta.Height / 6;
			var scale = Matrix.FromScaleAt(scale_siz, e.Location);

			Matrix.Append(_transform, scale);

			this.Invalidate();

			e.Handled = true;
		}
		public void MoveGraphic(Point offset)
		{
			var move = Matrix.FromTranslation(offset);

			Matrix.Prepend(_transform, move);
		}
#region mouse pan
		PointF _mouse_down;
		protected override void OnMouseDown(MouseEventArgs e)
		{
			e.Handled = e.Buttons == DragButton && e.Modifiers == DragModifier;

			if (e.Handled)
			{
				_mouse_down = e.Location;

				Cursor = Cursors.Move;
			}
			else
			{
				base.OnMouseDown(e);
			}
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			e.Handled = _mouse_down != PointF.Empty;

			if (e.Handled)
			{
				var move = Matrix.FromTranslation(e.Location - _mouse_down);

				Matrix.Append(_transform, move);

				//base.Size = (Size) _transform.TransformSize(this.Size);

				_mouse_down = e.Location;

				this.Invalidate();
			}
			else
			{
				base.OnMouseMove(e);
			}
		}
		protected override void OnMouseUp(MouseEventArgs e)
		{
			e.Handled = _mouse_down != PointF.Empty;

			if (e.Handled)
			{
				_mouse_down = PointF.Empty;

				Cursor = Cursors.Default;
			}
			else
			{
				base.OnMouseUp(e);
			}
		}
#endregion
	}
}
