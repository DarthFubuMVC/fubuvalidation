begin
  require 'bundler/setup'
  require 'fuburake'
rescue LoadError
  puts 'Bundler and all the gems need to be installed prior to running this rake script. Installing...'
  system("gem install bundler --source http://rubygems.org")
  sh 'bundle install'
  system("bundle exec rake", *ARGV)
  exit 0
end

@solution = FubuRake::Solution.new do |sln|
	sln.compile = {
		:solutionfile => 'src/FubuValidation.sln'
	}

	sln.assembly_info = {
		:product_name => "FubuValidaton",
		:copyright => 'Copyright 2008-2013 Jeremy D. Miller, Josh Arnold, et al. All rights reserved.'
	}

	sln.ripple_enabled = true
	sln.fubudocs_enabled = true
    
    sln.assembly_bottle 'FubuMVC.Validation'
end



desc "Target used for the CI on mono"
task :mono_ci => [:compile, :mono_unit_test]



desc "Runs some of the unit tests for Mono"
task :mono_unit_test => :compile do
  runner = NUnitRunner.new :compilemode => @solution.compilemode, :source => 'src', :platform => 'x86'
  runner.executeTests ['FubuValidation.Tests', 'FubuValidation.StructureMap.Tests', 'FubuMVC.Validation.Tests']
end
