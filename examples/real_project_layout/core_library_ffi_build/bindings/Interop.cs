// Automatically generated by Interoptopus.

#pragma warning disable 0105
using System;
using System.Text;
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


        [LibraryImport(NativeLib, EntryPoint = "start_server")]
        public static partial void start_server([MarshalAs(UnmanagedType.LPStr)] string server_name);

        /// Destroys the given instance.
        ///
        /// # Safety
        ///
        /// The passed parameter MUST have been created with the corresponding init function;
        /// passing any other value results in undefined behavior.
        [LibraryImport(NativeLib, EntryPoint = "game_engine_destroy")]
        public static partial FFIError game_engine_destroy(ref IntPtr context);

        /// Destroys the given instance.
        ///
        /// # Safety
        ///
        /// The passed parameter MUST have been created with the corresponding init function;
        /// passing any other value results in undefined behavior.
        public static unsafe void game_engine_destroy_checked(ref IntPtr context)
        {
            var rval = game_engine_destroy(ref context);;
            if (rval != FFIError.Ok)
            {
                throw new InteropException<FFIError>(rval);
            }
        }

        [LibraryImport(NativeLib, EntryPoint = "game_engine_new")]
        public static partial FFIError game_engine_new(ref IntPtr context);

        public static unsafe void game_engine_new_checked(ref IntPtr context)
        {
            var rval = game_engine_new(ref context);;
            if (rval != FFIError.Ok)
            {
                throw new InteropException<FFIError>(rval);
            }
        }

        [LibraryImport(NativeLib, EntryPoint = "game_engine_place_object")]
        public static partial FFIError game_engine_place_object(IntPtr context, [MarshalAs(UnmanagedType.LPStr)] string name, Vec2 position);

        public static unsafe void game_engine_place_object_checked(IntPtr context, [MarshalAs(UnmanagedType.LPStr)] string name, Vec2 position)
        {
            var rval = game_engine_place_object(context, name, position);;
            if (rval != FFIError.Ok)
            {
                throw new InteropException<FFIError>(rval);
            }
        }

        [LibraryImport(NativeLib, EntryPoint = "game_engine_num_objects")]
        public static partial uint game_engine_num_objects(IntPtr context);

    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public partial struct Vec2
    {
        public float x;
        public float y;
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
            var rval = Interop.game_engine_new(ref i._context);
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

}
