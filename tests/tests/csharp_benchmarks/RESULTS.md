
# FFI Call Overheads

The numbers below are to help FFI design decisions by giving order-of-magnitude estimates how
expensive certain constructs are.

## Notes

- Times were determined by running the given construct 100k times, taking the elapsed time in ticks,
and computing the cost per 1k invocations.

- The time of the called function is included.

- However, the reference project was written so that each function is _minimal_, i.e., any similar
function you wrote, would have to at least as expensive operations if it were to do anything sensible with
the given type.

- The list is ad-hoc, PRs adding more tests to `Benchmark.cs` are welcome.

- Bindings were generated with the C# `use_unsafe` config, which dramatically (between 2x and 150x(!)) speeds
  up slice access and copies in .NET and Unity, [see the FAQ for details](https://github.com/ralfbiedert/interoptopus/blob/master/FAQ.md#existing-backends).

## System

The following system was used:

```
System: AMD Ryzen 9 7950X3D, 64 GB RAM; Windows 11
rustc: stable (i.e., 1.85 or later)
profile: --release
.NET: v9.0
```

## Results

| Construct | ns per call |
| --- | --- |
| `primitive_void()` | 3 |
| `primitive_u8(0)` | 4 |
| `primitive_u16(0)` | 4 |
| `primitive_u32(0)` | 4 |
| `primitive_u64(0)` | 4 |
| `pattern_ffi_option_1(OptionInner.None)` | 18 |
| `pattern_ffi_slice_delegate(x => x[0])` | 949 |
| `pattern_ffi_slice_delegate_huge(x => x[0])` | 1067 |
| `pattern_ffi_slice_delegate_huge(callback_huge_prealloc)` | 21 |
| `pattern_ffi_slice_2(short_vec, 0)` | 16 |
| `pattern_ffi_slice_2(long_vec, 0)` | 23 |
| `pattern_ffi_slice_4(short_byte, short_byte)` | 12 |
| `pattern_ascii_pointer_1('hello world')` | 58 |
| `pattern_string_2('hello world')` | 62 |
| `new VecU8([])` | 85 |
| `new VecUtf8String([])` | 14 |
| `pattern_vec_u8_return()` | 37 |
| `await new TaskCompletionSource().Task` | 101 |
| `await serviceAsync.Success()` | 443 |
