using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using SFML.Graphics;

namespace BlackCoat.Tools
{
    /// <summary>
    /// Provides conversions between <see cref="string"/> values and <see cref="BlendMode"/> values.
    /// </summary>
    /// <seealso cref="System.ComponentModel.ExpandableObjectConverter" />
    public class BlendmodeConverter : ExpandableObjectConverter
    {
        // STATICS #########################################################################
        private static Dictionary<BlendMode, String> _MODES_BY_VALUE;
        private static Dictionary<String, BlendMode> _MODES_BY_NAME;

        /// <summary>
        /// Initializes the <see cref="BlendmodeConverter"/> class.
        /// </summary>
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


        // Methods #########################################################################
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
            return destinationType == typeof(BlendMode) || base.CanConvertTo(context, destinationType);
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
            if (value is BlendMode blendMode)
            {
                if (_MODES_BY_VALUE.TryGetValue(blendMode, out string name)) return name;
                else return "Custom";
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
        /// <exception cref="System.ArgumentException">Can not convert '{str}' to {nameof(BlendMode)}</exception>
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