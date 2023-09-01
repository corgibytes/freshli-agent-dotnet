Feature: Invoke GetValidatingPackages via gRPC

  The `GetValidatingPackages` gRPC call is used by the `freshli agent verify` command to get a list of package urls that
  are known to work when provided as inputs to the `RetrieveReleaseHistory` gRPC call.

  Scenario: Get the repository urls
    When I run `freshli-agent-dotnet start-server 8392` interactively
    Then I wait for the freshli_agent.proto gRPC service to be running on port 8392
    And I call GetValidatingPackages on port 8392
    Then the GetValidatingPackages response should contain:
    """
    pkg:nuget/Corgibytes.Freshli.Lib@0.5.0
    pkg:nuget/System.CommandLine@2.0.0-beta4.22272.1
    pkg:nuget/Microsoft.CSharp@4.7.0
    """
    When the gRPC service on port 8392 is sent the shutdown command
    Then there are no services running on port 8392
    And the exit status should be 0
