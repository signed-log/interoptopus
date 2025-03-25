// Automatically generated by Interoptopus.

#pragma warning disable 0105
using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Runtime.CompilerServices;
using My.Company;
#pragma warning restore 0105

namespace My.Company
{
    public static partial class Interop
    {
        public const string NativeLib = "library";

        static Interop()
        {
        }


        [LibraryImport(NativeLib, EntryPoint = "sample_function")]
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public static partial void sample_function(SliceU8 ignored);

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public static unsafe void sample_function(ReadOnlySpan<byte> ignored)
        {
            fixed (void* ptr_ignored = ignored)
            {
                var ignored_slice = new SliceU8(new IntPtr(ptr_ignored), (ulong) ignored.Length);
                try
                {
                    sample_function(ignored_slice);
                }
                finally
                {
                }
            }
        }

    }

    public partial struct SliceU8
    {
        GCHandle _handle;
        IntPtr _data;
        ulong _len;
    }

    [NativeMarshalling(typeof(MarshallerMeta))]
    public partial struct SliceU8 : IEnumerable<byte>, IDisposable
    {
        public int Count => (int) _len;

        public unsafe ReadOnlySpan<byte> ReadOnlySpan
        {
            [MethodImpl(MethodImplOptions.AggressiveOptimization)]
            get => new(_data.ToPointer(), (int)_len);
        }

