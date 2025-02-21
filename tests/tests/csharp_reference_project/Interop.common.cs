// Automatically generated by Interoptopus.

// Debug - write_imports 
#pragma warning disable 0105
using System;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Runtime.CompilerServices;
using My.Company;
using My.Company.Common;
#pragma warning restore 0105

// Debug - write_namespace_context 
namespace My.Company.Common
{

    // Debug - write_type_definition_composite 
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public partial struct Vec
    {
        public double x;
        public double z;
    }
    // Debug - write_type_marshaller 

    // Debug - write_type_definition_fn_pointer 
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte InteropDelegate_fn_u8_rval_u8(byte x0);

    // Debug - write_type_definition_fn_pointer 
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void InteropDelegate_fn_CharArray(CharArray x0);
    delegate void InteropDelegate_fn_CharArray_native(CharArray x0);

    // Debug - write_type_definition_composite 
    ///Option type containing boolean flag and maybe valid data.
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public partial struct OptionVec
    {
        ///Element that is maybe valid.
        Vec t;
        ///Byte where `1` means element `t` is valid.
        byte is_some;
    }
    // Debug - write_type_marshaller 

    // Debug - write_pattern_option 
    public partial struct OptionVec
    {
        public static OptionVec FromNullable(Vec? nullable)
        {
            var result = new OptionVec();
            if (nullable.HasValue)
            {
                result.is_some = 1;
                result.t = nullable.Value;
            }

            return result;
        }

        public Vec? ToNullable()
        {
            return this.is_some == 1 ? this.t : (Vec?)null;
        }
    }


    // Debug - write_type_definition_ffibool 
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public partial struct Bool
    {
        byte value;
    }

    public partial struct Bool
    {
        public static readonly Bool True = new Bool { value =  1 };
        public static readonly Bool False = new Bool { value =  0 };
        public Bool(bool b)
        {
            value = (byte) (b ? 1 : 0);
        }
        public bool Is => value == 1;
    }


    // Debug - write_type_definition_named_callback 
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint MyCallbackNamespacedDelegate(uint value);
    public delegate uint MyCallbackNamespacedNative(uint value, IntPtr callback_data);

    [NativeMarshalling(typeof(MarshallerMeta))]
    public struct MyCallbackNamespaced : IDisposable
    {
        private MyCallbackNamespacedDelegate _callbackUser;
        private IntPtr _callbackNative;

        public MyCallbackNamespaced(MyCallbackNamespacedDelegate callbackUser)
        {
            _callbackUser = callbackUser;
            _callbackNative = Marshal.GetFunctionPointerForDelegate(Call);
        }

        public uint Call(uint value, IntPtr _)
        {
            return _callbackUser(value);
        }

        public void Dispose()
        {
            if (_callbackNative == IntPtr.Zero) return;
            Marshal.FreeHGlobal(_callbackNative);
            _callbackNative = IntPtr.Zero;
        }


        [CustomMarshaller(typeof(MyCallbackNamespaced), MarshalMode.Default, typeof(Marshaller))]
        private struct MarshallerMeta {  }

        [StructLayout(LayoutKind.Sequential)]
        public struct Unmanaged
        {
            internal IntPtr Callback;
            internal IntPtr Data;
        }


        public ref struct Marshaller
        {
            private MyCallbackNamespaced managed;
            private Unmanaged native;
            private Unmanaged sourceNative;
            private GCHandle? pinned;

            public void FromManaged(MyCallbackNamespaced managed)
            {
                this.managed = managed;
            }

            public Unmanaged ToUnmanaged()
            {
                return new Unmanaged
                {
                    Callback = managed._callbackNative,
                    Data = IntPtr.Zero
                };
            }

            public void FromUnmanaged(Unmanaged unmanaged)
            {
                sourceNative = unmanaged;
            }

            public MyCallbackNamespaced ToManaged()
            {
                return new MyCallbackNamespaced
                {
                    _callbackNative = sourceNative.Callback,
                };
            }

            public void Free() { }
        }
    }


    // Debug - write_pattern_generic_slice_helper 
    // This is a helper for the marshallers for Slice<T> and SliceMut<T> of Ts that require custom marshalling.
    // It is used to precompile the conversion logic for the custom marshaller.
    internal static class CustomMarshallerHelper<T> where T : struct
    {
        // Delegate to convert a managed T to its unmanaged representation at the given pointer.
        // Precompiling these conversions minimizes overhead during runtime marshalling.
        public static readonly Action<T, IntPtr> ToUnmanagedFunc;
        // Delegate that converts unmanaged data at a specified pointer back to a managed instance of T.
        public static readonly Func<IntPtr, T> ToManagedFunc;

