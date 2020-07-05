using System;
using Eto.Forms;
using Eto.Drawing;
using System.IO;

namespace EtoApp1
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();

			var view = new ImageViewScrollable();
			view.Image = get_bitmap();

			Content = view;
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);

			var scroll = Content as Scrollable;

			if (scroll != null)
				scroll.ScrollSize = Size * 2;
		}

		static Bitmap get_bitmap()
		{
			var sys_bitmap = Wpf.Properties.Resources.test_image;

			using (var stream = new MemoryStream())
			{
				sys_bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

				return new Bitmap(stream.ToArray());
			}
		}
	}
}
