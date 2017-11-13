using Newtonsoft.Json;
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
	public partial class OAuthWindow : Form
	{
		public dynamic result;
		public OAuthWindow()
		{
			InitializeComponent();
		}

		private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			string url = webBrowser1.Url.AbsolutePath;
			Console.WriteLine(url);
			if(url.IndexOf("finishlogin") != -1)
			{
				string text = webBrowser1.Document.Body.InnerText;
				Console.WriteLine(text);
				result = JsonConvert.DeserializeObject(text);
				DialogResult = DialogResult.OK;
				Close();
			}
		}
	}
}
