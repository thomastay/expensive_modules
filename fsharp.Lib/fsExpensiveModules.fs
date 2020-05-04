module ExpensiveModules

open System
open System.Collections.Generic

open UtilityCollections
            
        

type Digraph = 
    {size: int;
    labels: Map<string, int>;
    adjacency: int array array; //adjacency[i] is the neighbors of the ith node
    costMap: Dictionary<int, ChildNodes>}


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
    costMap = new Dictionary<int, ChildNodes>();}



let costOfModules (graph: Digraph) = 
    let rec dfs (node: int) = 
        let found, value = graph.costMap.TryGetValue node
        if found then value else
            let adj = graph.adjacency.[node]
            let s = ChildNodes.init node
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
                    |> ChildNodes.build
                graph.costMap.Add(node, s)
                s
    for i in 0..graph.size-1 do
        let found = graph.costMap.ContainsKey i 
        if found then () else dfs i |> ignore
    graph
