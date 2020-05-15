# Lessons I learnt from optimizing a graph algorithm in F#

In this article I'm going to share how I optimized a algorithms interview question down from **58s** down to **1.2s**, nearly a 50x improvement! *Spoiler alert*: exploit **cache efficiency**, and **do less work**. 

For those unaware, F# is a *functional-first language that runs on the .NET platform (think C#)*. Despite the title of the article, very little of the optimization techniques applies solely to F#, and so I have intentionally wrote this article to make it comprehensible **even if** you don't understand a line of code in the article. 

*Why F#*? All significant optimizations come from *careful profiling* and *significant refactoring*, and F# has very good support for both. F#'s strong type system makes refactoring safe, and Visual Studio integration makes profiling easy. Also, I love that I get highly tuned data structures via .NET to speed up my algorithms. 

## The question

As with all interview questions, they usually come with a good 'ol blurb which attempts to give some context to the horridly abstract question you're expected to comprehend. Here's my attempt at paraphrasing it (note that all this is original writing, nothing copied):

Applications in 2020 typically have hundreds of dependencies, all of which need to be compiled. The downside is, changing one module might cause all the downstream modules to be recompiled. The task is as follows: given a set of 2000 modules, all of which depend on each other in some way, print the number of modules downstream of each module.

The input will look like the following. In this representation, module A depends on B, E, and F. As a result, module A is downstream of module B (and E,F too).

```
A B E F
B D
C B D E
D E
E
F D
```

This corresponds to the following dependency graph:
```
            E
            |
            D
           / \
          B   F
        /  \ /
        C   A
```
Notice that not all edges are drawn, in particular any transitive edges are not drawn. This will come in useful later.

This program will produce the following output, corresponding to the number of downstream modules (including itself):
```
A 1
B 3
C 1
D 5
E 6
F 2
```

## The original solution

The exact solution isn't super critical, so I won't go into too much depth (as well as not to spoil the fun for those new to the question!). Basically, it involves building a Directed Acyclic Graph (DAG) out of the list of lists, and then running a Depth-First Search in order to identify the number of children under each node. 

Before a recursive call to the DFS on some node returns, it updates a shared hashtable, to indicate which children are under the node. For instance, in the tree:
```
            E
            |
            D
           / \
          B   F
        /  \ /
        C   A
```
When the call to node B returns, the map will contain {B: {B, C, A}}. 

With this information, finding the child nodes under a node is as simple as calling the DFS on each child node, then merging the set of all its child nodes. 

When I first implemented this algorithm in F# a year back, it worked very well, but took 55s for a large dataset. And honestly, that was fine. It solved the problem! Problem solving is always the priority. 

But that's not enough for you, dear reader. You want to know how to squeeze that last drop of performance, and that's what the rest of this article is about.


