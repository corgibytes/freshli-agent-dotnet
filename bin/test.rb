#!/usr/bin/env ruby
# frozen_string_literal: true

require 'bundler/setup'

require 'English'
require 'optparse'
require 'fileutils'

require 'corgibytes/freshli/commons/execute'
# rubocop:disable Style/MixinUsage
include Corgibytes::Freshli::Commons::Execute
# rubocop:enable Style/MixinUsage

enable_dotnet_command_colors

perform_build = true

parser = OptionParser.new do |options|
  options.banner = <<~BANNER
    Description:
        Test Runner

    Usage:
        test.rb [options]

    Options:
  BANNER

  options.on('-s', '--skip-build', 'Run tests without first calling build') do
    perform_build = false
  end

  options.on('-h', '--help', 'Show help and usage information') do
    puts options
    exit
  end
end

begin
  parser.parse!
rescue OptionParser::InvalidOption => e
  puts e
  puts parser
  exit(-1)
end

# based on https://stackoverflow.com/a/29743469/243215
def download(url, target_path)
  # rubocop:disable Security/Open
  require 'open-uri'
  download = URI.open(url)
  # rubocop:enable Security/Open
  IO.copy_stream(download, target_path)
end

status = execute("ruby #{File.dirname(__FILE__)}/build.rb") if perform_build

if status.nil? || status.success?
  status = execute('bundle install')

  if status.success?
    status = execute(
      'dotnet test --logger "console;verbosity=detailed" --blame-hang-timeout 10min ' \
      './exe/Corgibytes.Freshli.Agent.DotNet.Test.dll'
    )
  end
  status = execute('bundle exec cucumber --color --backtrace') if status.success?
end

exit(status.exitstatus)
