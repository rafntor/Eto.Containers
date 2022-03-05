
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
		Image? _image;
		IMatrix _base_transform = Matrix.Create();
		IMatrix _size_transform = Matrix.Create();
		IMatrix _zoom_transform = Matrix.Create();
		Cursor _defaultCursor = Cursors.Default;
		public Cursor DragCursor { get; set; } = Cursors.Move;
		public Keys DragModifier { get; set; } = Keys.None;
		public MouseButtons DragButton { get; set; } = MouseButtons.Primary;
		public Image? Image
		{
			get => _image;
			set
			{
				_image = value;
				size_transform();
				Invalidate();
			}
		}
		void size_transform()
		{
			if (Image != null && Width > 0 && Height > 0)
			{
				var scale_x = (float)Width / Image.Width;
				var scale_y = (float)Height / Image.Height;
				var scale = Math.Min(scale_x, scale_y);

				var xoff = Width - scale * Image.Width;
				var yoff = Height - scale * Image.Height;

				_base_transform = Matrix.FromTranslation(xoff / 2, yoff / 2);
				_base_transform.Scale(scale);

				_size_transform = Matrix.FromScale(scale, scale).Inverse();
			}
		}
		protected override void OnSizeChanged(EventArgs e)
		{
			size_transform();

			base.OnSizeChanged(e);
		}
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
				e.Graphics.MultiplyTransform(_base_transform);
				e.Graphics.MultiplyTransform(_zoom_transform);
				e.Graphics.DrawImage(Image, PointF.Empty);
			}
		}
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			var scale_siz = SizeF.Empty + 1 + e.Delta.Height / 6;
			var scale = Matrix.FromScaleAt(scale_siz, e.Location);

			Matrix.Append(_zoom_transform, scale);

			this.Invalidate();

			e.Handled = true;
		}
		#region mouse pan
		PointF _mouse_down;
		protected override void OnMouseDown(MouseEventArgs e)
		{
			e.Handled = e.Buttons == DragButton && e.Modifiers == DragModifier;

			if (e.Handled)
			{
				_mouse_down = e.Location;

				_defaultCursor = Cursor;
				Cursor = DragCursor;
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
				var dist = e.Location - _mouse_down;

				dist = _size_transform.TransformPoint(dist);

				var move = Matrix.FromTranslation(dist);

				Matrix.Append(_zoom_transform, move);

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

				Cursor = _defaultCursor;
			}
			else
			{
				base.OnMouseUp(e);
			}
		}
		#endregion
	}
}
