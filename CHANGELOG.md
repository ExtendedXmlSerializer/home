# [ExtendedXmlSerializer v3.7.9](https://github.com/ExtendedXmlSerializer/home/releases/tag/v3.7.9)
> 12/27/2022 13:09:14 UTC
##### ``v3.7.9``
### Other Changes 
- [Automated] Generated CHANGELOG.md #587 @github-actions[bot] 
- Updated deploy key for documentation #591 @Mike-E-angelo 

# [ExtendedXmlSerializer v3.7.8](https://github.com/ExtendedXmlSerializer/home/releases/tag/v3.7.8)
> 12/27/2022 11:57:14 UTC
##### ``v3.7.8``
### Other Changes 
- Build Nudge #589 @Mike-E-angelo 

# [ExtendedXmlSerializer v3.7.7](https://github.com/ExtendedXmlSerializer/home/releases/tag/v3.7.7)
> 12/27/2022 11:21:24 UTC
##### ``v3.7.7``
### &#128027; Bug Fixes &#128295; 
- Added `ConfigurationContainer.AllowMultipleReferences` #586 @Mike-E-angelo 
### Other Changes 
- [Automated] Generated CHANGELOG.md #560 @github-actions[bot] 
- Demonstration of EnableReferences #564 @Mike-E-angelo 
- Quick sanity check to ensure implicit double can be serialized #562 @Mike-E-angelo 
- Demonstration of EmitWhen #566 @Mike-E-angelo 
- Updated remaining projects to netcoreapp3.1 #568 @Mike-E-angelo 
- Comparer example #569 @Mike-E-angelo 
- Demonstrated default behavior #572 @Mike-E-angelo 
- Update tests to .net 4.7.2 #576 @WojciechNagorski 
- Update BenchmarkDotNet #577 @WojciechNagorski 
- Update some nuget packages #579 @WojciechNagorski 
- Remove netappcore3.1 replace w/ net6.0 + net7.0 #585 @Mike-E-angelo 

# [ExtendedXmlSerializer v3.7.6](https://github.com/ExtendedXmlSerializer/home/releases/tag/v3.7.6)
> 10/26/2021 08:41:13 UTC
##### ``v3.7.6``
### &#128027; Bug Fixes &#128295; 
- Improved Unknown Assembly Experience/Messaging #559 @Mike-E-angelo 
### Other Changes 
- Update BenchmarkDotNet #553 @WojciechNagorski 
- Update packages #554 @WojciechNagorski 

# [ExtendedXmlSerializer v3.7.5](https://github.com/ExtendedXmlSerializer/home/releases/tag/v3.7.5)
> 09/07/2021 13:03:29 UTC
##### ``v3.7.5``
### &#128027; Bug Fixes &#128295; 
- Added hypen as a valid character for assembly names #544 @Mike-E-angelo 
- Fixed AutoFormatting types that do not have converters #551 @Mike-E-angelo 

# [ExtendedXmlSerializer v3.7.4](https://github.com/ExtendedXmlSerializer/home/releases/tag/v3.7.4)
> 09/01/2021 16:06:51 UTC
##### ``v3.7.4``
### &#128027; Bug Fixes &#128295; 
- Fixed problem with namespace not emitting properly for optimized name… #546 @Mike-E-angelo 

# [ExtendedXmlSerializer v3.7.3](https://github.com/ExtendedXmlSerializer/home/releases/tag/v3.7.3)
> 08/31/2021 12:15:48 UTC
##### ``v3.7.3``
This is solely a CI-based release as build problems occurred with the previous one:
https://github.com/ExtendedXmlSerializer/home/releases/tag/v3.7.2

# [ExtendedXmlSerializer v3.7.2](https://github.com/ExtendedXmlSerializer/home/releases/tag/v3.7.2)
> 08/31/2021 11:19:12 UTC
##### ``v3.7.2``
### &#128027; Bug Fixes &#128295; 
- Adjusted writer to properly store root/default namespace #529 @Mike-E-angelo 
- Accounted for converter registration with nullable structures #534 @Mike-E-angelo 
- Accounted for empty content for nullable attributes #531 @Mike-E-angelo 
### Other Changes 
- Upgraded tests to netcoreapp3.1 #536 @Mike-E-angelo 
- Attended to warnings #537 @Mike-E-angelo 

# [ExtendedXmlSerializer v3.7.1](https://github.com/ExtendedXmlSerializer/home/releases/tag/v3.7.1)
> 05/25/2021 06:34:21 UTC
##### ``v3.7.1``
### &#128027; Bug Fixes &#128295; 
- Fixed `WithUnknownContent.Continue` to account for complex content #522 @Mike-E-angelo 

