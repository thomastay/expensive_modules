open System
open System.Collections.Generic

module SortedArr =
    type SortedArr = HashSet<Int16>
    let build (lst: SortedArr[]): SortedArr =
        let d = Array.head lst
        for i in 1..(Array.length lst - 1) do
            d.UnionWith lst.[i]
        d



open SortedArr

type Digraph = 
    {size: int;
    labels: Map<string, int>;
    adjacency: int array array; //adjacency[i] is the neighbors of the ith node
    costMap: Dictionary<int, SortedArr>}


let createTransposeEdges labels sl = 
    let tailLength = Array.length sl - 1
    let sink = 
        Map.find (Array.head sl) labels
        |> Array.replicate tailLength
    let sources = 
        Array.tail sl
        |> Array.map (fun s -> Map.find s labels)
    Array.zip sources sink

let createMapFromEdgeList (el: ('T * 'T)[]) = 
    el
    |> Array.groupBy fst
    |> Array.map (fun (u,l) -> (u, Array.map snd l))
    |> Map.ofArray

let makeFixedSizeMap size m =
    [0..size-1]
    |> List.map (fun i -> 
        match Map.tryFind i m with
        | Some(l) -> l
        | None -> Array.empty
    )
    |> Array.ofList
    

let createTransposeGraph size (sll: string[][]) : Digraph = 
    let nodes = Array.map Array.head sll
    assert (nodes.Length = size)
    let labels =
        Array.zip nodes [|0..size-1|]
        |> Map.ofArray
    let adj = 
        sll
        |> Array.collect (createTransposeEdges labels)
        |> createMapFromEdgeList
        |> makeFixedSizeMap size
    {size=size; labels = labels; adjacency = adj;
    costMap = new Dictionary<int, SortedArr>();}



let costOfModules (graph: Digraph) = 
    let rec dfs (node: int) = 
        let found, value = graph.costMap.TryGetValue node
        if found then value else
            let adj = graph.adjacency.[node]
            let (s: SortedArr) = HashSet()
            s.Add (int16 node) |> ignore
            match adj.Length with
            | 0 -> 
                // Node is a leaf, so add itself to costMap
                graph.costMap.Add(node, s)
                s
            | _ ->
                let s = 
                    adj
                    |> Array.map dfs
                    |> Array.append [|s|]
                    |> SortedArr.build
                graph.costMap.Add(node, s)
                s
    for i in 0..graph.size-1 do
        let found = graph.costMap.ContainsKey i 
        if found then () else dfs i |> ignore
    graph


(*
module Test = 
    let testing = 
        [
            ["A"; "S"; "E"; "N"];
            ["S"; "H"; "N"];
            ["E"; "N"];
            ["H"];
            ["N"];
        ]
    let getRandomBool p = 
        let randBoolStream = 
            let rnd = Random()
            Seq.initInfinite(fun _ -> rnd.NextDouble() < p)
        Seq.head randBoolStream

    let genRandomDag p size = 
        let m = 
            [1..size]
            |> List.collect (fun i ->
                [i+1..size]
                |> List.map (fun j ->
                    if getRandomBool p then Some (string i, string j) else None
                    )
                )
            |> List.choose id
            |> createMapFromEdgeList
        let n = [1..size] |> List.map string
        {nodes = n; adjacency = m;}

    let testRandom() = 
        genRandomDag 0.3 200
        |> costOfModules
        |> ignore

    let makeRandomList p size = 
        let allNodesEmpty = 
            List.init size (fun i -> string (i+1))
        [1..size]
        |> List.collect (fun i ->
            [i+1..size]
            |> List.map (fun j ->
                if getRandomBool p then Some (string i, string j) else None
                )
            )
        |> List.choose id
        |> createMapFromEdgeList
        |> (fun m -> 
            List.filter (fun elt -> not <| Map.containsKey elt m) allNodesEmpty
            |> List.fold (fun M elt -> Map.add elt [] M) m
        )
        |> Map.toList
        |> List.map (fun (u, l) -> [yield u; yield! l])
    
    let printStringList sll = 
        sll
        |> List.iter (String.concat " " >> printfn "%s")

*)

let printDigraph (g: Digraph): unit = 
    for KeyValue(label, node) in g.labels do
        let beneath = (g.costMap.Item node)
        printfn "%s: %A" label (beneath.Count)
        
let runOnInput() = 
    let numLines = Console.ReadLine() |> int
    [|1..numLines|]
    |> Array.map (fun _ -> Console.ReadLine().Split(' '))
    |> createTransposeGraph numLines
    |> costOfModules
    |> printDigraph


//let printRandomGraph p n = 
//    printfn "%d" n
//    Test.makeRandomList p n
//    |> Test.printStringList


[<EntryPoint>]
let main argv =
    match Array.length argv with
    | 0 -> runOnInput()
    //| 2 -> printRandomGraph(float argv.[0]) (int argv.[1])
    // | _ -> failwith "Enter no arguments" |> ignore
    0   // Return 0;
