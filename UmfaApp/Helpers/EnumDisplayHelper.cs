using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace UmfaApp.Helpers
{
    public static class EnumHelper
    {
        public static string GetEnumDisplayName(this Enum enumType)
        {
            return enumType!.GetType()!.GetMember(enumType.ToString()!)!
                           .First()!
                           .GetCustomAttribute<DisplayAttribute>()!
                           .Name!;
        }
    }
}
