use crate::types::basic::Vec3f32;
use interoptopus::ffi_type;

/// Documented enum.
#[ffi_type]
pub enum EnumDocumented {
    /// Variant A.
    A,
    /// Variant B.
    B,
    /// Variant B.
    C,
}

#[ffi_type(name = "EnumRenamed")]
#[derive(Debug)]
pub enum EnumRenamedXYZ {
    X,
}

#[ffi_type]
#[derive(Clone)]
pub enum EnumPayload {
    A,
    B(Vec3f32),
    C(u32),
    // We don't support these for now
    // D { x: Vec3f32 },
    // E(u8, u8, u8),
}