        // Indicates whether type T is decorated with a NativeMarshallingAttribute.
        public static readonly bool HasCustomMarshaller;
        // Size of the unmanaged type in bytes. This is used for memory allocation.
        public static readonly int UnmanagedSize;
        // The unmanaged type that corresponds to T as defined by the custom marshaller.
        // This assumes that the custom marshaller has a nested type named 'Unmanaged'.
        public static readonly Type UnmanagedType;

        // This runs once per type T, ensuring that the conversion logic is set up only once.
        static CustomMarshallerHelper()
        {
            var nativeMarshalling = typeof(T).GetCustomAttribute<NativeMarshallingAttribute>();
            if (nativeMarshalling != null)
            {
                var marshallerType = nativeMarshalling.NativeType;
                var convertToUnmanaged = marshallerType.GetMethod("ConvertToUnmanaged", BindingFlags.Public | BindingFlags.Static);
                var convertToManaged = marshallerType.GetMethod("ConvertToManaged", BindingFlags.Public | BindingFlags.Static);
                UnmanagedType = marshallerType.GetNestedType("Unmanaged")!;
                UnmanagedSize = Marshal.SizeOf(UnmanagedType);

                // If the stateless custom marshaller shape is not available we currently do not support marshalling T in a slice.
                if (convertToUnmanaged == null || convertToManaged == null)
                {
                    ToUnmanagedFunc = Expression.Lambda<Action<T, IntPtr>>(Expression.Throw(Expression.New(typeof(NotSupportedException))), Expression.Parameter(typeof(T)), Expression.Parameter(typeof(IntPtr))).Compile();
                    ToManagedFunc = Expression.Lambda<Func<IntPtr, T>>(Expression.Throw(Expression.New(typeof(NotSupportedException)), typeof(T)), Expression.Parameter(typeof(IntPtr))).Compile();
                }
                else
                {
                    var unsafeRead = typeof(CustomMarshallerHelper<T>).GetMethod(nameof(ReadPointer), BindingFlags.NonPublic | BindingFlags.Static)!.MakeGenericMethod(UnmanagedType)!;
                    var parameter = Expression.Parameter(typeof(IntPtr));
                    var unsafeCall = Expression.Call(unsafeRead, parameter);
                    var callExpression = Expression.Call(convertToManaged, unsafeCall);
                    ToManagedFunc = Expression.Lambda<Func<IntPtr, T>>(callExpression, parameter).Compile();

                     var unsafeWrite = typeof(CustomMarshallerHelper<T>).GetMethod(nameof(WritePointer), BindingFlags.NonPublic | BindingFlags.Static)!.MakeGenericMethod(UnmanagedType)!;
                    var managedParameter = Expression.Parameter(typeof(T));
                    var destParameter = Expression.Parameter(typeof(IntPtr));
                    var toUnmanagedCall = Expression.Call(convertToUnmanaged, managedParameter);
                    var unsafeWriteCall = Expression.Call(unsafeWrite, toUnmanagedCall, destParameter);
                    ToUnmanagedFunc = Expression.Lambda<Action<T, IntPtr>>(unsafeWriteCall, managedParameter, destParameter).Compile();
                }

                HasCustomMarshaller = true;
            }
            else
            {
                UnmanagedType = typeof(T);
                ToUnmanagedFunc = Expression.Lambda<Action<T, IntPtr>>(Expression.Throw(Expression.New(typeof(InvalidOperationException))), Expression.Parameter(typeof(T)), Expression.Parameter(typeof(IntPtr))).Compile();
                ToManagedFunc = Expression.Lambda<Func<IntPtr, T>>(Expression.Throw(Expression.New(typeof(InvalidOperationException)), typeof(T)), Expression.Parameter(typeof(IntPtr))).Compile();
                HasCustomMarshaller = false;
            }
        }

        // This exists to simplify the creation of the expression tree.
        private static void WritePointer<TUnmanaged>(TUnmanaged unmanaged, IntPtr dest)
        {
            unsafe { Unsafe.Write((void*)dest, unmanaged); }
        }

        // This exists to simplify the creation of the expression tree.
        private static TUnmanaged ReadPointer<TUnmanaged>(IntPtr ptr)
        {
             unsafe { return Unsafe.Read<TUnmanaged>((void*)ptr); }
        }
    }

