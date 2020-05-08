# Set-PSDebug -Trace 1
$ErrorActionPreference = "Stop"     # stop on error
dotnet build .\fsharp.Console\fsharp.Console.fsproj --no-restore -c Release
Write-Host "Testing direct console input"
Get-Content test_sparse.in | 
    .\fsharp.Console\bin\Release\netcoreapp3.1\fsharp.Console > test_sparse_fsharp.out
Compare-Object (Get-Content .\test_sparse_fsharp.out) (Get-Content .\test_sparse.correct)
Get-Content test_sparse_big.in | 
    .\fsharp.Console\bin\Release\netcoreapp3.1\fsharp.Console > test_sparse_big_fsharp.out
Compare-Object (Get-Content .\test_sparse_big_fsharp.out) (Get-Content .\test_sparse_big.correct)

Write-Host "Testing read file"
.\fsharp.Console\bin\Release\netcoreapp3.1\fsharp.Console test_sparse.in > test_sparse_fsharp.out
Compare-Object (Get-Content .\test_sparse_fsharp.out) (Get-Content .\test_sparse.correct)
.\fsharp.Console\bin\Release\netcoreapp3.1\fsharp.Console test_sparse_big.in > test_sparse_fsharp.out
Compare-Object (Get-Content .\test_sparse_big_fsharp.out) (Get-Content .\test_sparse_big.correct)


