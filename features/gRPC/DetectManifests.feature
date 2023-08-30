Feature: Invoking DetectManifests via gRPC
  The `DetectManifests` gRPC call is used to detect the manifest files in a directory tree that will be used to generate
  CycloneDX-based bill of materials (bom) files. For dotnet projects, this is typically project files such as `.csproj`
  or `.vbproj`.

  The call is provided with the full path to a directory that should be scanned for manifest files.

  For each manifest file that is listed, the response needs to contain the full path to the file.

  Scenario: A test fixture project
    This project contains a `.csproj` file in the root directory, and another one in the `directory` directory.
    Both files should be returned in the response.

    Given I clone the git repository "https://github.com/corgibytes/freshli-fixture-csharp-test" with the sha "583d813db3e28b9b44a29db352e2f0e1b4c6e420"
    When I run `freshli-agent-dotnet start-server 8392` interactively
    Then I wait for the freshli_agent.proto gRPC service to be running on port 8392
    And I call DetectManifests with the full path to "tmp/repositories/freshli-fixture-csharp-test" on port 8392
    Then the DetectManifests response contains the following file paths expanded beneath "tmp/repositories/freshli-fixture-csharp-test":
    """
    TestProject.csproj
    directory/TestProjectInDirectory.csproj
    """
    When the gRPC service on port 8392 is sent the shutdown command
    Then there are no services running on port 8392
    And the exit status should be 0

  Scenario: A typical application project
    This project contains several `.csproj` files, in multiple sub-directories.

    Given I clone the git repository "https://github.com/jellyfin/jellyfin" with the sha "fbd18e250394f953aa873cbdf7b4771ce0bdf693"
    When I run `freshli-agent-dotnet start-server 8392` interactively
    Then I wait for the freshli_agent.proto gRPC service to be running on port 8392
    And I call DetectManifests with the full path to "tmp/repositories/jellyfin" on port 8392
    Then the DetectManifests response contains the following file paths expanded beneath "tmp/repositories/jellyfin":
    """
    Emby.Dlna/Emby.Dlna.csproj
    Emby.Naming/Emby.Naming.csproj
    Emby.Photos/Emby.Photos.csproj
    Emby.Server.Implementations/Emby.Server.Implementations.csproj
    Jellyfin.Api/Jellyfin.Api.csproj
    Jellyfin.Data/Jellyfin.Data.csproj
    Jellyfin.Networking/Jellyfin.Networking.csproj
    Jellyfin.Server.Implementations/Jellyfin.Server.Implementations.csproj
    Jellyfin.Server/Jellyfin.Server.csproj
    MediaBrowser.Common/MediaBrowser.Common.csproj
    MediaBrowser.Controller/MediaBrowser.Controller.csproj
    MediaBrowser.LocalMetadata/MediaBrowser.LocalMetadata.csproj
    MediaBrowser.MediaEncoding/MediaBrowser.MediaEncoding.csproj
    MediaBrowser.Model/MediaBrowser.Model.csproj
    MediaBrowser.Providers/MediaBrowser.Providers.csproj
    MediaBrowser.XbmcMetadata/MediaBrowser.XbmcMetadata.csproj
    RSSDP/RSSDP.csproj
    fuzz/Emby.Server.Implementations.Fuzz/Emby.Server.Implementations.Fuzz.csproj
    fuzz/Jellyfin.Server.Fuzz/Jellyfin.Server.Fuzz.csproj
    src/Jellyfin.Drawing.Skia/Jellyfin.Drawing.Skia.csproj
    src/Jellyfin.Drawing/Jellyfin.Drawing.csproj
    src/Jellyfin.Extensions/Jellyfin.Extensions.csproj
    src/Jellyfin.MediaEncoding.Hls/Jellyfin.MediaEncoding.Hls.csproj
    src/Jellyfin.MediaEncoding.Keyframes/Jellyfin.MediaEncoding.Keyframes.csproj
    tests/Jellyfin.Api.Tests/Jellyfin.Api.Tests.csproj
    tests/Jellyfin.Common.Tests/Jellyfin.Common.Tests.csproj
    tests/Jellyfin.Controller.Tests/Jellyfin.Controller.Tests.csproj
    tests/Jellyfin.Dlna.Tests/Jellyfin.Dlna.Tests.csproj
    tests/Jellyfin.Extensions.Tests/Jellyfin.Extensions.Tests.csproj
    tests/Jellyfin.MediaEncoding.Hls.Tests/Jellyfin.MediaEncoding.Hls.Tests.csproj
    tests/Jellyfin.MediaEncoding.Keyframes.Tests/Jellyfin.MediaEncoding.Keyframes.Tests.csproj
    tests/Jellyfin.MediaEncoding.Tests/Jellyfin.MediaEncoding.Tests.csproj
    tests/Jellyfin.Model.Tests/Jellyfin.Model.Tests.csproj
    tests/Jellyfin.Naming.Tests/Jellyfin.Naming.Tests.csproj
    tests/Jellyfin.Networking.Tests/Jellyfin.Networking.Tests.csproj
    tests/Jellyfin.Providers.Tests/Jellyfin.Providers.Tests.csproj
    tests/Jellyfin.Server.Implementations.Tests/Jellyfin.Server.Implementations.Tests.csproj
    tests/Jellyfin.Server.Integration.Tests/Jellyfin.Server.Integration.Tests.csproj
    tests/Jellyfin.Server.Tests/Jellyfin.Server.Tests.csproj
    tests/Jellyfin.XbmcMetadata.Tests/Jellyfin.XbmcMetadata.Tests.csproj
    """
    When the gRPC service on port 8392 is sent the shutdown command
    Then there are no services running on port 8392
    And the exit status should be 0

  Scenario: Another typical project
    This project contains several `pom.xml` files. Many of the files appear within the `java` directory or one of it's
    sub-directories. The file located at `java/pom.xml` references all but one of the `pom.xml` files in the `java`
    directory as sub-modules. The rest of the `pom.xml` files contain no modules.

    Given I clone the git repository "https://github.com/reactiveui/Akavache" with the sha "44c4110d3cf9f779607ae254c3d4eff706f2f5cc"
    When I run `freshli-agent-dotnet start-server 8392` interactively
    Then I wait for the freshli_agent.proto gRPC service to be running on port 8392
    And I call DetectManifests with the full path to "tmp/repositories/Akavache" on port 8392
    Then the DetectManifests response contains the following file paths expanded beneath "tmp/repositories/Akavache":
    """
    src/Akavache.Core/Akavache.Core.csproj
    src/Akavache.CosmosDB/Akavache.CosmosDB.csproj
    src/Akavache.Drawing/Akavache.Drawing.csproj
    src/Akavache.Mobile/Akavache.Mobile.csproj
    src/Akavache.Sqlite3/Akavache.Sqlite3.csproj
    src/Akavache.Tests/Akavache.Tests.csproj
    src/Akavache/Akavache.Sqlite3.BundleE.csproj
    src/Akavache/Akavache.csproj
    tests/NuGetInstallationIntegrationTests/Android/Android.csproj
    tests/NuGetInstallationIntegrationTests/NET45/NET45.csproj
    tests/NuGetInstallationIntegrationTests/UWP/UWP.csproj
    tests/NuGetInstallationIntegrationTests/WPF/WPF.csproj
    tests/NuGetInstallationIntegrationTests/WinForms/WinForms.csproj
    tests/NuGetInstallationIntegrationTests/XamarinFormsApp/XamarinFormsApp.Android/XamarinFormsApp.Android.csproj
    tests/NuGetInstallationIntegrationTests/XamarinFormsApp/XamarinFormsApp.UWP/XamarinFormsApp.UWP.csproj
    tests/NuGetInstallationIntegrationTests/XamarinFormsApp/XamarinFormsApp.iOS/XamarinFormsApp.iOS.csproj
    tests/NuGetInstallationIntegrationTests/XamarinFormsApp/XamarinFormsApp/XamarinFormsApp.csproj
    tests/NuGetInstallationIntegrationTests/iOS/iOS.csproj
    tests/NuGetInstallationIntegrationTests/netstandard1.1/netstandard1.1.csproj
    """
    When the gRPC service on port 8392 is sent the shutdown command
    Then there are no services running on port 8392
    And the exit status should be 0
