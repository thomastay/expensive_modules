module ExpensiveModules

open System
open System.Collections.Generic

open UtilityCollections
open GraphCreation

let genAdj arr row =
    let numCols = Array2D.length2 arr
    let rec helper col lst =
        if col = numCols then lst else
        if arr.[row, col] && row <> col // adjacency
        then helper (col + 1) (col::lst)
        else helper (col + 1) lst
    helper 0 []
    |> Array.ofList

let costOfModules (graph: Digraph) =
    let rec dfs (node: int) =
        let found, value = graph.costMap.TryGetValue node
        if found then value else
            let adj = genAdj graph.adjacency node
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