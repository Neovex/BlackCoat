using System;
using System.ComponentModel;
using System.Globalization;
using SFML.System;

namespace BlackCoat.Tools
{
    public class Vector2Converter<T> : ExpandableObjectConverter where T : struct
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(T) || base.CanConvertTo(context, destinationType);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            switch (value)
            {
                case Vector2f f: return $"{f.X}{Constants.SEPERATOR}{f.Y}";
                case Vector2i i: return $"{i.X}{Constants.SEPERATOR}{i.Y}";
                case Vector2u u: return $"{u.X}{Constants.SEPERATOR}{u.Y}";
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
                    if (typeof(T) == typeof(Vector2f)) return new Vector2f(float.Parse(parts[0]), float.Parse(parts[1]));
                    else
                    if (typeof(T) == typeof(Vector2i)) return new Vector2i(int.Parse(parts[0]), int.Parse(parts[1]));
                    else
                    if (typeof(T) == typeof(Vector2u)) return new Vector2u(uint.Parse(parts[0]), uint.Parse(parts[1]));
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