# [ExtendedXmlSerializer v3.7.0](https://github.com/ExtendedXmlSerializer/home/releases/tag/v3.7.0)
> 03/30/2021 03:23:20 UTC
##### ``v3.7.0``
### ✨ New Features &#128640; 
- Added `ConfigurationContainer.EnableStaticReferenceChecking` #512 @Mike-E-angelo 
- Added `ConfigurationContainer.EnableThreadAwareRecursionContent` #514 @Mike-E-angelo 
-  Added basic support for `TypeConfiguration.Ignore` #516 @Mike-E-angelo 
### &#128027; Bug Fixes &#128295; 
- Fixed namespace resolution bug in migration extension #518 @Mike-E-angelo 

# [ExtendedXmlSerializer v3.6.2](https://github.com/ExtendedXmlSerializer/home/releases/tag/v3.6.2)
> 03/10/2021 10:27:09 UTC
##### ``v3.6.2``
### Other Changes 
- Build fix (hopefully) #505 @Mike-E-angelo 

# [ExtendedXmlSerializer v3.6.1](https://github.com/ExtendedXmlSerializer/home/releases/tag/v3.6.1)
> 03/10/2021 07:26:03 UTC
##### ``v3.6.1``
### &#128027; Bug Fixes &#128295; 
- Fixed reference + attribute-based deserialization issues #503 @Mike-E-angelo 

# [ExtendedXmlSerializer v3.6.0](https://github.com/ExtendedXmlSerializer/home/releases/tag/v3.6.0)
> 12/15/2020 06:34:03 UTC
##### ``v3.6.0``
### ✨ New Features &#128640; 
- Added non-generic versions of `EnableImplicitTypingFrom*` methods #494 @netaques 
- Added `EnableImplicitTypingByInspecting&lt;T&gt;` extension method #495 @netaques 

# [ExtendedXmlSerializer v3.5.0](https://github.com/ExtendedXmlSerializer/home/releases/tag/v3.5.0)
> 12/08/2020 04:16:45 UTC
##### ``v3.5.0``
### ✨ New Features &#128640; 
- Added non-generic equivalent for `AddMigration` #484 @Mike-E-angelo 
- Added support for `System.Collections.Immutable` Types #486 @Mike-E-angelo 
### &#128027; Bug Fixes &#128295; 
- Fixed list support in `WithUnknownContent` #487 @Mike-E-angelo 
- Applied serializer type check using `WithUnknownContent` #492 @Mike-E-angelo 

# [ExtendedXmlSerializer v3.4.3](https://github.com/ExtendedXmlSerializer/home/releases/tag/v3.4.3)
> 11/24/2020 06:11:48 UTC
##### ``v3.4.3``
### &#128027; Bug Fixes &#128295; 
- Adjusted support for nullable structure types. #478 @Mike-E-angelo 
# [ExtendedXmlSerializer v3.4.2](https://github.com/ExtendedXmlSerializer/home/releases/tag/v3.4.2)
> 11/03/2020 06:05:39 UTC
##### ``v3.4.2``
### &#128027; Bug Fixes &#128295; 
- Added check for &quot;unspeakable&quot; types. #471 @Mike-E-angelo 

# [ExtendedXmlSerializer v3.4.1](https://github.com/ExtendedXmlSerializer/home/releases/tag/v3.4.1)
> 10/13/2020 03:28:52 UTC
##### ``v3.4.1``
### Other Changes 
- Generated API Key. &#129310; #468 @Mike-E-angelo 

# [ExtendedXmlSerializer v3.4.0](https://github.com/ExtendedXmlSerializer/home/releases/tag/v3.4.0)
> 10/13/2020 03:02:05 UTC
##### ``v3.4.0``
### ✨ New Features &#128640; 
- Added basic support for custom `IDictionary&lt;TKey, TValue&gt;`. #465 @Mike-E-angelo 
### &#128027; Bug Fixes &#128295; 
- Scoped reference encounter counter to per-writer. #455 @Mike-E-angelo 
- Skipped processing of parameterized-content members that have custom … #464 @Mike-E-angelo 
### Other Changes 
- Update NuGet API Key #456 @WojciechNagorski 
- Use BenchmarkSwitcher #459 @WojciechNagorski 
- Update nuget packages #457 @WojciechNagorski 
- Update performance tests #460 @WojciechNagorski 

# [ExtendedXmlSerializer v3.3.0](https://github.com/ExtendedXmlSerializer/home/releases/tag/v3.3.0)
> 09/22/2020 06:05:26 UTC
##### ``v3.3.0``
### ✨ New Features &#128640; 
- Introduced type-based serialization interception. #452 @Mike-E-angelo 

# [ExtendedXmlSerializer v3.2.7](https://github.com/ExtendedXmlSerializer/home/releases/tag/v3.2.7)
> 09/15/2020 13:28:08 UTC
##### ``v3.2.7``
### &#128027; Bug Fixes &#128295; 
- Accounted for empty CDATA elements. #444 @Mike-E-angelo 
- Accounted for successive CDATA blocks. #448 @Mike-E-angelo 

