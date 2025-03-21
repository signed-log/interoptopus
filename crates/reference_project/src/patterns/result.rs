use interoptopus::pattern::result::Result;
use interoptopus::{ffi_function, ffi_type};
use std::fmt::{Display, Formatter};
// This file may look complex but the Interoptopus parts are actually really simple,
// with some Rust best practices making up most of the code.

// This is the FFI error enum you want your users to see. You are free to name and implement this
// almost any way you want.
#[ffi_type]
#[derive(Debug, PartialOrd, PartialEq, Copy, Clone)]
pub enum ErrorREMOVEME {
    Ok = 0,
    Null = 100,
    Panic = 200,
    Delegate = 300,
    Fail = 400,
}

// This is the error type you use in a Rust library. Again, you are almost
// entirely free how you want to implement it.
#[derive(Debug)]
pub enum ErrorREMOVEME2 {
    Bad,
}

// Provide a mapping how your Rust error enums translate
// to your FFI error enums.
impl From<ErrorREMOVEME2> for ErrorREMOVEME {
    fn from(x: ErrorREMOVEME2) -> Self {
        match x {
            ErrorREMOVEME2::Bad => Self::Fail,
        }
    }
}

// Implement Default so we know what the "good" case is.
impl Default for ErrorREMOVEME {
    fn default() -> Self {
        Self::Ok
    }
}

// Implement Interoptopus' `FFIError` trait for your FFIError enum.
// Here you must map 3 "well known" variants to your enum.
impl interoptopus::pattern::result::FFIError for ErrorREMOVEME {
    const SUCCESS: Self = Self::Ok;
    const NULL: Self = Self::Null;
    const PANIC: Self = Self::Panic;
}

// Lazy "Display" implementation so your error can be logged.
impl Display for ErrorREMOVEME2 {
    fn fmt(&self, _: &mut Formatter<'_>) -> std::fmt::Result {
        Ok(())
    }
}

// Tell Rust your error type is an actual Rust Error.
impl std::error::Error for ErrorREMOVEME2 {}

#[ffi_function]
pub fn pattern_result_1(x: Result<u32, ErrorREMOVEME>) -> Result<u32, ErrorREMOVEME> {
    x
}

#[ffi_function]
pub fn pattern_result_2() -> Result<(), ErrorREMOVEME> {
    Result::ok(())
}

#[ffi_function]
pub fn pattern_result_3(x: Result<(), ErrorREMOVEME>) -> Result<(), ErrorREMOVEME> {
    x
}
