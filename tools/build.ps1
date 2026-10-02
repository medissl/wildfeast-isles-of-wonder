param(
    [string]$UnityEditor = 'C:/Program Files/Unity/Hub/Editor/6000.3.7f1/Editor/Unity.exe',
    [string]$Destination
)
$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path -Parent $PSScriptRoot
$gamePath = Join-Path $projectRoot 'Game'
$logRoot = Join-Path $projectRoot 'artifacts'
New-Item -ItemType Directory -Path $logRoot -Force | Out-Null
if (-not $Destination) { $Destination = Join-Path $projectRoot 'Builds/Windows/Wildfeast.exe' }
if (-not (Test-Path -LiteralPath $UnityEditor)) { throw 'Unity 6000.3.7f1 was not found. Supply -UnityEditor with its executable path.' }
$buildLog = Join-Path $logRoot 'build.log'
$buildArguments = '-batchmode -quit -projectPath "{0}" -executeMethod Wildfeast.Editor.ProjectBuilder.Build -wildfeastBuild "{1}" -logFile "{2}"' -f $gamePath, $Destination, $buildLog
$buildProcess = Start-Process -FilePath $UnityEditor -ArgumentList $buildArguments -WindowStyle Hidden -PassThru -Wait
if ($buildProcess.ExitCode -ne 0) { throw "Unity build failed; inspect $buildLog" }
Write-Output "Windows player built: $Destination"
