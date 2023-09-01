Feature: Invoke GetValidatingRepositories via gRPC

  The `GetValidatingRepositories` gRPC call is used by the `freshli agent verify` command to get a list of repository
  urls that are known to work when running against the `DetectManifests` and `ProcessManifest` gRPC calls.

  Scenario: Get the repository urls
    When I run `freshli-agent-dotnet start-server 8392` interactively
    Then I wait for the freshli_agent.proto gRPC service to be running on port 8392
    And I call GetValidatingRepositories on port 8392
    Then GetValidatingRepositories response should contain:
    """
    https://github.com/corgibytes/freshli-fixture-csharp-test
    https://github.com/jellyfin/jellyfin
    https://github.com/reactiveui/Akavache
    """
    When the gRPC service on port 8392 is sent the shutdown command
    Then there are no services running on port 8392
    And the exit status should be 0
