using System;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Linq;
using static System.Windows.Forms.LinkLabel;

namespace MAFileProcessor
{
    public partial class MainForm : Form
    {
        private Label label2;
        private Label label1;
        private LinkLabel linkLabel1;
        private Button ChooseMaFile;
        private Button confirmButton;
        private TextBox pathTextBox_MaFiles;
        private RadioButton radiobuttonChangemafilesAccountname;
        private RadioButton radioButtonChangeMaFileSteamId;
        private RadioButton radioButtonBanMaFiles;
        private RadioButton radioButtonChooseMaFileFromTxt;
        private TextBox pathTextBox_TXT;
        private Button ChooseTXT;
        private string directoryPathMaFile;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private GroupBox groupBox4;
        private string directoryPathTxt;
        

        public MainForm()
        {
            InitializeComponent();
        }

        private void processButton_Click(object sender, EventArgs e)
        {
            FolderSelectDialog folderDialog = new FolderSelectDialog();
            if (folderDialog.ShowDialog(this.Handle))
            {
                this.directoryPathMaFile = folderDialog.FileName;
                pathTextBox_MaFiles.Text = folderDialog.FileName;
            }

            //FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            //DialogResult result = folderDialog.ShowDialog();
            //Console.WriteLine("1");
            //if (result == DialogResult.OK)
            //{
            //    string directoryPathMaFile = folderDialog.SelectedPath;
            //    /* BanMaFiles(directoryPathMaFile);*/
            //    this.directoryPathMaFile = directoryPathMaFile;
            //    pathTextBox_MaFiles.Text = directoryPathMaFile;
            //}
        }

