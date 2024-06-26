use interoptopus::{callback, ffi_function, ffi_type};
use std::ffi::c_void;

callback!(MyCallback(value: u32) -> u32);
callback!(MyCallbackNamespaced(value: u32) -> u32, namespace = "common");
callback!(MyCallbackVoid(ptr: *const c_void));
callback!(MyCallbackContextual(context: *const c_void, value: u32));

impl DelegateResult for MyCallbackContextual {
    type Input = u32;
    fn call_trait(&self, ctx: *const c_void, value: Self::Input) {
        self.call_if_some(ctx, value);
    }
}

#[ffi_type]
#[repr(C)]
pub struct DelegateCallback<DeleResult> {
    pub callback: DeleResult,
    pub context: *const c_void,
}

impl<DeleResult> DelegateCallback<DeleResult>
where
    DeleResult: DelegateResult,
{
    pub fn call(&self, value: DeleResult::Input) {
        self.callback.call_trait(self.context, value)
    }
}

pub trait DelegateResult {
    type Input;
    fn call_trait(&self, ctx: *const c_void, value: Self::Input);
}

#[ffi_function]
#[no_mangle]
pub extern "C" fn pattern_callback_1(callback: MyCallback, x: u32) -> u32 {
    callback.call(x)
}

#[ffi_function]
#[no_mangle]
pub extern "C" fn pattern_callback_2(callback: MyCallbackVoid) -> MyCallbackVoid {
    callback
}

#[ffi_function]
#[no_mangle]
pub extern "C" fn pattern_callback_3(callback: DelegateCallback<MyCallbackContextual>, x: u32) {
    callback.call(x)
}

#[ffi_function]
#[no_mangle]
pub extern "C" fn pattern_callback_4(callback: MyCallbackNamespaced, x: u32) -> u32 {
    callback.call(x)
}

#[cfg(test)]
mod tests {
    use super::{MyCallback, MyCallbackNamespaced};
    use interoptopus::lang::rust::CTypeInfo;

    #[test]
    fn namespaces_assigned_correctly() {
        let ti1 = MyCallback::type_info();
        let ti2 = MyCallbackNamespaced::type_info();

        assert_eq!(ti1.namespace(), Some(""));
        assert_eq!(ti2.namespace(), Some("common"));
    }
}
