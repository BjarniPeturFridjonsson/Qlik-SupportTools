using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Gjallarhorn.Common
{
    public static class Extensions
    {
        /// <summary>
        /// Creates an array from parts of another array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr">The array</param>
        /// <param name="start">The first value to copy</param>
        /// <param name="length">End with this index, including it.</param>
        /// <returns></returns>
        public static T[] SubArray<T>(this T[] arr, int start, int length)
        {
            T[] result = new T[length];
            Array.Copy(arr, start, result, 0, length);
            return result;
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

        public static string SanitizeFileName(this string fileName, char replacementChar = '_')
        {
            var blackList = new HashSet<char>(System.IO.Path.GetInvalidFileNameChars());
            var output = fileName.ToCharArray();
            for (int i = 0, ln = output.Length; i < ln; i++)
            {
                if (blackList.Contains(output[i]))
                {
                    if (output[i] == '\\')
                        output[i] = '∖'; // faking the path location in filename
                    else
                        output[i] = replacementChar;
                }
            }
            return new String(output);
        }
    }
}
