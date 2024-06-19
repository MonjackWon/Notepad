using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Notepad
{
    public partial class Find : MaterialForm
    {
        public Find()
        {
            InitializeComponent();
        }
        MaterialMultiLineTextBox textBox;
        public Find(MaterialMultiLineTextBox _textBox)
        {
            InitializeComponent();
            textBox = _textBox;
        }

        int start = 0;

        private void materialButton1_Click(object sender, EventArgs e)
        {
            string text = materialTextBox1.Text;
            start = textBox.Text.IndexOf(text, start);
            if (start == -1)
            {
                MessageBox.Show("已经查找到文档结尾了");
                start = 0;
            }
            else
            {
                textBox.Select(start, text.Length);
                start += text.Length;
            }
        }
    }
}
