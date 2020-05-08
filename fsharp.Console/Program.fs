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
    |> Array.map (fun _ -> Console.ReadLine().Split())

let readFromFile (filename: string) =
    (fun () ->
        use sr = new StreamReader (filename)
        let numLines = sr.ReadLine () |> int
        [|1..numLines|]
        |> Array.map (fun _ -> sr.ReadLine().Split())
    )

let runExpensiveModules readerStrategy =
    readerStrategy()
    |> createTransposeGraph
    |> TransitiveReduction.reduce
    |> costOfModules
    |> printDigraph

[<EntryPoint>]
let main argv =
    match Array.length argv with
    | 0 -> runExpensiveModules readFromConsole
    | 1 -> runExpensiveModules (readFromFile argv.[0])
    //| 2 -> printRandomGraph(float argv.[0]) (int argv.[1])
    // | _ -> failwith "Enter no arguments" |> ignore
    0   // Return 0;