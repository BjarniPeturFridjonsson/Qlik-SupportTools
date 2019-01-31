using System;

namespace SenseApiLibrary
{
    public class ApiQueryField
    {
        private readonly string _name;
        private readonly string _value;

        public ApiQueryField(string name, string value = null)
        {
            _name = name;
            _value = value;
        }

        public override string ToString() => _value == null ? _name : $"{_name}={_value}";

        public string GetEscapedString()
        {
            return _value == null
                ? Uri.EscapeDataString(_name)
                : Uri.EscapeDataString(_name) + "=" + Uri.EscapeDataString(_value);
        }
    }
}