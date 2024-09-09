using System.Reflection;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace ABPDemo.Web.Filters.StringTrim
{
    public static class ReflectionHelper
    {
        public static bool IsStringType(this Type type)
            => type == typeof(string);
        public static bool IsArrayType(this Type type)
            => type.IsArray;
        public static bool IsClassType(this Type type)
            => !type.IsPrimitive
               && type.IsClass
               && !type.IsStringType()
               && !typeof(IEnumerable).IsAssignableFrom(type);
        public static bool IsCollectionType(this Type type)
            => !type.IsPrimitive
            && (typeof(ICollection).IsAssignableFrom(type)
            || type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>)));
        public static bool IsDictionaryType(this Type type)
            => !type.IsPrimitive
            && typeof(IDictionary).IsAssignableFrom(type);

        public static bool HasAttribute<T>(this Type type) where T : Attribute
            => type.IsDefined(typeof(T), false);

        public static bool HasAttribute<T>(this PropertyInfo property) where T : Attribute
            => property.IsDefined(typeof(T), false);
    }
}
