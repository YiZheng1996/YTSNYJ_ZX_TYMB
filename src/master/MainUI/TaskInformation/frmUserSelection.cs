using MainUI.CurrencyHelper;
using MainUI.Config;

namespace MainUI.TaskInformation
{
    public partial class frmUserSelection : UIForm
    {
        AuxiliaryBLL bLL = new();
        public AuxiliaryModel model = new();
        private UserSelectionConfig userSelectionConfig;

        public frmUserSelection()
        {
            InitializeComponent();
            var data = bLL.AuxiliaryModels(new AuxiliaryModel());
            foreach (var item in data) ComMutualPerson.Items.Add(item.mutualPerson);

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
                // 如果配置中有互检人工号,尝试恢复选择
                if (string.IsNullOrEmpty(userSelectionConfig.MutualPerson)) return;
                // 在下拉框中查找对应的工号
                for (var i = 0; i < ComMutualPerson.Items.Count; i++)
                {
                    if (ComMutualPerson.Items[i].ToString() != userSelectionConfig.MutualPerson) continue;
                    ComMutualPerson.SelectedIndex = i;
                    break;
                }

                // 手动设置文本框的值(防止下拉框中没有对应项)
                if (ComMutualPerson.SelectedIndex != -1) return;
                txtmutualPersonName.Text = userSelectionConfig.MutualPersonName;
                txtqualityPerson.Text = userSelectionConfig.QualityPerson;
                txtqualityPersonName.Text = userSelectionConfig.QualityPersonName;
                txtauxiliariesCode1.Text = userSelectionConfig.AuxiliariesCode1;
                txtauxiliariesName1.Text = userSelectionConfig.AuxiliariesName1;
                txtauxiliariesCode2.Text = userSelectionConfig.AuxiliariesCode2;
                txtauxiliariesName2.Text = userSelectionConfig.AuxiliariesName2;
                txtauxiliariesCode3.Text = userSelectionConfig.AuxiliariesCode3;
                txtauxiliariesName3.Text = userSelectionConfig.AuxiliariesName3;
                txtauxiliariesCode4.Text = userSelectionConfig.AuxiliariesCode4;
                txtauxiliariesName4.Text = userSelectionConfig.AuxiliariesName4;
            }
            catch (Exception ex)
            {
                NlogHelper.Default.Error("加载上次人员选择错误：", ex);
            }
        }

        /// <summary>
        /// 保存当前选择的人员信息到配置文件
        /// </summary>
        private void SaveUserSelection()
        {
            try
            {
                userSelectionConfig.MutualPerson = ComMutualPerson.SelectedItem?.ToString() ?? "";
                userSelectionConfig.MutualPersonName = txtmutualPersonName.Text;
                userSelectionConfig.QualityPerson = txtqualityPerson.Text;
                userSelectionConfig.QualityPersonName = txtqualityPersonName.Text;
                userSelectionConfig.AuxiliariesCode1 = txtauxiliariesCode1.Text;
                userSelectionConfig.AuxiliariesName1 = txtauxiliariesName1.Text;
                userSelectionConfig.AuxiliariesCode2 = txtauxiliariesCode2.Text;
                userSelectionConfig.AuxiliariesName2 = txtauxiliariesName2.Text;
                userSelectionConfig.AuxiliariesCode3 = txtauxiliariesCode3.Text;
                userSelectionConfig.AuxiliariesName3 = txtauxiliariesName3.Text;
                userSelectionConfig.AuxiliariesCode4 = txtauxiliariesCode4.Text;
                userSelectionConfig.AuxiliariesName4 = txtauxiliariesName4.Text;

                userSelectionConfig.Save();
            }
            catch (Exception ex)
            {
                NlogHelper.Default.Error("保存人员选择错误：", ex);
            }
        }

        /// <summary>
        /// 检查登录工号是否与界面中的工号重复
        /// </summary>
        /// <returns>如果重复返回重复的工号名称,否则返回null</returns>
        private string CheckLoginJobNumberDuplicate()
        {
            // 获取当前登录用户的工号
            string loginJobNumber = NewUsers.NewUserInfo.JobNumber;

            // 检查互检人工号
            if (ComMutualPerson.SelectedItem?.ToString() == loginJobNumber)
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

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ComMutualPerson.SelectedItem?.ToString()))
            {
                MessageHelper.MessageOK(this, "未选择人员！");
                return;
            }

            // 检查登录工号是否与界面中的工号重复
            var duplicateField = CheckLoginJobNumberDuplicate();
            if (duplicateField != null)
            {
                var message = $"当前登录人员：{NewUsers.NewUserInfo.Username}(工号：{NewUsers.NewUserInfo.JobNumber})\n与{duplicateField}重复，请重新选择！";
                MessageHelper.MessageOK(this, message);
            }

            // 保存当前选择的人员信息
            SaveUserSelection();

            Close();
        }

        private void ComMutualPerson_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var Com = sender as Sunny.UI.UIComboBox;
                var data = bLL.AuxiliaryModels(new AuxiliaryModel
                {
                    mutualPerson = Com.SelectedItem.ToString(),
                }).FirstOrDefault();

                if (data == null) return;
                model = data;
                txtmutualPersonName.Text = data.mutualPersonName;
                txtqualityPerson.Text = data.qualityPerson;
                txtqualityPersonName.Text = data.qualityPersonName;
                txtauxiliariesCode1.Text = data.auxiliariesCode1;
                txtauxiliariesName1.Text = data.auxiliariesName1;
                txtauxiliariesCode2.Text = data.auxiliariesCode2;
                txtauxiliariesName2.Text = data.auxiliariesName2;
                txtauxiliariesCode3.Text = data.auxiliariesCode3;
                txtauxiliariesName3.Text = data.auxiliariesName3;
                txtauxiliariesCode4.Text = data.auxiliariesCode4;
                txtauxiliariesName4.Text = data.auxiliariesName4;
            }
            catch (Exception ex)
            {
                NlogHelper.Default.Error("人员选择错误：", ex);
                MessageHelper.MessageOK(this, "人员选择错误：" + ex);
            }
        }
    }
}