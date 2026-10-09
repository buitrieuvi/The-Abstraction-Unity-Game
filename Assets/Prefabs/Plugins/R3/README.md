# R3 runtime dependencies

The Unity package in Packages/manifest.json supplies the Unity integration only.
These runtime assemblies were extracted from official NuGet packages to complete
that installation without requiring an additional package manager.

| Package | Version | DLL target |
| --- | --- | --- |
| R3 | 1.3.1 | netstandard2.1 |
| Microsoft.Bcl.TimeProvider | 8.0.0 | netstandard2.0 |
| Microsoft.Bcl.AsyncInterfaces | 6.0.0 | netstandard2.1 |
| System.ComponentModel.Annotations | 5.0.0 | netstandard2.1 |
| System.Runtime.CompilerServices.Unsafe | 6.0.0 | netstandard2.0 |
| System.Threading.Channels | 8.0.0 | netstandard2.1 |

Source: https://api.nuget.org/v3-flatcontainer/{lowercase-package-id}/{version}/{lowercase-package-id}.{version}.nupkg
Installation documentation: https://github.com/Cysharp/R3/tree/1.3.1#unity
Licenses and third-party notices are in Licenses/.

Keep these DLLs and their Unity .meta files in source control. When updating R3,
update the Unity package and its core DLL together and check NuGet dependencies.
If migrating to NuGetForUnity, remove these manually installed DLLs as part of
that migration to avoid duplicate assemblies.
