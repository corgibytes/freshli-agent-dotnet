#!/usr/bin/env ruby
# frozen_string_literal: true

require 'bundler/setup'

require 'English'
require 'fileutils'

require 'corgibytes/freshli/commons/execute'
# rubocop:disable Style/MixinUsage
include Corgibytes::Freshli::Commons::Execute
# rubocop:enable Style/MixinUsage

enable_dotnet_command_colors

ENV['PROTOBUF_TOOLS_CPU'] = 'x64' if Gem.win_platform?

status = execute('dotnet tool restore')
status = execute('dotnet build -o exe Corgibytes.Freshli.Agent.DotNet.Test') if status.success?
status = execute('dotnet build -o exe Corgibytes.Freshli.Agent.DotNet') if status.success?

exit(status.exitstatus)
