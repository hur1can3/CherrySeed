﻿using System;
using System.Linq.Expressions;
using System.Reflection;

namespace CherrySeed.Utils
{
    class ReflectionUtil
    {
        public static object GetPropertyValue(object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName).GetValue(obj, null);
        }

        public static bool ExistProperty(Type type, string propertyName)
        {
            return type.GetProperty(propertyName) != null;
        }

        public static void SetProperty(object obj, string propertyName, object propertyValue)
        {
            var type = obj.GetType();
            var prop = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

            if (prop == null || !prop.CanWrite)
            {
                throw new NullReferenceException("Property is missing");
            }

            try
            {
                prop.SetValue(obj, propertyValue, null);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Set property failed", ex);
            }
        }

        public static bool IsNullableValueType(Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        public static Type GetPropertyType(Type type, string propertyName)
        {
            var prop = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

            if (prop == null)
            {
                throw new NullReferenceException("Property is missing");
            }

            return prop.PropertyType;
        }

        public static string GetMemberName(LambdaExpression memberSelector)
        {
            var currentExpression = memberSelector.Body;

            while (true)
            {
                switch (currentExpression.NodeType)
                {
                    case ExpressionType.Parameter:
                        return ((ParameterExpression)currentExpression).Name;
                    case ExpressionType.MemberAccess:
                        return ((MemberExpression)currentExpression).Member.Name;
                    case ExpressionType.Call:
                        return ((MethodCallExpression)currentExpression).Method.Name;
                    case ExpressionType.Convert:
                    case ExpressionType.ConvertChecked:
                        currentExpression = ((UnaryExpression)currentExpression).Operand;
                        break;
                    case ExpressionType.Invoke:
                        currentExpression = ((InvocationExpression)currentExpression).Expression;
                        break;
                    case ExpressionType.ArrayLength:
                        return "Length";
                    default:
                        throw new Exception("not a proper member selector");
                }
            }
        }

        public static bool IsReferenceType(Type type)
        {
            return type != typeof(string) && type.IsClass;
        }
    }
}