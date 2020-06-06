# Graph question solved in F#

See the accompanying blog post [here](https://ttay.me/blog/optimizing_graph_algo_fsharp/), or in the repo at WRITEUP.md.

The code can be found in [fsharp.Lib/](fsharp.Lib)

## Building

To build this repo, you will need the .NET Core SDK 3.1. 
You can download it from the Microsoft website.

Once done, navigate to the directory holding this repo, and type `dotnet build -c Release`

The finished executable can be found in `./fsharp.Console/bin/Release/netcoreapp3.1/fsharp.Console.exe`

## Testing

There are very small unit tests in fsharp.Test/

Two bigger tests cases can be found in test\_sparse.in and test\_sparse\_big.in (the latter is the test case from the blog post)

You can run the tests using bigChecks.ps1. It is a Powershell script, you will need to install Powershell Core to run it. (That said, it could easily be ported to bash if you wish)

## C++
The C++ files in `cpp/` are not optimized at all and should not be used for benchmarking. I wrote them to verify correctness of the F# program for big random input files.
