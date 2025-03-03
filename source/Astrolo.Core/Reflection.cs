using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Astrolo.Core;

public static class Reflection
{
    public static IEnumerable<T>? GetAttributesOfType<T>(this Enum value) where T : Attribute
    {
        return value
            .GetType()
            .GetField(value.ToString())?
            .GetCustomAttributes<T>();
    }

    public static T? GetAttributeOfType<T>(this Enum value) where T : Attribute
    {
        return value
            .GetAttributesOfType<T>()?
            .FirstOrDefault();
    }

    public static string GetDescriptionOrDefault(this Enum value)
    {
        return value.GetAttributeOfType<DescriptionAttribute>()?.Description ?? value.ToString();
    }
}
