# Project Versioning

## How do we define our versions?

Per normal semantic versioning, versions are `<major>.<minor>.<patch>.<build>`.

For our versions below 1.0, the pattern is `0.<major>.<minor>.<build>`.

## Branches

Currently our branches are as follows:

|        Name | Current Version | Notes
|------------:|-----------------|------------------------------------------------------------------------------
|        main | 0.6.2.460       | Receives markdown documentation or GitHub automation related changes
|  prerelease | 0.7.0.129       | Non-breaking or security bug fixes only (the actual stable branch)
| development | 0.7.0           | Non-breaking or security fixes and features only

After the 0.7.0-beta release the branches will be as follows:

|        Name | Current Version | Notes
|------------:|-----------------|----------------------------------------------------------------------------------------------
|        main | 0.7.0.x         | Non-breaking or security bug fixes, markdown documentation, and GitHub automation changes
|  prerelease | 0.7.x.y         | Non-breaking or security fixes and features
| development | 0.8.x.y         | Breaking changes permitted


## Breaking Changes

Breaking changes **are** any changes that alter/remove field, property, method, or class signatures, or alter or delete assemblies

Breaking changes **are not** any changes that introduce overloads or new signatures, but are still not permitted in the "main" branch
