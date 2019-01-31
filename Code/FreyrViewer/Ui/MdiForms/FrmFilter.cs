using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Eir.Common.Extensions;
using FreyrViewer.Ui.Grids.ModelFilter;
using System.Text.RegularExpressions;
using FreyrViewer.Common;

namespace FreyrViewer.Ui.MdiForms
{
    public partial class FrmFilter : Form
    {
        private ModelFilterValues _filter;
        private readonly ModelFilterValues _orginal;

        public FrmFilter(ModelFilterValues filter,  List<string> headers)
        {
            InitializeComponent();
            _filter = filter;
            headers.ForEach(p=> cboColumn.Items.Add(p));
            cboColumn.SelectedIndex = 0;
            _orginal = filter.DeepCopy();
            RecalcFilterBox();

        }

        private void CloseCancel()
        {
            _filter = _orginal;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void cmdFilterCancel_Click(object sender, EventArgs e)
        {
            CloseCancel();
        }

        private void RecalcFilterBox()
        {

            txtFilter.Text = "";
            lstCheckBox.Items.Clear();
            
            var qfDisplay = new ModelFilterHelper().CreateDisplayOfQuickFilters(_filter);
            qfDisplay.ForEach(p =>
            {
                lstCheckBox.Items.Add(p, true);
            });

            QuickFilterValues remove = null;
            _filter.QuickFilters.ForEach(p =>
            {
                if (p.ToBeModifiedInFilterEditor)
                {
                    txtFilter.Text = p.FilterValue;
                    cboColumn.SelectedIndex = cboColumn.FindStringExact(p.ColumnName);
                    radRegex.Checked = true;
                    remove = p;
                    
                }
                
            });

            if(remove != null)
                _filter.QuickFilters.Remove(remove);
        }

        private void cmdClear_Click(object sender, EventArgs e)
        {
            _filter.QuickFilters = new List<QuickFilterValues>();
            RecalcFilterBox();
        }

        private void cmdClearQuick_Click(object sender, EventArgs e)
        {
            _filter.QuickFilters = new List<QuickFilterValues>();
            RecalcFilterBox();
        }

        private void cmdApplyAndClose_Click(object sender, EventArgs e)
        {
            _filter.QuickFilters = new List<QuickFilterValues>();

            foreach (var item in lstCheckBox.CheckedItems)
            {
                _filter.QuickFilters.AddRange(((QuickFilterDisplay)item).GetFilters());
            }
            if (!string.IsNullOrWhiteSpace(txtFilter.Text))
            {
                var newFilters = txtFilter.Lines?.ToList().Where(p => !string.IsNullOrEmpty(p));
                if (newFilters != null)
                {
                    foreach (var newFilter in newFilters)
                    {
                        bool negFilter = false;
                        var filter = newFilter;
                        if (radStartsWith.Checked)
                            filter = $"^{filter}";
                        if (radNotContains.Checked)
                            negFilter = true;
                        try
                        {
                            var regex = new Regex(filter);
                            Trace.WriteLine(regex.IsMatch(""));
                            _filter.QuickFilters.Add(new QuickFilterValues
                            {
                                FilterValue = filter,
                                FriendlyName = (negFilter ? "<>" : "=") + filter,
                                ColumnName = cboColumn.SelectedIndex > 0 ? cboColumn.SelectedItem.ToString() : null,
                                NegativeFilter = negFilter
                            });
                        }
                        catch
                        {
                            Mbox.Show("The filter is not correct. Edit or Cancel", "No you dont!");
                            return;
                        }
                    }
                }
                    
            }
            Close();
        }

        private void lstCheckBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://regex101.com/");
        }

        private void FrmFilter_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!(sender is Button))
            {
                CloseCancel();
            }
        }
    }
}
