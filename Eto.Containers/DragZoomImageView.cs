
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
		RectangleF _selectRectangle;
		IMatrix _base_transform = Matrix.Create();
		IMatrix _size_transform = Matrix.Create();
		IMatrix _zoom_transform = Matrix.Create();
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
				size_transform();
				Invalidate();
			}
		}
		public void ResetView()
		{
			_zoom_transform = Matrix.Create();
		}
		public void MoveView(PointF offset)
		{
			offset = _size_transform.TransformPoint(offset);

			var move = Matrix.FromTranslation(offset);

			Matrix.Append(_zoom_transform, move);

			this.Invalidate();
		}
		void size_transform()
		{
			if (Image != null && Width > 0 && Height > 0)
			{
				var scale = (SizeF) Size / Image.Size;
				scale.Width = scale.Height = Math.Min(scale.Width, scale.Height);

				var offset = (PointF) (Size - scale * Image.Size) / 2;

				_base_transform = Matrix.FromTranslation(offset);
				_base_transform.Scale(scale);

				_size_transform = Matrix.FromScale(scale).Inverse();
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
				e.Graphics.MultiplyTransform(_base_transform);
				e.Graphics.MultiplyTransform(_zoom_transform);
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
			var scale_siz = SizeF.Empty + 1 + e.Delta.Height / 6;

			var location = _base_transform.Inverse().TransformPoint(e.Location);

			var scale = Matrix.FromScaleAt(scale_siz, location);

			Matrix.Append(_zoom_transform, scale);

			this.Invalidate();

			base.OnMouseWheel(e);
		}
		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			MoveView((Point)Size / 2 - e.Location); // move clicked location to center

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

					var dist = e.Location - _mouse_down;

					dist = _size_transform.TransformPoint(dist);

					var move = Matrix.FromTranslation(dist);

					Matrix.Append(_zoom_transform, move);

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
					scale_siz.Width = scale_siz.Height = Math.Min(scale_siz.Width, scale_siz.Height);

					var location = _base_transform.Inverse().TransformPoint(_selectRectangle.Center);

					var scale = Matrix.FromScaleAt(scale_siz, location);

					Matrix.Append(_zoom_transform, scale);

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
