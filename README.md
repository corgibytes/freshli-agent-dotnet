# Freshli Agent: DotNet

This application is used by the [`freshli` CLI](https://github.com/corgibytes/freshli-cli) to detect and process manifest files from the DotNet (.NET) ecosystem.

## Developing

### Requirements

#### Runtime Requirements

* DotNet SDK version 7.0.400
* The CycloneDX `dotnet tool` needs to be installed in such a way that it is accessible by running `DotNet-CycloneDX`. Running `dotnet tool restore` will install a compatible version, but you'll need to make sure that the install location is referenced in your `PATH`.

#### Development Requirements

* Ruby version 3.0.5
  * `bundler` gem version 2.4.18
  * Be able to install gems without being logged in as root
* [eclint](https://gitlab.com/greut/eclint) version 0.3.3
* [CycloneDX CLI](https://github.com/CycloneDX/cyclonedx-cli) version 0.24.0


### Building

#### Using `bin/build.rb`

You can use the `bin/build.rb` script to build the application.

On macOS or Linux:
```bash
bin/build.rb
```

On Windows:
```pwsh
ruby .\bin\build.rb
```

#### Using `dotnet`

If you don't want to use the Ruby-based build script you can use the `DotNet` command directly.

On macOS, Linux, or Windows:
```bash
dotnet build
```

Running the following command will place the build output in the `exe` directory, which is where the Cucumber/Aruba tests expects the executable to be.

On macOS, Linux, or Windows:
```bash
dotnet build -o exe Corgibytes.Freshli.Agent.DotNet
```

If you're working on the `freshli` CLI, then you may want to put the `exe` directory in your `PATH` so that the `freshli` CLI is able to find the DotNet language agent executable.

### Tests

#### Using `bin/test.rb`

You can run both the application's unit and integration tests that are written in C# and the application's acceptance tests that are written using Cucumber and Aruba by running:

On macOS and Linux:
```bash
bin/test.rb
```

On Windows:
```pwsh
ruby .\bin\test.rb
```

#### Running Directly

The application's unit tests can be run with:

On macOS, Linux, and Windows:
```bash
dotnet test
```

And the application's acceptance tests can be run with:

```bash
dotnet build -o exe Corgibytes.Freshli.Agent.DotNet
bundle exec cucumber
```

## Running

### Running with `dotnet run`

It's possible to instruct Gradle to run the command for you.

On macOS, Linux, and Windows:
```bash
dotnet run --project Corgibytes.Freshli.Agent.DotNet -- --help
```

### Running from `exe` directory

You can run the program from the `exe` that's created by the `bin/build.rb` script.

On macOS and Linux:
```bash
bin/build.rb
./exe/freshli-agent-dotnet --help
```

On Windows:
```pwsh
ruby .\bin\build.rb
.\exe\freshli-agent-dotnet --help
```
