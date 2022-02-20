
namespace Eto.Containers
{
	using Eto.Forms;
	using Eto.Drawing;
	//
	// Summary:
	//     Scrollable container containing a Zoomable & Panable ImageView
	public class ImageViewScrollable : DragScrollable
	{
		private readonly DragZoomImageView _zoom = new DragZoomImageView();
		public ImageViewScrollable()
		{
			base.DragButton = MouseButtons.Primary;
			_zoom.DragButton = MouseButtons.Alternate;
			base.Content = _zoom;
		}
		new public DragZoomImageView Content { get { return _zoom; } } // no set !

		public Image? Image // shortcut
		{
			get { return _zoom.Image; }
			set { _zoom.Image = value; }
		}
	}
}
