using System;
using System.IO;

using Eto.Drawing;

namespace Eto.Forms
{
	//
	// Summary:
	//     Scrollable container containing a Zoomable & Panable ImageView
	public class ImageViewScrollable : DragScrollable
	{
		private readonly ImageViewZoomable _zoom = new ImageViewZoomable();
		public ImageViewScrollable()
		{
			base.DragButton = MouseButtons.Primary;
			_zoom.PanButton = MouseButtons.Alternate;
			base.Content = _zoom;
		}
		new public ImageViewZoomable Content { get { return _zoom; } } // no set !

		public Image Image // shortcut
		{
			get { return _zoom.Image; }
			set { _zoom.Image = value; }
		}
	}
}
