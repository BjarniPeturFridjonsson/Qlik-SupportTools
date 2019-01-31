using System;
using System.ComponentModel;
using System.Reflection;

namespace Eir.Common.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            return value?
                       .GetType()
                       .GetTypeInfo()
                       .GetDeclaredField(value.ToString())
                       .GetCustomAttribute<DescriptionAttribute>()?.Description
                   ?? value?.ToString()
                   ?? "null";
        }

        public static string ToStorageString<TEnum>(this TEnum value) where TEnum : struct, IConvertible
        {
            return Enum.GetName(typeof(TEnum), value);
        }

        public static TEnum FromStorageString<TEnum>(this string name) where TEnum : struct, IConvertible
        {
            return (TEnum)Enum.Parse(typeof(TEnum), name);
        }
    }
}