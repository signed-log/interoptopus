use crate::patterns::result::{Error, FFIError};
use interoptopus::patterns::string::CStrPointer;
use interoptopus::{ffi_service, ffi_service_ctor, ffi_service_method, ffi_type};

/// Some struct we want to expose as a class.
#[ffi_type(opaque)]
pub struct ServiceStrings {}

// Regular implementation of methods.
#[ffi_service(error = "FFIError")]
impl ServiceStrings {
    #[ffi_service_ctor]
    pub fn new() -> Result<Self, Error> {
        Ok(Self {})
    }

    pub fn pass_string(&mut self, _: CStrPointer) {}

    // If we actually return a value we have to declare what happens upon panic.
    #[ffi_service_method(on_panic = "undefined_behavior")]
    pub fn return_string(&mut self) -> CStrPointer {
        CStrPointer::empty()
    }
}
