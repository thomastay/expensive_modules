﻿module TransitiveReduction
open GraphCreation

// Implements the transitive reduction of a Directed Acyclic Graph
// represented as an adjacency list

// Implements the transitve reduction algorithm
// First implemented by Gries, D., Martin, A. J. et al (1987)
let reduce (graph: Digraph): Digraph =
    let adj = graph.adjacency
    let high = Array2D.length1 adj - 1
    for i in 0..high do
        for j in 0..high do
            for k = high downto 0 do
                if adj.[i, k] && adj.[k, j] then
                    adj.[i, j] <- false
    graph