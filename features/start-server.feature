Feature: `start-server` command

  The `start-server` starts a gRPC server as described in `freshli_agent.proto`. After the server
  is started, the process blocks until either the process is terminated or the `Shutdown` gRPC
  function is called. Once the gRPC server is ready for connections, then a message is written
  to the console, and the health check service will indicate that the service is healthy. If the
  specified port is not available, then the process terminates immediately with a non-zero exit
  code after outputting an error message.

  Scenario: Starting the server with a provided port number
    Given there are no services running on port 8324
    When I run `freshli-agent-dotnet start-server 8324` interactively
    Then I wait for the freshli_agent.proto gRPC service to be running on port 8324
    When the gRPC service on port 8324 is sent the shutdown command
    Then there are no services running on port 8324
    And the exit status should be 0

  Scenario: Starting the service with a provided port number that is already in use
    Given a test service is started on port 8334
    When I run `freshli-agent-dotnet start-server 8334`
    Then the output should contain:
    """
    Unable to start the gRPC service. Port 8334 is in use.
    """
    And the exit status should not be 0
    When the test service running on port 8334 is stopped
    Then there are no services running on port 8334
