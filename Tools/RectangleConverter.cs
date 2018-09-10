using System;
using System.ComponentModel;
using System.Globalization;
using SFML.System;
using SFML.Graphics;

namespace BlackCoat.Tools
{
    public class RectangleConverter<T> : ExpandableObjectConverter where T : struct
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(T) || base.CanConvertTo(context, destinationType);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            switch (value)
            {
                case IntRect ir: return $"{ir.Left}{Constants.SEPERATOR}{ir.Top}{Constants.SEPERATOR}{ir.Width}{Constants.SEPERATOR}{ir.Height}";
                case FloatRect fr: return $"{fr.Left}{Constants.SEPERATOR}{fr.Top}{Constants.SEPERATOR}{fr.Width}{Constants.SEPERATOR}{fr.Height}";
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
                try
                {
                    var parts = str.Split(Constants.SEPERATOR);
                    if (typeof(T) == typeof(IntRect)) return new IntRect(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[3]));
                    else
                    if (typeof(T) == typeof(FloatRect)) return new FloatRect(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
                    else
                        return base.ConvertFrom(context, culture, value);
                }
                catch (Exception e)
                {
                    throw new ArgumentException($"Can not convert '{str}' to type {typeof(T).Name}", e);
                }
            }
            else return base.ConvertFrom(context, culture, value);
        }
    }
}