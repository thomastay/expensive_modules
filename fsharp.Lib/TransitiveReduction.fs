[<RequireQualifiedAccess>]
module TransitiveReduction
open System.Threading.Tasks
open GraphCreation

// Implements the transitive reduction of a Directed Acyclic Graph
// represented as an adjacency list

// Implements the transitve reduction algorithm
// First implemented by Gries, D., Martin, A. J. et al (1987)
// minor perf improvements have been made
let reduce (graph: Digraph): Digraph =
    let adj = graph.adjacency
    let n = Array2D.length1 adj
    for k = n-1 downto 0 do
        Parallel.For(0, n,
            (fun i ->
                if adj.[i, k] then
                    for j in 0..n-1 do
                        if adj.[k, j] then
                            adj.[i, j] <- false
            )
        ) |> ignore
    graph