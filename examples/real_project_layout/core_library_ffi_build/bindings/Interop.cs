// Automatically generated by Interoptopus.

#pragma warning disable 0105
using System;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq.Expressions;
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
        public const string NativeLib = "core_library";

        static Interop()
        {
        }


        [LibraryImport(NativeLib, EntryPoint = "start_server")]
        public static partial void start_server([MarshalAs(UnmanagedType.LPStr)] string server_name);


        /// Destroys the given instance.
        ///
        /// # Safety
        ///
        /// The passed parameter MUST have been created with the corresponding init function;
        /// passing any other value results in undefined behavior.
        [LibraryImport(NativeLib, EntryPoint = "game_engine_destroy")]
        public static partial FFIError game_engine_destroy(ref IntPtr _context);


        [LibraryImport(NativeLib, EntryPoint = "game_engine_new")]
        public static partial FFIError game_engine_new(ref IntPtr _context);


        [LibraryImport(NativeLib, EntryPoint = "game_engine_place_object")]
        public static partial FFIError game_engine_place_object(IntPtr _context, [MarshalAs(UnmanagedType.LPStr)] string name, Vec2 position);


        [LibraryImport(NativeLib, EntryPoint = "game_engine_num_objects")]
        public static partial uint game_engine_num_objects(IntPtr _context);


    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public partial struct Vec2
    {
        public float x;
        public float y;
    }

    [NativeMarshalling(typeof(MarshallerMeta))]
    public partial struct Vec2
    {
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct Unmanaged
        {
            public float x;
            public float y;
        }

        [CustomMarshaller(typeof(Vec2), MarshalMode.Default, typeof(Marshaller))]
        private struct MarshallerMeta { }

        public ref struct Marshaller
        {
            private Vec2 _managed; // Used when converting managed -> unmanaged
            private Unmanaged _unmanaged; // Used when converting unmanaged -> managed

            public Marshaller(Vec2 managed) { _managed = managed; }
            public Marshaller(Unmanaged unmanaged) { _unmanaged = unmanaged; }

            public void FromManaged(Vec2 managed) { _managed = managed; }
            public void FromUnmanaged(Unmanaged unmanaged) { _unmanaged = unmanaged; }

            public unsafe Unmanaged ToUnmanaged()
            {;
                _unmanaged = new Unmanaged();

                _unmanaged.x = _managed.x;
                _unmanaged.y = _managed.y;

                return _unmanaged;
            }

            public unsafe Vec2 ToManaged()
            {
                _managed = new Vec2();

                _managed.x = _unmanaged.x;
                _managed.y = _unmanaged.y;

                return _managed;
            }
            public void Free() { }
        }
    }

    public enum FFIError
    {
        Ok = 0,
        Null = 100,
        Panic = 200,
        Delegate = 300,
        Fail = 400,
    }


    public partial class GameEngine : IDisposable
    {
        private IntPtr _context;

        private GameEngine() {}

        public static GameEngine New()
        {
            var self = new GameEngine();
            var rval = Interop.game_engine_new(ref self._context);
            if (rval != FFIError.Ok)
            {
                throw new InteropException<FFIError>(rval);
            }
            return self;
        }

        public void Dispose()
        {
            var rval = Interop.game_engine_destroy(ref _context);
            if (rval != FFIError.Ok)
            {
                throw new InteropException<FFIError>(rval);
            }
        }

        public void PlaceObject([MarshalAs(UnmanagedType.LPStr)] string name, Vec2 position)
        {
            var rval = Interop.game_engine_place_object(_context, name, position);
            if (rval != FFIError.Ok)
            {
                throw new InteropException<FFIError>(rval);
            }
        }

        public uint NumObjects()
        {
            return Interop.game_engine_num_objects(_context);
        }

        public IntPtr Context => _context;
    }



    public class InteropException<T> : Exception
    {
        public T Error { get; private set; }

        public InteropException(T error): base($"Something went wrong: {error}")
        {
            Error = error;
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

        public AsyncHelper(AsyncHelperDelegate managed)
        {
            _managed = managed;
            _native = Call;
            _ptr = Marshal.GetFunctionPointerForDelegate(_native);
        }

        void Call(IntPtr data, IntPtr _)
        {
            _managed(data);
        }

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

            public void FromManaged(AsyncHelper managed) { _managed = managed; }
            public void FromUnmanaged(Unmanaged unmanaged) { _unmanaged = unmanaged; }

            public Unmanaged ToUnmanaged()
            {
                _unmanaged = new Unmanaged();
                _unmanaged.Callback = _managed._ptr;
                _unmanaged.Data = IntPtr.Zero;
                return _unmanaged;
            }

            public AsyncHelper ToManaged()
            {
                _managed = new AsyncHelper();
                _managed._ptr = _unmanaged.Callback;
                return _managed;
            }

            public void Free() { }
        }
    }
    public partial struct Utf8String
    {
        string _s;
    }

    [NativeMarshalling(typeof(MarshallerMeta))]
    public partial struct Utf8String
    {
            public Utf8String(string s) { _s = s; }

            public string String { get { return _s; } }

        /// UTF-8 string marshalling helper.
        ///
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
        }

        public partial class InteropHelper
        {
            [LibraryImport(Interop.NativeLib, EntryPoint = "interoptopus_string_create")]
            public static partial long interoptopus_string_create(IntPtr utf8, ulong len, out Unmanaged rval);

            [LibraryImport(Interop.NativeLib, EntryPoint = "interoptopus_string_destroy")]
            public static partial long interoptopus_string_destroy(Unmanaged utf8);
        }

        [CustomMarshaller(typeof(Utf8String), MarshalMode.Default, typeof(Marshaller))]
        private struct MarshallerMeta { }

        public ref struct Marshaller
        {
            private Utf8String _managed; // Used when converting managed -> unmanaged
            private Unmanaged _unmanaged; // Used when converting unmanaged -> managed

            public Marshaller(Utf8String managed) { _managed = managed; }
            public Marshaller(Unmanaged unmanaged) { _unmanaged = unmanaged; }

            public void FromManaged(Utf8String managed) { _managed = managed; }
            public void FromUnmanaged(Unmanaged unmanaged) { _unmanaged = unmanaged; }

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

            public unsafe Utf8String ToManaged()
            {
                var span = new ReadOnlySpan<byte>((byte*)_unmanaged.ptr, (int)_unmanaged.len);

                _managed = new Utf8String();
                _managed._s = Encoding.UTF8.GetString(span);

                InteropHelper.interoptopus_string_destroy(_unmanaged);

                return _managed;
            }

            public void Free() { }
        }
    }

}
