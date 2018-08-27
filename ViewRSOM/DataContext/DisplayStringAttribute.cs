using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViewRSOM
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class DisplayStringAttribute : Attribute
    {
        private readonly string _value;
        public string Value
        {
            get { return _value; }
        }

        public string ResourceKey { get; set; }

        public DisplayStringAttribute(string value)
        {
            this._value = value;
        }

        public DisplayStringAttribute()
        {
        }
    }
}
