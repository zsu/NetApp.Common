using System;
using System.Text;
using System.IO;
using System.Configuration;
using System.Reflection;
using System.Web;
using System.Globalization;
using System.ComponentModel;

namespace NetApp.Common
{
    public static class Util
    {
        #region Fields
        /// <summary>
        /// Generate random digit code
        /// </summary>
        /// <param name="length">Length</param>
        /// <returns>Result string</returns>
        public static string GenerateRandomDigitCode(int length)
        {
            var random = new Random();
            string str = string.Empty;
            for (int i = 0; i < length; i++)
                str = String.Concat(str, random.Next(10).ToString());
            return str;
        }
        public static string MakeValidFileName(string name)
        {
            string fileName = name;
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }
            return fileName;
        }
        public static T ConvertTo<T>(object value)
        {
            //return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            return (T)ConvertTo(value, typeof(T));
        }
        public static object ConvertTo(object value, Type destinationType)
        {
            return ConvertTo(value, destinationType, CultureInfo.InvariantCulture);
        }
        public static object ConvertTo(object value, Type destinationType, CultureInfo culture)
        {
            if (value != null)
            {
                Type dstType = Nullable.GetUnderlyingType(destinationType) ?? destinationType;

                var sourceType = Nullable.GetUnderlyingType(value.GetType())??value.GetType();

                TypeConverter destinationConverter = TypeDescriptor.GetConverter(dstType);
                TypeConverter sourceConverter = TypeDescriptor.GetConverter(sourceType);
                if (destinationConverter != null && destinationConverter.CanConvertFrom(value.GetType()))
                    return destinationConverter.ConvertFrom(null, culture, value);
                if (sourceConverter != null && sourceConverter.CanConvertTo(destinationType))
                    return sourceConverter.ConvertTo(null, culture, value, destinationType);
                if (destinationType.IsEnum && value is int)
                    return Enum.ToObject(destinationType, (int)value);
                if (!destinationType.IsAssignableFrom(value.GetType()))
                    return Convert.ChangeType(value, destinationType, culture);
            }
            return value;
        }

        #endregion Methods
    }

    public static class SystemTime
    {
        public static Func<DateTime> Now = () => DateTime.UtcNow;
    }
}