# [ExtendedXmlSerializer v3.2.6](https://github.com/ExtendedXmlSerializer/home/releases/tag/v3.2.6)
> 09/01/2020 08:58:59 UTC
##### ``v3.2.6``
### &#128027; Bug Fixes &#128295; 
- Allowed `null` Namespace for Framework Types #430 @oliver-chime 
- Allowed `null`/Empty Namespaces for Custom/Non-system Assemblies #432 @Mike-E-angelo 
- Removed Caching from Reference Resolution #437 @Mike-E-angelo 

# [ExtendedXmlSerializer v3.2.5](https://github.com/ExtendedXmlSerializer/home/releases/tag/v3.2.5)
> 08/18/2020 07:24:58 UTC
##### ``v3.2.5``
### &#128027; Bug Fixes &#128295; 
- Accounted for rare state when namespace prefix is `null`. #424 @Mike-E-angelo 
- Improved member resolution for parameterized content #428 @Mike-E-angelo 

# [ExtendedXmlSerializer v3.2.4](https://github.com/ExtendedXmlSerializer/home/releases/tag/v3.2.4)
> 07/21/2020 17:47:55 UTC
##### ``v3.2.4``
### &#128027; Bug Fixes &#128295; 
- Member resolution now uses ReflectedType first, then DeclaredType. #417 @Mike-E-wins 
- Fixed references resolution bugs during deserialization #420 @Mike-E-wins 
- Added support for IReadOnlyList properties. #421 @Mike-E-wins 

# [ExtendedXmlSerializer v3.2.3](https://github.com/ExtendedXmlSerializer/home/releases/tag/v3.2.3)
> 07/21/2020 14:12:24 UTC
##### ``v3.2.3``
### &#128027; Bug Fixes &#128295; 
- Assigned parser contexts for migrations. #415 @Mike-E-wins 

# [ExtendedXmlSerializer v3.2.2](https://github.com/ExtendedXmlSerializer/home/releases/tag/v3.2.2)
> 07/07/2020 07:25:26 UTC
##### ``v3.2.2``
### &#128027; Bug Fixes &#128295; 
- Enabled member comparison by base-type on EmitWhen call. #412 @Mike-E-wins 

# [ExtendedXmlSerializer v3.2.1](https://github.com/ExtendedXmlSerializer/home/releases/tag/v3.2.1)
> 06/22/2020 14:51:17 UTC
##### ``v3.2.1``
### &#128027; Bug Fixes &#128295; 
- Fixed bug with references w/ exs:member=&quot;&quot; attributes #408 @Mike-E-wins 

# [ExtendedXmlSerializer v3.2.0](https://github.com/ExtendedXmlSerializer/home/releases/tag/v3.2.0)
> 06/16/2020 07:28:35 UTC
##### ``v3.2.0``
### ✨ New Features &#128640; 
- Allowed members to be configured from base classes. #399 @Mike-E-wins 
- Ensured exs xmlns is applied to root for optimized namespace + enabled-reference scenarios. #402 @Mike-E-wins 
### &#128027; Bug Fixes &#128295; 
- Demonstrated WithUnknownContent().Continue() #395 @Mike-E-wins 
- Adjusted reference detection to be more accurate in the case of attri… #400 @Mike-E-wins 
### Other Changes 
- Added basic (throw) support for anonymous/dynamic types. #389 @Mike-E-wins 
- [Automated] Generated CHANGELOG.md #390 @github-actions[bot] 
- Emit initialization times. #394 @Mike-E-wins 
- Sample code for documentation demonstrating implicit and explicit ref… #404 @Mike-E-wins 

# [ExtendedXmlSerializer v3.1.4](https://github.com/ExtendedXmlSerializer/home/releases/tag/v3.1.4)
> 04/28/2020 06:37:20 UTC
##### ``v3.1.4``
### &#128027; Bug Fixes &#128295; 
- Added support for Flags-based enumerations. #387 @Mike-E-wins 

# [ExtendedXmlSerializer v3.1.3](https://github.com/ExtendedXmlSerializer/home/releases/tag/v3.1.3)
> 03/25/2020 12:57:39 UTC
##### ``v3.1.3``
### &#128027; Bug Fixes &#128295; 
- Added test demonstrating private setters. #376 @Mike-EEE 
- Fixed Caching Issues for Better Performance. #381 @Mike-EEE 
### Other Changes 
- Added Sample code for question. #373 @Mike-EEE 

# [ExtendedXmlSerializer v3.1.2](https://github.com/ExtendedXmlSerializer/home/releases/tag/v3.1.2)
> 02/07/2020 09:56:47 UTC
##### ``v3.1.2``
### &#128027; Bug Fixes &#128295; 
- Adjusted assembly-loading to be a bit more robust for .NETFramework-b… #367 @Mike-EEE 


