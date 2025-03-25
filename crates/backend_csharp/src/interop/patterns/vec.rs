use crate::Interop;
use crate::converter::to_typespecifier_in_param;
use interoptopus::backend::IndentWriter;
use interoptopus::pattern::vec::VecType;
use interoptopus::{Error, indented};

pub fn write_pattern_vec(i: &Interop, w: &mut IndentWriter, vec: &VecType) -> Result<(), Error> {
    i.debug(w, "write_pattern_vec")?;

    let name = vec.rust_name();
    let inner = to_typespecifier_in_param(vec.t());

    indented!(w, r"// This must be a class because we only ever want to hold on to the")?;
    indented!(w, r"// same instance, as we overwrite fields when this is sent over the FFI")?;
    indented!(w, r"// boundary")?;
    indented!(w, r"public partial class {}", name)?;
    indented!(w, r"{{")?;
    indented!(w, [()], r"internal IntPtr _ptr;")?;
    indented!(w, [()], r"internal ulong _len;")?;
    indented!(w, [()], r"internal ulong _capacity;")?;
    indented!(w, r"}}")?;
    w.newline()?;

    ////
    indented!(w, r"[NativeMarshalling(typeof(MarshallerMeta))]")?;
    indented!(w, r"public partial class {} : IDisposable", name)?;
    indented!(w, r"{{")?;
    w.indent();
    indented!(w, r"public int Count")?;
    indented!(w, r"{{")?;
    i.inline_hint(w, 1)?;
    indented!(w, [()], r"get {{ if (_ptr == IntPtr.Zero) {{ throw new InteropException(); }} else {{ return (int) _len; }} }}")?;
    indented!(w, r"}}")?;
    w.newline()?;
    indented!(w, r"public unsafe {} this[int i]", inner)?;
    indented!(w, r"{{")?;
    w.indent();
    i.inline_hint(w, 0)?;
    indented!(w, r"get")?;
    indented!(w, r"{{")?;
    indented!(w, [()], r"if (i >= Count) throw new IndexOutOfRangeException();")?;
    indented!(w, [()], r"if (_ptr == IntPtr.Zero) throw new InteropException();")?;
    indented!(w, [()], r"return Marshal.PtrToStructure<{}>(new IntPtr(_ptr.ToInt64() + i * sizeof({})));", inner, inner)?;
    indented!(w, r"}}")?;
    w.unindent();
    indented!(w, r"}}")?;
    w.newline()?;
    indented!(w, r"[CustomMarshaller(typeof({}), MarshalMode.Default, typeof(Marshaller))]", name)?;
    indented!(w, r"private struct MarshallerMeta {{ }}")?;
    w.newline()?;
    indented!(w, r"[StructLayout(LayoutKind.Sequential)]")?;
    indented!(w, r"public struct Unmanaged")?;
    indented!(w, r"{{")?;
    indented!(w, [()], r"internal IntPtr _ptr;")?;
    indented!(w, [()], r"internal ulong _len;")?;
    indented!(w, [()], r"internal ulong _capacity;")?;
    w.newline()?;
    indented!(w, r"}}")?;
    w.newline()?;
    indented!(w, r"public ref struct Marshaller")?;
    indented!(w, r"{{")?;
    w.indent();
    indented!(w, r"private {} _managed;", name)?;
    indented!(w, r"private Unmanaged _unmanaged;")?;
    w.newline()?;
    i.inline_hint(w, 0)?;
    indented!(w, r"public void FromManaged({} managed) {{ _managed = managed; }}", name)?;
    i.inline_hint(w, 0)?;
    indented!(w, r"public void FromUnmanaged(Unmanaged unmanaged) {{ _unmanaged = unmanaged; }}")?;
    w.newline()?;
    i.inline_hint(w, 0)?;
    indented!(w, r"public Unmanaged ToUnmanaged()")?;
    indented!(w, r"{{")?;
    indented!(w, [()], r"if (_managed._ptr == IntPtr.Zero) throw new InteropException(); // Don't use for serialization if moved already.")?;
    indented!(w, [()], r"_unmanaged = new Unmanaged();")?;
    indented!(w, [()], r"_unmanaged._len = _managed._len;")?;
    indented!(w, [()], r"_unmanaged._capacity = _managed._capacity;")?;
    indented!(w, [()], r"_unmanaged._ptr = _managed._ptr;")?;
    indented!(w, [()], r"_managed._ptr = IntPtr.Zero; // Mark this instance as moved.")?;
    indented!(w, [()], r"return _unmanaged;")?;
    indented!(w, r"}}")?;
    w.newline()?;
    i.inline_hint(w, 0)?;
    indented!(w, r"public unsafe {name} ToManaged()")?;
    indented!(w, r"{{")?;
    indented!(w, [()], r"_managed = new {name}();")?;
    indented!(w, [()], r"_managed._len = _unmanaged._len;")?;
    indented!(w, [()], r"_managed._capacity = _unmanaged._capacity;")?;
    indented!(w, [()], r"_managed._ptr = _unmanaged._ptr;")?;
    indented!(w, [()], r"return _managed;")?;
    indented!(w, r"}}")?;
    w.newline()?;
    i.inline_hint(w, 0)?;
    indented!(w, r"public void Free() {{ }}")?;
    w.unindent();
    indented!(w, r"}}")?;
    w.newline()?;
    i.inline_hint(w, 0)?;
    indented!(w, r"public void Dispose()")?;
    indented!(w, r"{{")?;
    indented!(w, [()], r"if (_ptr == IntPtr.Zero) return;")?;
    indented!(w, [()], r"Interop.interoptopus_vec_TODO_destroy(this);")?;
    indented!(w, r"}}")?;
    w.unindent();
    indented!(w, r"}}")?;
    Ok(())
}
