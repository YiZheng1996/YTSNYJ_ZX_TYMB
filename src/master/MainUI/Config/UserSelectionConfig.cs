namespace MainUI.Config
{
    /// <summary>
    /// 人员选择配置类 - 保存最后一次选择的人员信息
    /// </summary>
    public class UserSelectionConfig : IniConfig
    {
        public UserSelectionConfig()
            : base(Application.StartupPath + "\\config\\UserSelectionConfig.ini")
        {
            Load();
        }

        public UserSelectionConfig(string sectionName)
            : base(Application.StartupPath + "\\config\\UserSelectionConfig.ini")
        {
            SetSectionName(sectionName);
            Load();
        }

        /// <summary>
        /// 互检人工号
        /// </summary>
        [IniKeyName("互检人工号")]
        public string MutualPerson { get; set; }

        /// <summary>
        /// 互检人姓名
        /// </summary>
        [IniKeyName("互检人姓名")]
        public string MutualPersonName { get; set; }

        /// <summary>
        /// 质检人工号
        /// </summary>
        [IniKeyName("质检人工号")]
        public string QualityPerson { get; set; }

        /// <summary>
        /// 质检人姓名
        /// </summary>
        [IniKeyName("质检人姓名")]
        public string QualityPersonName { get; set; }

        /// <summary>
        /// 辅助人工号1
        /// </summary>
        [IniKeyName("辅助人工号1")]
        public string AuxiliariesCode1 { get; set; }

        /// <summary>
        /// 辅助人姓名1
        /// </summary>
        [IniKeyName("辅助人姓名1")]
        public string AuxiliariesName1 { get; set; }

        /// <summary>
        /// 辅助人工号2
        /// </summary>
        [IniKeyName("辅助人工号2")]
        public string AuxiliariesCode2 { get; set; }

        /// <summary>
        /// 辅助人姓名2
        /// </summary>
        [IniKeyName("辅助人姓名2")]
        public string AuxiliariesName2 { get; set; }

        /// <summary>
        /// 辅助人工号3
        /// </summary>
        [IniKeyName("辅助人工号3")]
        public string AuxiliariesCode3 { get; set; }

        /// <summary>
        /// 辅助人姓名3
        /// </summary>
        [IniKeyName("辅助人姓名3")]
        public string AuxiliariesName3 { get; set; }

        /// <summary>
        /// 辅助人工号4
        /// </summary>
        [IniKeyName("辅助人工号4")]
        public string AuxiliariesCode4 { get; set; }

        /// <summary>
        /// 辅助人姓名4
        /// </summary>
        [IniKeyName("辅助人姓名4")]
        public string AuxiliariesName4 { get; set; }
    }
}