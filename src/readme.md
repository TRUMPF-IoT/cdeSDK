# C-DEngine SDK

## Libraries

The Libraries are provided as NuGet packages that contain both source code and a DLL. By default, the sources will become part of the consuming project (avoiding dll collisions and conflicts between plugins at the cost of patchability), but you will see warnings about duplicate type definitions.

To consume the code as shared code, without warnings, you can exclude everything but the contentFiles from the package reference, for example:

```XML
    <PackageReference Include="CDESenderBaseShared" Version="5.*">
      <IncludeAssets>contentFiles</IncludeAssets>
    </PackageReference>
```

To consume the code as a DLL, you can exclude the contentFiles from the package reference, for example:

```XML
    <PackageReference Include="CDESenderBaseShared" Version="5.*">
      <ExcludeAssets>contentFiles</ExcludeAssets>
    </PackageReference>
```

## MessageContracts

The [MessageContracts](MessageContracts/readme.md) directory contains message  contract definitions of specific plugins that can be referenced by other plugins that need to communicate with them.

## C-DEngine Visual Studio  Templates

The [CDEngineSDKTemplates](CDEngineSDKTemplates) directory contains various Visual Studio Project and Item templates to create a variety of application hosts and plug-iuns using the C-DEngine.
