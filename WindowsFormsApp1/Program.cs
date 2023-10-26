using System;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using WindowsFormsApp1;

namespace MAFileProcessor
{
    public partial class MainForm : Form
    {
        private Label label2;
        private Label label1;
        private LinkLabel linkLabel1;
        private Button button1;

        public MainForm()
        {
            InitializeComponent();
        }

        private void processButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            DialogResult result = folderDialog.ShowDialog();
            Console.WriteLine("1");
            if (result == DialogResult.OK)
            {
                string directoryPath = folderDialog.SelectedPath;
                ProcessMAFiles(directoryPath);
            }
        }

        private void ProcessMAFiles(string directoryPath)
        {
            string[] maFiles = Directory.GetFiles(directoryPath, "*.maFile");

            string newFolderPath = Path.Combine(directoryPath, "maFiles-改");
            Directory.CreateDirectory(newFolderPath);

            foreach (string maFile in maFiles)
            {
                try
                {
                    string content = File.ReadAllText(maFile);

                    // 将文件内容解析为 JSON 对象
                    JObject jsonObject = JObject.Parse(content);


                    // 检查属性是否存在，然后将其设置为 null
                    if (jsonObject.ContainsKey("serial_number"))
                    {
                        jsonObject["serial_number"] = "";
                    }
                    if (jsonObject.ContainsKey("revocation_code"))
                    {
                        jsonObject["revocation_code"] = "";
                    }
                    if (jsonObject.ContainsKey("uri"))
                    {
                        jsonObject["uri"] = "";
                    }
                    if (jsonObject.ContainsKey("token_gid"))
                    {
                        jsonObject["token_gid"] = "";
                    }
                    if (jsonObject.ContainsKey("identity_secret"))
                    {
                        jsonObject["identity_secret"] = "";
                    }
                    if (jsonObject.ContainsKey("secret_1"))
                    {
                        jsonObject["secret_1"] = "";
                    }
                    if (jsonObject.ContainsKey("device_id"))
                    {
                        jsonObject["device_id"] = "";
                    }
                    if (jsonObject.ContainsKey("server_time"))
                    {
                        jsonObject["server_time"] = 0;
                    }

                    // 获取 Session 对象
                    JObject sessionObject = (JObject)jsonObject["Session"];

                    if (sessionObject != null)
                    {
                        
                        // 获取 SteamID
                        long steamID = (long)sessionObject["SteamID"];

                        // 将 SteamID 转换为字符串
                        string steamIDString = steamID.ToString();

                        // 创建并写入新的 .txt 文件
                        string txtFilePath = Path.Combine(newFolderPath, $"{steamIDString}.maFile");
                        File.WriteAllText(txtFilePath, jsonObject.ToString());

                        // 弹出消息框显示修改后的 JSON 内容
                        /*MessageBox.Show(jsonObject.ToString());*/
                    }
                    else
                    {
                        Console.WriteLine("JSON 中没有名为 Session 的键");
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
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(103, 50);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(169, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "请选择maFiles";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(38, 112);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(457, 21);
            this.label2.TabIndex = 2;
            this.label2.Text = "请点击此处加群：            以获取后续支撑\r\n";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(242, 209);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(170, 18);
            this.label1.TabIndex = 1;
            this.label1.Text = "EasyCase工作室出品";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.linkLabel1.Location = new System.Drawing.Point(157, 112);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(109, 21);
            this.linkLabel1.TabIndex = 3;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "720210074";
            this.linkLabel1.Click += new System.EventHandler(this.linkLabel1_Click);
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(367, 230);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "EasyCase工作室-令牌阉割工具";
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
                string directoryPath = folderDialog.SelectedPath;
                ProcessMAFiles(directoryPath);
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
