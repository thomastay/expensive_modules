open GraphCreation
open ExpensiveModules
open System

let printDigraph (g: Digraph): unit =
    for KeyValue(label, node) in g.labels do
        let beneath = (g.costMap.Item node)
        printfn "%s: %A" label (beneath.Length)

let runOnInput() =
    let numLines = Console.ReadLine() |> int
    [|1..numLines|]
    |> Array.map (fun _ -> Console.ReadLine().Split(' '))
    |> createTransposeGraph
    |> costOfModules
    |> printDigraph

[<EntryPoint>]
let main argv =
    match Array.length argv with
    | 0 -> runOnInput()
    //| 2 -> printRandomGraph(float argv.[0]) (int argv.[1])
    // | _ -> failwith "Enter no arguments" |> ignore
    0   // Return 0;