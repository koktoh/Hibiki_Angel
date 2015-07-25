using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hibiki_Angel
{
	public partial class ErrorForm : Form
	{
		public ErrorForm(string s)
		{
			InitializeComponent();
			this.FormBorderStyle = FormBorderStyle.FixedSingle;
			pictureBox1.Image = Properties.Resources.ugya;
			label1.Font = System.Drawing.SystemFonts.MessageBoxFont;
			button1.Font = System.Drawing.SystemFonts.MessageBoxFont;

			label1.Text = s;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
