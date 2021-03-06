/**
 * Taken from http://blog.waynehartman.com/articles/84.aspx
 **/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace DanbooruDownloader3.Utils
{
    public static class EnumUtils
    {
        public static string StringValueOf(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return value.ToString();
            }
        }

        public static string StringValueOf(string name, Type enumType)
        {
            string[] names = Enum.GetNames(enumType);
            foreach (string name2 in names)
            {
                Console.WriteLine(name2+": "+name);
                if (name2.Equals(name))
                {
                    return StringValueOf((Enum)Enum.Parse(enumType, name));
                }
            }
            return name;
        }

        public static object EnumValueOf(string value, Type enumType)
        {
            string[] names = Enum.GetNames(enumType);
            foreach (string name in names)
            {
                if (StringValueOf((Enum)Enum.Parse(enumType, name)).Equals(value))
                {
                    return Enum.Parse(enumType, name);
                }
            }

            throw new ArgumentException("The string is not a description or value of the specified enum.");
        }
    }
}
