using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestApp
{
    public partial class Form1 : Form
    {
        public Form1(string title)
        {
            InitializeComponent();
            Text = title;
        }

        public bool OK { get; internal set; } = false;

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "1234")
            {
                OK = true;
            }
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1("llll");
            do
            {
                form1.ShowDialog();
            } while (!form1.OK);
        }
    }
}
