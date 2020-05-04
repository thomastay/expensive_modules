# Set-PSDebug -Trace 1
$ErrorActionPreference = "Stop"     # stop on error
cd fsharp.Console
dotnet build --no-restore -c Release
cd ..
$time = Measure-Command { 
    Get-Content .\test_sparse_big.in |
    fsharp.Console\bin\Release\netcoreapp3.1\fsharp.Console
}
$timeFmt = "{0:s\.fff}" -f $time
$day = Get-Date
"$day : $timeFmt" >> .\timings.txt

