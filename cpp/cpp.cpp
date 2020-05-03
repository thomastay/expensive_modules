#include <vector>
#include <string>
#include <unordered_map>
#include <map>
#include <unordered_set>
#include <algorithm>
#include <iostream>
#include <iterator>
#include <sstream>

using namespace std;
using CostMap = map<string, unordered_set<string>>;

struct Digraph {
    vector<string> nodes;
    unordered_map<string, vector<string>> adjacency;
};

Digraph create_transpose_graph(const vector<vector<string>>& adj)
{
    Digraph digraph;
    for (auto& lst : adj) {
        digraph.nodes.push_back(lst.front()); // unique node
        for (int i = 1; i < (int)lst.size(); ++i) {
            digraph.adjacency[lst[i]].push_back(lst.front());
        }
    }
    return digraph;
}

void dfs(const Digraph& digraph, CostMap& cost_map, const string& node)
{
    auto it = cost_map.find(node);
    if (it != cost_map.end()) return;
    // not found
    auto it2 = digraph.adjacency.find(node);
    if (it2 == digraph.adjacency.end()) {
        // Node is a leaf
        cost_map[node].insert(node);
    }
    else {
        // Node has children
        unordered_set<string> s;
        auto& children = digraph.adjacency.at(node);
        for (auto& child : children) {
            dfs(digraph, cost_map, child);
            auto& grand_children = cost_map[child];
            for (auto& grand_child : grand_children) {
                s.insert(grand_child);
            }
        }
        s.insert(node);
        cost_map[node] = s;
    }
}

void cost_of_modules(const Digraph&& digraph)
{
    CostMap cost_map;
    for (auto& node : digraph.nodes) {
        auto it = cost_map.find(node);
        if (it == cost_map.end()) {
            dfs(digraph, cost_map, node);
        }
    }

    for (auto& cost : cost_map) {
        cout << cost.first << ", " << cost.second.size() << endl;
    }
}


int main()
{
    int numLines;
    cin >> numLines;
    string temp;
    getline(cin, temp);
    vector<vector<string>> sll;
    for (int i = 0; i < numLines; ++i) {
        string s;
        getline(cin, s);
        istringstream iss(s);
        vector<string> nodes{ istream_iterator<string> {iss}, istream_iterator<string>{} };
        sll.push_back(nodes);
    }
    cost_of_modules(create_transpose_graph(sll));
}
