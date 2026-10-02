param([string]$UnityEditor = 'C:/Program Files/Unity/Hub/Editor/6000.3.7f1/Editor/Unity.exe')
$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path -Parent $PSScriptRoot
$gamePath = Join-Path $projectRoot 'Game'
$logRoot = Join-Path $projectRoot 'artifacts'
New-Item -ItemType Directory -Path $logRoot -Force | Out-Null
if (-not (Test-Path -LiteralPath $UnityEditor)) { throw 'Unity 6000.3.7f1 was not found. Supply -UnityEditor with its executable path.' }
$testLog = Join-Path $logRoot 'tests.log'
$resultFile = Join-Path $logRoot 'editmode.xml'
$testArguments = '-batchmode -nographics -projectPath "{0}" -runTests -testPlatform EditMode -testResults "{1}" -logFile "{2}"' -f $gamePath, $resultFile, $testLog
$testProcess = Start-Process -FilePath $UnityEditor -ArgumentList $testArguments -WindowStyle Hidden -PassThru -Wait
if ($testProcess.ExitCode -ne 0) { throw "Unity tests failed; inspect $testLog" }
Write-Output "Unity test results: $resultFile"
