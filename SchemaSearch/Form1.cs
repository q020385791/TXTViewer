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

namespace SchemaSearch
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public string SourceFolderPath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
        public string SourceConcatTableScript = "";
        private void Form1_Load(object sender, EventArgs e)
        {
            SourceConcatTableScript = SourceFolderPath+"\\Schema";//指定資料夾名稱要為"Schema"
            GetTreeViewByText("");
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string[] FilesName = Directory.GetFiles(SourceConcatTableScript);
            string SelectedText= treeView1.SelectedNode.Text;
            string TargetPath = SourceConcatTableScript + "\\" + SelectedText+".txt";
            if (File.Exists(TargetPath))
            {
                Encoding unicode = Encoding.GetEncoding(950);
                try
                {
                    txtViewer.Text = File.ReadAllText(TargetPath, unicode);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("無法讀取:"+ TargetPath+"請確認檔案"+Environment.NewLine+ex.InnerException);
                }
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            txtSearch.Text = txtSearch.Text.ToUpper();
            txtSearch.SelectionStart = txtSearch.Text.Length;
            txtSearch.SelectionLength = 0;
            GetTreeViewByText(txtSearch.Text);
        }
        public void GetTreeViewByText(string SearchText) 
        {
            treeView1.Nodes.Clear();
            string[] FilesName = Directory.GetFiles(SourceConcatTableScript,"*",SearchOption.AllDirectories);
            Dictionary<string, List<string>> Dic = new Dictionary<string, List<string>>();
            foreach (string FullFilePath in FilesName)
            {
                string FileName = Path.GetFileName(FullFilePath).Split('.')[0];
                if (FileName.Contains(SearchText)|| SearchText=="")
                {
                    string SubTitleName = FileName.Substring(0, 3);
                    if (!Dic.Keys.Contains(SubTitleName))
                    {
                        //如果是全新目錄
                        List<string> ListChildren = new List<string>();
                        ListChildren.Add(FileName);
                        Dic.Add(SubTitleName, ListChildren);
                    }
                    else
                    {
                        // 如果已存在目錄
                        Dic[SubTitleName].Add(FileName);
                    }
                }
            }
            foreach (var items in Dic)
            {
                string Title = items.Key.Substring(0, 3).ToUpper(); ;//標題取三個字元 大寫
                TreeNode ParentNode = new TreeNode(Title);
                foreach (var item in items.Value)
                {
                    TreeNode ChildrenNode = new TreeNode(item);
                    ParentNode.Nodes.Add(ChildrenNode);
                }
                treeView1.Nodes.Add(ParentNode);
            }
        }

        private void treeView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string SelectedText = treeView1.SelectedNode.Text;
            if (SelectedText.Length<=3)
            {
                return;
            }
            string[] FilesName = Directory.GetFiles(SourceConcatTableScript,"*",SearchOption.AllDirectories);
            
            string TargetPath = FilesName.Where(x=>x.Contains(SelectedText)).FirstOrDefault() ;
            if (File.Exists(TargetPath))
            {
                Encoding unicode = Encoding.GetEncoding(950);
                try
                {
                    if (SelectedText != null&& SelectedText!="")
                    {
                   
                        TabPage tp = new TabPage(SelectedText);
                        tp.Name = SelectedText;

                        if (!tabControl.TabPages.ContainsKey(SelectedText))
                        {
                            tp.Name = SelectedText;
                            tabControl.TabPages.Add(tp);
                            TextBox tb = new TextBox();
                            tb.Multiline = true;
                            tb.Dock = DockStyle.Fill;
                            tb.BackColor = Color.Black;
                            tb.ForeColor = Color.White;
                            tp.Controls.Add(tb);
                            tb.Text = File.ReadAllText(TargetPath, unicode);
                            tabControl.SelectedTab = tp;
                        }
                        else 
                        {
                            tabControl.SelectedTab = tabControl.TabPages[SelectedText];
                        }


                    }
                   

                   
                }
                catch (Exception ex)
                {
                    MessageBox.Show("無法讀取:" + TargetPath + "請確認檔案" + Environment.NewLine + ex.InnerException);
                }
            }
        }

        private TabPage savedTabPage;
        private int savedTabPageIndex;
        private void TSMIClose_Click(object sender, EventArgs e)
        {
            if (tabControl.TabPages.Count == 0 || tabControl.SelectedTab == null) return;
            savedTabPage = tabControl.SelectedTab;
            savedTabPageIndex = tabControl.TabPages.IndexOf(savedTabPage);
            tabControl.TabPages.Remove(savedTabPage);
        }

        private void tabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            SolidBrush BackBrush = new SolidBrush(Color.Blue);
            Rectangle Rec = tabControl.GetTabRect(0);
            e.Graphics.FillRectangle(BackBrush, Rec);
        }
    }
}
