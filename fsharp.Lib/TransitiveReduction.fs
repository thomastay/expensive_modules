module TransitiveReduction
open GraphCreation

// Implements the transitive reduction of a Directed Acyclic Graph
// represented as an adjacency list

// Implements the transitve reduction algorithm
// First implemented by Gries, D., Martin, A. J. et al (1987)
let reduce (graph: Digraph): Digraph =
    let adj = graph.adjacency
    let high = Array2D.length1 adj - 1
    let mutable counter = 0
    for k = high downto 0 do
        for i in 0..high do
            for j in 0..high do
                if adj.[i, k] && adj.[k, j] then
    //                counter <- counter + 1
                    adj.[i, j] <- false
    //printfn "Removed %d edges" counter
    graph