using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace FreyrViewer.Common
{
    public static class EnumHelper
    {
        /// <summary>
        /// To be able to use name/value pares in comboboxes
        /// <para>Usage:</para>
        /// <para>In combobox</para>
        /// <para>EnumHelper.EnumAsCombobox&lt;myEnum&gt;(comboBox1, (int)myData.MyEnumValue);</para>
        /// <para>Getting the value:</para>
        /// <para>((EnumHelper.EnumDropdownValue)comboBox1.SelectedItem).Value</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static void EnumAsCombobox<T>(ComboBox ctrl, int currentSelectedValue)
        {
            var d = GetValues<T>().ToList();
            var values = d.Select(val => new WinformHelper.ComboboxNameValuePairs { IntValue = (int)(Enum.Parse(typeof(T), val.ToString())), DisplayValue = val.GetDescription() }).ToArray();
            ctrl.Items.AddRange(values);
            SetSelectedValueInComboBox(ctrl, currentSelectedValue);
            //WinformHelper.SetSelectedValueInComboBox(currentSelectedValue.ToString(), ctrl);
        }

        /// <summary>
        /// To be able to use name/value pares in comboboxes
        /// <para>Usage:</para>
        /// <para>In combobox</para>
        /// <para>EnumHelper.EnumAsCombobox&lt;myEnum&gt;(comboBox1, (int)myData.MyEnumValue);</para>
        /// <para>Getting the value:</para>
        /// <para>((EnumHelper.EnumDropdownValue)comboBox1.SelectedItem).Value</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<WinformHelper.ComboboxNameValuePairs> EnumAsCombobox<T>(string currentSelectedValue)
        {
            var d = GetValues<T>().ToList();
            var values = d.Select(val => new WinformHelper.ComboboxNameValuePairs { IntValue = (int)(Enum.Parse(typeof(T), val.ToString())), DisplayValue = val.GetDescription() }).ToList();
            values.ForEach(p => p.ShouldBeSelected(currentSelectedValue));
            return values;
        }

        /// <summary>
        /// returns the description attribute of an enum
        /// </summary>
        /// <param name="myObject"></param>
        /// <returns></returns>
        public static string GetDescription(this object myObject)
        {
            try
            {
                MemberInfo[] memInfo = myObject.GetType().GetMember(myObject.ToString());
                if (memInfo.Length > 0)
                {
                    object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                    if (attrs.Length > 0)
                        return ((DescriptionAttribute)attrs[0]).Description;
                }
                return myObject.ToString();
            }
            catch
            {
                return myObject.ToString();
            }
        }


        private static void SetSelectedValueInComboBox(ComboBox cmb, int valueToSet)
        {
            for (int i = 0; i < cmb.Items.Count; i++)
            {
                if (((WinformHelper.ComboboxNameValuePairs)cmb.Items[i]).IntValue == valueToSet)
                {
                    cmb.SelectedIndex = i;
                    break;
                }
            }
        }

        /// <summary>
        /// Takes an Enum with description attribute and returns the descriptions as a list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<string> DescriptionsToList<T>()
        {
            var d = GetValues<T>().ToList();
            return d.Select(val => val.GetDescription()).ToList();
        }

        private static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static void AddEnumValues<T>(this ComboBox comboBox, Func<T, bool> predicate = null) where T : struct
        {
            if (predicate == null)
            {
                predicate = x => true;
            }

            comboBox.Items.AddRange(GetValues<T>()
                .Where(predicate)
                .Select(value => new WinformHelper.ComboboxNameValuePairs
                {
                    IntValue = (int)(object)value,
                    DisplayValue = value.GetDescription()
                })
                .OfType<object>()
                .ToArray());
        }

        public static void SetSelectedEnumValue<T>(this ComboBox comboBox, T value) where T : struct
        {
            int intValue = (int)(object)value;

            comboBox.SelectedItem = comboBox.Items
                .OfType<WinformHelper.ComboboxNameValuePairs>()
                .FirstOrDefault(x => x.IntValue == intValue);
        }

        public static bool TryGetSelectedEnumValue<T>(this ComboBox comboBox, out T value) where T : struct
        {
            var nameValuePairs = comboBox.SelectedItem as WinformHelper.ComboboxNameValuePairs;
            if (nameValuePairs == null)
            {
                value = default(T);
                return false;
            }

            value = (T)(object)nameValuePairs.IntValue;
            return true;
        }

        public static T GetSelectedEnumValue<T>(this ComboBox comboBox, T defaultValue) where T : struct
        {
            T value;
            return TryGetSelectedEnumValue(comboBox, out value) ? value : defaultValue;
        }
    }
}