namespace fsharp.Test

open System.Collections
open Microsoft.VisualStudio.TestTools.UnitTesting
open fsExpensiveModules
(*
type Digraph = 
    {size: int;
    labels: Map<string, int>;
    adjacency: int array array; //adjacency[i] is the neighbors of the ith node
    costMap: Dictionary<int, ChildNodes>}
*)
[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.TestCreateGraph () =
        let graph = 
            [|[|"a"; "b"; "c"|]; [|"b"; "c"|]; [|"c"|]|]
            |> createTransposeGraph 3
        let m = [("a", 0); ("b", 1); ("c", 2)] |> Map.ofList
        let adj = [|[||];[|0|];[|0; 1|]|]
        Assert.AreEqual(graph.size, 3)
        Assert.AreEqual(graph.labels, m)
        Assert.IsTrue((graph.adjacency = adj))

    [<TestMethod>]
    member this.TestSimple1 () =
        let graph = 
            [|[|"a"; "b"; "c"|]; [|"b"; "c"|]; [|"c"|]|]
            |> createTransposeGraph 3
            |> costOfModules
        let aSize = 1
        let bSize = 2
        let cSize = 3
        Assert.IsTrue(graph.costMap.[0].Count = aSize)
        Assert.IsTrue(graph.costMap.[1].Count = bSize)
        Assert.IsTrue(graph.costMap.[2].Count = cSize)

    [<TestMethod>]
    member this.TestSimple2 () =
        let graph = 
            [|
                [|"a"; "b"; "c"; "d"; "e"|];
                [|"b"; "c"; "e"|];
                [|"c"; "e"|];
                [|"d"|];
                [|"e"|];
            |]
            |> createTransposeGraph 5
            |> costOfModules
        let aSize = 1
        let bSize = 2
        let cSize = 3
        let dSize = 2
        let eSize = 4
        Assert.IsTrue(graph.costMap.[0].Count = aSize)
        Assert.IsTrue(graph.costMap.[1].Count = bSize)
        Assert.IsTrue(graph.costMap.[2].Count = cSize)
        Assert.IsTrue(graph.costMap.[3].Count = dSize)
        Assert.IsTrue(graph.costMap.[4].Count = eSize)
