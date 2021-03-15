using System.ComponentModel;
using System.Reflection;
using System.Collections.Generic;
using System;

namespace LTL.Common
{
    /// <summary>
    /// Extension utility methods for Enum related operations
    /// TODO: Add unit test coverage
    /// </summary>
    public static class EnumExtensions
    {
        public static bool TryGetEnumValueFromDescription<T>(string strategy, out T enumValue)
        {
            var isValid = false;
            try
            {
                enumValue = GetEnumValueFromDescription<T>(strategy);
                isValid = true;
            }
            catch
            {
                isValid = false;
                enumValue = default;
            }

            return isValid;
        }

        public static T GetEnumValueFromDescription<T>(string description)
        {
            MemberInfo[] fis = typeof(T).GetFields();

            foreach (var fi in fis)
            {
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes != null && attributes.Length > 0 && attributes[0].Description == description)
                    return (T)Enum.Parse(typeof(T), fi.Name);
            }

            throw new KeyNotFoundException($"{description} Not found in enum collection {typeof(T)}");
        }

        public static IEnumerable<string> GetEnumDescriptions(Type type)
        {
            var descriptions = new List<string>();
            var names = Enum.GetNames(type);
            foreach (var name in names)
            {
                var field = type.GetField(name);
                var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), true);
                foreach (DescriptionAttribute fd in attributes)
                {
                    descriptions.Add(fd.Description);
                }
            }
            return descriptions;
        }

        public static string UserFriendlyDescription(this Enum value)
        {
            // Get the Description attribute value for the enum value
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static string GetEnumName(this Enum value)
        {
            return Enum.GetName(value.GetType(), value);
        }
    }
}