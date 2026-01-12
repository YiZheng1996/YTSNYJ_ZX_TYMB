using MainUI.CurrencyHelper;
using MainUI.Config;

namespace MainUI.TaskInformation
{
    public partial class frmUserSelection : UIForm
    {
        public AuxiliaryModel model = new();
        private UserSelectionConfig userSelectionConfig;

        public frmUserSelection()
        {
            InitializeComponent();

            // 初始化配置
            userSelectionConfig = new UserSelectionConfig();

            // 加载上次保存的人员信息
            LoadLastUserSelection();
        }

        /// <summary>
        /// 加载上次选择的人员信息
        /// </summary>
        private void LoadLastUserSelection()
        {
            try
            {
                // 从配置文件加载所有字段
                txtMutualPerson.Text = userSelectionConfig.MutualPerson ?? "";
                txtmutualPersonName.Text = userSelectionConfig.MutualPersonName ?? "";
                txtqualityPerson.Text = userSelectionConfig.QualityPerson ?? "";
                txtqualityPersonName.Text = userSelectionConfig.QualityPersonName ?? "";
                txtauxiliariesCode1.Text = userSelectionConfig.AuxiliariesCode1 ?? "";
                txtauxiliariesName1.Text = userSelectionConfig.AuxiliariesName1 ?? "";
                txtauxiliariesCode2.Text = userSelectionConfig.AuxiliariesCode2 ?? "";
                txtauxiliariesName2.Text = userSelectionConfig.AuxiliariesName2 ?? "";
                txtauxiliariesCode3.Text = userSelectionConfig.AuxiliariesCode3 ?? "";
                txtauxiliariesName3.Text = userSelectionConfig.AuxiliariesName3 ?? "";
                txtauxiliariesCode4.Text = userSelectionConfig.AuxiliariesCode4 ?? "";
                txtauxiliariesName4.Text = userSelectionConfig.AuxiliariesName4 ?? "";
            }
            catch (Exception ex)
            {
                NlogHelper.Default.Error("加载上次人员选择错误:", ex);
            }
        }

        /// <summary>
        /// 保存当前选择的人员信息到配置文件
        /// </summary>
        private void SaveUserSelection()
        {
            try
            {
                userSelectionConfig.MutualPerson = txtMutualPerson.Text.Trim();
                userSelectionConfig.MutualPersonName = txtmutualPersonName.Text.Trim();
                userSelectionConfig.QualityPerson = txtqualityPerson.Text.Trim();
                userSelectionConfig.QualityPersonName = txtqualityPersonName.Text.Trim();
                userSelectionConfig.AuxiliariesCode1 = txtauxiliariesCode1.Text.Trim();
                userSelectionConfig.AuxiliariesName1 = txtauxiliariesName1.Text.Trim();
                userSelectionConfig.AuxiliariesCode2 = txtauxiliariesCode2.Text.Trim();
                userSelectionConfig.AuxiliariesName2 = txtauxiliariesName2.Text.Trim();
                userSelectionConfig.AuxiliariesCode3 = txtauxiliariesCode3.Text.Trim();
                userSelectionConfig.AuxiliariesName3 = txtauxiliariesName3.Text.Trim();
                userSelectionConfig.AuxiliariesCode4 = txtauxiliariesCode4.Text.Trim();
                userSelectionConfig.AuxiliariesName4 = txtauxiliariesName4.Text.Trim();

                userSelectionConfig.Save();
            }
            catch (Exception ex)
            {
                NlogHelper.Default.Error("保存人员选择错误:", ex);
            }
        }

