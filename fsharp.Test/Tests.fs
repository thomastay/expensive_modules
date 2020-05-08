namespace fsharp.Test

open System.Collections.Generic
open Microsoft.VisualStudio.TestTools.UnitTesting
open UtilityCollections
open GraphCreation
open ExpensiveModules
(*
type Digraph =
    {size: int;
    labels: Map<string, int>;
    adjacency: int array array; //adjacency[i] is the neighbors of the ith node
    costMap: Dictionary<int, ChildNodes>}
*)
[<TestClass>]
type TestGraphCreation () =
    [<TestMethod>]
    member this.TestCreateGraph1 () =
        let graph =
            [|[|"a"; "b"; "c"|]; [|"b"; "c"|]; [|"c"|]|]
            |> createTransposeGraph
        let m = [("a", 0); ("b", 1); ("c", 2)] |> Map.ofList
        let adj = array2D [[false; false; false];
                           [true; false; false];
                           [true; true; false]]
        Assert.AreEqual(graph.size, 3)
        Assert.AreEqual(graph.labels, m)
        Assert.IsTrue((graph.adjacency = adj))

    [<TestMethod>]
    member this.TestCreateGraph2 () =
        let graph =
            [|
                [|"d"|];
                [|"a"; "b"; "c"; "d"; "e"|];
                [|"e"|];
                [|"b"; "c"; "e"|];
                [|"c"; "e"|];
            |]
            |> createTransposeGraph
        let m = [("d", 0); ("a", 1); ("e", 2); ("b", 3); ("c", 4)] |> Map.ofList
        let adj = array2D [ [false; true; false; false; false]
                            [false; false; false; false; false]
                            [false; true; false; true; true]
                            [false; true; false; false; false]
                            [false; true; false; true; false]
                          ]
        Assert.AreEqual(graph.size, 5)
        Assert.AreEqual(graph.labels, m)
        Assert.IsTrue((graph.adjacency = adj))

    [<TestMethod>]
    member this.TestCreateGraph1V2 () =
        let graph =
            [|"a b c"; "b c"; "c"|]
            |> parseCreateDigraph 3
        let m = [("a", 0); ("b", 1); ("c", 2)] |> Map.ofList
        let adj = array2D [[false; false; false];
                           [true; false; false];
                           [true; true; false]]
        Assert.AreEqual(graph.size, 3)
        Assert.AreEqual(graph.labels, m)
        Assert.IsTrue((graph.adjacency = adj))

    [<TestMethod>]
    member this.TestCreateGraph2V2 () =
        let graph =
            [|
                "d";
                "a b c d e";
                "e";
                "b c e";
                "c e"
            |]
            |> parseCreateDigraph 5
        let m = [("d", 0); ("a", 1); ("b", 2); ("c", 3); ("e", 4)] |> Map.ofList
        let adj = array2D [ [false; true; false; false; false]
                            [false; false; false; false; false]
                            [false; true; false; false; false]
                            [false; true; true; false; false]
                            [false; true; true; true; false]
                          ]
        Assert.AreEqual(graph.size, 5)
        Assert.AreEqual(graph.labels, m)
        Assert.IsTrue((graph.adjacency = adj))

[<TestClass>]
type TestTransitiveReduction () =
    static member toMatrix (arr: int[,]): bool[,] =
        let m = Array2D.length1 arr
        let n = Array2D.length2 arr
        let result = Array2D.zeroCreate m n
        for i in 0..m-1 do
            for j in 0..n-1 do
                if arr.[i, j] = 1 then
                    result.[i, j] <- true
                else result.[i, j] <- false
        result

    [<TestMethod>]
    member this.SimpleReduction () =
        let l = [("a", 0); ("b", 1); ("c", 2); ("d", 3); ("e", 4); ("f", 5)] |> Map.ofList
        let adj = array2D [ [0; 1; 1; 1; 1; 0]
                            [0; 0; 0; 1; 0; 1]
                            [0; 0; 0; 0; 1; 0]
                            [0; 0; 0; 0; 0; 1]
                            [0; 0; 0; 0; 0; 1]
                            [0; 0; 0; 0; 0; 0]
                          ] |> TestTransitiveReduction.toMatrix
        let graph =
            {size=5; labels = l; adjacency = adj;
            costMap = Dictionary<int, ChildNodes>() }
            |> TransitiveReduction.reduce
        let expectedAdj = array2D [ [0; 1; 1; 0; 0; 0]
                                    [0; 0; 0; 1; 0; 0]
                                    [0; 0; 0; 0; 1; 0]
                                    [0; 0; 0; 0; 0; 1]
                                    [0; 0; 0; 0; 0; 1]
                                    [0; 0; 0; 0; 0; 0]
                          ] |> TestTransitiveReduction.toMatrix
        CollectionAssert.AreEqual(graph.adjacency, expectedAdj)

[<TestClass>]
type TestExpensiveModules() =
    [<TestMethod>]
    member this.TestSimple1 () =
        let graph =
            [|[|"a"; "b"; "c"|]; [|"b"; "c"|]; [|"c"|]|]
            |> createTransposeGraph
            |> costOfModules
        let aSize = 1
        let bSize = 2
        let cSize = 3
        Assert.IsTrue(graph.costMap.[0].Length = aSize)
        Assert.IsTrue(graph.costMap.[1].Length = bSize)
        Assert.IsTrue(graph.costMap.[2].Length = cSize)

    [<TestMethod>]
    member this.TestSimple1withReduction () =
        let graph =
            [|[|"a"; "b"; "c"|]; [|"b"; "c"|]; [|"c"|]|]
            |> createTransposeGraph
            |> TransitiveReduction.reduce
            |> costOfModules
        let aSize = 1
        let bSize = 2
        let cSize = 3
        Assert.IsTrue(graph.costMap.[0].Length = aSize)
        Assert.IsTrue(graph.costMap.[1].Length = bSize)
        Assert.IsTrue(graph.costMap.[2].Length = cSize)

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
            |> createTransposeGraph
            |> costOfModules
        let aSize = 1
        let bSize = 2
        let cSize = 3
        let dSize = 2
        let eSize = 4
        Assert.IsTrue(graph.costMap.[0].Length = aSize)
        Assert.IsTrue(graph.costMap.[1].Length = bSize)
        Assert.IsTrue(graph.costMap.[2].Length = cSize)
        Assert.IsTrue(graph.costMap.[3].Length = dSize)
        Assert.IsTrue(graph.costMap.[4].Length = eSize)

    [<TestMethod>]
    member this.``TestSimple2 with reduction`` () =
        let graph =
            [|
                [|"a"; "b"; "c"; "d"; "e"|];
                [|"b"; "c"; "e"|];
                [|"c"; "e"|];
                [|"d"|];
                [|"e"|];
            |]
            |> createTransposeGraph
            |> TransitiveReduction.reduce
            |> costOfModules
        let aSize = 1
        let bSize = 2
        let cSize = 3
        let dSize = 2
        let eSize = 4
        Assert.IsTrue(graph.costMap.[0].Length = aSize)
        Assert.IsTrue(graph.costMap.[1].Length = bSize)
        Assert.IsTrue(graph.costMap.[2].Length = cSize)
        Assert.IsTrue(graph.costMap.[3].Length = dSize)
        Assert.IsTrue(graph.costMap.[4].Length = eSize)