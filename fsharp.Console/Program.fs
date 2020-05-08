open UtilityCollections.Helpers
open GraphCreation
open ExpensiveModules
open System
open System.IO

let printDigraph (g: Digraph): unit =
    for KeyValue(label, node) in g.labels do
        let beneath = (g.costMap.Item node)
        printfn "%s, %d" label (beneath.Length)

let readFromConsole() =
    let numLines = Console.ReadLine() |> int
    [|1..numLines|]
    |> Array.map (fun _ -> Console.ReadLine().Split(' '))

let readFromConsoleV2() =
    let numLines = Console.ReadLine() |> int
    let restOfFile =
        [|1..numLines|]
        |> Array.map (fun _ -> Console.ReadLine() )
    (numLines, restOfFile)

let readFromFile (filename: string) =
    (fun () ->
        use sr = new StreamReader (filename)
        let numLines = sr.ReadLine () |> int
        let restOfFile =
            seq{ while not sr.EndOfStream do sr.ReadLine()}
            |> Seq.toArray  // read entire file into memory
        (numLines, restOfFile)
    )

let runExpensiveModules readerStrategy =
    readerStrategy()
    ||> parseCreateDigraph
    |> TransitiveReduction.reduce
    |> costOfModules
    |> printDigraph

[<EntryPoint>]
let main argv =
    match Array.length argv with
    | 0 -> runExpensiveModules readFromConsoleV2
    | 1 -> runExpensiveModules (readFromFile argv.[0])
    //| 2 -> printRandomGraph(float argv.[0]) (int argv.[1])
    // | _ -> failwith "Enter no arguments" |> ignore
    0   // Return 0;