        public unsafe byte this[int i]
        {
            [MethodImpl(MethodImplOptions.AggressiveOptimization)]
            get
            {
                if (i >= Count) throw new IndexOutOfRangeException();
                return Unsafe.Read<byte>((void*)IntPtr.Add(_data, i * Unsafe.SizeOf<byte>()));
            }

        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public SliceU8(IntPtr data, ulong len)
        {
            _data = data;
            _len = len;
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public SliceU8(byte[] managed)
        {
            _handle = GCHandle.Alloc(managed, GCHandleType.Pinned);
            _data = _handle.AddrOfPinnedObject();
            _len = (ulong) managed.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public IEnumerator<byte> GetEnumerator()
        {
            for (var i = 0; i < Count; ++i) { yield return this[i]; }
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public void Dispose()
        {
            if (_handle is { IsAllocated: true }) { _handle.Free(); }
        }

        [CustomMarshaller(typeof(SliceU8), MarshalMode.Default, typeof(Marshaller))]
        private struct MarshallerMeta { }

        [StructLayout(LayoutKind.Sequential)]
        public struct Unmanaged
        {
            public IntPtr Data;
            public ulong Len;

            [MethodImpl(MethodImplOptions.AggressiveOptimization)]
            public SliceU8 ToManaged()
            {
                return new SliceU8(Data, Len);
            }
        }

        public ref struct Marshaller
        {
            private SliceU8 _managed;
            private Unmanaged _unmanaged;

            [MethodImpl(MethodImplOptions.AggressiveOptimization)]
            public void FromManaged(SliceU8 managed) { _managed = managed; }
            [MethodImpl(MethodImplOptions.AggressiveOptimization)]
            public void FromUnmanaged(Unmanaged unmanaged) { _unmanaged = unmanaged; }

            [MethodImpl(MethodImplOptions.AggressiveOptimization)]
            public Unmanaged ToUnmanaged()
            {
                _unmanaged = new Unmanaged();
                _unmanaged.Data = _managed._data;
                _unmanaged.Len = _managed._len;
                return _unmanaged;
            }

            [MethodImpl(MethodImplOptions.AggressiveOptimization)]
            public unsafe SliceU8 ToManaged()
            {
                _managed = new SliceU8();
                _managed._data = _unmanaged.Data;
                _managed._len = _unmanaged.Len;
                return _managed;
            }

            [MethodImpl(MethodImplOptions.AggressiveOptimization)]
            public void Free() { }
        }
    }



    public class InteropException: Exception
    {

        public InteropException(): base()
        {
        }
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void AsyncHelperNative(IntPtr data, IntPtr callback_data);
    public delegate void AsyncHelperDelegate(IntPtr data);

    public partial struct AsyncHelper
    {
        private AsyncHelperDelegate _managed;
        private AsyncHelperNative _native;
        private IntPtr _ptr;
    }

    [NativeMarshalling(typeof(MarshallerMeta))]
    public partial struct AsyncHelper : IDisposable
    {
        public AsyncHelper() { }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public AsyncHelper(AsyncHelperDelegate managed)
        {
            _managed = managed;
            _native = Call;
            _ptr = Marshal.GetFunctionPointerForDelegate(_native);
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        void Call(IntPtr data, IntPtr _)
        {
            _managed(data);
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public void Dispose()
        {
            if (_ptr == IntPtr.Zero) return;
            Marshal.FreeHGlobal(_ptr);
            _ptr = IntPtr.Zero;
        }

        [CustomMarshaller(typeof(AsyncHelper), MarshalMode.Default, typeof(Marshaller))]
        private struct MarshallerMeta { }

        [StructLayout(LayoutKind.Sequential)]
        public struct Unmanaged
        {
            internal IntPtr Callback;
            internal IntPtr Data;
        }

        public ref struct Marshaller
        {
            private AsyncHelper _managed;
            private Unmanaged _unmanaged;

            [MethodImpl(MethodImplOptions.AggressiveOptimization)]
            public void FromManaged(AsyncHelper managed) { _managed = managed; }
            [MethodImpl(MethodImplOptions.AggressiveOptimization)]
            public void FromUnmanaged(Unmanaged unmanaged) { _unmanaged = unmanaged; }

            [MethodImpl(MethodImplOptions.AggressiveOptimization)]
            public Unmanaged ToUnmanaged()
            {
                _unmanaged = new Unmanaged();
                _unmanaged.Callback = _managed._ptr;
                _unmanaged.Data = IntPtr.Zero;
                return _unmanaged;
            }

            [MethodImpl(MethodImplOptions.AggressiveOptimization)]
            public AsyncHelper ToManaged()
            {
                _managed = new AsyncHelper();
                _managed._ptr = _unmanaged.Callback;
                return _managed;
            }

            [MethodImpl(MethodImplOptions.AggressiveOptimization)]
            public void Free() { }
        }
    }
    public partial struct Utf8String
    {
        string _s;
    }

    [NativeMarshalling(typeof(MarshallerMeta))]
    public partial struct Utf8String: IDisposable
    {
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public Utf8String(string s) { _s = s; }

        public string String => _s;

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public void Dispose() { }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public Unmanaged ToUnmanaged()
        {
            var marshaller = new Marshaller(this);
            try { return marshaller.ToUnmanaged(); }
            finally { marshaller.Free(); }
        }

        /// A highly dangerous 'use once type' that has ownership semantics!
        /// Once passed over an FFI boundary 'the other side' is meant to own
        /// (and free) it. Rust handles that fine, but if in C# you put this
        /// in a struct and then call Rust multiple times with that struct
        /// you'll free the same pointer multiple times, and get UB!
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct Unmanaged
        {
            public IntPtr ptr;
            public ulong len;
            public ulong capacity;

            [MethodImpl(MethodImplOptions.AggressiveOptimization)]
            public string ToManaged()
            {
                var marshaller = new Marshaller(this);
                try { return marshaller.ToManaged().String; }
                finally { marshaller.Free(); }
            }

        }

        public partial class InteropHelper
        {
            [LibraryImport(Interop.NativeLib, EntryPoint = "interoptopus_string_create")]
            [MethodImpl(MethodImplOptions.AggressiveOptimization)]
            public static partial long interoptopus_string_create(IntPtr utf8, ulong len, out Unmanaged rval);

            [LibraryImport(Interop.NativeLib, EntryPoint = "interoptopus_string_destroy")]
            [MethodImpl(MethodImplOptions.AggressiveOptimization)]
            public static partial long interoptopus_string_destroy(Unmanaged utf8);
        }

        [CustomMarshaller(typeof(Utf8String), MarshalMode.Default, typeof(Marshaller))]
        private struct MarshallerMeta { }

        public ref struct Marshaller
        {
            private Utf8String _managed; // Used when converting managed -> unmanaged
            private Unmanaged _unmanaged; // Used when converting unmanaged -> managed

            [MethodImpl(MethodImplOptions.AggressiveOptimization)]
            public Marshaller(Utf8String managed) { _managed = managed; }
            [MethodImpl(MethodImplOptions.AggressiveOptimization)]
            public Marshaller(Unmanaged unmanaged) { _unmanaged = unmanaged; }

            [MethodImpl(MethodImplOptions.AggressiveOptimization)]
            public void FromManaged(Utf8String managed) { _managed = managed; }
            [MethodImpl(MethodImplOptions.AggressiveOptimization)]
            public void FromUnmanaged(Unmanaged unmanaged) { _unmanaged = unmanaged; }

            [MethodImpl(MethodImplOptions.AggressiveOptimization)]
            public unsafe Unmanaged ToUnmanaged()
            {
                var utf8Bytes = Encoding.UTF8.GetBytes(_managed._s);
                var len = utf8Bytes.Length;

                fixed (byte* p = utf8Bytes)
                {
                    InteropHelper.interoptopus_string_create((IntPtr)p, (ulong)len, out var rval);
                    _unmanaged = rval;
                }

                return _unmanaged;
            }

            [MethodImpl(MethodImplOptions.AggressiveOptimization)]
            public unsafe Utf8String ToManaged()
            {
                var span = new ReadOnlySpan<byte>((byte*)_unmanaged.ptr, (int)_unmanaged.len);

                _managed = new Utf8String();
                _managed._s = Encoding.UTF8.GetString(span);

                InteropHelper.interoptopus_string_destroy(_unmanaged);

                return _managed;
            }

            [MethodImpl(MethodImplOptions.AggressiveOptimization)]
            public void Free() { }
        }
    }

}