        /// <summary>
        /// 验证所有必填字段是否已填写
        /// </summary>
        /// <returns>验证是否通过</returns>
        private bool ValidateRequiredFields()
        {
            // 定义所有必填字段及其显示名称
            var requiredFields = new Dictionary<UITextBox, string>
            {
                { txtMutualPerson, "互检人工号" },
                { txtmutualPersonName, "互检人姓名" },
                { txtqualityPerson, "质检人工号" },
                { txtqualityPersonName, "质检人姓名" },
                { txtauxiliariesCode1, "辅助人工号1" },
                { txtauxiliariesName1, "辅助人姓名1" },
                { txtauxiliariesCode2, "辅助人工号2" },
                { txtauxiliariesName2, "辅助人姓名2" },
                { txtauxiliariesCode3, "辅助人工号3" },
                { txtauxiliariesName3, "辅助人姓名3" },
                { txtauxiliariesCode4, "辅助人工号4" },
                { txtauxiliariesName4, "辅助人姓名4" }
            };

            // 检查每个字段
            foreach (var field in requiredFields)
            {
                if (string.IsNullOrWhiteSpace(field.Key.Text))
                {
                    MessageHelper.MessageOK(this, $"请填写 [{field.Value}]！");
                    field.Key.Focus(); // 将焦点设置到未填写的字段
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 检查登录工号是否与界面中的工号重复
        /// </summary>
        /// <returns>如果重复返回重复的字段名称，否则返回null</returns>
        private string CheckLoginJobNumberDuplicate()
        {
            // 获取当前登录用户的工号
            string loginJobNumber = NewUsers.NewUserInfo.JobNumber;

            // 检查互检人工号
            if (!string.IsNullOrWhiteSpace(txtMutualPerson.Text) &&
                txtMutualPerson.Text.Trim() == loginJobNumber)
            {
                return "互检人工号";
            }

            // 检查质检人工号
            if (!string.IsNullOrWhiteSpace(txtqualityPerson.Text) &&
                txtqualityPerson.Text.Trim() == loginJobNumber)
            {
                return "质检人工号";
            }

            // 检查辅助人工号1
            if (!string.IsNullOrWhiteSpace(txtauxiliariesCode1.Text) &&
                txtauxiliariesCode1.Text.Trim() == loginJobNumber)
            {
                return "辅助人工号1";
            }

            // 检查辅助人工号2
            if (!string.IsNullOrWhiteSpace(txtauxiliariesCode2.Text) &&
                txtauxiliariesCode2.Text.Trim() == loginJobNumber)
            {
                return "辅助人工号2";
            }

            // 检查辅助人工号3
            if (!string.IsNullOrWhiteSpace(txtauxiliariesCode3.Text) &&
                txtauxiliariesCode3.Text.Trim() == loginJobNumber)
            {
                return "辅助人工号3";
            }

            // 检查辅助人工号4
            if (!string.IsNullOrWhiteSpace(txtauxiliariesCode4.Text) &&
                txtauxiliariesCode4.Text.Trim() == loginJobNumber)
            {
                return "辅助人工号4";
            }

            return null; // 没有重复
        }

        /// <summary>
        /// 检查界面中各个角色的工号是否有重复
        /// </summary>
        /// <returns>如果重复返回重复信息，否则返回null</returns>
        private (bool isDuplicate, string message) CheckJobNumberDuplicateAmongRoles()
        {
            // 构建工号列表（工号，姓名，角色名称）
            List<(string JobNumber, string Name, string RoleName)> userList = new()
            {
                (txtMutualPerson.Text.Trim(), txtmutualPersonName.Text.Trim(), "互检人"),
                (txtqualityPerson.Text.Trim(), txtqualityPersonName.Text.Trim(), "质检人"),
                (txtauxiliariesCode1.Text.Trim(), txtauxiliariesName1.Text.Trim(), "辅助人1"),
                (txtauxiliariesCode2.Text.Trim(), txtauxiliariesName2.Text.Trim(), "辅助人2"),
                (txtauxiliariesCode3.Text.Trim(), txtauxiliariesName3.Text.Trim(), "辅助人3"),
                (txtauxiliariesCode4.Text.Trim(), txtauxiliariesName4.Text.Trim(), "辅助人4")
            };

            // 检查工号重复（同一工号出现在多个角色中）
            var duplicateJobNumbers = userList
                .GroupBy(u => u.JobNumber)
                .Where(g => g.Count() > 1)
                .Select(g => new
                {
                    JobNumber = g.Key,
                    Roles = string.Join("、", g.Select(x => x.RoleName)),
                    FirstName = g.First().Name
                });

            if (duplicateJobNumbers.Any())
            {
                var duplicate = duplicateJobNumbers.First();
                string message = $"工号 {duplicate.JobNumber}({duplicate.FirstName}) 在 [{duplicate.Roles}] 中重复，请重新输入！";
                return (true, message);
            }

            return (false, null);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // 1. 先验证所有必填字段
            if (!ValidateRequiredFields())
            {
                return;
            }

            // 2. 检查登录工号是否与界面中的工号重复
            var duplicateField = CheckLoginJobNumberDuplicate();
            if (duplicateField != null)
            {
                var message = $"当前登录人员：{NewUsers.NewUserInfo.Username}(工号：{NewUsers.NewUserInfo.JobNumber})\n与 [{duplicateField}] 重复，请重新选择！";
                MessageHelper.MessageOK(this, message);
                return;
            }

            // 3. 检查界面中各个角色的工号是否有重复
            var (isDuplicate, duplicateMessage) = CheckJobNumberDuplicateAmongRoles();
            if (isDuplicate)
            {
                MessageHelper.MessageOK(this, duplicateMessage);
                return;
            }

            // 4. 构建 model 对象供外部使用
            model = new AuxiliaryModel
            {
                mutualPerson = txtMutualPerson.Text.Trim(),
                mutualPersonName = txtmutualPersonName.Text.Trim(),
                qualityPerson = txtqualityPerson.Text.Trim(),
                qualityPersonName = txtqualityPersonName.Text.Trim(),
                auxiliariesCode1 = txtauxiliariesCode1.Text.Trim(),
                auxiliariesName1 = txtauxiliariesName1.Text.Trim(),
                auxiliariesCode2 = txtauxiliariesCode2.Text.Trim(),
                auxiliariesName2 = txtauxiliariesName2.Text.Trim(),
                auxiliariesCode3 = txtauxiliariesCode3.Text.Trim(),
                auxiliariesName3 = txtauxiliariesName3.Text.Trim(),
                auxiliariesCode4 = txtauxiliariesCode4.Text.Trim(),
                auxiliariesName4 = txtauxiliariesName4.Text.Trim()
            };

            // 5. 保存当前选择的人员信息
            SaveUserSelection();

            Close();
        }
    }
}