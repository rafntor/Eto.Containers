
namespace Eto.Containers.Demo
{
	using Eto.Forms;
	using Eto.Drawing;

	public partial class MainForm : Form
	{
		Panel _image_view = new ();
		public MainForm()
		{
			this.InitializeComponent();

			var image = Bitmap.FromResource("Eto.Containers.Demo.Images.yggdrasil.jpg");

			// standard containers

			var b1 = new Button { Text = "ImageView", ToolTip = "(standard ImageView control)" };
			b1.Click += (o, e) => _image_view.Content = new ImageView { Image = image };

			var b2 = new Button { Text = "Scrollable", ToolTip = "(standard Scrollable control)" };
			b2.Click += (o, e) => _image_view.Content = new Scrollable { Content = image };

			// custom containers

			var b3 = new Button { Text = "DragScrollable", ToolTip = "(containing an Image)" };
			b3.Click += (o, e) => _image_view.Content = new DragScrollable { Content = image };

			var b4 = new Button { Text = "DragZoomImageView", ToolTip = "(containing an Image)" };
			b4.Click += (o, e) => _image_view.Content = new DragZoomImageView { Content = image };

			// layout

			var buttons = new StackLayout(b3, b4, null, b1, b2) { Orientation = Orientation.Horizontal, Padding = 2, Spacing = 4 };

			Content = new DynamicLayout(buttons, _image_view);
		}
	}
}
