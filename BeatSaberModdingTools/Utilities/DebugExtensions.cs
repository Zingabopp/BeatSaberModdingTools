using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace BeatSaberModdingTools.Utilities
{
    public static class DebugExtensions
    {
        public static string[] GetObjectProperties(this object obj)
        {
            Type type = obj?.GetType();

            if (DispatchUtility.ImplementsIDispatch(obj))
            {
                type = DispatchUtility.GetType(obj, false);
            }

            return type?.GetProperties().Select(p =>
            {
                string name = p.Name;
                string value = null;
                try
                {
                    value = p.GetValue(obj)?.ToString() ?? "NULL";
                }
                catch (Exception ex)
                {
                    value = $"ERROR: {ex.Message}";
                }
                return $"{p.DeclaringType.Name}.{name}: {value}";
            }).ToArray() ?? Array.Empty<string>();
        }

        public static string[] GetObjectMethods(this object obj)
        {
            Type type = obj?.GetType();

            if (DispatchUtility.ImplementsIDispatch(obj))
            {
                type = DispatchUtility.GetType(obj, false);
            }

            return type?.GetMethods().Select(m =>
            {
                string parameters = string.Join(", ", m.GetParameters().Select(p => p.ParameterType.Name));
                return $"{m.DeclaringType.Name}.{m.Name}({parameters})";
            }).ToArray();
        }

        public static Type[] GetObjectInterfaces(this object obj)
        {
            if (DispatchUtility.ImplementsIDispatch(obj))
            {
                return DispatchUtility.GetType(obj, false)?.GetInterfaces();
            }
            return obj?.GetType().GetInterfaces();
        }
    }

    /// <summary>
    /// Better Reflection with COM objects.
    /// From: https://stackoverflow.com/a/14208030
    /// </summary>
    public static class DispatchUtility
    {
        private const int S_OK = 0; //From WinError.h
        private const int LOCALE_SYSTEM_DEFAULT = 2 << 10; //From WinNT.h == 2048 == 0x800

        public static bool ImplementsIDispatch(object obj)
        {
            bool result = obj is IDispatchInfo;
            return result;
        }

        public static Type GetType(object obj, bool throwIfNotFound)
        {
            RequireReference(obj, "obj");
            Type result = GetType((IDispatchInfo)obj, throwIfNotFound);
            return result;
        }

        public static bool TryGetDispId(object obj, string name, out int dispId)
        {
            RequireReference(obj, "obj");
            bool result = TryGetDispId((IDispatchInfo)obj, name, out dispId);
            return result;
        }

        public static object Invoke(object obj, int dispId, object[] args)
        {
            string memberName = "[DispId=" + dispId + "]";
            object result = Invoke(obj, memberName, args);
            return result;
        }

        public static object Invoke(object obj, string memberName, object[] args)
        {
            RequireReference(obj, "obj");
            Type type = obj.GetType();
            object result = type.InvokeMember(memberName,
                BindingFlags.InvokeMethod | BindingFlags.GetProperty,
                null, obj, args, null);
            return result;
        }

        private static void RequireReference<T>(T value, string name) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        private static Type GetType(IDispatchInfo dispatch, bool throwIfNotFound)
        {
            RequireReference(dispatch, "dispatch");

            Type result = null;
            int typeInfoCount;
            int hr = dispatch.GetTypeInfoCount(out typeInfoCount);
            if (hr == S_OK && typeInfoCount > 0)
            {
                dispatch.GetTypeInfo(0, LOCALE_SYSTEM_DEFAULT, out result);
            }

            if (result == null && throwIfNotFound)
            {
                // If the GetTypeInfoCount called failed, throw an exception for that.
                Marshal.ThrowExceptionForHR(hr);

                // Otherwise, throw the same exception that Type.GetType would throw.
                throw new TypeLoadException();
            }

            return result;
        }

        private static bool TryGetDispId(IDispatchInfo dispatch, string name, out int dispId)
        {
            RequireReference(dispatch, "dispatch");
            RequireReference(name, "name");

            bool result = false;

            Guid iidNull = Guid.Empty;
            int hr = dispatch.GetDispId(ref iidNull, ref name, 1, LOCALE_SYSTEM_DEFAULT, out dispId);

            const int DISP_E_UNKNOWNNAME = unchecked((int)0x80020006); //From WinError.h
            const int DISPID_UNKNOWN = -1; //From OAIdl.idl
            if (hr == S_OK)
            {
                result = true;
            }
            else if (hr == DISP_E_UNKNOWNNAME && dispId == DISPID_UNKNOWN)
            {
                result = false;
            }
            else
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            return result;
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("00020400-0000-0000-C000-000000000046")]
        private interface IDispatchInfo
        {
            [PreserveSig]
            int GetTypeInfoCount(out int typeInfoCount);

            void GetTypeInfo(int typeInfoIndex, int lcid, [MarshalAs(UnmanagedType.CustomMarshaler,
            MarshalTypeRef = typeof(System.Runtime.InteropServices.CustomMarshalers.TypeToTypeInfoMarshaler))] out Type typeInfo);

            [PreserveSig]
            int GetDispId(ref Guid riid, ref string name, int nameCount, int lcid, out int dispId);

            // NOTE: The real IDispatch also has an Invoke method next, but we don't need it.
        }
    }
}
