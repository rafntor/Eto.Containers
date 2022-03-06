
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
		SizeF _prevsize;
		RectangleF _selectRectangle;
		IMatrix _transform = Matrix.Create();
		Cursor _defaultCursor = Cursors.Default;
		public Cursor DragCursor { get; set; } = Cursors.Move;
		public Keys DragModifier { get; set; } = Keys.None;
		public MouseButtons DragButton { get; set; } = MouseButtons.Primary;
		public Cursor ZoomCursor { get; set; } = Cursors.Pointer;
		public Keys ZoomModifier { get; set; } = Keys.None;
		public MouseButtons ZoomButton { get; set; } = MouseButtons.Alternate;
		public Color ZoomColor { get; set; } = new Color(Colors.Yellow, 0.3f);

		public Image? Image
		{
			get => _image;
			set
			{
				_image = value;
				ResetView();
				Invalidate();
			}
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
		public void ResetView()
		{
			init_transform();
		}
		public void MoveView(PointF start, PointF moveto)
		{
			start = _transform.Inverse().TransformPoint(start);
			moveto = _transform.Inverse().TransformPoint(moveto);

			_transform.Translate(moveto - start);

			this.Invalidate();
		}
		private void init_transform()
		{
			if (Image != null && Width > 0 && Height > 0)
			{
				var scale_size = (SizeF) Size / Image.Size;
				var scale = Math.Min(scale_size.Width, scale_size.Height);

				var offset = (PointF) (Size - Image.Size * scale) / 2;

				_transform = Matrix.FromTranslation(offset);
				_transform.Scale(scale);
			}
		}
		protected override void OnSizeChanged(EventArgs e)
		{
			if (_prevsize.IsEmpty) // initial ?
			{
				init_transform();
			}
			else
			{
				var scale_size = Size / _prevsize;

				var scale = (scale_size.Width + scale_size.Height) / 2; // approximation

				var location = _transform.Inverse().TransformPoint(PointF.Empty);

				_transform.ScaleAt(scale, location);
			}

			_prevsize = Size;

			base.OnSizeChanged(e);
		}
		private bool ZoomMode
		{
			get => !_selectRectangle.IsZero;
			set 
			{
				if (value)
					_selectRectangle.TopLeft = _selectRectangle.BottomRight + 1;
				else
					_selectRectangle = RectangleF.Empty;
			}
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			if (Image != null)
			{
				e.Graphics.SaveTransform();
				e.Graphics.MultiplyTransform(_transform);
				e.Graphics.DrawImage(Image, PointF.Empty);

				if (ZoomMode)
				{
					e.Graphics.RestoreTransform();

					var zoom_pen = Pens.Black;
					zoom_pen.DashStyle = DashStyles.Dash;

					e.Graphics.FillRectangle(ZoomColor, _selectRectangle);
					e.Graphics.DrawRectangle(zoom_pen, _selectRectangle);
				}
			}
			base.OnPaint(e);
		}
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			var location = _transform.Inverse().TransformPoint(e.Location);

			_transform.ScaleAt(1 + e.Delta.Height / 6, location);

			this.Invalidate();

			base.OnMouseWheel(e);
		}
		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			MoveView(e.Location, (Point)Size / 2); // move clicked location to center

			base.OnMouseDoubleClick(e);
		}
#region mouse pan
		PointF _mouse_down;
		protected override void OnMouseDown(MouseEventArgs e)
		{
			ZoomMode = e.Buttons == ZoomButton && (ZoomModifier == Keys.None || e.Modifiers == ZoomModifier);

			bool dragMode = e.Buttons == DragButton && (DragModifier == Keys.None || e.Modifiers == DragModifier);

			if (dragMode || ZoomMode)
				_mouse_down = e.Location;

			_defaultCursor = Cursor;

			base.OnMouseDown(e);
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (_mouse_down != PointF.Empty)
			{
				if (ZoomMode)
				{
					Cursor = ZoomCursor;

					_selectRectangle.TopLeft = PointF.Min(e.Location, _mouse_down);
					_selectRectangle.BottomRight = PointF.Max(e.Location, _mouse_down);
				}
				else
				{
					Cursor = DragCursor;

					MoveView(_mouse_down, e.Location);

					_mouse_down = e.Location;
				}

				this.Invalidate();
			}

			base.OnMouseMove(e);
		}
		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (_mouse_down != PointF.Empty)
			{
				if (ZoomMode)
				{
					var scale_siz = Size / _selectRectangle.Size;
					var scale = Math.Min(scale_siz.Width, scale_siz.Height);

					var distance = _selectRectangle.Center - (SizeF) Size / 2;
					var factor = distance / 8; // patch unknown drift (rounding err somewhere?) .. grows with distance from control.center

					var location = _transform.Inverse().TransformPoint(_selectRectangle.Center + factor);

					_transform.ScaleAt(scale, location);

					this.Invalidate();

					ZoomMode = false;
				}

				_mouse_down = PointF.Empty;

				Cursor = _defaultCursor;
			}

			base.OnMouseUp(e);
		}
#endregion
	}
}
