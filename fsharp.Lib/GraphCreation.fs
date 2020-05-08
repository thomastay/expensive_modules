module GraphCreation
open System.Collections.Generic
open UtilityCollections
open UtilityCollections.Helpers

type Digraph =
    {size: int;
    labels: Map<string, int>;
    adjacency: bool[,]; // adjcency is now a full-block adjacency matrix
    costMap: Dictionary<int, ChildNodes>}

// for testing in fsi
let private _testString = [|"a b c"; "b c"; "c"|]
let private _testing = [|[|"a"; "b"; "c"|]; [|"b"; "c"|]; [|"c"|]|]

let findStrOrUpdateMap (m: Map<string, int>) s =
    match Map.tryFind s m with
    | Some(i) -> (m, i)
    | None ->
        let size = Map.count m
        (Map.add s size m, size)

// Note: Modifies adj! Not a pure function
// Also creates the labels array along the way, tally hoo
// Idea: should re-tool to use Dictionary<string, int>
let private parseCreateLabelUpdateAdj readerInput (adj: bool[,]) m =
    let strs = stringSplit readerInput
    let sink = Array.head strs
    let sources = Array.tail strs
    let m, sink = findStrOrUpdateMap m sink
    (m, sources)
    ||> Array.fold (fun m source ->
        let m, source = findStrOrUpdateMap m source
        adj.[source, sink] <- true
        m
        )

let parseCreateDigraph size sl =
    let adj = Array2D.zeroCreate size size
    let labels =
        (Map.empty, sl)
        ||> Array.fold (fun m s -> parseCreateLabelUpdateAdj s adj m)
    {size=size; labels = labels; adjacency = adj;
    costMap = Dictionary<int, ChildNodes>();}

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
    costMap = new Dictionary<int, ChildNodes>();}

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