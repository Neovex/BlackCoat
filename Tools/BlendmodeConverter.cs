using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using SFML.Graphics;

namespace BlackCoat.Tools
{
    public class BlendmodeConverter : ExpandableObjectConverter
    {
        private static Dictionary<BlendMode, String> _MODES_BY_VALUE;
        private static Dictionary<String, BlendMode> _MODES_BY_NAME;

        static BlendmodeConverter()
        {
            _MODES_BY_VALUE = new Dictionary<BlendMode, String>(4)
            {
                { BlendMode.Add, nameof(BlendMode.Add) },
                { BlendMode.None, nameof(BlendMode.None) },
                { BlendMode.Alpha, nameof(BlendMode.Alpha) },
                { BlendMode.Multiply, nameof(BlendMode.Multiply) },
            };
            _MODES_BY_NAME = _MODES_BY_VALUE.ToDictionary((i) => i.Value, (i) => i.Key);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(BlendMode) || base.CanConvertTo(context, destinationType);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is BlendMode blendMode)
            {
                if (_MODES_BY_VALUE.TryGetValue(blendMode, out string name)) return name;
                else return "Custom";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string str)
            {
                if (_MODES_BY_NAME.TryGetValue(str, out BlendMode mode)) return mode;
                else throw new ArgumentException($"Can not convert '{str}' to {nameof(BlendMode)}");
            }
            else return base.ConvertFrom(context, culture, value);
        }
    }
}