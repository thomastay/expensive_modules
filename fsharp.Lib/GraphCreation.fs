﻿module GraphCreation
open System.Collections.Generic
open UtilityCollections

type Digraph =
    {size: int;
    labels: Map<string, int>;
    adjacency: bool[,]; // adjcency is now a full-block adjacency matrix
    costMap: Dictionary<int, ChildNodes>}

// for testing in fsi
let private _testString = [|"a b c"; "b c"; "c"|]
let private _testing = [|[|"a"; "b"; "c"|]; [|"b"; "c"|]; [|"c"|]|]

let createLabels nodes =
    let size = Array.length nodes
    Array.zip nodes [|0..size-1|]
    |> Map.ofArray

let createTransposeEdges labels sl =
    let sink = Map.find (Array.head sl) labels
    let sources =
        Array.tail sl
        |> Array.map (fun s -> Map.find s labels)
    (sources, sink)

let createTransposeGraph (sll: string[][]) =
    let nodes = Array.map Array.head sll
    let size = Array.length nodes
    let labels = createLabels nodes
    let adj = Array2D.zeroCreate size size
    // for each source, sink, update the adjacency matrix
    for i in 0..size-1 do
        let sources, sink = createTransposeEdges labels sll.[i]
        for source in sources do
            adj.[source, sink] <- true
    {size=size; labels = labels; adjacency = adj;
    costMap = new Dictionary<int, ChildNodes>(size);}

(* Old code based on adjacency lists
    Not deleting because it may be useful later on

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

// size deprecated for now
let createTransposeGraph _ (sll: string[][]) : Digraph =
    let nodes = Array.map Array.head sll
    let size = Array.length nodes
    let labels = createLabels nodes
    let adj =
        sll
        |> Array.collect (createTransposeEdges labels)
        |> createMapFromEdgeList
        |> makeFixedSizeMap size
    {size=size; labels = labels; adjacency = adj;
    costMap = new Dictionary<int, ChildNodes>();}
*)