    // Debug - write_pattern_read_only_span_marshaller 
    // Debug - write_pattern_generic_slice 
    [NativeMarshalling(typeof(SliceMarshaller<>))]
    public readonly partial struct Slice<T> : IEnumerable<T> where T : struct
    {
        internal readonly T[]? Managed;
        internal readonly IntPtr Data;
        internal readonly ulong Len;

        public int Count => Managed?.Length ?? (int)Len;

        public unsafe ReadOnlySpan<T> ReadOnlySpan
        {
            get
            {
                if (Managed is not null)
                {
                    return new ReadOnlySpan<T>(Managed);
                }
                return new ReadOnlySpan<T>(Data.ToPointer(), (int)Len);
            }
        }

        public unsafe T this[int i]
        {
            get
            {
                if (i >= Count) throw new IndexOutOfRangeException();
                if (Managed is not null)
                {
                    return Managed[i];
                }
                return Unsafe.Read<T>((void*)IntPtr.Add(Data, i * Unsafe.SizeOf<T>()));
            }
        }

        public Slice(GCHandle handle, ulong count)
        {
            this.Data = handle.AddrOfPinnedObject();
            this.Len = count;
        }

        public Slice(IntPtr handle, ulong count)
        {
            this.Data = handle;
            this.Len = count;
        }

        public Slice(T[] managed)
        {
            this.Managed = managed;
            this.Data = IntPtr.Zero;
            this.Len = 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < Count; ++i)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    // Debug - write_pattern_generic_slice_marshaller 
    [CustomMarshaller(typeof(Slice<>), MarshalMode.Default, typeof(SliceMarshaller<>.Marshaller))]
    internal static class SliceMarshaller<T> where T: struct
    {
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct Unmanaged
        {
            public IntPtr Data;
            public ulong Len;
        }

        public ref struct Marshaller
        {
            private Slice<T> managed;
            private Unmanaged native;
            private Unmanaged sourceNative;
            private GCHandle? pinned;
            private T[] marshalled;

            public void FromManaged(Slice<T> managed)
            {
                this.managed = managed;
            }

            public unsafe Unmanaged ToUnmanaged()
            {
                if(managed.Count == 0)
                {
                    return default;
                }

                if (CustomMarshallerHelper<T>.HasCustomMarshaller)
                {
                    var count = managed.Count;
                    var size = CustomMarshallerHelper<T>.UnmanagedSize;
                    native.Len = (ulong)count;
                    native.Data = Marshal.AllocHGlobal(count * size);
                    for (var i = 0; i < count; i++)
                    {
                        CustomMarshallerHelper<T>.ToUnmanagedFunc!( managed[i], IntPtr.Add(native.Data, i * size));
                    }
                    return native;
                }
                else if(managed.Managed is not null)
                {
                    pinned = GCHandle.Alloc(managed.Managed, GCHandleType.Pinned);
                    return new Unmanaged
                    {
                        Data = pinned.Value.AddrOfPinnedObject(),
                        Len = (ulong)managed.Count
                    };
                }
                else
                {
                    return new Unmanaged
                    {
                        Data = (IntPtr)managed.Data,
                        Len = (ulong)managed.Len
                    };
                }
            }

            public void FromUnmanaged(Unmanaged unmanaged)
            {
                sourceNative = unmanaged;
            }

            public unsafe Slice<T> ToManaged()
            {
                if (CustomMarshallerHelper<T>.HasCustomMarshaller)
                {
                    var count = (int)sourceNative.Len;
                    var size = CustomMarshallerHelper<T>.UnmanagedSize;
                    marshalled = new T[count];
                    for (var i = 0; i < count; i++)
                    {
                        marshalled[i] = CustomMarshallerHelper<T>.ToManagedFunc!(IntPtr.Add(sourceNative.Data, i * size));
                    }
                    return new Slice<T>(marshalled);
                }
                else
                {
                    return new Slice<T>(sourceNative.Data, sourceNative.Len);
                }
            }

            public void Free()
            {
                if (native.Data != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(native.Data);
                }
                pinned?.Free();
            }
        }
    }

    // Debug - write_pattern_span_marshaller 
    // Debug - write_pattern_generic_slice 
    [NativeMarshalling(typeof(SliceMutMarshaller<>))]
    public readonly partial struct SliceMut<T> : IEnumerable<T> where T : struct
    {
        internal readonly T[]? Managed;
        internal readonly IntPtr Data;
        internal readonly ulong Len;

        public int Count => Managed?.Length ?? (int)Len;

        public unsafe ReadOnlySpan<T> ReadOnlySpan
        {
            get
            {
                if (Managed is not null)
                {
                    return new ReadOnlySpan<T>(Managed);
                }
                return new ReadOnlySpan<T>(Data.ToPointer(), (int)Len);
            }
        }

        public unsafe Span<T> Span
        {
            get
            {
                if (Managed is not null)
                {
                    return new Span<T>(Managed);
                }
                return new Span<T>(Data.ToPointer(), (int)Len);
            }
        }

        public unsafe T this[int i]
        {
            get
            {
                if (i >= Count) throw new IndexOutOfRangeException();
                if (Managed is not null)
                {
                    return Managed[i];
                }
                return Unsafe.Read<T>((void*)IntPtr.Add(Data, i * Unsafe.SizeOf<T>()));
            }
            set
            {
                if (i >= Count) throw new IndexOutOfRangeException();
                if (Managed is not null)
                {
                    Managed[i] = value;
                }
                else
                {
                    Unsafe.Write((void*)IntPtr.Add(Data, i * Unsafe.SizeOf<T>()), value);
                }
            }
        }

        public SliceMut(GCHandle handle, ulong count)
        {
            this.Data = handle.AddrOfPinnedObject();
            this.Len = count;
        }

        public SliceMut(IntPtr handle, ulong count)
        {
            this.Data = handle;
            this.Len = count;
        }

        public SliceMut(T[] managed)
        {
            this.Managed = managed;
            this.Data = IntPtr.Zero;
            this.Len = 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < Count; ++i)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    // Debug - write_pattern_generic_slice_marshaller 
    [CustomMarshaller(typeof(SliceMut<>), MarshalMode.Default, typeof(SliceMutMarshaller<>.Marshaller))]
    internal static class SliceMutMarshaller<T> where T: struct
    {
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct Unmanaged
        {
            public IntPtr Data;
            public ulong Len;
        }

        public ref struct Marshaller
        {
            private SliceMut<T> managed;
            private Unmanaged native;
            private Unmanaged sourceNative;
            private GCHandle? pinned;
            private T[] marshalled;

            public void FromManaged(SliceMut<T> managed)
            {
                this.managed = managed;
            }

            public unsafe Unmanaged ToUnmanaged()
            {
                if(managed.Count == 0)
                {
                    return default;
                }

                if (CustomMarshallerHelper<T>.HasCustomMarshaller)
                {
                    var count = managed.Count;
                    var size = CustomMarshallerHelper<T>.UnmanagedSize;
                    native.Len = (ulong)count;
                    native.Data = Marshal.AllocHGlobal(count * size);
                    for (var i = 0; i < count; i++)
                    {
                        CustomMarshallerHelper<T>.ToUnmanagedFunc!( managed[i], IntPtr.Add(native.Data, i * size));
                    }
                    return native;
                }
                else if(managed.Managed is not null)
                {
                    pinned = GCHandle.Alloc(managed.Managed, GCHandleType.Pinned);
                    return new Unmanaged
                    {
                        Data = pinned.Value.AddrOfPinnedObject(),
                        Len = (ulong)managed.Count
                    };
                }
                else
                {
                    return new Unmanaged
                    {
                        Data = (IntPtr)managed.Data,
                        Len = (ulong)managed.Len
                    };
                }
            }

            public void FromUnmanaged(Unmanaged unmanaged)
            {
                sourceNative = unmanaged;
            }

            public unsafe SliceMut<T> ToManaged()
            {
                if (CustomMarshallerHelper<T>.HasCustomMarshaller)
                {
                    var count = (int)sourceNative.Len;
                    var size = CustomMarshallerHelper<T>.UnmanagedSize;
                    marshalled = new T[count];
                    for (var i = 0; i < count; i++)
                    {
                        marshalled[i] = CustomMarshallerHelper<T>.ToManagedFunc!(IntPtr.Add(sourceNative.Data, i * size));
                    }
                    return new SliceMut<T>(marshalled);
                }
                else
                {
                    return new SliceMut<T>(sourceNative.Data, sourceNative.Len);
                }
            }

            public void Free()
            {
                if (native.Data != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(native.Data);
                }
                pinned?.Free();
            }

            public void OnInvoked()
            {
                if (CustomMarshallerHelper<T>.HasCustomMarshaller)
                {
                    if (marshalled is not null)
                    {
                        var count = marshalled.Length;
                        var size = CustomMarshallerHelper<T>.UnmanagedSize;
                        for (var i = 0; i < count; i++)
                        {
                            CustomMarshallerHelper<T>.ToUnmanagedFunc!(marshalled[i], IntPtr.Add(sourceNative.Data, i * size));
                        }
                    }
                    else if (native.Data != IntPtr.Zero)
                    {
                        var count = managed.Count;
                        var size = CustomMarshallerHelper<T>.UnmanagedSize;
                        for (var i = 0; i < count; i++)
                        {
                            managed[i] = (T)CustomMarshallerHelper<T>.ToManagedFunc!(IntPtr.Add(native.Data, i * size));
                        }
                    }
                }
            }
        }
    }



    public class InteropException<T> : Exception
    {
        public T Error { get; private set; }

        public InteropException(T error): base($"Something went wrong: {error}")
        {
            Error = error;
        }
    }

}
