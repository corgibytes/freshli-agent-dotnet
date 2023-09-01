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
    And I call ProcessManifest with the expanded path "tmp/repositories/freshli-fixture-csharp-test/TestProject.csproj" and the moment "2022-01-01T00:00:00Z" on port 8392
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
