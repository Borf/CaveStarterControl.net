using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CaveStarterControl.net
{
	public partial class MainWindow : Form
	{
		public const string historyFile = "data/CaveStarter.net/history.txt";
		public const string simpleFile = "data/CaveStarter.net/simple.txt";

		public MainWindow()
		{
			InitializeComponent();
		}

		private void MainWindow_Load(object sender, EventArgs e)
		{
			try
			{
				string[] lines = File.ReadAllLines(historyFile);
				for (int i = 0; i < lines.Length; i += 4)
				{
					listView1.Items.Add(new ListViewItem(new string[] { lines[i], lines[i+1], lines[i + 2], lines[i + 3] }));
				}
			}
			catch (FileNotFoundException)
			{
			}
			listView1.Columns.Add("Last Run", 100, HorizontalAlignment.Left);
			listView1.Columns.Add("Command", 200, HorizontalAlignment.Left);
			listView1.Columns.Add("Arguments", 200, HorizontalAlignment.Left);
			listView1.Columns.Add("Directory", 150, HorizontalAlignment.Left);



			try
			{
				string[] lines = File.ReadAllLines(simpleFile);

				ImageList ilist = new ImageList();
				ilist.ImageSize = new Size(256, 256);
				ilist.ColorDepth = ColorDepth.Depth24Bit;
				this.listView2.LargeImageList = ilist;
				for (int i = 0; i < lines.Length; i+=6 )
				{
					Bitmap b = new Bitmap("data/CaveStarter.net/" + lines[i+1]);
					ilist.Images.Add(b);
					ListViewItem lvi = new ListViewItem(lines[i]);
					lvi.ImageIndex = i/6;
					lvi.SubItems.Add(lines[i + 2]);
					lvi.SubItems.Add(lines[i + 3]);
					lvi.SubItems.Add(lines[i + 4]);
					listView2.Items.Add(lvi);
				}
				listView2.Columns.Add("Last Run", 100, HorizontalAlignment.Left);
				listView2.Columns.Add("Command", 200, HorizontalAlignment.Left);
				listView2.Columns.Add("Arguments", 200, HorizontalAlignment.Left);
				listView2.Columns.Add("Directory", 150, HorizontalAlignment.Left);
			}
			catch (Exception)
			{
			}


			tabControl1.SelectedTab = tabPage2;


		}


		private void button1_Click(object sender, EventArgs e)
		{
			string command = txtCommand.Text;
			string directory = txtWorkingDirectory.Text;
			string arguments = txtArguments.Text;

			bool found = false;
			foreach (ListViewItem item in listView1.Items)
			{
				if (item.SubItems[1].Text == command &&
				    item.SubItems[2].Text == arguments &&
				    item.SubItems[3].Text == directory)
				{
					found = true;
					item.SubItems[0].Text = DateTime.Now.ToString();
				}
			}
			if(!found)
				listView1.Items.Add(new ListViewItem(new string[] { DateTime.Now.ToString(), command, arguments, directory }));
			saveListView();

			string cmd = "simplestart ";
			cmd += "\"" + directory + "\" ";
			cmd += "\"" + command + "\" ";
			cmd += "\"" + arguments + "\"\r\n";

			sendCommand(cmd);
		}

		private void sendCommand(string cmd)
		{
			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
			socket.Connect("localhost", 9999);

			string s = myRecv(socket);
			if (s != "hello")
			{
				MessageBox.Show(this, "Error: the service running is not a cavestarter server");
				socket.Close();
				return;
			}

			mySend(cmd, socket);
			socket.Close();
		}

		string myRecv(Socket socket)
		{
			string ret = "";
			while (ret.IndexOf("\r\n") == -1)
			{
				byte[] buffer = new byte[1024];
				int rc = socket.Receive(buffer, 0, 1024, SocketFlags.None);
				ret += System.Text.Encoding.ASCII.GetString(buffer, 0, rc);
			}
			ret = ret.Trim();
			return ret;
		}
		void mySend(string command, Socket socket)
		{
			byte[] data = System.Text.Encoding.ASCII.GetBytes(command);
			int sent = 0;
			while (sent < command.Length)
			{
				int rc = socket.Send(data, sent, command.Length - sent, SocketFlags.None);
				sent += rc;
			}
		}


		void saveListView()
		{
			string data = "";
			foreach (ListViewItem item in listView1.Items)
			{
				foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
					data += subItem.Text + "\r\n";
			}
			File.WriteAllText(historyFile,data);
			
		}

		private void listView1_ItemActivate(object sender, EventArgs e)
		{
			txtCommand.Text = listView1.SelectedItems[0].SubItems[1].Text;
			txtArguments.Text = listView1.SelectedItems[0].SubItems[2].Text;
			txtWorkingDirectory.Text = listView1.SelectedItems[0].SubItems[3].Text;
			button1_Click(sender, e);
		}

		private void button2_Click(object sender, EventArgs e)
		{
			sendCommand("stopvr\r\n");
		}

		private void listView2_ItemActivate(object sender, EventArgs e)
		{
			txtCommand.Text = listView2.SelectedItems[0].SubItems[1].Text;
			txtArguments.Text = listView2.SelectedItems[0].SubItems[2].Text;
			txtWorkingDirectory.Text = listView2.SelectedItems[0].SubItems[3].Text;
			button1_Click(sender, e);
		}
	}
}