        private void processButton_TXT_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string directoryPathTxt = openFileDialog.FileName;
                // 执行你的操作，比如调用处理方法
                this.directoryPathTxt = directoryPathTxt;
                pathTextBox_TXT.Text = directoryPathTxt;
            }
        }

        private void confirmButton_Click(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(this.directoryPathMaFile))
            {
                if (radioButtonBanMaFiles.Checked)
                {
                    // 阉割Ma
                    BanMaFiles(this.directoryPathMaFile);
                }
                if (radiobuttonChangemafilesAccountname.Checked)
                {
                    // 执行处理文件的操作
                    ChangeMaFilesByAccountName(this.directoryPathMaFile);
                }
                if (radioButtonChangeMaFileSteamId.Checked)
                {
                    // 执行处理文件的操作
                    ChangeMaFilesBySteamId(this.directoryPathMaFile);
                }
                if (radioButtonChooseMaFileFromTxt.Checked)
                {
                    if (!string.IsNullOrEmpty(this.directoryPathTxt))
                    {
                        CheckMaFiles(this.directoryPathMaFile, this.directoryPathTxt);
                    }
                    else
                    {
                        MessageBox.Show("请选择txt路径！");
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择令牌路径！");
            }
        }

        private void ChangeMaFilesByAccountName(string directoryPathMaFile)
        {
            string[] maFiles = Directory.GetFiles(directoryPathMaFile, "*.maFile");

            string newFolderPath = System.IO.Path.Combine(directoryPathMaFile, "maFiles-登录账户名");
            Directory.CreateDirectory(newFolderPath);

            foreach (string maFile in maFiles)
            {
                try
                {
                    string content = File.ReadAllText(maFile);
                    JObject jsonObject = JObject.Parse(content);

                    if (jsonObject.ContainsKey("account_name"))
                    {
                        string accountName = (String)jsonObject["account_name"];
                        // 创建并写入新的 .txt 文件
                        string matxtFilePath = System.IO.Path.Combine(newFolderPath, $"{accountName}.maFile");
                        File.WriteAllText(matxtFilePath, jsonObject.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"无法读取文件 {maFile}: {ex.Message}");
                }
            }
            MessageBox.Show("操作完成！");
        }

        private void ChangeMaFilesBySteamId(string directoryPathMaFile)
        {
            string[] maFiles = Directory.GetFiles(directoryPathMaFile, "*.maFile");

            string newFolderPath = System.IO.Path.Combine(directoryPathMaFile, "maFiles-SteamId");
            Directory.CreateDirectory(newFolderPath);

            foreach (string maFile in maFiles)
            {
                try
                {
                    string content = File.ReadAllText(maFile);
                    JObject jsonObject = JObject.Parse(content);

                    if (jsonObject.ContainsKey("account_name"))
                    {
                        long steamID = (long)jsonObject["Session"]["SteamID"];
                        string steamIDString = steamID.ToString();

                        // 创建并写入新的 .txt 文件
                        string maFilesPath = System.IO.Path.Combine(newFolderPath, $"{steamIDString}.maFile");
                        File.WriteAllText(maFilesPath, jsonObject.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"无法读取文件 {maFile}: {ex.Message}");
                }
            }
            MessageBox.Show("操作完成！");
        }

        private void CheckMaFiles(string directoryPathMaFile, string directoryPathTxt)
        {
            string[] accountNamesTxt = File.ReadAllLines(directoryPathTxt);
            string[] maFiles = Directory.GetFiles(directoryPathMaFile, "*.maFile");

            string newFolderPath = System.IO.Path.Combine(directoryPathMaFile, "maFiles-筛选后");
            Directory.CreateDirectory(newFolderPath);

            List<string> notFoundAccounts = new List<string>();

            foreach (string txtaccountName in accountNamesTxt)
            {
                bool found = false;

                foreach (string maFile in maFiles)
                {
                    try
                    {
                        string content = File.ReadAllText(maFile);
                        JObject jsonObject = JObject.Parse(content);

                        if (jsonObject.ContainsKey("account_name"))
                        {
                            string maAccountName = jsonObject["account_name"].ToString();
                            if (maAccountName == txtaccountName)
                            {
                                found = true;

                                string fileName = System.IO.Path.GetFileNameWithoutExtension(maFile);
                                string txtFilePath = System.IO.Path.Combine(newFolderPath, $"{txtaccountName}.maFile");
                                File.WriteAllText(txtFilePath, content);
                            }
                        }
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine($"Error occurred while processing file {maFile}: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error occurred while processing file {maFile}: {ex.Message}");
                    }
                }

                if (!found)
                {
                    notFoundAccounts.Add(txtaccountName);
                }
            }

            // Write not found accounts to a text file
            File.WriteAllLines(System.IO.Path.Combine(newFolderPath, "未找到.txt"), notFoundAccounts);

            MessageBox.Show("操作完成！请查看 maFiles-筛选后 文件夹！");
        }

        //阉割令牌
        private void BanMaFiles(string directoryPathMaFile)
        {
            string[] maFiles = Directory.GetFiles(directoryPathMaFile, "*.maFile");

            string newFolderPath = System.IO.Path.Combine(directoryPathMaFile, "maFiles-阉割");
            string newFolderPathAccountName = System.IO.Path.Combine(newFolderPath, "maFiles-登录账户名(已阉割)");
            string newFolderPathSteamId = System.IO.Path.Combine(newFolderPath, "maFiles-SteamId(已阉割)");
            Directory.CreateDirectory(newFolderPath);
            Directory.CreateDirectory(newFolderPathAccountName);
            Directory.CreateDirectory(newFolderPathSteamId);

            foreach (string maFile in maFiles)
            {
                try
                {
                    string content = File.ReadAllText(maFile);

                    // 将文件内容解析为 JSON 对象
                    JObject jsonObject = JObject.Parse(content);

                    // 创建新的 JSON 对象，仅包含需要保留的属性
                    JObject newJsonObject = new JObject();

                    // 保留特定的键值对，删除其他键值对
                    List<string> keysToKeep = new List<string> { "shared_secret", "account_name" };

                    JObject sessionObject = null;

                    if (jsonObject.ContainsKey("Session"))
                    {
                        sessionObject = (JObject)jsonObject["Session"];
                        keysToKeep.Add("SteamID");
                    }

                    foreach (string key in keysToKeep)
                    {
                        if (jsonObject.ContainsKey(key))
                        {
                            newJsonObject[key] = jsonObject[key];
                        }
                    }

                    if (sessionObject != null && sessionObject.ContainsKey("SteamID"))
                    {
                        if (newJsonObject.ContainsKey("Session"))
                        {
                            JObject newSessionObject = (JObject)newJsonObject["Session"];
                            newSessionObject["SteamID"] = sessionObject["SteamID"];
                        }
                        else
                        {
                            JObject newSessionObject = new JObject();
                            newSessionObject["SteamID"] = sessionObject["SteamID"];
                            newJsonObject["Session"] = newSessionObject;
                        }
                    }

                    newJsonObject["createDate"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    newJsonObject["createBy"] = "EasyCase免费令牌工具，QQ群：720210074";

                    // 获取 SteamID
                    if (newJsonObject.ContainsKey("Session"))
                    {
                        long steamID = (long)newJsonObject["Session"]["SteamID"];
                        string steamIDString = steamID.ToString();
                        string accountName = (string)newJsonObject["account_name"];

                        // 创建并写入新的 .maFile 文件
                        string maFilesPathAccountName = System.IO.Path.Combine(newFolderPathAccountName, $"{accountName}.maFile");
                        File.WriteAllText(maFilesPathAccountName, newJsonObject.ToString());

                        // 创建并写入新的 .maFile 文件
                        string maFilesPathSteamId = System.IO.Path.Combine(newFolderPathSteamId, $"{steamIDString}.maFile");
                        File.WriteAllText(maFilesPathSteamId, newJsonObject.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"无法读取文件 {maFile}: {ex.Message}");
                }
            }
            MessageBox.Show("操作完成！请查看同级目录下maFiles-改！");
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.ChooseMaFile = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.confirmButton = new System.Windows.Forms.Button();
            this.pathTextBox_MaFiles = new System.Windows.Forms.TextBox();
            this.radiobuttonChangemafilesAccountname = new System.Windows.Forms.RadioButton();
            this.radioButtonChangeMaFileSteamId = new System.Windows.Forms.RadioButton();
            this.radioButtonBanMaFiles = new System.Windows.Forms.RadioButton();
            this.radioButtonChooseMaFileFromTxt = new System.Windows.Forms.RadioButton();
            this.pathTextBox_TXT = new System.Windows.Forms.TextBox();
            this.ChooseTXT = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // ChooseMaFile
            // 
            this.ChooseMaFile.Location = new System.Drawing.Point(304, 20);
            this.ChooseMaFile.Name = "ChooseMaFile";
            this.ChooseMaFile.Size = new System.Drawing.Size(111, 23);
            this.ChooseMaFile.TabIndex = 0;
            this.ChooseMaFile.Text = "选择";
            this.ChooseMaFile.UseVisualStyleBackColor = true;
            this.ChooseMaFile.Click += new System.EventHandler(this.processButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(97, 293);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(245, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "请点击此处加群：          以获取后续支撑\r\n";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(168, 316);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "EasyCase工作室出品";
            this.label1.Click += new System.EventHandler(this.confirmButton_Click);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.linkLabel1.Location = new System.Drawing.Point(192, 293);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(59, 12);
            this.linkLabel1.TabIndex = 3;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "720210074";
            this.linkLabel1.Click += new System.EventHandler(this.linkLabel1_Click);
            // 
            // confirmButton
            // 
            this.confirmButton.Location = new System.Drawing.Point(325, 254);
            this.confirmButton.Name = "confirmButton";
            this.confirmButton.Size = new System.Drawing.Size(111, 23);
            this.confirmButton.TabIndex = 4;
            this.confirmButton.Text = "执行";
            this.confirmButton.UseVisualStyleBackColor = true;
            this.confirmButton.Click += new System.EventHandler(this.confirmButton_Click);
            // 
            // pathTextBox_MaFiles
            // 
            this.pathTextBox_MaFiles.Location = new System.Drawing.Point(8, 20);
            this.pathTextBox_MaFiles.Name = "pathTextBox_MaFiles";
            this.pathTextBox_MaFiles.Size = new System.Drawing.Size(290, 21);
            this.pathTextBox_MaFiles.TabIndex = 5;
            this.pathTextBox_MaFiles.TextChanged += new System.EventHandler(this.pathTextBox_TextChanged);
            // 
            // radiobuttonChangemafilesAccountname
            // 
            this.radiobuttonChangemafilesAccountname.AutoSize = true;
            this.radiobuttonChangemafilesAccountname.Location = new System.Drawing.Point(63, 45);
            this.radiobuttonChangemafilesAccountname.Name = "radiobuttonChangemafilesAccountname";
            this.radiobuttonChangemafilesAccountname.Size = new System.Drawing.Size(173, 16);
            this.radiobuttonChangemafilesAccountname.TabIndex = 11;
            this.radiobuttonChangemafilesAccountname.Text = "修改令牌名称 - 登陆账户名";
            this.radiobuttonChangemafilesAccountname.UseVisualStyleBackColor = true;
            // 
            // radioButtonChangeMaFileSteamId
            // 
            this.radioButtonChangeMaFileSteamId.AutoSize = true;
            this.radioButtonChangeMaFileSteamId.Location = new System.Drawing.Point(253, 45);
            this.radioButtonChangeMaFileSteamId.Name = "radioButtonChangeMaFileSteamId";
            this.radioButtonChangeMaFileSteamId.Size = new System.Drawing.Size(155, 16);
            this.radioButtonChangeMaFileSteamId.TabIndex = 12;
            this.radioButtonChangeMaFileSteamId.Text = "修改令牌名称 - SteamId";
            this.radioButtonChangeMaFileSteamId.UseVisualStyleBackColor = true;
            // 
            // radioButtonBanMaFiles
            // 
            this.radioButtonBanMaFiles.AutoSize = true;
            this.radioButtonBanMaFiles.Checked = true;
            this.radioButtonBanMaFiles.Location = new System.Drawing.Point(63, 23);
            this.radioButtonBanMaFiles.Name = "radioButtonBanMaFiles";
            this.radioButtonBanMaFiles.Size = new System.Drawing.Size(71, 16);
            this.radioButtonBanMaFiles.TabIndex = 13;
            this.radioButtonBanMaFiles.TabStop = true;
            this.radioButtonBanMaFiles.Text = "阉割令牌";
            this.radioButtonBanMaFiles.UseVisualStyleBackColor = true;
            // 
            // radioButtonChooseMaFileFromTxt
            // 
            this.radioButtonChooseMaFileFromTxt.AutoSize = true;
            this.radioButtonChooseMaFileFromTxt.Location = new System.Drawing.Point(253, 23);
            this.radioButtonChooseMaFileFromTxt.Name = "radioButtonChooseMaFileFromTxt";
            this.radioButtonChooseMaFileFromTxt.Size = new System.Drawing.Size(107, 16);
            this.radioButtonChooseMaFileFromTxt.TabIndex = 14;
            this.radioButtonChooseMaFileFromTxt.Text = "筛选指定的令牌";
            this.radioButtonChooseMaFileFromTxt.UseVisualStyleBackColor = true;
            // 
            // pathTextBox_TXT
            // 
            this.pathTextBox_TXT.Location = new System.Drawing.Point(8, 20);
            this.pathTextBox_TXT.Name = "pathTextBox_TXT";
            this.pathTextBox_TXT.Size = new System.Drawing.Size(290, 21);
            this.pathTextBox_TXT.TabIndex = 15;
            // 
            // ChooseTXT
            // 
            this.ChooseTXT.Location = new System.Drawing.Point(304, 20);
            this.ChooseTXT.Name = "ChooseTXT";
            this.ChooseTXT.Size = new System.Drawing.Size(111, 23);
            this.ChooseTXT.TabIndex = 16;
            this.ChooseTXT.Text = "选择";
            this.ChooseTXT.UseVisualStyleBackColor = true;
            this.ChooseTXT.Click += new System.EventHandler(this.processButton_TXT_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.pathTextBox_MaFiles);
            this.groupBox1.Controls.Add(this.ChooseMaFile);
            this.groupBox1.Location = new System.Drawing.Point(15, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(421, 55);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "令牌文件夹路径:";
            // 
            // groupBox2
            // 
            this.groupBox2.Location = new System.Drawing.Point(0, 61);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(421, 54);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "groupBox2";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.pathTextBox_TXT);
            this.groupBox3.Controls.Add(this.ChooseTXT);
            this.groupBox3.Location = new System.Drawing.Point(15, 93);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(421, 55);
            this.groupBox3.TabIndex = 20;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "账号路径:";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.radioButtonBanMaFiles);
            this.groupBox4.Controls.Add(this.radioButtonChooseMaFileFromTxt);
            this.groupBox4.Controls.Add(this.radiobuttonChangemafilesAccountname);
            this.groupBox4.Controls.Add(this.radioButtonChangeMaFileSteamId);
            this.groupBox4.Location = new System.Drawing.Point(15, 165);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(421, 83);
            this.groupBox4.TabIndex = 21;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "功能选择:";
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(454, 337);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.confirmButton);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "EasyCase工作室-令牌工具-v1.0.3";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            DialogResult result = folderDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                Console.WriteLine("1");
                string directoryPathMaFile = folderDialog.SelectedPath;
                BanMaFiles(directoryPathMaFile);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void label2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://qm.qq.com/cgi-bin/qm/qr?_wv=1027&k=KsACo7_kRTj2w2VLyU4ER5-Dnrp8mtWr&authKey=TnTEyqKLLiDx1AVmYq%2FOtnUoixVA%2FUK%2FcItcap2OwO43B3sEsReUS0OspCTf0e6l&noverify=0&group_code=720210074");  //利用Process.Start来打开
        }

        private void linkLabel1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://qm.qq.com/cgi-bin/qm/qr?_wv=1027&k=KsACo7_kRTj2w2VLyU4ER5-Dnrp8mtWr&authKey=TnTEyqKLLiDx1AVmYq%2FOtnUoixVA%2FUK%2FcItcap2OwO43B3sEsReUS0OspCTf0e6l&noverify=0&group_code=720210074");  //利用Process.Start来打开
            this.linkLabel1.LinkVisited = true;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void pathTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm()); // 这里的 Form1 是你的主窗体类名
        }
    }

}
