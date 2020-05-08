namespace UtilityCollections

open System.Collections.Generic
open System.IO
// This lets us change the underlying type without changing the
// implementation
type ChildNodes = int[]

module ChildNodes =
    let init (node: int): ChildNodes = [|node|]

    // Builds a childNode set from a list of child nodes
    let build (lst: ChildNodes[]): ChildNodes =
        let totalSize =   // num elts total
            lst
            |> Array.sumBy (fun arr -> Array.length arr)
        let d = HashSet(totalSize / 4)  // 4 det'd by trial
        for s in lst do
            d.UnionWith s
        d |> Seq.toArray

    // Old function, remained for benchmarking
    // Builds a childNode set from a list of child nodes
    let buildNoAlloc (lst: ChildNodes[]): ChildNodes =
        let d = HashSet()
        for s in lst do
            d.UnionWith s
        d |> Seq.toArray

// Helper functions that can be reused
module Helpers =
    let readLines (filePath: string) =
        seq {
            use sr = new StreamReader (filePath)
            while not sr.EndOfStream do
                yield sr.ReadLine ()
        }

    let inline stringSplit (s: string) = s.Split()