using System;
using System.Diagnostics.CodeAnalysis;
using CaseExtensions;

namespace artiso.AdsdHotel.ITOps.NoSql
{
    public static class Extensions
    {
        public static bool HasData([NotNullWhen(true)] this string? s) =>
            !string.IsNullOrWhiteSpace(s);
            
        public static string NameForCollection(this Type type)
        {
            var entityTypeName = type.Name;
            var entityTypeFormattedName = entityTypeName.ToKebabCase();
            return entityTypeFormattedName;
        }
    }
}
