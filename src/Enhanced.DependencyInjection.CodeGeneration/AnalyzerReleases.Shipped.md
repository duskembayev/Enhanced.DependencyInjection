; Shipped analyzer releases
; https://github.com/dotnet/roslyn-analyzers/blob/master/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

## Release 1.0

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|--------------------
ENHDI01 | EnhDiModuleGenerator | Error    | Type resolve error
ENHDI02 | EnhDiModuleGenerator | Error    | Root namespace resolve error
ENHDI03 | EnhDiModuleGenerator | Error    | Invalid entry definition


## Release 1.1

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|--------------------
ENHDI04 | EnhDiModuleGenerator | Error    | Incompatible attributes
ENHDI05 | EnhDiModuleGenerator | Warning  | Property resolve error
ENHDI06 | EnhDiModuleGenerator | Error    | Incompatible attributes

### Removed Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|--------------------
ENHDI02 | EnhDiModuleGenerator | Error    | Root namespace resolve error

## Release 1.2

### Removed Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|--------------------
ENHDI05 | EnhDiModuleGenerator | Warning  | Property resolve error