use interoptopus::ffi_type;
use interoptopus::lang::c::{CType, CompositeType, Field, PrimitiveType};

use crate::thirdparty::ThirdPartyVecF32;

/// A vector used in our game engine.
#[ffi_type]
#[repr(C)]
#[derive(Copy, Clone, Debug)]
pub struct Vec2f32 {
    pub x: f32,
    pub y: f32,
    pub z: f32,
}

/// A vector used in our game engine.
#[ffi_type]
#[repr(C)]
#[derive(Copy, Clone, Debug)]
pub struct SuperComplexEntity {
    pub player_1: Vec2f32,
    pub player_2: Vec2f32,
    pub ammo: u64,
    /// Point to a ASCII encoded whatnot.
    pub some_str: *const u8,
    pub str_len: u32,
}

/// A type containing a third-party type.
#[repr(C)]
#[ffi_type(surrogates(third_party = "third_party_option"))]
#[derive(Copy, Clone, Debug)]
pub struct WithForeignType {
    pub secret_number: u64,
    pub third_party: Option<*const ThirdPartyVecF32>,
}

// Won't win a beauty contest, but does the job.
fn third_party_option() -> CType {
    CType::ReadPointer(Box::new(third_party_vec_f32()))
}

// We can use this function wherever we refer to `ThirdPartyVecF32`.
fn third_party_vec_f32() -> CType {
    let mut composite = CompositeType::new("ThirdPartyVecF32".to_string());
    composite.add_field(Field::new("x".to_string(), CType::Primitive(PrimitiveType::F32)));
    composite.add_field(Field::new("y".to_string(), CType::Primitive(PrimitiveType::F32)));
    composite.add_field(Field::new("z".to_string(), CType::Primitive(PrimitiveType::F32)));
    composite.add_field(Field::new("w".to_string(), CType::Primitive(PrimitiveType::F32)));
    CType::Composite(composite)
}

/// Worst game engine ever.
///
/// Note that this struct has no `ffi_type` and won't be exported. It's part of
/// the opaque Context used in `ffi`.
#[derive(Default, Debug)]
pub struct GameEngine {
    pub player_score: u32,
}