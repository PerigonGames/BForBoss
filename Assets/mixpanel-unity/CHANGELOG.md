## [v2.2.3](https://github.com/mixpanel/mixpanel-unity/releases/tag/v2.2.3)
### Mar 8 - 2020
## Fixes
- Remove `$ios_ifa`

---

## [v2.2.2](https://github.com/mixpanel/mixpanel-unity/releases/tag/v2.2.2)
### October 26 - 2020

## Fixes
- Fix in some rare cases, event payload being sent incorrectly formatted or with changed values

---

## [v2.2.1](https://github.com/mixpanel/mixpanel-unity/releases/tag/v2.2.1)
### July 31st - 2020

- Remove `$ios_ifa` user property for iOS devices: iOS 14 will not allow to read the IDFA value without permission.

## Fixes
- Improve objects re-utilization.

---

## [v2.2.0](https://github.com/mixpanel/mixpanel-unity/releases/tag/v2.2.0)
### June 2nd - 2020

## Features
- You can now manually initialize the library. You first need to enable this setting 
from your Project Settings menu. To use the library, call `Mixpanel.Init()` before you interact with it 
and `Mixpanel.Disable()` to dispose the component. 

## Fixes
- Fix fatal errror in `Mixpanel.Reset()` at app boot (thanks @RedHatJef!)

---

## [v2.1.4](https://github.com/mixpanel/mixpanel-unity/releases/tag/v2.1.4)
### February 18th - 2020

## Fixes
- Performance improvements.
- Fix set `PushDeviceToken` for Android where an string is used.

---

## [v2.1.3](https://github.com/mixpanel/mixpanel-unity/releases/tag/v2.1.3)
### February 10th - 2020

## Fixes
- Remove `ClearCharges` from `OptOutTracking` to avoid having orphan profiles at mixpanel

---

## [v2.1.2](https://github.com/mixpanel/mixpanel-unity/releases/tag/v2.1.2)
### January 9th - 2020

## New features
- Add `SetToken()` method to set project token programatically

---

## [v2.1.1](https://github.com/mixpanel/mixpanel-unity/releases/tag/v2.1.1)
### December 17th - 2019

## Fixes
- Added support for older Unity versions if .NET 4.x equivalent is the selected scripting runtime version
- Fix value serialize/deserialize bug (#93)

---

## [v2.1.0](https://github.com/mixpanel/mixpanel-unity/releases/tag/v2.1.0)
### November 14th - 2019

## Fixes
- API Error: Invalid JSON Encoding for numbers (https://github.com/mixpanel/mixpanel-unity/issues/74)
- Default people properties not been set properly
- `PushDeviceToken` not working (https://github.com/mixpanel/mixpanel-unity/issues/73)
- JSON encoding of special characters like `\"` or `\t`, etc...
- A flush operation now sends everything that happened until just right before the API is called.
- Properly migrate state from SDK 1.X to 2.X to preserve super properties and distinct ids.
- Major performance improvements

## New features
- Added de-duplication logic to prevent duplicated events to exist in your project
- Added an integration event
- Added new default event and people properties

---

## [v2.0.0](https://github.com/mixpanel/mixpanel-unity/releases/tag/v2.0.0)
### September 24th - 2019

#### This version is a complete rewrite of the library to support ALL platforms that unity can compile to.

The basis for this rewrite was https://github.com/mixpanel/mixpanel-unity/issues/10 to support WebGL but since the library was rewriten in plain c# it should work for any platform unity can compile to.

The API has stayed compliant with the documentation though there maybe a few changes to a few of the mixpanel properties that come though automatically due to unity not having access to certain system/device information easily please reachout to us if there is something missing after you upgrade and we can introspect it

The github repo has also been structured so that it supports the Unity 2018.4 package manager (please see the README for package manager install instructions)

This version of the library should support backwards compatibility with Unity 2018.x but it has only been tested with the 2018 LTS release.

---

## [v1.1.1](https://github.com/mixpanel/mixpanel-unity/releases/tag/v1.1.1)
### December 18th - 2017

Bug fixes

---

## [v1.1.0](https://github.com/mixpanel/mixpanel-unity/releases/tag/v1.1.0)
### October 6th - 2017

Improvements
Persist alias and protect users from identifying the user as their alias
Reset distinct_id and alias when reset is called
Clean ups
Fixes
Switching platforms could lead to MixpanelPostProcessor been executed at the wrong time
Reversed the attribution of app build number and version string ($app_build_number and $app_version_string)
Fix crash occurring only for Android when AdvertisingIdClient.Info.getId() was returning null.

---

## [v1.0.1](https://github.com/mixpanel/mixpanel-unity/releases/tag/1.0.1)
### June 16th - 2016

iOS
Added optional support for advertisingIdentifier
Added support for Bitcode
Windows
Added missing dependency for x86 and x86_64
All Platforms
Networking now respects the HTTP Retry-After header
Networking now backs off exponentially on failure

---

## [v1.0.0](https://github.com/mixpanel/mixpanel-unity/releases/tag/1.0.0)
### June 3rd - 2016

We are thrilled to release the official Mixpanel Unity SDK. Some links to get started:

* [Official documentation](https://mixpanel.com/help/reference/unity)
* [Full API Reference](http://mixpanel.github.io/mixpanel-unity/api-reference/annotated.html)
* [Sample application](https://github.com/mixpanel/mixpanel-unity/tree/master/deployments/UnityMixpanel/Assets/Mixpanel/Sample)
