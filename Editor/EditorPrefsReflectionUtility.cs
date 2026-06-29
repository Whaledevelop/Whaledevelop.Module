#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Whaledevelop
{
    public static class EditorPrefsReflectionUtility
    {
        public static void Load(object target)
        {
            var type = target.GetType();
            foreach (var field in GetPersistedFields(type))
            {
                var key = GetEditorPrefsKey(type, field);
                var defaultValue = field.GetValue(target);
                var loadedValue = LoadEditorPrefsValue(key, defaultValue, field.FieldType);
                field.SetValue(target, loadedValue);
            }
        }

        public static void Save(object target)
        {
            var type = target.GetType();
            foreach (var field in GetPersistedFields(type))
            {
                var key = GetEditorPrefsKey(type, field);
                var value = field.GetValue(target);
                SaveEditorPrefsValue(key, value, field.FieldType);
            }
        }

        private static IEnumerable<FieldInfo> GetPersistedFields(Type type)
        {
            return type
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(field => field.IsPublic || field.IsDefined(typeof(SerializeField), false));
        }

        private static string GetEditorPrefsKey(Type type, FieldInfo field)
        {
            return type.FullName + field.Name;
        }

        private static object LoadEditorPrefsValue(string key, object defaultValue, Type fieldType)
        {
            if (fieldType == typeof(string))
            {
                return EditorPrefs.GetString(key, defaultValue as string ?? string.Empty);
            }

            if (fieldType == typeof(int))
            {
                return EditorPrefs.GetInt(key, defaultValue is int defaultInt ? defaultInt : default);
            }

            if (fieldType == typeof(float))
            {
                return EditorPrefs.GetFloat(key, defaultValue is float defaultFloat ? defaultFloat : default);
            }

            if (fieldType == typeof(bool))
            {
                return EditorPrefs.GetBool(key, defaultValue is bool defaultBool && defaultBool);
            }

            if (fieldType.IsEnum)
            {
                var storedValue = EditorPrefs.GetString(key, defaultValue?.ToString() ?? string.Empty);
                if (string.IsNullOrEmpty(storedValue))
                {
                    return defaultValue;
                }

                return Enum.Parse(fieldType, storedValue);
            }

            return defaultValue;
        }

        private static void SaveEditorPrefsValue(string key, object value, Type fieldType)
        {
            if (fieldType == typeof(string))
            {
                EditorPrefs.SetString(key, value as string ?? string.Empty);

                return;
            }

            if (fieldType == typeof(int))
            {
                EditorPrefs.SetInt(key, value is int intValue ? intValue : default);

                return;
            }

            if (fieldType == typeof(float))
            {
                EditorPrefs.SetFloat(key, value is float floatValue ? floatValue : default);

                return;
            }

            if (fieldType == typeof(bool))
            {
                EditorPrefs.SetBool(key, value is bool boolValue && boolValue);

                return;
            }

            if (fieldType.IsEnum)
            {
                EditorPrefs.SetString(key, value?.ToString() ?? string.Empty);

                return;
            }
        }
    }
}
#endif
