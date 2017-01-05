using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageQuantization.Pallete
{
    public partial class ShowColor : Form
    {
        public ShowColor(Color col)
        {
            
            InitializeComponent();
            pictureBox1.BackColor = col;
            Red.Text = col.R.ToString();
            Green.Text = col.G.ToString();
            Blue.Text = col.B.ToString();
            HtmlHex.Text = System.Drawing.ColorTranslator.ToHtml(col);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Dispose();
            this.Close();
            
        }
    }
}
