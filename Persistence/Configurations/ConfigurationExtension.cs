using System.Runtime.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public static class ConfigurationExtension
{
    public static PropertyBuilder<TProperty> HzHasEnumConversion<TProperty>(this PropertyBuilder<TProperty> builder)
    {
        return builder.HasConversion(p => p.ToEnumString(), p => p.ToEnum<TProperty>()).IsRequired();
    }
    
    public static string ToEnumString<T>(this T type)
    {
        Type enumType = typeof (T);
        string prettyText = ((IEnumerable<EnumMemberAttribute>) enumType.GetField(Enum.GetName(enumType, (object) type)).GetCustomAttributes(typeof (EnumMemberAttribute), true)).SingleOrDefault<EnumMemberAttribute>()?.Value;

        return prettyText;
    }
    
    public static T ToEnum<T>(this string str)
    {
        Type enumType = typeof (T);
        foreach (string name in Enum.GetNames(enumType))
        {
            if (((IEnumerable<EnumMemberAttribute>) enumType.GetField(name).GetCustomAttributes(typeof (EnumMemberAttribute), true)).Single<EnumMemberAttribute>().Value == str)
                return (T) Enum.Parse(enumType, name);
        }
        return default (T);
    }

}