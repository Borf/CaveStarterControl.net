using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CaveStarterControl.net
{
	public partial class OptionWindow : Form
	{

		public string argument;

		public OptionWindow()
		{
			InitializeComponent();
		}

		private void OptionWindow_Load(object sender, EventArgs e)
		{

		}

		internal void init(dynamic options)
		{
			ImageList ilist = new ImageList();
			ilist.ImageSize = new Size(256, 256);
			ilist.ColorDepth = ColorDepth.Depth24Bit;
			this.listView1.LargeImageList = ilist;
			int index = 0;
			foreach (dynamic option in options)
			{
				Bitmap b = new Bitmap("data/CaveStarter.net/" + (string)option.icon);
				ilist.Images.Add(b);
				ListViewItem lvi = new ListViewItem((string)option.name);
				lvi.SubItems.Add((string)option.arguments);
				lvi.ImageIndex = index;
				listView1.Items.Add(lvi);
				index++;
			}
			listView1.Columns.Add("Last Run", 100, HorizontalAlignment.Left);
			listView1.Columns.Add("Command", 200, HorizontalAlignment.Left);
			listView1.Columns.Add("Arguments", 200, HorizontalAlignment.Left);
			listView1.Columns.Add("Directory", 150, HorizontalAlignment.Left);


		}

		private void listView1_ItemActivate(object sender, EventArgs e)
		{
			argument = listView1.SelectedItems[0].SubItems[1].Text;
			this.DialogResult = DialogResult.OK;
			this.Close();
		}
	}
}