## Performance with Data Structures 
As [Chandler Carruth](https://www.youtube.com/watch?v=fHNmRkzxHWs) puts it, Data Structures for performance, Algorithms for efficiency. The thing that made the most difference in my F# code was **Data structures, Data structures, Data structures**. 

When I first wrote my F# code, it was filled with Maps and Lists. Now, that was very good F# code: *idiomatic* and *easy to read*, but it also meant that it was **really slow**. As you may be aware, F# maps are implemented as Binary Trees, which means that insert and search are O(log n) time operations. The hot path of my application involved a lot of Map lookups, and so I changed the F# maps to .NET dictionaries, which have O(1) lookup time. 

Similarly, I changed F#'s sets to HashSets, and changed the F#'s lists (which are linked lists) to .NET arrays. Changing immutable collections to mutable ones for performance is not surprising in itself, but it's important to remember that mutablity makes the code harder to reason about. 

So, I kept the Maps and Lists around in the parsing section of the code, which my benchmarking showed only took less than half a second - recall that the merge step took almost a *full minute*! 

We'll get around to optimizing that later on, but as always, profile your code and only make life more complex if you have to.

Here's a snippet to demonstrate what I mean. The old code is presented first, which uses F#'s sets, maps and lists, and the new code is below. 

This does a Depth-First Search (DFS) to obtain the set of child nodes that are under each parent node. There's obviously a lot to explain in this code, which I won't go into, but here's a summary of the Data Structures changed:

  - Adjacency lists (Map from string to Set\<string\>) got changed to a 2D adjacency matrix of bools
  - The cost Map, which used to be a Map from string to string, got changed to a Dictionary from int to int[]. Note that ChildNodes is an alias for int[], I will explain this unusual coding style later.

```F#
// old code
type Digraph =
    {nodes: string list;
    adjacency: Map<string, Set<string>>;}

// Note: graph is a Digraph in this function,
// because this function is actually a closure
let countNumChildren node (costMap: Map<string, Set<string>>) =
    let mutable costMap = costMap
    let rec dfs node =
        match Map.tryFind node costMap with
        | Some (s) -> s
        | None ->
            match Map.tryFind node graph.adjacency with
            | None ->
                let s = Set.add node Set.empty
                costMap <- Map.add node s costMap
                s
            | Some(lst) ->
                let s =
                    lst
                    |> List.map dfs
                    |> List.reduce (Set.union)
                    |> Set.add node
                costMap <- Map.add node s costMap
                s
    dfs node |> ignore
    costMap
```
```F#
// New code - uses Dictionaries and 2D arrays
type Digraph =
    {size: int;
    labels: Map<string, int>;
    adjacency: bool[,];
    costMap: Dictionary<int, ChildNodes>}

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
```

If you've been following closely, then you'll notice one change which I didn't mention yet. How come the Maps used to go from string to string, but now go from int to int[]? 

That leads me to my second major perf improvement - **Not using strings**. Strings are basically blobs of memory that have to be yanked out from all over the heap, that the GC has to care about, and they have to be hashed (that takes time!) If you can avoid using strings, then don't.

In my application, I was using strings as the names of each of the nodes in the graph, and I changed every use of a string to an int. Now, the strings as names for the nodes still had to be kept around, since I needed to print the nodes at the end, so I simply kept a Map called "labels" around that would let me know which strings corresponded to which ints. 

Notice that I kept this as an F# map, despite everything I said earlier, since it isn't involved in the performance critical sections. Remember, mutability means more work for the reader.

With these two perf improvements, I was able to get my total time down from *58s* to *5.5s*, an **10x** improvement!

## Efficiency with Algorithms

Now at this point I had spent about three days writing the improved code, and I was feeling really good. All that stuff about exploiting cache efficiency was paying off, and I felt that I could get it under a second by Monday. The next step, though, wasn't so easy. The bottleneck proved to be calculating the set of child nodes under each parent.

Right now, I was storing each set of child nodes as an int[]. To calculate the set of all nodes under a particular *parent* node, I would recursively compute the set of all nodes under each of the *parent*'s children, then use a HashSet to merge them together. Repeated efforts to improve the code proved futile, shaving only milliseconds off. 

```F#
module ChildNodes = 
    let build (lst: int[][]): int[] =
        let d = HashSet()
        for s in lst do
            d.UnionWith s
        d |> Seq.toArray
```

Now what? Well, after some testing, I noticed a big problem. **Most edges in the graph are useless**. To show you what I mean, let's take a very simple example of a graph, where A depends on B, which depends on C.
```
A <- B <- C
// represented as:
A B C
B C
C
```
Looking at this graph, to calculate the nodes under C, it suffices to calculate the nodes under B. But under my current scheme, I would calculate the nodes under B **and nodes under A**, then merge those two together! Obviously, this is unnecessary work, since whatever is under A will also be under B. 

Let's take a more real-world example to see this in action:
( insert graph drawing here )

As you can see, in this graph there are many **redundant edges**. In particular, we have something called *transitive redundancy*, i.e. if I can get from A to B in two or more steps, then I shouldn't have an edge from A to B at all!

This problem is closely related to the problem of a *transitive closure*. In a transitive closure, we take some DAG and add in all the transitive edges. That is, if I can go from *A to B* and *B to C*, then I should also be able to go from *A to C*.

We need to do what's called a Transitive Reduction, going in the opposite direction.

If you've taken a Discrete Mathematics class (spoiler: I used to teach one), you'll know that there exists an O(n^3) algorithm for computing Transitive Closures, called [Warshall's algorithm](https://cs.winona.edu/lin/cs440/ch08-2.pdf). I won't go too much into depth as the linked PDF explains it well.

Instead, we're going to use a much lesser known algorithm called [Gries' algorithm](http://www.sciencedirect.com/science/article/pii/0167642389900397/pdf?md5=478ed0e9fa69b427f947fd2bd864b463&pid=1-s2.0-0167642389900397-main.pdf&_valck=1)[1] to compute the transitive reduction of a graph. It basically runs Warshall's algorithm in reverse, removing transitive edges from the graph as it finds them. Again, read the paper for more details.

Here is my implementation of Gries' algorithm:
```F#
// Implements the transitve reduction algorithm
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
```
A few words on this algorithm. When I first implemented it *as-is* from the paper, I found that the total runtime shot up from 5.5s to **12s**! That was really disheartening, since I was sure that implementing a better algorithm was the key to speeding this algorithm up. 

Nonetheless, with a few performance improvements, I managed to get the program to run at **1.4s**, another order of magnitude improvement! Firstly, I implemented the middle loop as a parallel for loop, which helped to speed things up a little[2].

But the main improvement came by avoiding work (_again_). Notice that I do an **if true** check on adj.[i,k] before I run the innermost loop. If that is false, we skip a whole inner loop of work. That's **2000** iterations saved - No wonder the speed up was an order of magnitude!

Compare the contrast to the old version (some omissions):
```F#
let reduceOld (graph: Digraph): Digraph =
    for k = n-1 downto 0 do
        for i in 0..n-1 do
            for j in 0..n-1 do
                if adj.[i, k] && adj.[k, j] then
                    adj.[i, j] <- false

let reduce (graph: Digraph): Digraph =
    for k = n-1 downto 0 do
        Parallel.For(0, n,
            (fun i ->
                if adj.[i, k] then
                    for j in 0..n-1 do
                        if adj.[k, j] then
                            adj.[i, j] <- false
            )
        ) |> ignore
```

## Squeezing the last drop of performance

## Failed ideas

## Footnotes
1. An algorithm for transitive reduction of an acyclic graph (1987). Gries, D., Martin, A. J. et al
2. You may notice that I'm performing parallel reads and writes to a shared mutable data structure, namely the 2D adjacency matrix. While this is normally highly discouraged, in this case it is fine, since the reads and writes are guaranteed to never overlap. That said, although conceptually it makes sense (each thread has a different row), it is not obvious that it holds at the processor level. 

To verify this, I dug into the System.Corelib.Private source code to check how they perform writes on a 2D array. The fear would be that if arr.[i, 2000] and arr.[i+1, 0] overlap. This can only happen if the CLR isn't addressing each boolean by itself, but rather batching boolean reads and writes. Thankfully, the CLR uses regular unsafe pointer arithmetic to get and set booleans. Since most modern processors are byte-addressable, this means that there is no overlap. Phew! These are the details one has to worry about when performing lock-free reads and writes to shared mutable state.
