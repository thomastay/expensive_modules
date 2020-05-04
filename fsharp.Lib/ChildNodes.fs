namespace UtilityCollections

open System.Collections.Generic
// This lets us change the underlying type without changing the
// implementation
type ChildNodes = int[]

module ChildNodes =
    let init (node: int): ChildNodes = [|node|]

    // Builds a childNode set from a list of child nodes
    let build (lst: ChildNodes[]): ChildNodes =
        let megaArray = Array.concat lst
        let d = HashSet megaArray
        d |> Seq.toArray