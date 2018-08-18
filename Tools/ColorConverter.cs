using System;
using System.ComponentModel;
using System.Globalization;
using SFML.Graphics;

namespace BlackCoat.Tools
{
    public class ColorConverter : ExpandableObjectConverter
    {
        private const Char _SEPERATOR = ';';

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(Color) || base.CanConvertTo(context, destinationType);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if(value is Color color) return $"{color.R}{_SEPERATOR}{color.G}{_SEPERATOR}{color.B}{_SEPERATOR}{color.A}";
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
                try
                {
                    var parts = str.Split(_SEPERATOR);
                    return new Color(byte.Parse(parts[0]), byte.Parse(parts[1]), byte.Parse(parts[2]), byte.Parse(parts[3]));
                }
                catch (Exception e)
                {
                    throw new ArgumentException($"Can not convert '{str}' to {nameof(Color)}", e);
                }
            }
            else return base.ConvertFrom(context, culture, value);
        }
    }
}