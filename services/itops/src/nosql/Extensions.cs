using System;
using CaseExtensions;

namespace artiso.AdsdHotel.ITOps.NoSql
{
    public static class Extensions
    {
        public static string NameForCollection(this Type type)
        {
            var entityTypeName = type.Name;
            var entityTypeFormattedName = entityTypeName.ToKebabCase();
            return entityTypeFormattedName;
        }
    }
}
