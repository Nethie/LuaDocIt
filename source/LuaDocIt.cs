using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using StylishForms;

namespace LuaDocIt
{
    class LuaDocIt
    {
        static void GenerateDocumentation(LuaFile[] files, string path)
        {
            string json = "[";
            for (int n = 0; n < files.Length; n++)
            {
                for (int f = 0; f < files[n].Functions.Length; f++)
                {
                    json = json + files[n].Functions[f].GenerateJson() + ",";
                }
                for (int f = 0; f < files[n].Hooks.Length; f++)
                {
                    json = json + files[n].Hooks[f].GenerateJson() + ",";
                }
            }
            json = json.TrimEnd(',');
            json = json + "]";
            Directory.CreateDirectory($@"output/{path}");
            File.WriteAllText($@"output/{path}/documentation.json", json);

            //copy web files
            string[] webfiles = Directory.GetFiles(@"webfiles/");
            foreach( string s in webfiles )
            {
                File.Copy(s, Path.Combine($@"output/{path}/", Path.GetFileName(s)), true);
            }

            MessageBox.Show($"Your documentation has been generated succesfully in 'output/{path}/'\n\nWeb files have also been copied to your folder, you can upload it all to your website.");
        }

        static LuaFile[] PrepLuaFiles(string[] files)
        {
            List<LuaFile> generated = new List<LuaFile>();
            for( int n = 0; n<files.Length;n++ )
            {
                generated.Add(new LuaFile(files[n]));
            }
            return generated.ToArray();
        }

        static string[] BuildFileTree(string path)
        {
            string[] files = Directory.GetFiles(path, "*.lua", SearchOption.AllDirectories);
            return files;
        }

        [STAThread]
        static void Main()
        {
            StylishForm mainMenu = new StylishForm();
            mainMenu.Text = "LuaDocIt";
            mainMenu.SetBounds(0, 0, 800, 600);
            mainMenu.Icon = new Icon("content/icon.ico");

            Panel fileTreePanel = new Panel();
            fileTreePanel.Parent = mainMenu;
            fileTreePanel.SetBounds(230, 5, 550, 552);
            fileTreePanel.BackColor = Color.FromArgb(238, 238, 255);
            fileTreePanel.AutoScroll = false;
            fileTreePanel.HorizontalScroll.Enabled = false;
            fileTreePanel.HorizontalScroll.Visible = false;
            fileTreePanel.HorizontalScroll.Maximum = 0;
            fileTreePanel.AutoScroll = true;

            StylishButton addonFolder = new StylishButton();
            addonFolder.Text = "Select Folder";
            addonFolder.Parent = mainMenu;
            addonFolder.SetBounds(5, 5, 220, 35);
            addonFolder.TextFont = new Font("Arial", 12);
            addonFolder.Background = Color.FromArgb(255, 193, 7);
            addonFolder.SideGround = Color.FromArgb(194, 146, 0);
            addonFolder.TextColor = Color.White;
            FolderBrowserDialog fileSelection = new FolderBrowserDialog();
            addonFolder.Click += (sender, e) =>
            {
                fileSelection.ShowDialog();
            };

            StylishButton generateTree = new StylishButton();
            generateTree.Text = "List Files";
            generateTree.Parent = mainMenu;
            generateTree.SetBounds(5, 42, 220, 35);
            generateTree.TextFont = new Font("Arial", 12);
            generateTree.Background = Color.FromArgb(139, 195, 74);
            generateTree.SideGround = Color.FromArgb(104, 151, 50);
            generateTree.TextColor = Color.White;
            generateTree.Click += (sender, e) =>
            {
                for( int c = 0; c < fileTreePanel.Controls.Count; c++ )
                {
                    fileTreePanel.Controls[c].Dispose();
                    fileTreePanel.Invalidate();
                }

                string[] files = BuildFileTree(fileSelection.SelectedPath);
                LuaFile[] luaFileObj = PrepLuaFiles(files);

                for (int i = 0; i < files.Length; i++)
                {
                    PictureBox icon = new PictureBox();
                    icon.Image = Image.FromFile("content/lua_icon.png");
                    icon.Size = new Size(24, 24);
                    icon.SizeMode = PictureBoxSizeMode.StretchImage;
                    icon.Parent = fileTreePanel;
                    icon.Left = 5;
                    icon.Top = 5 + 29 * i;

                    Label name = new Label();
                    name.AutoSize = true;
                    name.Text = Path.GetFileName(files[i]);
                    name.Parent = fileTreePanel;
                    name.Left = 34;
                    name.Top = 10 + 29 * i;

                    Label total = new Label();
                    total.AutoSize = true;
                    total.Text = $"Elements: {luaFileObj[i].Functions.Length + luaFileObj[i].Hooks.Length}";
                    total.ForeColor = Color.Blue;
                    total.Parent = fileTreePanel;
                    total.Top = 10 + 29 * i;
                    total.Left = name.Left + name.Width + 5;

                    int goodf = 0;
                    for (int f = 0; f < luaFileObj[i].Functions.Length; f++)
                    {
                        if( luaFileObj[i].Functions[f].param.Count > 0 )
                        {
                            goodf++;
                        }
                    }

                    for (int f = 0; f < luaFileObj[i].Hooks.Length; f++)
                    {
                        if (luaFileObj[i].Hooks[f].param.Count > 0)
                        {
                            goodf++;
                        }
                    }

                    int badf = luaFileObj[i].Functions.Length + luaFileObj[i].Hooks.Length - goodf;

                    Label good = new Label();
                    good.AutoSize = true;
                    good.Text = goodf > 0 ? $"To-Document: {goodf}" : "";
                    good.ForeColor = Color.Green;
                    good.Parent = fileTreePanel;
                    good.Top = 10 + 29 * i;
                    good.Left = total.Left + total.Width + 2;

                    Label bad = new Label();
                    bad.AutoSize = true;
                    bad.Text = badf > 0 ? $"To-Skip: {badf}" : "";
                    bad.ForeColor = Color.Red;
                    bad.Parent = fileTreePanel;
                    bad.Top = 10 + 29 * i;
                    bad.Left = good.Left + good.Width + 2;
                }
            };

            StylishTextEntry docName = new StylishTextEntry();
            docName.Parent = mainMenu;
            docName.SetBounds(5, 495, 220, 35);
            docName.Text = "Project name";

            StylishButton generateDoc = new StylishButton();
            generateDoc.Text = "Generate Documentation";
            generateDoc.Parent = mainMenu;
            generateDoc.SetBounds(5, 522, 220, 35);
            generateDoc.TextFont = new Font("Arial", 12);
            generateDoc.Background = Color.FromArgb(233, 30, 99);
            generateDoc.SideGround = Color.FromArgb(178, 17, 72);
            generateDoc.TextColor = Color.White;
            generateDoc.Click += (sender, e) =>
            {
                string[] files = BuildFileTree(fileSelection.SelectedPath);
                LuaFile[] luaFileObj = PrepLuaFiles(files);

                GenerateDocumentation(luaFileObj, docName.Text);
            };

            mainMenu.ShowDialog();
        }
    }
}
