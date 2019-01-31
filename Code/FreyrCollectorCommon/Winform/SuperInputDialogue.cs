using System;
using System.Windows.Forms;

namespace FreyrCollectorCommon.Winform
{

    public partial class SuperInputDialogue : Form
    {

        private DropDownData _dropDownWithTextbox;

        public SuperInputDialogue()
        {
            InitializeComponent();
        }
        public SuperInputDialogue(string title,string text, string defaultValue="")
        {
            InitializeComponent();
            Title = title;
            PromptText = text;
            txtInput.Text = defaultValue;
        }

        public void UseCombobox(DropDownData dropdown)
        {
            _dropDownWithTextbox = dropdown;
        }

        public string Title { get; set; }

        public string PromptText { get; set; }

        /// <summary>
        /// The return value from a simple input box
        /// </summary>
        public string InputTextValue { get; set; }
        /// <summary>
        /// The return value from a combo box
        /// </summary>
        //public WinformHelper.ComboboxNameValuePairs ComboBoxValue { get; set; }

        private void SetControlsToHeartOfTheSun()
        {
            Text = Title;
            lblPrompt.Text = PromptText;

            if (_dropDownWithTextbox != null)
            {
                //var cboSelVal = string.Empty;
                //_dropDownWithTextbox.DropdownValues.ForEach(p =>
                //{
                //    cboInput.Items.Add(p);
                //    if (p.IsSelected)
                //        cboSelVal = p.DisplayValue;
                //});
                //WinformHelper.SetSelectedValueInComboBox(cboSelVal, cboInput);
                //panCboControls.Visible = true;
                //panCboControls.Top = txtInput.Top;
                //txtInput.Visible = false;
                //txtCboInput.Text = _dropDownWithTextbox.InputBox.ControlDefaultValue;

                //var orgwidth = lblCbo.Width;
                //lblCbo.Text = _dropDownWithTextbox.LabelText;
                //var newWidth = lblCbo.Width;

                //lblForCboTextbox.Text = _dropDownWithTextbox.InputBox.LabelText;
                //if (lblForCboTextbox.Width > newWidth)
                //    newWidth = lblForCboTextbox.Width;
                //if (newWidth > orgwidth)
                //{
                //    cboInput.Width = cboInput.Width - (newWidth - orgwidth);
                //    cboInput.Left = cboInput.Left + (newWidth - orgwidth);
                //    txtCboInput.Width = txtCboInput.Width - (newWidth - orgwidth);
                //    txtCboInput.Left = txtCboInput.Left + (newWidth - orgwidth);
                //}
                //Height = 213;
            }
            else
            {
                panCboControls.Visible = false;
                Height = 250;
            }
        }

        private void PromptDialog_Load(object sender, EventArgs e)
        {
            SetControlsToHeartOfTheSun();
        }

        private void SuperInputDialogue_Shown(object sender, EventArgs e)
        {
            if (txtInput.CanFocus)
            {
                txtInput.Focus();
            }
            else if (txtCboInput.CanFocus)
            {
                txtCboInput.Focus();
            }
            else if (cboInput.CanFocus)
            {
                cboInput.Focus();
            }
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void cmdOk_Click(object sender, EventArgs e)
        {
            if (_dropDownWithTextbox == null)
            {
                InputTextValue = txtInput.Text;
            }
            else
            {
                //ComboBoxValue = cboInput.SelectedItem as WinformHelper.ComboboxNameValuePairs;
                InputTextValue = txtCboInput.Text;
            }

            DialogResult = DialogResult.OK;
        }
    }
    public class ControlData
    {
        public string LabelText { get; set; }
        public string ControlDefaultValue { get; set; }
    }

    public class DropDownData
    {
        public string LabelText { get; set; }
        //public List<WinformHelper.ComboboxNameValuePairs> DropdownValues { get; set; }
        public ControlData InputBox { get; set; }
        public int Width { get; set; }

        public DropDownData()
        {
            Width = 255;
        }
    }
}