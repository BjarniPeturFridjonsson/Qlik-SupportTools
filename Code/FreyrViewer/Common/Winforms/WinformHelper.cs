using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Eir.Common.Logging;
using FreyrViewer.Extensions;
using FreyrViewer.Ui.Controls;

namespace FreyrViewer.Common
{
    public static class WinformHelper
    {
        /// <summary>
        /// Use in comboboxes where you need a value for the database different from the value shown to the user.
        /// <para>Also if youre here, look at EnumHelper where you can use an enum for the dropdown</para>
        /// </summary>
        public class ComboboxNameValuePairs
        {
            public string DisplayValue { get; set; }
            public string StringValue { get; set; }
            public int IntValue { get; set; }
            public Guid GuidValue { get; set; }
            public DateTime DateValue { get; set; }
            public Object Tag { get; set; }

            /// <summary>
            /// Set this value for sending valuePairs to a combobox so it can set the correct item as selected
            /// </summary>
            public bool IsSelected { get; set; }

            public override string ToString()
            {
                return DisplayValue;
            }

            public void ShouldBeSelected(string value)
            {
                IsSelected = DisplayValue == value;
            }

            public void ShouldBeSelected(int value)
            {
                IsSelected = IntValue == value;
            }
        }

        /// <summary>
        /// If a string exists in the combobox, it will set the correct selectedIndex.
        /// </summary>
        /// <param name="valueToSet"></param>
        /// <param name="cmb"></param>
        public static void SetSelectedValueInComboBox(string valueToSet, ComboBox cmb)
        {
            if (string.IsNullOrEmpty(valueToSet))
                return;
            for (int i = 0; i < cmb.Items.Count; i++)
            {
                if (valueToSet.Equals(cmb.Items[i].ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    cmb.SelectedIndex = i;
                    break;
                }
            }
        }

        /// <summary>
        /// Creates values in the ListBox and selects the current value if found. 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="lst"></param>
        public static void AddValuesToListBox(IEnumerable<ComboboxNameValuePairs> values, ListBox lst)
        {
            values.ToList().ForEach(p => { lst.Items.Add(p); });
        }

        /// <summary>
        /// Creates values in the combobox and selects the current value if found. 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="cmb"></param>
        /// <param name="currentValue">The value that should be selected in the combobox.</param>
        public static void AddValuesToComboBox(IEnumerable<object> values, ComboBox cmb, string currentValue = null)
        {
            values.ToList().ForEach(p => { cmb.Items.Add(p); });
            if (!string.IsNullOrEmpty(currentValue))
                SetSelectedValueInComboBox(currentValue, cmb);
        }


        public static void AddValuesToComboBox<T>(IEnumerable<T> values, ComboBox cmb, Predicate<T> currentValuePredicate) where T : class
        {
            values.ToList().ForEach(p => { cmb.Items.Add(p); });
            if (currentValuePredicate != null)
                SetSelectedValueInComboBox(currentValuePredicate, cmb);
        }

        public static void SetSelectedValueInComboBox<T>(Predicate<T> currentValuePredicate, ComboBox cmb) where T : class
        {
            for (int i = 0; i < cmb.Items.Count; i++)
            {
                T item = cmb.Items[i] as T;
                if (currentValuePredicate(item))
                {
                    cmb.SelectedIndex = i;
                    return;
                }
            }

            cmb.SelectedIndex = -1;
        }

        public static DialogResult GetInputBoxValue(string title, string promptText, out string result)
        {
            DialogResult res;
            result = string.Empty;
            using (var dlg = new SuperInputDialogue())
            {
                dlg.PromptText = "this is my promt text";
                dlg.Title = "Select host name";
                dlg.ShowDialog();
                res = dlg.DialogResult;
                if (dlg.DialogResult == DialogResult.OK)
                {
                    result = dlg.InputTextValue;
                }
            }
            return res;
        }

        public static DialogResult GetInputAndCboValue(string title, string promptText, string textLabel, string cboLabel, List<ComboboxNameValuePairs> dropdownValues, ref string result, out ComboboxNameValuePairs comboboxValue)
        {
            DialogResult res;
            comboboxValue = null;
            using (var dlg = new SuperInputDialogue())
            {
                dlg.PromptText = promptText;
                dlg.Title = title;
                dlg.UseCombobox(new DropDownData
                {
                    DropdownValues = dropdownValues,
                    LabelText = cboLabel,
                    InputBox = new ControlData
                    {
                        LabelText = textLabel,
                        ControlDefaultValue = result
                    }
                });
                dlg.ShowDialog();
                res = dlg.DialogResult;
                if (dlg.DialogResult == DialogResult.OK)
                {
                    result = dlg.InputTextValue;
                    comboboxValue = dlg.ComboBoxValue;
                }
            }
            return res;
        }

        public static void SetTextAndColor(this Label label, string text, Color forecolor)
        {
            try
            {
                if (label.IsDisposed || !label.IsHandleCreated)
                    return;
                label.OnUiThread(() =>
                {
                    if (label.IsDisposed) return;
                    label.Text = text;
                    label.ForeColor = forecolor;
                });
            }
            catch (Exception ex)
            {
                Log.To.Main.AddException("Error in SetTextAndColor", ex);
            }
        }
    }
}