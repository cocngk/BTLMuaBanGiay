# fix-roslyn.ps1 - sua loi "Could not find a part of the path '...\BTL\bin\roslyn\csc.exe'"
#
# Chay tren Windows, dung tai thu muc goc repo (noi co BTL.sln):
#   powershell -ExecutionPolicy Bypass -File .\tools\fix-roslyn.ps1
#
# Khong can Visual Studio, khong can nuget.exe: neu thieu package thi tai thang
# tu nuget.org, roi chep Roslyn vao BTL\bin\roslyn (ASP.NET Web Forms bien dich
# .aspx/.ascx luc chay bang csc.exe trong thu muc do).

$ErrorActionPreference = 'Stop'

$root = (Get-Location).Path
$proj = Join-Path $root 'BTL'
$bin  = Join-Path $proj 'bin'
$ver  = '2.0.1'
$name = "Microsoft.CodeDom.Providers.DotNetCompilerPlatform.$ver"
$pkg  = Join-Path $root "packages\$name"

if (-not (Test-Path (Join-Path $proj 'BTL.csproj'))) {
    throw "Khong thay BTL\BTL.csproj - hay chay script nay tai thu muc goc repo (noi co BTL.sln)."
}

Write-Host "Kiem tra IIS Express con giu file trong bin..."
Get-Process iisexpress -ErrorAction SilentlyContinue | ForEach-Object {
    Write-Warning "Dang tat IIS Express (PID $($_.Id)) de chep duoc file."
    $_.Kill()
    $_.WaitForExit()
}

if (-not (Test-Path (Join-Path $pkg 'tools\RoslynLatest\csc.exe'))) {
    Write-Host "Thieu package $name, dang tai tu nuget.org..."
    $tmp = Join-Path $env:TEMP "roslyn-$ver.zip"
    Invoke-WebRequest -UseBasicParsing `
        -Uri "https://www.nuget.org/api/v2/package/Microsoft.CodeDom.Providers.DotNetCompilerPlatform/$ver" `
        -OutFile $tmp
    New-Item -ItemType Directory -Force $pkg | Out-Null
    Expand-Archive -Path $tmp -DestinationPath $pkg -Force
    Remove-Item $tmp -Force
}

$dest = Join-Path $bin 'roslyn'
New-Item -ItemType Directory -Force $bin   | Out-Null
New-Item -ItemType Directory -Force $dest  | Out-Null

Copy-Item (Join-Path $pkg 'tools\RoslynLatest\*') $dest -Recurse -Force
Copy-Item (Join-Path $pkg 'lib\net45\Microsoft.CodeDom.Providers.DotNetCompilerPlatform.dll') $bin -Force

$csc = Join-Path $dest 'csc.exe'
if (Test-Path $csc) {
    Write-Host "OK: $csc" -ForegroundColor Green
    Write-Host "Mo lai site (F5 trong Visual Studio) la chay duoc."
} else {
    throw "Van thieu $csc"
}
