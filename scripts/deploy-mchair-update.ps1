# Copy bản publish lên server KHÔNG ghi đè DB và ảnh resources.
# Usage:
#   .\scripts\deploy-mchair-update.ps1 -Source "src\Web\Friday.MCHair.Web\bin\Release\net10.0\linux-x64\publish" -Target "D:\sites\mchair"
param(
    [Parameter(Mandatory = $true)]
    [string] $Source,
    [Parameter(Mandatory = $true)]
    [string] $Target
)

$Source = (Resolve-Path $Source).Path
$Target = $Target.TrimEnd('\', '/')

if (-not (Test-Path $Source)) {
    Write-Error "Source not found: $Source"
    exit 1
}

if (-not (Test-Path $Target)) {
    New-Item -ItemType Directory -Path $Target -Force | Out-Null
}

Write-Host "Source: $Source"
Write-Host "Target: $Target"
Write-Host "Skipping: Data\ (database), wwwroot\resources\ (uploaded images)"
Write-Host ""

robocopy $Source $Target /E /XO /R:2 /W:3 `
    /XD "Data" `
    /XD "resources" `
    /NFL /NDL /NJH /NJS /nc /ns /np

# robocopy exit codes 0-7 = success
if ($LASTEXITCODE -ge 8) {
    Write-Error "Robocopy failed with code $LASTEXITCODE"
    exit $LASTEXITCODE
}

Write-Host ""
Write-Host "Done. DB and wwwroot/resources on server were NOT overwritten."
Write-Host "Restart the app service on the server."
