using System;
using System.ComponentModel;
using System.Globalization;
using SFML.System;

namespace BlackCoat.Tools
{
    /// <summary>
    /// Provides conversions between <see cref="string"/> values and vectors of type <see cref="Vector2f"/>, <see cref="Vector2i"/> or <see cref="Vector2u"/>.
    /// </summary>
    /// <seealso cref="System.ComponentModel.ExpandableObjectConverter" />
    public class Vector2Converter<T> : ExpandableObjectConverter where T : struct
    {
        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="destinationType">A <see cref="T:System.Type" /> that represents the type you want to convert to.</param>
        /// <returns>
        ///   <see langword="true" /> if this converter can perform the conversion; otherwise, <see langword="false" />.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(T) || base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" />. If <see langword="null" /> is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert.</param>
        /// <param name="destinationType">The <see cref="T:System.Type" /> to convert the <paramref name="value" /> parameter to.</param>
        /// <returns>
        /// An <see cref="T:System.Object" /> that represents the converted value.
        /// </returns>
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

        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type" /> that represents the type you want to convert from.</param>
        /// <returns>
        ///   <see langword="true" /> if this converter can perform the conversion; otherwise, <see langword="false" />.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo" /> to use as the current culture.</param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert.</param>
        /// <returns>
        /// An <see cref="T:System.Object" /> that represents the converted value.
        /// </returns>
        /// <exception cref="System.ArgumentException">Can not convert '{str}' to type {typeof(T).Name}</exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string str)
            {
                try
                {
                    var parts = str.Split(Constants.SEPERATOR);
                    if (typeof(T) == typeof(Vector2f)) return new Vector2f(float.Parse(parts[0]), float.Parse(parts[1]));
                    else
                    if (typeof(T) == typeof(Vector2i)) return new Vector2i(int.Parse(parts[0]),  int.Parse(parts[1]));
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