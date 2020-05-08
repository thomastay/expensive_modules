// Learn more about F# at http://fsharp.org

open System
open System.IO
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running
open GraphCreation
open UtilityCollections.Helpers

type BenchExpensiveModules() =
    let filename = @"C:\Users\z124t\source\repos\expensive_modules\test_sparse_big.in"

    //[<Params(2, 4, 8)>]
    // member val k = 0 with get, set

    // [<GlobalSetup>]
    // member self.Setup() =

    [<Benchmark>]
    member self.parseNaive() =
        use sr = new StreamReader (filename)
        let _numLines = sr.ReadLine () |> int
        let restOfFile =
            seq{ while not sr.EndOfStream do sr.ReadLine()}
            |> Seq.toArray  // read entire file into memory
        restOfFile
        |> Array.map stringSplit
        |> createTransposeGraph

    [<Benchmark>]
    member self.parseWhileLexing() =
        use sr = new StreamReader (filename)
        let numLines = sr.ReadLine () |> int
        let restOfFile =
            seq{ while not sr.EndOfStream do sr.ReadLine()}
            |> Seq.toArray  // read entire file into memory
        restOfFile
        |> parseCreateDigraph numLines

[<EntryPoint>]
let main _ =
    let summary = BenchmarkRunner.Run<BenchExpensiveModules>()
    0 // return an integer exit code