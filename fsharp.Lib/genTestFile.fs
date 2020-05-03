module genTestFile
    // NOTE: this module needs rework
    // it needs to be re-written to match the new API
    open System
    open fsExpensiveModules
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

    let printRandomGraph p n = 
        printfn "%d" n
        Test.makeRandomList p n
        |> Test.printStringList

