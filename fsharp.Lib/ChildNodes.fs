namespace UtilityCollections

open System.Collections.Generic
// This lets us change the underlying type without changing the
// implementation
type ChildNodes = HashSet<int>

module ChildNodes =
    let init (node: int): ChildNodes =
        let d = HashSet()
        d.Add node |> ignore
        d

    // Builds a childNode set from a list of child nodes
    let build (lst: ChildNodes[]): ChildNodes =
        let d = HashSet()
        for s in lst do
            d.UnionWith s
        d