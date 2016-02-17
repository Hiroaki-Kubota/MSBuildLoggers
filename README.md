# MSBuildLoggers

Custom logger for MSBuild over Jenkins.

## MinimalSummaryConsoleLogger

This logger only outputs errors and warnings.
Each item separeted by tab.

### Example for jenkins

MSBuild task arguments:
/noconsolelogger /logger:MinimalSummaryConsoleLogger,%MSBuildLoggers_HOME%\MSBuildLoggers.dll;TrimPath="%WORKSPACE%\\"