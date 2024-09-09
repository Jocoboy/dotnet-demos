using ABPDemo.Attributes;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ABPDemo.Web.Filters.StringTrim
{
    public class StringTrimmer
    {
        public static readonly ConcurrentDictionary<int, Action<object>> Cache = new();

        public static void TrimObject(object dto)
        {
            if (dto != null)
            {
                var type = dto.GetType();
                var hashCode = type.GetHashCode();
                var action = Cache.GetOrAdd(hashCode, Generate(type));
                action(dto);
            }
        }

        public static T TrimValue<T>(T dto)
        {
            if (dto != null)
            {
                var type = dto.GetType();
                var genericType = typeof(StringTirmObject<>).MakeGenericType(type);
                var instance = Activator.CreateInstance(genericType, dto);
                TrimObject(instance);
                return (T)instance.GetType().GetProperty(nameof(StringTirmObject<T>.Object)).GetValue(instance);
            }
            return dto;
        }

        static Action<object> Generate(Type type)
        {
            var param = Expression.Parameter(typeof(object), "obj");
            var dto = Expression.Variable(type, "dto");
            var dtoAssign = Expression.Assign(dto, Expression.Convert(param, type));
            var list = new List<Expression>() { dtoAssign };
            var properties = type.GetProperties()
                .Where(x => !x.GetIndexParameters().Any() && x.HasAttribute<StringTrimAttribute>());
            foreach (var prop in properties)
            {
                var property = Expression.Property(dto, prop.Name);
                var properType = prop.PropertyType;

                if (properType.IsStringType())
                {
                    list.Add(GenerateStringExpression(property));   // 字符串
                }
                else if (properType.IsArrayType())
                {
                    list.Add(GenerateArrayExpression(property));    // 数组
                }
                else if (properType.IsDictionaryType())
                {
                    list.Add(GenerateDictionaryExpression(property));    // 字典
                }
                else if (properType.IsCollectionType())
                {
                    list.Add(GenerateCollectionExpression(property));   // 集合
                }
                else if (properType.IsClass)
                {
                    list.Add(GenerateClassExpression(property));    // 对象
                }
            }
            var block = Expression.Block(new[] { dto }, list);
            var action = Expression.Lambda<Action<object>>(block, param).Compile();
            return action;
        }

        static Expression GenerateStringExpression(MemberExpression property)
        {
            var callTrim = Expression.Call(property, typeof(string).GetMethod("Trim", Type.EmptyTypes)!);
            var condition = Expression.Condition(Expression.Equal(property, Expression.Constant(null)), property, callTrim);
            var propertyAssign = Expression.Assign(property, condition);
            return propertyAssign;
        }

        static Expression GenerateClassExpression(MemberExpression property)
        {
            var callTrim = Expression.Call(typeof(StringTrimmer).GetMethod(nameof(StringTrimmer.TrimObject))!, property);
            var valueCondition = Expression.NotEqual(property, Expression.Constant(null));
            var valueIfThen = Expression.IfThen(valueCondition, callTrim);
            return valueIfThen;
        }

        static Expression GenerateArrayExpression(MemberExpression property)
        {
            var length = Expression.ArrayLength(property);
            var index = Expression.Variable(typeof(int));
            var label = Expression.Label();
            var loopCondition = Expression.LessThan(index, length);
            var element = Expression.ArrayAccess(property, index);
            var elementType = property.Type.GetElementType()!;

            var callTrim = elementType.IsStringType()
                ? Expression.Call(element, typeof(string).GetMethod("Trim", Type.EmptyTypes)!)
                : Expression.Call(typeof(StringTrimmer).GetMethod(nameof(StringTrimmer.TrimObject))!, element);

            var elementAssign = elementType.IsStringType()
                ? Expression.Assign(element, callTrim)
                : Expression.Assign(element, element);

            var indexAssign = Expression.Assign(index, Expression.Increment(index));
            var iterationBlock = Expression.Block(
                new Expression[] { callTrim, elementAssign, indexAssign }
            );
            var labelBreak = Expression.Break(label);
            var loopThen = Expression.IfThenElse(loopCondition, iterationBlock, labelBreak);
            var loop = Expression.Loop(loopThen);

            var valueCondition = Expression.NotEqual(property, Expression.Constant(null));
            var valueIfThen = Expression.IfThen(valueCondition, loop);
            return Expression.Block(new[] { index }, valueIfThen, Expression.Label(label));
        }

        static Expression GenerateCollectionExpression(MemberExpression property)
        {
            var genericType = property.Type.GenericTypeArguments[0];
            var collection = Expression.Variable(property.Type, "collection");
            var collectionAssign = Expression.Assign(collection, Expression.New(property.Type));
            var enumrator = Expression.Variable(typeof(IEnumerator), "enumrator");
            var getEnumrator = Expression.Call(property, typeof(IEnumerable).GetMethod("GetEnumerator")!);
            var enumratorAssign = Expression.Assign(enumrator, getEnumrator);
            var label = Expression.Label();
            var movenext = Expression.Call(enumrator, typeof(IEnumerator).GetMethod("MoveNext")!);
            var current = Expression.Variable(genericType, "current");
            var currentInitAssign = Expression.Assign(current, Expression.Convert(Expression.Property(enumrator, "Current"), genericType));

            var callTrim = genericType.IsStringType()
                ? Expression.Call(current, typeof(string).GetMethod("Trim", Type.EmptyTypes)!)
                : Expression.Call(typeof(StringTrimmer).GetMethod(nameof(StringTrimmer.TrimObject))!, current);
            var currentAssign = Expression.Assign(current, genericType.IsStringType() ? callTrim : current);

            var callAdd = Expression.Call(collection, property.Type.GetMethod("Add")!, current);
            var propertyAssign = Expression.Assign(property, collection);
            var ifelse = Expression.IfThenElse(movenext,
                Expression.Block(new[] { current }, currentInitAssign, currentAssign, callAdd),
                Expression.Break(label)
            );

            var loop = Expression.Loop(ifelse, label);

            var valueCondition = Expression.NotEqual(property, Expression.Constant(null));
            var valueIfThen = Expression.IfThen(valueCondition, Expression.Block(new[] { collection, enumrator }, collectionAssign, enumratorAssign, loop, propertyAssign));
            return valueIfThen;
        }

        static Expression GenerateDictionaryExpression(MemberExpression property)
        {
            var keyType = property.Type.GenericTypeArguments[0];
            var valueType = property.Type.GenericTypeArguments[1];
            var keyValuePairType = typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType);
            var dictionary = Expression.Variable(property.Type, "dictionary");
            var dictionaryAssign = Expression.Assign(dictionary, Expression.New(property.Type));
            var enumrator = Expression.Variable(typeof(IEnumerator), "enumrator");
            var getEnumrator = Expression.Call(property, typeof(IEnumerable).GetMethod("GetEnumerator")!);
            var enumratorAssign = Expression.Assign(enumrator, getEnumrator);

            var label = Expression.Label();
            var movenext = Expression.Call(enumrator, typeof(IEnumerator).GetMethod("MoveNext")!);
            var current = Expression.Variable(keyValuePairType, "current");
            var currentAssign = Expression.Assign(current, Expression.Convert(Expression.Property(enumrator, "Current"), keyValuePairType));

            var key = Expression.Variable(keyType, "Key");
            var value = Expression.Variable(valueType, "Value");
            var keyInitAssign = Expression.Assign(key, Expression.Property(current, "Key"));
            var valueInitAssign = Expression.Assign(value, Expression.Property(current, "Value"));

            var callKeyTrim = keyType.IsStringType()
                ? Expression.Call(key, typeof(string).GetMethod("Trim", Type.EmptyTypes)!)
                : Expression.Call(typeof(StringTrimmer).GetMethod(nameof(StringTrimmer.TrimObject))!, key);
            var callValueTrim = valueType.IsStringType()
                ? Expression.Call(value, typeof(string).GetMethod("Trim", Type.EmptyTypes)!)
                : Expression.Call(typeof(StringTrimmer).GetMethod(nameof(StringTrimmer.TrimObject))!, value);

            var keyAssign = Expression.Assign(key, keyType.IsStringType() ? callKeyTrim : key);
            var valueAssign = Expression.Assign(value, keyType.IsStringType() ? callValueTrim : value);

            var callAdd = Expression.Call(dictionary, property.Type.GetMethod("Add")!, key, value);
            var addCondition = Expression.NotEqual(Expression.Call(dictionary, property.Type.GetMethod("ContainsKey")!, key), Expression.Constant(true));
            var addIfThen = Expression.IfThen(addCondition, callAdd);
            var propertyAssign = Expression.Assign(property, dictionary);
            var ifelse = Expression.IfThenElse(movenext,
                Expression.Block(new[] { current, key, value }, currentAssign, keyInitAssign, valueInitAssign, keyAssign, valueAssign, addIfThen),
                Expression.Break(label)
            );
            var loop = Expression.Loop(ifelse, label);

            var valueCondition = Expression.NotEqual(property, Expression.Constant(null));
            var valueIfThen = Expression.IfThen(valueCondition, Expression.Block(new[] { dictionary, enumrator }, dictionaryAssign, enumratorAssign, loop, propertyAssign));
            return valueIfThen;
        }

        public class StringTirmObject<T>
        {
            public StringTirmObject(T @object)
            {
                Object = @object;
            }

            [StringTrim]
            public T Object { get; set; }
        }
    }
}
