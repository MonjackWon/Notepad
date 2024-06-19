using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Notepad
{
    public partial class Form1 : MaterialForm
    {
        private readonly MaterialSkinManager materialSkinManager;
        public Form1()
        {
            InitializeComponent();
            // Initialize MaterialSkinManager
            materialSkinManager = MaterialSkinManager.Instance;

            // Set this to false to disable backcolor enforcing on non-materialSkin components
            // This HAS to be set before the AddFormToManage()
            materialSkinManager.EnforceBackcolorOnAllComponents = true;

            // MaterialSkinManager properties
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Indigo500, Primary.Indigo700, Primary.Indigo100, Accent.Pink200, TextShade.WHITE);
        }

        MaterialMultiLineTextBox materialMultiLineTextBox1 = new MaterialMultiLineTextBox();

        private void Form1_Load(object sender, EventArgs e)
        {
            string iniPath = Path.Combine(Directory.GetCurrentDirectory(), "record.ini");
            if (!File.Exists(iniPath))
            {
                File.WriteAllText(iniPath, "This is an initialization text, usually generated when the program is run for the first time");
                //新建文本框
                toolStripMenuItem2_Click(new object(), new EventArgs());
                materialMultiLineTextBox1.TextChanged += MaterialMultiLineTextBox1_TextChanged;
            }
            else
            {
                string str = readIni(iniPath);
                if (string.IsNullOrWhiteSpace(str))
                {
                    //新建文本框
                    toolStripMenuItem2_Click(new object(), new EventArgs());
                }
                else
                {
                    using (StringReader reader = new StringReader(str))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            // 去除行首和行尾的空白字符
                            line = line.Trim();

                            // 分割行内容
                            string[] parts = line.Split('\n');

                            if (parts.Length > 0)
                            {
                                string[] eachLine = parts[0].Trim().Split('\t');
                                // 添加到hashtable
                                TabFilePaths.Add(eachLine[1]);
                            }
                        }
                    }
                    foreach (string path in TabFilePaths)
                    {
                        if (path.Contains("tempFiles/"))            //不严谨，暂搁置
                        {
                            paths.Add("");
                            tabControl1.TabPages.Add("新建文档");
                        }
                        else
                        {
                            paths.Add(path);
                            int lastSlashIndex = path.LastIndexOf('\\');
                            int lastDotIndex = path.LastIndexOf(".");
                            if (lastSlashIndex > 0)
                            {
                                if (path.Contains(".txt"))
                                {
                                    tabControl1.TabPages.Add((path.Substring(lastSlashIndex + 1)).Remove(lastDotIndex - lastSlashIndex - 1));
                                }
                                else
                                {
                                    tabControl1.TabPages.Add(path.Substring(lastSlashIndex + 1));
                                }

                            }
                            else
                            {
                                tabControl1.TabPages.Add(path);
                            }
                        }
                        MaterialMultiLineTextBox materialMultiLineTextBox2 = new MaterialMultiLineTextBox();
                        tabControl1.TabPages[tabControl1.TabPages.Count - 1].Controls.Add(materialMultiLineTextBox2);
                        StreamReader sr = new StreamReader(path);
                        materialMultiLineTextBox2.Text = sr.ReadToEnd();
                        materialMultiLineTextBox2.Dock = DockStyle.Fill;
                        sr.Close();
                        tabControl1.SelectedIndex = tabControl1.TabPages.Count - 1;

                        //将文本框值传入全局
                        box2ToBox1();
                        materialMultiLineTextBox1.TextChanged += MaterialMultiLineTextBox1_TextChanged;
                    }
                }
            }
        }
        private void box2ToBox1()
        {
            foreach (System.Windows.Forms.Control control in tabControl1.SelectedTab.Controls)
            {
                if (control is MaterialMultiLineTextBox)
                {
                    materialMultiLineTextBox1 = (MaterialMultiLineTextBox)control;
                }
            }
        }

        private void MaterialMultiLineTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (paths[tabControl1.SelectedIndex] == "")
            {
                TempSave(TabFilePaths[tabControl1.SelectedIndex]);
            }
        }

        List<string> paths = new List<string>();
        List<string> TabFilePaths = new List<string>();
        bool isSaved = false;

        private void MySave()
        {
            if (paths[tabControl1.SelectedIndex] == "")
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    paths[tabControl1.SelectedIndex] = saveFileDialog1.FileName;
                    tabControl1.TabPages[tabControl1.SelectedIndex].Text = Path.GetFileNameWithoutExtension(paths[tabControl1.SelectedIndex]);
                    StreamWriter sw = new StreamWriter(paths[tabControl1.SelectedIndex], false, Encoding.UTF8);
                    sw.Write(materialMultiLineTextBox1.Text);
                    sw.Close();
                    isSaved = true;
                }
                else
                {
                    isSaved = false;
                }

            }
            else
            {
                StreamWriter sw = new StreamWriter(paths[tabControl1.SelectedIndex], false, Encoding.UTF8);
                sw.Write(materialMultiLineTextBox1.Text);
                sw.Close();
            }

        }

        private void 保存SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MySave();
        }

        public void MyOpen()
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (TabFilePaths.Contains(openFileDialog1.FileName))
                {
                    tabControl1.SelectedIndex = TabFilePaths.IndexOf(openFileDialog1.FileName);
                }
                else
                {
                    paths.Add(openFileDialog1.FileName);
                    TabFilePaths.Add(openFileDialog1.FileName);
                    tabControl1.TabPages.Add(Path.GetExtension(paths[paths.Count - 1]) == ".txt" ?
                        Path.GetFileNameWithoutExtension(paths[paths.Count - 1]) : Path.GetFileName(paths[paths.Count - 1]));
                    MaterialMultiLineTextBox materialMultiLineTextBox2 = new MaterialMultiLineTextBox();
                    tabControl1.TabPages[tabControl1.TabPages.Count - 1].Controls.Add(materialMultiLineTextBox2);
                    materialMultiLineTextBox2.Dock = DockStyle.Fill;
                    StreamReader sr = new StreamReader(paths[paths.Count - 1], Encoding.UTF8);
                    materialMultiLineTextBox2.Text = sr.ReadToEnd();
                    sr.Close();
                    tabControl1.SelectedIndex = tabControl1.TabPages.Count - 1;

                    box2ToBox1();
                    materialMultiLineTextBox1.TextChanged += MaterialMultiLineTextBox1_TextChanged;
                }

            }
        }

        private void 打开OToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MyOpen();
        }

        private void 另存为AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path_temp = paths[tabControl1.SelectedIndex];
            paths[tabControl1.SelectedIndex] = "";
            MySave();
            if (!isSaved)
            {
                paths[tabControl1.SelectedIndex] = path_temp;
            }
            isSaved = false;
        }

        private void 退出EToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            this.Text = "笔记本";
            tabControl1.TabPages.Add("新建文档");
            MaterialMultiLineTextBox materialMultiLineTextBox2 = new MaterialMultiLineTextBox();
            tabControl1.TabPages[tabControl1.TabPages.Count - 1].Controls.Add(materialMultiLineTextBox2);
            materialMultiLineTextBox2.Dock = DockStyle.Fill;
            paths.Add("");
            tabControl1.SelectedIndex = tabControl1.TabPages.Count - 1;
            string GeneratedFilePath = GenerateFilePath();
            TabFilePaths.Add(GeneratedFilePath);
            TempSave(GeneratedFilePath);
            box2ToBox1();
            materialMultiLineTextBox1.TextChanged += MaterialMultiLineTextBox1_TextChanged;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            box2ToBox1();
        }

        private void 关闭选项卡ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl1.TabPages.Count == 1)
            {
                toolStripMenuItem2_Click(new object(), new EventArgs());
                if (paths[tabControl1.SelectedIndex] == "")
                {
                    DialogResult dr = MessageBox.Show("当前文档还未保存，是否保存？", "保存提示", MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

                    if (dr == DialogResult.Yes)
                    {
                        MySave();
                        tabControl1.TabPages.RemoveAt(0);
                        tabControl1.SelectedIndex = tabControl1.TabCount - 1;
                        TabFilePaths.RemoveAt(0);
                        paths.RemoveAt(0);
                    }
                    else if (dr == DialogResult.No)
                    {
                        tabControl1.TabPages.RemoveAt(0);
                        tabControl1.SelectedIndex = tabControl1.TabCount - 1;
                        TabFilePaths.RemoveAt(0);
                        paths.RemoveAt(0);
                    }
                }
                else
                {
                    tabControl1.TabPages.RemoveAt(0);
                    tabControl1.SelectedIndex = tabControl1.TabCount - 1;
                    TabFilePaths.RemoveAt(0);
                    paths.RemoveAt(0);
                }
            }
            else
            {
                if (paths[tabControl1.SelectedIndex] == "")
                {
                    DialogResult dr = MessageBox.Show("当前文档还未保存，是否保存？", "保存提示", MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

                    if (dr == DialogResult.Yes)
                    {
                        MySave();
                        tabControl1.TabPages.RemoveAt(tabControl1.SelectedIndex);
                        tabControl1.SelectedIndex = tabControl1.TabCount - 1;
                        TabFilePaths.RemoveAt(tabControl1.SelectedIndex);
                        paths.RemoveAt(tabControl1.SelectedIndex);
                    }
                    else if (dr == DialogResult.No)
                    {
                        tabControl1.TabPages.RemoveAt(tabControl1.SelectedIndex);
                        tabControl1.SelectedIndex = tabControl1.TabCount - 1;
                        TabFilePaths.RemoveAt(tabControl1.SelectedIndex);
                        paths.RemoveAt(tabControl1.SelectedIndex);
                    }
                }
                else
                {
                    tabControl1.TabPages.RemoveAt(tabControl1.SelectedIndex);
                    tabControl1.SelectedIndex = tabControl1.TabCount - 1;
                    TabFilePaths.RemoveAt(tabControl1.SelectedIndex);
                    paths.RemoveAt(tabControl1.SelectedIndex);
                }
            }

        }

        private string GenerateFilePath()
        {
            DateTime CurrentTime = DateTime.Now;
            string currentTime = CurrentTime.ToString("yyyyMMdd-HHmmss");

            int randomNumber = new Random().Next(1000, 10000);
            byte[] bytes = BitConverter.GetBytes(randomNumber);
            string base64String = Convert.ToBase64String(bytes);
            if (!Directory.Exists("tempFiles"))
            {
                Directory.CreateDirectory("tempFiles");
            }

            return "tempFiles/" + base64String + currentTime;
        }

        private void TempSave(string filePath)
        {
            StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8);
            sw.Write(materialMultiLineTextBox1.Text);
            sw.Close();
        }


        private void writeIni()
        {
            StreamWriter sw = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), "record.ini"), false, Encoding.UTF8);
            for (int i = 0; i < TabFilePaths.Count; i++)
            {
                sw.WriteLine(i.ToString() + "\t" + TabFilePaths[i]);
            }
            sw.Close();
        }

        private string readIni(string filePath)
        {
            StreamReader sr = new StreamReader(filePath);
            string str = sr.ReadToEnd();
            sr.Close();
            return str;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            writeIni();
        }

        private void 全选AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            materialMultiLineTextBox1.SelectAll();
        }

        private void 复制CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            materialMultiLineTextBox1.Copy();
        }
        private void 剪切XToolStripMenuItem_Click(object sender, EventArgs e)
        {
            materialMultiLineTextBox1.Cut();
        }

        private void 粘贴VToolStripMenuItem_Click(object sender, EventArgs e)
        {
            materialMultiLineTextBox1.Paste();
        }

        private void 撤销ZToolStripMenuItem_Click(object sender, EventArgs e)
        {
            materialMultiLineTextBox1.Undo();
        }

        private void 查找FCtrlFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form find = new Find(materialMultiLineTextBox1);
            find.Show();


        }

        private void 放大ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            float newFontSize = materialMultiLineTextBox1.Font.Size + 1;
            materialMultiLineTextBox1.Font = new Font(materialMultiLineTextBox1.Font.FontFamily, newFontSize);
        }

        private void 缩小ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            float newFontSize = materialMultiLineTextBox1.Font.Size - 1;
            materialMultiLineTextBox1.Font = new Font(materialMultiLineTextBox1.Font.FontFamily, newFontSize);
        }

        private void 自动换行ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            自动换行ToolStripMenuItem.Text = 自动换行ToolStripMenuItem.Text == "自动换行(&L)" ? "取消自动换行(&L)" : "自动换行(&L)";
            materialMultiLineTextBox1.WordWrap = 自动换行ToolStripMenuItem.Text != "自动换行(&L)";

        }

        private void 关闭此标签页外所有标签页ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl1.TabPages.Count != 1)
            {
                TabPage currentTab = tabControl1.SelectedTab;
                for (int i = tabControl1.TabPages.Count - 1; i >= 0; i--)
                {
                    if (tabControl1.TabPages[i] != currentTab)
                        {
                            if (paths[i] == "")
                            {
                                DialogResult dr = MessageBox.Show("当前文档还未保存，是否保存？", "保存提示", MessageBoxButtons.YesNoCancel,
                                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

                                if (dr == DialogResult.Yes)
                                {
                                    MySave();
                                    tabControl1.TabPages.RemoveAt(i);
                                    tabControl1.SelectedIndex = tabControl1.TabCount - 1;
                                    TabFilePaths.RemoveAt(i);
                                    paths.RemoveAt(i);
                                }
                                else if (dr == DialogResult.No)
                                {
                                    tabControl1.TabPages.RemoveAt(i);
                                    tabControl1.SelectedIndex = tabControl1.TabCount - 1;
                                    TabFilePaths.RemoveAt(i);
                                    paths.RemoveAt(i);
                                }
                            }
                            else
                            {
                                tabControl1.TabPages.RemoveAt(i);
                                tabControl1.SelectedIndex = tabControl1.TabCount - 1;
                                TabFilePaths.RemoveAt(i);
                                paths.RemoveAt(i);
                            }
                        }
                }
            }
        }

        private void 关闭右侧标签页ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl1.TabPages.Count != 1)
            {
                int temp = tabControl1.SelectedIndex;
                TabPage currentTab = tabControl1.SelectedTab;
                for (int i = tabControl1.TabPages.Count - 1; i > temp; i--)
                {
                    if (tabControl1.TabPages[i] != currentTab)
                    {
                        if (paths[i] == "")
                        {
                            DialogResult dr = MessageBox.Show("当前文档还未保存，是否保存？", "保存提示", MessageBoxButtons.YesNoCancel,
                                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

                            if (dr == DialogResult.Yes)
                            {
                                MySave();
                                tabControl1.TabPages.RemoveAt(i);
                                tabControl1.SelectedIndex = tabControl1.TabCount - 1;
                                TabFilePaths.RemoveAt(i);
                                paths.RemoveAt(i);
                            }
                            else if (dr == DialogResult.No)
                            {
                                tabControl1.TabPages.RemoveAt(i);
                                tabControl1.SelectedIndex = tabControl1.TabCount - 1;
                                TabFilePaths.RemoveAt(i);
                                paths.RemoveAt(i);
                            }
                        }
                        else
                        {
                            tabControl1.TabPages.RemoveAt(i);
                            tabControl1.SelectedIndex = tabControl1.TabCount - 1;
                            TabFilePaths.RemoveAt(i);
                            paths.RemoveAt(i);
                        }
                    }
                }
            }
        }
    }

}
