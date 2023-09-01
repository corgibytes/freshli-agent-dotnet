Feature: Invoking RetrieveReleaseHistory via gRPC
  The `RetrieveReleaseHistory` gRPC call responds with the full release history for a package. The package is specified
  using a [PURL or Package URL](https://github.com/package-url/purl-spec).

  Results are sorted sorted by date with the oldest date provided first.

  Scenario: With Freshli-Lib package
    Since the `RetrieveReleaseHistory` gRPC call always provides the full release history, it's expected that the
    version component will be omitted from the provided package url.

    Note: This scenario included all of the versions for the `apache-maven` package at the time that it was authored. It
    is expected that this scenario will still pass when newer versions become available and are added to the end of the
    output.

    When I run `freshli-agent-dotnet start-server 8392` interactively
    Then I wait for the freshli_agent.proto gRPC service to be running on port 8392
    And I call RetrieveReleaseHistory with "pkg:nuget/Corgibytes.Freshli.Lib" on port 8392
    Then RetrieveReleaseHistory response should contain the following versions and release dates:
    """
    0.4.0-alpha0116	2021-05-17T12:54:26.4800000Z
    0.4.0-alpha0117	2021-05-17T12:55:16.0200000Z
    0.4.0-alpha0121	2021-05-17T13:06:07.4930000Z
    0.4.0-alpha0124	2021-05-17T13:12:20.7570000Z
    0.4.0-alpha0127	2021-05-17T13:22:29.6970000Z
    0.4.0-alpha0131	2021-05-17T13:28:47.4230000Z
    0.4.0-alpha0140	2021-05-17T16:14:22.8900000Z
    0.4.0-alpha0141	2021-05-17T16:14:33.1170000Z
    0.4.0-alpha0143	2021-05-19T11:56:15.1200000Z
    0.4.0-alpha0146	2021-05-19T12:05:10.2100000Z
    0.4.0-alpha0149	2021-05-22T17:45:13.4230000Z
    0.4.0-alpha0169	2021-05-24T21:53:47.4130000Z
    0.4.0-alpha0170	2021-05-24T21:56:22.7870000Z
    0.4.0-alpha0172	2021-05-28T14:35:23.5570000Z
    0.4.0-alpha0173	2021-05-28T14:36:05.1830000Z
    0.4.0-alpha0177	2021-05-31T21:44:12.9400000Z
    0.4.0-alpha0178	2021-05-31T21:45:30.1100000Z
    0.4.0-alpha0183	2021-05-31T22:31:53.6630000Z
    0.4.0-alpha0184	2021-05-31T22:32:27.5870000Z
    0.4.0-alpha0186	2021-06-07T18:55:34.7400000Z
    0.4.0-alpha0187	2021-06-07T18:56:30.4230000Z
    0.4.0-alpha0191	2021-06-07T22:28:08.5500000Z
    0.4.0-alpha0192	2021-06-07T22:28:55.7400000Z
    0.4.0-alpha0196	2021-06-07T22:42:36.0700000Z
    0.4.0-alpha0198	2021-06-09T22:03:16.8430000Z
    0.4.0-alpha0199	2021-06-09T22:05:00.7830000Z
    0.4.0-alpha0201	2021-06-09T22:53:07.2100000Z
    0.4.0-alpha0202	2021-06-09T22:55:04.8300000Z
    0.4.0-alpha0208	2021-06-17T17:23:12.7400000Z
    0.4.0-alpha0212	2021-08-12T22:05:59.1470000Z
    0.4.0-alpha0214	2021-08-12T22:22:22.6370000Z
    0.4.0-alpha0215	2021-08-12T22:24:04.0700000Z
    0.4.0-alpha0217	2021-08-12T22:49:27.4770000Z
    0.4.0-alpha0218	2021-08-12T22:50:04.9000000Z
    0.4.0-alpha0220	2021-08-12T23:17:22.9500000Z
    0.4.0-alpha0221	2021-08-12T23:18:05.8170000Z
    0.4.0	2021-09-27T22:23:17.2430000Z
    0.5.0	2022-09-08T19:56:14.3770000Z
    """
    When the gRPC service on port 8392 is sent the shutdown command
    Then there are no services running on port 8392
    And the exit status should be 0

  Scenario: Valid Package URL for an unknown package
    If the command is unable to find any release history for the specified package, then it should output a friendly
    error message and use the program's status code to indicate that there's been a failure.

    When I run `freshli-agent-dotnet start-server 8392` interactively
    Then I wait for the freshli_agent.proto gRPC service to be running on port 8392
    And I call RetrieveReleaseHistory with "pkg:nuget/missing" on port 8392
    Then RetrieveReleaseHistory response should be empty
    When the gRPC service on port 8392 is sent the shutdown command
    Then there are no services running on port 8392
    And the exit status should be 0

  Scenario: Invalid Package URL
    If the command is unable parse the package url, then it should output a friendly error message and use the
    program's status code to indicate that there's been a failure.

    When I run `freshli-agent-dotnet start-server 8392` interactively
    Then I wait for the freshli_agent.proto gRPC service to be running on port 8392
    And I call RetrieveReleaseHistory with "invalid" on port 8392
    Then RetrieveReleaseHistory response should be empty
    When the gRPC service on port 8392 is sent the shutdown command
    Then there are no services running on port 8392
    And the exit status should be 0
