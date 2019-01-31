using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FreyrCommon.Models;

namespace FreyrSenseCollector.Dialogues
{
    public partial class FrmProblemsFound : Form
    {
        public FrmProblemsFound(List<IssueRegister> issues)
        {
            InitializeComponent();
            txtProblems.Text = string.Join(Environment.NewLine, issues);
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
