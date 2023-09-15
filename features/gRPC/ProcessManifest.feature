Feature: Invoking ProcessManifest via gRPC
  The `ProcessManifest` gRPC callused by the `freshli` CLI to instruct the agent to generate a CycloneDX bill of materials (BOM)
  file for a dependency manifest file. In the case of `freshli-agent-dotnet`, these dependency manifest files will be
  `pom.xml` files. In addition to requiring the path to the manifest file that should be processed, this command needs
  to know the date that should be used when resolving dependency version ranges. The range will be resolved to the
  latest version that satisfies the range as of the provided date.

  Scenario: Test fixture project
    This project uses a range expression to reference the `commons-io` library. On January 1st, 2021, the latest version
    of that package was `2.8.0`. That is the date that the version range should be resolved to, and the appropriate
    package url for that version should show up in the generated CycloneDX BOM file.

    Given I clone the git repository "https://github.com/corgibytes/freshli-fixture-csharp-test" with the sha "583d813db3e28b9b44a29db352e2f0e1b4c6e420"
    When I run `freshli-agent-dotnet start-server 8392` interactively
    Then I wait for the freshli_agent.proto gRPC service to be running on port 8392
    When I call ProcessManifest with the expanded path "tmp/repositories/freshli-fixture-csharp-test/TestProject.csproj" and the moment "2022-01-01T00:00:00Z" on port 8392
    Then the ProcessManifest response contains the following file paths expanded beneath "tmp/repositories/freshli-fixture-csharp-test":
    """
    obj/bom.json
    """
    And the CycloneDX file "tmp/repositories/freshli-fixture-csharp-test/obj/bom.json" should be valid
    And the CycloneDX file "tmp/repositories/freshli-fixture-csharp-test/obj/bom.json" should contain "pkg:nuget/NLog@4.7.7"
    And running git status should not report any modifications for "tmp/repositories/freshli-fixture-csharp-test"
    When the gRPC service on port 8392 is sent the shutdown command
    Then there are no services running on port 8392
    And the exit status should be 0

  Scenario: Unlisted package in manifest file
    This project includes a manifest file that contains references to packages versions
    that have been unlisted. NuGet restore fails as a result, and the CycloneDX DotNet
    tool isn't able to generate a BOM file.

    Given I clone the git repository "https://github.com/opserver/Opserver" with the sha "ac8f6d8e75d061137017d2bd6a47a81100dcf40e"
    When I run `freshli-agent-dotnet start-server 8392` interactively
    Then I wait for the freshli_agent.proto gRPC service to be running on port 8392
    When I call ProcessManifest with the expanded path "tmp/repositories/Opserver/Opserver.Core/packages.config" and the moment "2022-01-01T00:00:00Z" on port 8392
    Then the ProcessManifest response is empty
    And running git status should not report any modifications for "tmp/repositories/Opserver"
    When the gRPC service on port 8392 is sent the shutdown command
    Then there are no services running on port 8392
    And the exit status should be 0

  Scenario: Project with range expression referencing a stable package using a date where the latest version is a pre-release
    This project uses a range expression to reference the `Microsoft.Extensions.Logging.Abstractions` package.
    On 10/01/2022, the latest version of that package was `7.0.0-rc.1.22426.10`. Since the start of the version range
    references a stable version, `6.0.0`, then the pinned version should be `6.0.2`.

    Given I clone the git repository "https://github.com/Nethereum/Nethereum" with the sha "a46cbd3b68ed1fdc309c6dff623dc1301d674e6d"
    When I run `freshli-agent-dotnet start-server 8392` interactively
    Then I wait for the freshli_agent.proto gRPC service to be running on port 8392
    When I call ProcessManifest with the expanded path "tmp/repositories/Nethereum/src/Nethereum.JsonRpc.Client/Nethereum.JsonRpc.Client.csproj" and the moment "2022-10-01T00:00:00Z" on port 8392
    Then the ProcessManifest response contains the following file paths expanded beneath "tmp/repositories/Nethereum/src/Nethereum.JsonRpc.Client":
    """
    obj/bom.json
    """
    And the CycloneDX file "tmp/repositories/Nethereum/src/Nethereum.JsonRpc.Client/obj/bom.json" should be valid
    And the CycloneDX file "tmp/repositories/Nethereum/src/Nethereum.JsonRpc.Client/obj/bom.json" should contain "pkg:nuget/Microsoft.Extensions.Logging.Abstractions@6.0.2"
    And running git status should not report any modifications for "tmp/repositories/Nethereum"
    When the gRPC service on port 8392 is sent the shutdown command
    Then there are no services running on port 8392
    And the exit status should be 0

  Scenario: Project with range expression referencing a pre-release package using a date where the latest version is a pre-release
    This project uses a range expression to reference the `PowerShellStandard.Library` package.
    On 08/15/2018, the latest version of that package which satisfies the full range is `5.1.0-preview-06`. Since the
    start of the version range references a pre-release version, `5.1.0-preview-04`, then the pinned version should be
    `5.1.0-preview-06`.

    Given I clone the git repository "https://github.com/PowerShell/SHiPS" with the sha "91762f57fab5725d58ee325c21b48c90ef6e57f0"
    When I run `freshli-agent-dotnet start-server 8392` interactively
    Then I wait for the freshli_agent.proto gRPC service to be running on port 8392
    When I call ProcessManifest with the expanded path "tmp/repositories/SHiPS/src/Microsoft.PowerShell.SHiPS/Microsoft.PowerShell.SHiPS.csproj" and the moment "2018-08-15T00:00:00Z" on port 8392
    Then the ProcessManifest response contains the following file paths expanded beneath "tmp/repositories/SHiPS/src/Microsoft.PowerShell.SHiPS":
    """
    obj/bom.json
    """
    And the CycloneDX file "tmp/repositories/SHiPS/src/Microsoft.PowerShell.SHiPS/obj/bom.json" should be valid
    And the CycloneDX file "tmp/repositories/SHiPS/src/Microsoft.PowerShell.SHiPS/obj/bom.json" should contain "pkg:nuget/PowerShellStandard.Library@5.1.0-preview-06"
    And running git status should not report any modifications for "tmp/repositories/SHiPS"
    When the gRPC service on port 8392 is sent the shutdown command
    Then there are no services running on port 8392
    And the exit status should be 0
