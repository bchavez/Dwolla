## v2.0.1
* Move to **.NET Standard 2** and **.NET Framework 4.5.2**.

## v1.0.3
* Fixed bug in `WebProxy` and RestSharp.
* Allow `WebProxy` for easier HTTP debugging.
* Updated dependencies.
* Added `DwollaRedirect.ErrorDescription`
* Changed `DwollaRedirect.ClearingDate` type to `string`

## v0.3.5.0
* Using new API off-site gateway endpoint.
* Support for ProfileID.
* Support for CheckoutWithApi.
* Support for AdditionalFundingSources.

## v0.3.4.2
* New fields added to Callback.
* New Redirect MVC type for capturing redirect parameters.
* Added Helper method to verify redirect authenticity.
* Slight refactoring of names to check callback and redirect types.
* Renamed VerifyCallbackAuthenticity to VerifyAuthenticity.

## v0.3.3.0
* Added support for AllowGuestCheckout.