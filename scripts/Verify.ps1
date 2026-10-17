[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string] $Configuration = 'Release',
    [ValidateSet('All', 'Forms', 'Workflows')]
    [string] $Suite = 'All',
    [switch] $NoRestore
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

if ([Environment]::OSVersion.Platform -ne [PlatformID]::Win32NT) {
    throw 'Bina verification requires Windows (WinForms and bundled native libraries).'
}

$repositoryPath = Split-Path -Parent $PSScriptRoot
$testProject = Join-Path $repositoryPath 'tests/SmokeTests/SmokeTests.csproj'
# Keep verification output separate from a running desktop app; bin/ is gitignored.
$outputPath = Join-Path $repositoryPath "artifacts/bin/verification/$Configuration"
$outputArgument = '-p:OutDir=' + $outputPath + [IO.Path]::DirectorySeparatorChar

Push-Location $repositoryPath
try {
    if (-not $NoRestore) {
        & dotnet restore $testProject --disable-parallel -m:1
        if ($LASTEXITCODE -ne 0) { throw "Restore failed (exit $LASTEXITCODE)." }
    }

    & dotnet build $testProject -c $Configuration --no-restore -m:1 $outputArgument --nologo
    if ($LASTEXITCODE -ne 0) { throw "Build failed (exit $LASTEXITCODE)." }

    $suiteArguments = @()
    if ($Suite -eq 'Forms') { $suiteArguments = @('--forms-only') }
    if ($Suite -eq 'Workflows') { $suiteArguments = @('--refactoring-only') }

    & dotnet (Join-Path $outputPath 'DSP.Khana.Ocr.SmokeTests.dll') @suiteArguments
    if ($LASTEXITCODE -ne 0) { throw "Verification failed (exit $LASTEXITCODE)." }
}
finally {
    Pop-Location
}
