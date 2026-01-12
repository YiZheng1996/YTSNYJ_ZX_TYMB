using MainUI.CurrencyHelper;
using MainUI.TaskInformation;
using RW.EventLog;
using System.Text.RegularExpressions;

namespace MainUI
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
            InitializeLoginConfig();
        }
        ucFinger finger = new();
        private LoginConfig loginConfig;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;
        //public 
        private void frmLogin_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }
        private void frmLogin_Load(object sender, EventArgs e)
        {
            FingerprInt();
            txtJobNumber.Focus();
            btnAuthentication.Visible = VarHelper.deviceConfig.isAuthentication == "是";
        }

        private void FingerprInt()
        {
            try
            {
                //初始化指纹仪：
                int rst = new ucFinger().Init();
                if (rst == 0)
                {
                    lblMessage.Text = "指纹仪初始化完毕，请按指纹";
                    //VarHelper.speech.Speak("指纹仪初始化完毕，请按指纹");
                }
                if (rst != 0)
                {
                    lblMessage.Text = "指纹仪初始化失败，请使用密码登录";
                    //VarHelper.speech.Speak("指纹仪初始化失败，请使用密码登录！");
                    return;
                }
                // 打开
                finger.Open();
                Thread.Sleep(100);
                //加内存
                finger.LoadFingerToMemory();
                Thread.Sleep(100);
                //启动登录指纹扫描线程，比对
                finger.ThreadFingerLogin();
                //监控指纹匹配返回信息
                finger.LoginChanged += new ucFinger.LoginHandler(finger_LoginChanged);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("指纹仪初始化失败：" + ex.Message);
                //VarHelper.speech.Speak("初始化失败！");
                lblMessage.Text = "初始化失败！";
                finger.bIsTimeToDie = false; //重新启动指纹对比
            }
        }

        void finger_LoginChanged(int userid)
        {
            try
            {
                //指纹匹配失败。 返回-1，
                //指纹匹配成功。返回用户ID。
                if (userid != -1)
                {
                    OperateUserBLL bLL = new();
                    //用户权限赋值；
                    var user = bLL.SelectUser(new OperateUserModel { ID = userid });
                    if (user != null) NewUsers.NewUserInfo.InitUser(user);
                    IsFingerLogin = true;
                    Invoke(() =>
                    {
                        txtJobNumber.Text = NewUsers.NewUserInfo.JobNumber;
                        txtPassword.Text = NewUsers.NewUserInfo.Password;
                        LogOn();
                    });
                }
                else
                {
                    Invoke(() =>
                    {
                        lblMessage.Text = "指纹登录失败，请先录入指纹，已录入则请重按指纹！";
                        //VarHelper.speech.Speak("指纹登录失败，请先录入指纹，已录入则请重按指纹！");
                        finger.bIsTimeToDie = false; //重新启动指纹对比 
                    });
                }
            }
            catch (Exception ex)
            {
                NlogHelper.Default.Error("指纹登录失败：" + ex);
                //VarHelper.speech.Speak("指纹登录失败，请先录入指纹，已录入则请重按指纹！");
                lblMessage.Text = "指纹登录失败，请先录入指纹，已录入则请重按指纹！";
                finger.bIsTimeToDie = false; //重新启动指纹对比
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private bool IsFingerLogin = false; //是否为指纹登录
        private void btnSignIn_Click(object sender, EventArgs e)
        {
            IsFingerLogin = false;
            LogOn();
        }

        private void LogOn()
        {
            OperateUserBLL bLL = new();
            string jobNumber = txtJobNumber.Text.Trim();
            string password = string.Empty;
            if (!IsFingerLogin)
            {
                password = VarHelper.SHA512Encoding(jobNumber, txtPassword.Text.Trim());
            }
            else
            {
                password = txtPassword.Text.Trim();
            }

            if (string.IsNullOrEmpty(jobNumber))
            {
                lblMessage.Text = "请输入要登录的工号!";
                lblMessage.Visible = true;
                txtJobNumber.Focus();
                return;
            }
            if (string.IsNullOrEmpty(password))
            {
                lblMessage.Text = "密码不能为空，请重新输入!";
                lblMessage.Visible = true;
                txtPassword.Focus();
                return;
            }

            var DataCount = bLL.SelectUserID(jobNumber);
            if (DataCount == null)
            {
                lblMessage.Text = "工号不存在，请重新输入!";
                lblMessage.Visible = true;
                txtPassword.Focus();
                return;
            }

            var user = bLL.SelectUser(new OperateUserModel { ID = DataCount.ID });
            if (user != null)
            {
                if (user.Password != password)
                {
                    lblMessage.Text = "密码错误，请重新输入!";
                    lblMessage.Visible = true;
                    txtPassword.Focus();
                    return;
                }
                else
                {
                    finger.bIsTimeToDie = true; // 停止登录扫描指纹线程
                    NewUsers.NewUserInfo.InitUser(user);
                    EventLogHelper.Log(EventLogType.Normal, "工号" + NewUsers.NewUserInfo.JobNumber + "登录。");

                    // 登录成功后保存登录配置
                    SaveLoginConfig();

                    DialogResult = DialogResult.OK;
                    Close();
                    finger.CloseFinger();
                }
            }
            else
            {
                lblMessage.Text = "未找到该用户!";
                lblMessage.Visible = true;
                return;
            }
        }

        private void btnAuthentication_Click(object sender, EventArgs e)
        {
            frmAuthentication frm = new();
            frm.ShowDialog();
        }

        #region 记住密码

        /// <summary>
        /// 初始化登录配置 - 加载保存的账号密码
        /// </summary>
        private void InitializeLoginConfig()
        {
            try
            {
                loginConfig = new LoginConfig();

                // 如果启用了记住密码功能
                if (loginConfig.RememberPassword != "1") return;
                chkRememberPassword.Checked = true;

                // 自动填充工号
                if (!string.IsNullOrEmpty(loginConfig.SavedJobNumber))
                {
                    txtJobNumber.Text = loginConfig.SavedJobNumber;
                }

                // 自动填充密码
                if (!string.IsNullOrEmpty(loginConfig.SavedPassword))
                {
                    txtPassword.Text = DecryptPassword(loginConfig.SavedPassword);
                }
            }
            catch (Exception ex)
            {
                NlogHelper.Default.Error("加载登录配置失败：", ex);
            }
        }

        /// <summary>
        /// 保存登录配置
        /// </summary>
        private void SaveLoginConfig()
        {
            try
            {
                loginConfig = new LoginConfig();

                if (chkRememberPassword.Checked)
                {
                    loginConfig.RememberPassword = "1";
                    loginConfig.SavedJobNumber = txtJobNumber.Text.Trim();
                    loginConfig.SavedPassword = EncryptPassword(txtPassword.Text.Trim());
                }
                else
                {
                    // 不记住密码时清空保存的信息
                    loginConfig.RememberPassword = "0";
                    loginConfig.SavedJobNumber = "";
                    loginConfig.SavedPassword = "";
                }

                loginConfig.Save();
            }
            catch (Exception ex)
            {
                NlogHelper.Default.Error("保存登录配置失败：", ex);
            }
        }

        /// <summary>
        /// 简单加密密码（使用Base64）
        /// </summary>
        private string EncryptPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return string.Empty;

            try
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(password);
                return Convert.ToBase64String(bytes);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 解密密码
        /// </summary>
        private string DecryptPassword(string encryptedPassword)
        {
            if (string.IsNullOrEmpty(encryptedPassword))
                return string.Empty;

            try
            {
                byte[] bytes = Convert.FromBase64String(encryptedPassword);
                return System.Text.Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                return string.Empty;
            }
        }

        #endregion

    }
}