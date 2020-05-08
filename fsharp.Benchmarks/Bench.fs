// Learn more about F# at http://fsharp.org

open System
open System.IO
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running
open GraphCreation
open ExpensiveModules

type BenchExpensiveModules() =
    let filename = @"C:\Users\z124t\source\repos\expensive_modules\test_sparse_big.in"
    let printDigraph (g: Digraph): unit =
        for KeyValue(label, node) in g.labels do
            let beneath = (g.costMap.Item node)
            printfn "%s, %d" label (beneath.Length)

    //[<Params(2, 4, 8)>]
    // member val k = 0 with get, set

    // [<GlobalSetup>]
    // member self.Setup() =

    [<Benchmark>]
    member self.runWholeProgram() =
        use sr = new StreamReader (filename)
        let numLines = sr.ReadLine () |> int
        [|1..numLines|]
        |> Array.map (fun _ -> sr.ReadLine().Split())
        |> createTransposeGraph
        |> TransitiveReduction.reduce
        |> costOfModules
        //|> printDigraph

[<EntryPoint>]
let main _ =
    let summary = BenchmarkRunner.Run<BenchExpensiveModules>()
    0 // return an integer exit code