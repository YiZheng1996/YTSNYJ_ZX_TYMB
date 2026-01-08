namespace MainUI.Config
{
    public class LoginConfig: IniConfig
    {
        public LoginConfig()
          : base(Application.StartupPath + "config\\LoginConfig.ini")
        {
            Load();
        }
        public LoginConfig(string sectionName)
            : base(Application.StartupPath + "config\\LoginConfig.ini")
        {
            this.SetSectionName(sectionName);
            Load();
        }

        /// <summary>
        /// 是否记住密码
        /// </summary>
        [IniKeyName("是否记住密码")]
        public string RememberPassword { get; set; }

        /// <summary>
        /// 保存的工号
        /// </summary>
        [IniKeyName("保存的工号")]
        public string SavedJobNumber { get; set; }

        /// <summary>
        /// 保存的密码（加密后）
        /// </summary>
        [IniKeyName("保存的密码")]
        public string SavedPassword { get; set; }
    }
}
