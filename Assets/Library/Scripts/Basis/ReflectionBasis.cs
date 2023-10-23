using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using GenericBasis;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace Basis
{
    public static class ReflectionFunc
    {
        //public static IEnumerable<MemberInfo> GetNewMembersOfClass(this Type derivedType, Type baseType, BindingFlags bindingAttr = BindingFlags.Default)
        //{
        //    Assert.IsTrue(baseType.IsSubclassOf(derivedType));

        //    var existedMemberNames = new HashSet<string>();

        //    existedMemberNames.AddRange(baseType.GetAllMembers().Select(memberInfo => memberInfo.Name));

        //    return derivedType.GetMe.Where(memberInfo => !existedMemberNames.Contains(memberInfo.Name));
        //}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ConvertTo<T>(this object value) => (T)Convert.ChangeType(value, typeof(T));

        #region Field

        #region GetByReturnType

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<FieldInfo> GetFieldsByReturnType(this Type type, Type returnType,
            BindingFlags bindingFlags =
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
        {
            return type.GetFields(bindingFlags).
                Where(fieldInfo => fieldInfo.FieldType.IsDerivedFrom(returnType, true));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldInfo GetFieldByReturnType(this Type type, Type returnType, BindingFlags bindingFlags =
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
        {
            return type.GetFieldsByReturnType(returnType, bindingFlags).FirstOrDefault();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasFieldByReturnType(this Type type, Type returnType, BindingFlags bindingFlags =
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
        {
            return type.GetFieldByReturnType(returnType, bindingFlags) != null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object GetFieldValueByReturnType(this Type type, Type returnType, object targetObject,
            BindingFlags bindingFlags =
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
        {
            var field = type.GetFieldByReturnType(returnType, bindingFlags);

            return field?.GetValue(targetObject);
        }

        #endregion

        #region GetByName

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldInfo GetFieldByName(this Type type, string fieldName, BindingFlags bindingFlags =
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
        {
            return type.GetField(fieldName, bindingFlags);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetFieldValueByName<T>(this object obj, string fieldName, BindingFlags bindingFlags =
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
        {
            T result = default;
            var field = obj.GetType().GetFieldByName(fieldName, bindingFlags);
            if (field != null)
            {
                result = (T)field.GetValue(obj);
            }
            return result;
        }

        public static bool HasFieldByName(this Type type, string fieldName,
            BindingFlags bindingFlags =
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
        {
            return type.GetField(fieldName, bindingFlags) != null;
        }

        #endregion

        #region SetByName

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetFieldValueByName(this object obj, string fieldName, object value)
        {
            var fieldInfo = obj.GetType().GetFieldByName(fieldName);

            if (fieldInfo != null)
            {
                fieldInfo.SetValue(obj, value);
            }
            else
            {
                Note.note.Warning($"{obj.GetType()}不存在名为{fieldName}的字段");
            }
        }

        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<(FieldInfo, object)> GetAllFieldValues(this object obj,
            BindingFlags bindingFlags = 
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
        {
            foreach (var fieldInfo in obj.GetType().GetFields(bindingFlags))
            {
                var value = fieldInfo.GetValue(obj);
                yield return (fieldInfo, value);
            }
        }

        #endregion

        #region Property

        #region Static

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<PropertyInfo> GetStaticProperties(this Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        } 

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<PropertyInfo> GetAllStaticProperties(this Type type, bool includingSelf = true)
        {
            foreach (var t in type.GetBaseTypes(includingSelf, false, false))
            {
                foreach (var propertyInfo in t.GetStaticProperties())
                {
                    yield return propertyInfo;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<PropertyInfo> GetAllStaticPropertiesByReturnType(this Type type, Type returnType)
        {
            return GetAllStaticProperties(type)
                .Where(propertyInfo => propertyInfo.PropertyType.IsDerivedFrom(returnType, true));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<object> GetAllStaticPropertyValuesByReturnType(this Type type, Type returnType)
        {
            return GetAllStaticPropertiesByReturnType(type, returnType).
                Select(targetType => targetType.GetValue(null));
        }

        #endregion

        #region GetByReturnType

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<PropertyInfo> GetPropertiesByReturnType(this Type type, Type returnType,
            BindingFlags bindingFlags =
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
        {
            var result = 
                type.GetProperties(bindingFlags).
                    Where(propertyInfo => propertyInfo.PropertyType.IsDerivedFrom(returnType, true));
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PropertyInfo GetPropertyByReturnType(this Type type, Type returnType,
            BindingFlags bindingFlags =
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
        {
            return type.GetPropertiesByReturnType(returnType, bindingFlags).FirstOrDefault();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasPropertyByReturnType(this Type type, Type returnType,
            BindingFlags bindingFlags =
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
        {
            return type.GetPropertyByReturnType(returnType, bindingFlags) != null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object GetPropertyValueByReturnType(this Type type, Type returnType, object targetObject,
            BindingFlags bindingFlags =
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
        {
            var property = type.GetPropertyByReturnType(returnType, bindingFlags);

            return property?.GetValue(targetObject);
        }

        #endregion

        #region GetByName

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PropertyInfo GetPropertyByName(this Type type, string name, BindingFlags bindingFlags =
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
        {
            return type.GetProperty(name, bindingFlags);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetPropertyValueByName<T>(this object obj, string propertyName, BindingFlags bindingFlags =
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
        {
            T result = default;
            var property = obj.GetType().GetPropertyByName(propertyName, bindingFlags);
            if (property != null)
            {
                result = (T)property.GetValue(obj);
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasPropertyByName(this Type type, string name, BindingFlags bindingFlags =
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
        {
            return type.GetProperty(name, bindingFlags) != null;
        }

        #endregion

        #region GetByAttribute

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasPropertyByAttribute<TAttribute>(this Type type, BindingFlags bindingFlags =
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
            where TAttribute : Attribute
        {
            return type.GetPropertyByAttribute<TAttribute>(bindingFlags).Any();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<PropertyInfo> GetPropertyByAttribute<TAttribute>(this Type type, 
            BindingFlags bindingFlags =
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
            where TAttribute : Attribute
        {
            return type.GetProperties(bindingFlags).Where(methodInfo => methodInfo.HasAttribute<TAttribute>());
        }

        #endregion

        /// <summary>
        /// 获取对象所有的Property的值，但不包含父对象的Static Property
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<(PropertyInfo, object)> GetAllPropertyValues(this object obj,
            BindingFlags bindingFlags =
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
        {
            foreach (var propertyInfo in obj.GetType().GetProperties(bindingFlags))
            {
                var value = propertyInfo.GetValue(obj);
                yield return (propertyInfo, value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsVirtual(this PropertyInfo propertyInfo)
        {
            var getMethod = propertyInfo?.GetMethod;
            if (getMethod == null)
            {
                return false;
            } 

            return getMethod.IsVirtual;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOverride(this PropertyInfo propertyInfo)
        {
            var baseType = propertyInfo.DeclaringType?.BaseType;
            if (baseType == null)
            {
                return false;
            }

            if (propertyInfo.IsVirtual() == false)
            {
                return false;
            }

            var baseProperty = baseType.GetProperty(propertyInfo.Name,
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

            return !baseProperty.IsVirtual();
        }

        #endregion

        #region Enum

        public static string GetEnumLabel<T>(this T enumValue) where T : Enum
        {
            string label = enumValue.ToString();
            FieldInfo field = enumValue.GetType().GetField(enumValue.ToString());
            if (field != null)
            {
                LabelTextAttribute labelTextAttribute = field.GetCustomAttribute<LabelTextAttribute>();
                if (labelTextAttribute != null)
                {
                    label = labelTextAttribute.Text;
                }
            }
            return label;
        }

        #endregion

        #region Method

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetMethodValueByName<T>(this object obj, string methodName, params object[] parameters)
        {
            T result = default;
            MethodInfo method = obj.GetType().GetMethod(methodName);
            if (method != null)
            {
                result = (T)method.Invoke(obj, parameters);
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasMethodByName(this Type type, string methodName)
        {
            return type.GetMethods().Any(methodInfo => methodInfo.Name == methodName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasMethodByAttribute<TAttribute>(this Type type) where TAttribute : Attribute
        {
            return type.GetMethods().Any(methodInfo => methodInfo.HasAttribute<TAttribute>());
        }

        public static void InvokeMethod(this object obj, string methodName, params object[] parameters)
        {
            MethodInfo method = obj.GetType().GetMethod(methodName, 
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            if (method != null)
            {
                method.Invoke(obj, parameters);
            }
            else
            {
                Note.note.Warning($"没从{obj}找到方法：{methodName}");
            }
        }

        #endregion

        #region Attribute

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TAttribute GetAttribute<TAttribute>(this MemberInfo member) where TAttribute : Attribute
        {
            return member.GetCustomAttributes<TAttribute>().FirstOrDefault();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAttribute<TAttribute>(this MemberInfo member) where TAttribute : Attribute
        {
            return member.GetAttribute<TAttribute>() != null;
        }

        #endregion

        #region Assembly

        public static Assembly GetAssembly(this string assemblyName)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            return assemblies.FirstOrDefault(assembly => assembly.GetName().Name == assemblyName);
        }

        public static Assembly GetAssemblyStrictly(this string assemblyName)
        {
            var result = assemblyName.GetAssembly();

            if (result == null)
            {
                Note.note.Error($"未找到Assembly:{assemblyName}");
            }

            return result;
        }

        #endregion

        #region CreateInstance

        /// <summary>
        /// 创建一个实例
        /// </summary>
        /// <returns>值类型返回default，如果是class就返回一个new()</returns>
        /// <exception cref="InvalidOperationException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue CreateInstance<TValue>()
        {
            return (TValue)CreateInstance(typeof(TValue));
        }

        /// <summary>
        /// 创建一个实例
        /// </summary>
        /// <param name="type"></param>
        /// <returns>值类型返回default，如果是class就返回一个new()</returns>
        /// <exception cref="InvalidOperationException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object CreateInstance(this Type type)
        {
            if (type.IsValueType)
            {
                return default;
            }

            if (type.IsGenericTypeDefinition)
            {
                throw new InvalidOperationException($"Type {type.FullName} is a generic type definition.");
            }

            if (type.GetConstructor(Type.EmptyTypes) != null)
            {
                return Activator.CreateInstance(type);
            }

            if (type == typeof(string))
            {
                return string.Empty;
            }

            throw new InvalidOperationException($"Type {type.FullName}does not have a parameterless constructor.");
        }

        #endregion

        #region GetCopy

        public static T ConvertToChildOrParent<T>(this T origin, Type targetType) where T : class
        {
            Note.note.AssertIsNotNull(targetType, nameof(targetType));
            Note.note.AssertIsClass(targetType);

            if (typeof(T).IsAssignableFrom(targetType) == false && targetType.IsAssignableFrom(typeof(T)) == false)
            {
                Note.note.Warning($"{typeof(T)}不是{targetType}或其子类或父类");
                return null;
            }

            var newInstance = targetType.CreateInstance();

            foreach (var fieldInfo in newInstance.GetType().
                         GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
            {
                var originFieldInfo = origin.GetType().GetField(fieldInfo.Name,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

                if (originFieldInfo == null)
                {
                    continue;
                }

                var originValue = fieldInfo.GetValue(origin);
                var copiedValue = Clone(originValue);
                fieldInfo.SetValue(newInstance, copiedValue);
            }

            return (T)newInstance;
        }

        public static List<T> Clone<T>(this List<T> list)
        {
            return (List<T>)Clone(list as ICollection<T>);
        }

        public static ICollection<T> Clone<T>(this ICollection<T> collection)
        {
            var newCollection = (ICollection<T>)collection.GetType().CreateInstance();

            foreach (var item in collection)
            {
                newCollection.Add(item.Clone<T>());
            }

            return newCollection;
        }

        public static T Clone<T>(this T origin)
        {
            return (T)Clone((object)origin);
        }

        public static object Clone(this object origin) 
        {
            if (origin == null)
            {
                return null;
            }

            if (origin is Object)
            {
                return origin;
            }

            if (origin is ICloneable cloneable)
            {
                return cloneable.Clone();
            }

            if (origin is IConvertible)
            {
                return origin;
            }

            var originType = origin.GetType();

            if (origin is ICollection originCollection)
            {

                if (originType.IsGenericType)
                {
                    var elementType = originType.GetGenericArguments()[0];

                    var newCollection = originType.CreateInstance();

                    foreach (var item in originCollection)
                    {
                        newCollection.InvokeMethod("Add", item.Clone());
                    }
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }

            if (originType.IsNumber() || originType.IsVector())
            {
                return origin;
            }

            var newInstance = originType.CreateInstance();

            foreach (var (fieldInfo, value) in origin.
                         GetAllFieldValues(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (fieldInfo.FieldType.IsClass && value is not Object)
                {
                    var copiedValue = Clone(value);
                    fieldInfo.SetValue(newInstance, copiedValue);
                }
                else
                {
                    try
                    {
                        fieldInfo.SetValue(newInstance, value);
                    }
                    catch (Exception e)
                    {
                        Debug.Log($"newInstance:{newInstance.GetType()},{newInstance}");
                        Debug.Log($"value:{value.GetType()},{value}");
                        Debug.LogError(e);
                        throw;
                    }

                }
            }

            return newInstance;
        }

        [Obsolete]
        public static T GetCopy<T>(this T origin, bool deep = false)
        {
            return (T)GetCopy((object)origin, deep);
        }

        /// <summary>
        /// 拷贝，会忽略拷贝所有继承自UnityEngine.Object的类，即便是深拷贝
        /// </summary>
        /// <param name="origin">来源</param>
        /// <param name="deep">是否是深拷贝</param>
        /// <returns></returns>
        [Obsolete]
        public static object GetCopy(this object origin, bool deep = false)
        {
            if (origin is Object)
            {
                throw new ArgumentException("不能拷贝来自UnityEngine.Object的对象");
            }

            var newInstance = origin.GetType().CreateInstance();

            if (origin is ICollection collection)
            {
                foreach (var item in collection)
                {
                    newInstance.InvokeMethod("Add", item.GetCopy());
                }

                return newInstance;
            }

            foreach (var (fieldInfo, value) in origin.
                         GetAllFieldValues(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (deep && fieldInfo.FieldType.IsClass && value is not Object)
                {
                    var copiedValue = GetCopy(value, true);
                    fieldInfo.SetValue(newInstance, copiedValue);
                }
                else
                {
                    try
                    {
                        fieldInfo.SetValue(newInstance, value);
                    }
                    catch (Exception e)
                    {
                        Debug.Log($"newInstance:{newInstance.GetType()},{newInstance}");
                        Debug.Log($"value:{value.GetType()},{value}");
                        Debug.LogError(e);
                        throw;
                    }
                    
                }
            }

            return newInstance;
        }

        #endregion

        #region DerivedClasses

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Type> FindDerivedClasses(this Type baseType, Assembly assembly, bool includingSelf,
            bool includingGeneric)
        {
            foreach (Type t in assembly.GetTypes())
            {
                if ((includingSelf || t != baseType) && (includingGeneric || t.IsGenericTypeDefinition == false) && 
                    baseType.IsAssignableFrom(t))
                {
                    yield return t;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Type> FindDerivedClasses(this Type baseType, bool includingSelf,
            bool includingGeneric)
        {
            return FindDerivedClasses(baseType, baseType.Assembly, includingSelf, includingGeneric);
        }

        #endregion

        #region IsDerivedFrom

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDerivedFrom(this Type derivedType, Type parentType, bool includingSelf,
            bool includingInterfaces, bool includingGeneric)
        {
            return derivedType.GetBaseTypes(includingSelf, includingInterfaces, includingGeneric)
                .Any(type => type == parentType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDerivedFrom(this Type derivedType, Type parentType, bool includingSelf)
        {
            return IsDerivedFrom(derivedType, parentType, includingSelf, false, false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDerivedFrom<TParent>(this Type derivedType, bool includingSelf = false)
        {
            return derivedType.IsDerivedFrom(typeof(TParent), includingSelf);
        }


        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static bool IsDerivedFrom(this Type derivedType, Type parentType, bool includingParentSelf)
        //{
        //    if (derivedType == null || parentType == null)
        //    {
        //        return false;
        //    }

        //    if (parentType.IsInterface)
        //    {
        //        foreach (var interfaceType in derivedType.GetInterfaces())
        //        {
        //            if (interfaceType == parentType)
        //            {
        //                return true;
        //            }

        //            if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == parentType)
        //            {
        //                return true;
        //            }

        //            if (IsDerivedFrom(interfaceType, parentType, true))
        //            {
        //                return true;
        //            }
        //        }
        //    }

        //    if (derivedType.IsInterface)
        //    {
        //        return false;
        //    }

        //    if (includingParentSelf)
        //    {
        //        if (derivedType == parentType)
        //        {
        //            return true;
        //        }

        //        if (derivedType.IsGenericType && derivedType.GetGenericTypeDefinition() == parentType)
        //        {
        //            return true;
        //        }
        //    }

        //    return IsDerivedFrom(derivedType.BaseType, parentType, true);
        //}

        #endregion

        #region TypeParents

        public static IEnumerable<Type> GetBaseTypes(this Type type, bool includingSelf, bool includingInterfaces,
            bool includingGeneric)
        {
            if (type == null)
            {
                yield break;
            }

            IEnumerable<Type> GetParents(Type derivedType)
            {
                if (derivedType.BaseType != null)
                {
                    yield return derivedType.BaseType;

                    if (includingGeneric && derivedType.BaseType.IsGenericType)
                    {
                        yield return derivedType.BaseType.GetGenericTypeDefinition();
                    }
                }

                if (includingInterfaces)
                {
                    foreach (var interfaceType in derivedType.GetInterfaces())
                    {
                        yield return interfaceType;

                        if (includingGeneric && interfaceType.IsGenericType)
                        {
                            yield return interfaceType.GetGenericTypeDefinition();
                        }
                    }
                }
            }

            if (includingSelf && includingGeneric && type.IsGenericType)
            {
                yield return type.GetGenericTypeDefinition();
            }

            foreach (var parentType in type.GetAllChildren(includingSelf, GetParents))
            {
                yield return parentType;
            }
        }

        #endregion
    }

}
