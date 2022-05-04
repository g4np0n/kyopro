using System;
using System.Collections.Generic;
using System.Linq;

class Dijkstra
{
    public Dijkstra(int V)
    {
        _V = V;
        _G = new List<(int to, long dist)>[V];
        for (int i = 0; i < V; i++) _G[i] = new List<(int to, long dist)>();
    }

    long _INF = long.MaxValue;
    int _V;
    List<(int to, long dist)>[] _G;
    int[] _from;

    /// <summary>
    /// 無向辺を追加します。
    /// </summary>
    public void AddEdge(int u, int v, long dist)
    {
        _G[u].Add((v, dist));
        _G[v].Add((u, dist));
    }

    /// <summary>
    /// 有向辺を追加します。
    /// </summary>
    public void AddDirectedEdge(int from, int to, long dist)
    {
        _G[from].Add((to, dist));
    }

    /// <summary>
    /// 各頂点への最短経路長を求めます。
    /// 計算量はO((E+V)logV)です。
    /// </summary>
    public long[] GetDist(int s)
    {
        var dist = new long[_V];
        dist.AsSpan().Fill(_INF);
        _from = new int[_V];
        _from.AsSpan().Fill(-1);
        var pq = new PriorityQueue<int>();
        dist[s] = 0;
        pq.Enqueue(0, s);
        while (pq.Count > 0)
        {
            var (d, v) = pq.Dequeue();
            if (dist[v] != d) continue;
            foreach (var edge in _G[v])
            {
                var alt = d + edge.dist;
                if (alt >= dist[edge.to]) continue;
                dist[edge.to] = alt;
                _from[edge.to] = v;
                pq.Enqueue(alt,edge.to);
            }
        }
        for (int i = 0; i < _V; i++) if (dist[i] == _INF) dist[i] = -1;
        return dist;
    }
}

class Prim
{
    public Prim(int V)
    {
        _V = V;
        _G = new List<(int to, long dist)>[V];
        for (int i = 0; i < V; i++) _G[i] = new List<(int to, long dist)>();
    }

    long _INF = long.MaxValue;
    int _V;
    List<(int to, long dist)>[] _G;
    int[] from;

    /// <summary>
    /// 無向辺を追加します。
    /// </summary>
    public void AddEdge(int u, int v, long dist)
    {
        _G[u].Add((v, dist));
        _G[v].Add((u, dist));
    }

    /// <summary>
    /// <para>rootを含む最小全域木を構築し、距離の総和を求めます。</para>
    /// <para>計算量はO(V+(E'+V')logV')です。</para>
    /// <para>ただし、V'は頂点rootの連結成分の数、E'は連結成分同士を結ぶ辺の数とします。</para>
    /// </summary>
    public long GetDistOfMST(int root)
    {
        var dist = new long[_V];
        dist.AsSpan().Fill(_INF);
        from = new int[_V];
        from.AsSpan().Fill(-1);
        var used = new bool[_V];
        dist[root] = 0;
        var queue = new PriorityQueue<int>();
        queue.Enqueue(0,root);
        long sum = 0;
        while (queue.Count > 0)
        {
            var (d,v) = queue.Dequeue();
            if (used[v]) continue;
            used[v] = true;
            sum += d;
            foreach (var next in _G[v])
            {
                if (dist[next.to] <= next.dist || used[next.to]) continue;
                dist[next.to] = next.dist;
                from[next.to] = v;
                queue.Enqueue(next.dist,next.to);
            }
        }
        return dist.Contains(_INF)?-1:sum;
    }

    /// <summary>
    /// 構築した最小全域木の辺の組み合わせを返します。
    /// </summary>
    public IEnumerable<(int, int)> GetPaths()
    {
        for (int i = 0; i < _V; i++) yield return (i, from[i]);
    }
}

class TopologicalSort
{
    public TopologicalSort(int V)
    {
        _G = new List<int>[V];
        for (int i = 0; i < V; i++) _G[i] = new List<int>();
        _indegree = new int[V];
    }

    List<int>[] _G;
    int[] _indegree;

    /// <summary>
    /// 無向辺を追加します。
    /// </summary>
    public void AddDirectedEdge(int from, int to)
    {
        _G[from].Add(to);
        _indegree[to]++;
    }

    /// <summary>
    /// トポロジカルソートを実行します。
    /// 計算量はO(V+E)です。
    /// </summary>
    /// <returns></returns>
    public IList<int> Sort()
    {
        var result = new List<int>();
        var queue = new Queue<int>();
        for (int i = 0; i < _G.Length; i++)
        {
            if (_indegree[i] == 0) queue.Enqueue(i);
        }
        while (queue.Count > 0)
        {
            var now = queue.Dequeue();
            foreach (var next in _G[now])
            {
                _indegree[next]--;
                if (_indegree[next] == 0) queue.Enqueue(next);
            }
            result.Add(now);
        }
        return result;
    }
}

class StronglyConnectedComponent
{
    public StronglyConnectedComponent(int V)
    {
        _V = V;
        _G = new List<int>[V];
        _rG = new List<int>[V];
        for (int i = 0; i < V; i++)
        {
            _G[i] = new List<int>();
            _rG[i] = new List<int>();
        }
        _compo = new int[V];
        _compo.AsSpan().Fill(-1);
        _order = new List<int>();
        _used = new bool[V];
        _nodes = new List<HashSet<int>>();
    }

    int _V;
    List<int>[] _G, _rG;
    int[] _compo;
    List<int> _order;
    bool[] _used;
    List<HashSet<int>> _nodes;

    /// <summary>
    /// 有効辺を追加します。
    /// </summary>
    public void AddDirectedEdge(int from, int to)
    {
        _G[from].Add(to);
        _rG[to].Add(from);
    }

    void nDFS(int now)
    {
        if (_used[now]) return;
        _used[now] = true;
        foreach (var next in _G[now]) nDFS(next);
        _order.Add(now);
    }

    void rDFS(int now, int count)
    {
        if (_compo[now] != -1) return;
        _compo[now] = count;
        if (_nodes.Count <= count) _nodes.Add(new HashSet<int>());
        _nodes[count].Add(now);
        foreach (var next in _rG[now]) rDFS(next, count);
    }

    /// <summary>
    /// 強連結成分分解を実行します。計算量はO(E+V)です。
    /// </summary>
    /// <returns></returns>
    public List<(int from, int to)> Build()
    {
        for (int i = 0; i < _V; i++) nDFS(i);
        int groupNum = 0;
        foreach (var i in _order.AsEnumerable().Reverse())
        {
            if (_compo[i] != -1) continue;
            rDFS(i, groupNum);
            groupNum++;
        }
        var res = new List<(int from, int to)>();
        for (int i = 0; i < _V; i++)
        {
            foreach (var next in _G[i])
            {
                var s = _compo[i];
                var t = _compo[next];
                if (s != t) res.Add((s, t));
            }
        }
        return res;
    }

    /// <summary>
    /// 指定したノードを含む強連結成分のidを取得します。
    /// </summary>
    public int this[int k] { get { return _compo[k]; } }

    /// <summary>
    /// 強連結成分に含まれるノードの集合を取得します。
    /// </summary>
    public HashSet<int> GetNodes(int compo_id) => _nodes[compo_id];
}

class BipartiteGraph
{
    public BipartiteGraph(int V)
    {
        _V = V;
        _uf = new UnionFind(V + V + 1);
        _uf.Merge(0, V + V);
        IsRed = new bool[V];
    }

    int _V;
    UnionFind _uf;
    bool[] IsRed;

    public void AddEdge(int u, int v)
    {
        _uf.Merge(u, v + _V);
        _uf.Merge(u + _V, v);
    }

    public bool Coloring()
    {
        for (int i = 0; i < _V; i++)
        {
            if (_uf.Same(i, i + _V)) return false;
            IsRed[i] = _uf.Same(i, _V + _V);
        }
        return true;
    }

    public bool this[int i] { get { return IsRed[i]; } }
}

class Dinic
{
    public Dinic(int node_size)
    {
        _V = node_size;
        _G = new List<Edge>[_V];
        for (int i = 0; i < _V; i++) _G[i] = new List<Edge>();
        _level = new int[_V];
        _iter = new int[_V];
    }

    class Edge
    {
        public Edge(int to, long cap, int rev)
        {
            To = to; Cap = cap; Rev = rev;
        }
        public int To { get; set; }
        public long Cap { get; set; }
        public int Rev { get; set; }
    }

    List<Edge>[] _G;
    int _V;
    int[] _level;
    int[] _iter;
    const long _INF = long.MaxValue / 3;

    public void AddDirectedEdge(int from, int to, int cap)
    {
        _G[from].Add(new Edge(to, cap, _G[to].Count));
        _G[to].Add(new Edge(from, 0, _G[from].Count - 1));
    }

    public long MaxFlow(int s, int t)
    {
        long flow = 0;
        while (true)
        {
            BFS(s);
            if (_level[t] < 0) { return flow; }
            _iter = new int[_V];
            var f = DFS(s, t, _INF);
            while (f > 0)
            {
                flow += f;
                f = DFS(s, t, _INF);
            }
        }
    }

    void BFS(int s)
    {
        _level = new int[_V];
        _level.AsSpan().Fill(-1);
        _level[s] = 0;
        var que = new Queue<int>();
        que.Enqueue(s);
        while (que.Count != 0)
        {
            var v = que.Dequeue();
            for (int i = 0; i < _G[v].Count; i++)
            {
                var e = _G[v][i];
                if (e.Cap > 0 && _level[e.To] < 0)
                {
                    _level[e.To] = _level[v] + 1;
                    que.Enqueue(e.To);
                }
            }
        }
    }

    long DFS(int v, int t, long f)
    {
        if (v == t) return f;
        for (int i = _iter[v]; i < _G[v].Count; i++)
        {
            _iter[v] = i;
            var e = _G[v][i];
            if (e.Cap > 0 && _level[v] < _level[e.To])
            {
                var d = DFS(e.To, t, Math.Min(f, e.Cap));
                if (d > 0)
                {
                    e.Cap -= d;
                    _G[e.To][e.Rev].Cap += d;
                    return d;
                }
            }
        }
        return 0;
    }
}

class LowestCommonAncestor
{
    int _V;
    int[][] _parent;
    int[] _depth;
    List<int>[] _G;

    public LowestCommonAncestor(int V)
    {
        _V = V;
        _G = new List<int>[V];
        for (int i = 0; i < V; i++) _G[i] = new List<int>();
    }

    /// <summary>
    /// 無向辺を追加します。
    /// </summary>
    public void AddEdge(int u, int v)
    {
        _G[u].Add(v);
        _G[v].Add(u);
    }

    /// <summary>
    /// ダブリングテーブルを構築します。
    /// 計算量はO(VlogV)です。
    /// </summary>
    public void Doubling(int root)
    {
        int K = 1;
        while ((1 << K) < _V) K++;
        _parent = new int[_V][];
        for (int i = 0; i < _V; i++) _parent[i] = new int[K];
        _depth = new int[_V];
        _depth.AsSpan().Fill(-1);
        DFS(root, -1, 0);

        for (int k = 0; k + 1 < K; k++)
        {
            for (int v = 0; v < _V; v++)
            {
                _parent[v][k + 1] = _parent[v][k] < 0 ? -1 : _parent[_parent[v][k]][k];
            }
        }

        void DFS(int now, int parent, int depth)
        {
            _parent[now][0] = parent;
            _depth[now] = depth;
            foreach (var next in _G[now])
            {
                if (next != parent) DFS(next, now, depth + 1);
            }
        }
    }

    /// <summary>
    /// 2点の最小共通祖先を求めます。
    /// 計算量はO(logV)です
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public int Query(int a, int b)
    {
        if (_depth[a] < _depth[b]) (a, b) = (b, a);

        var K = _parent[0].Length;
        for (int k = 0; k < K; k++)
        {
            if ((((_depth[a] - _depth[b]) >> k) & 1) == 1)
            {
                a = _parent[a][k];
            }
        }
        if (a == b) return a;
        for (int k = K - 1; k >= 0; k--)
        {
            if (_parent[a][k] != _parent[b][k])
            {
                a = _parent[a][k];
                b = _parent[b][k];
            }
        }
        return _parent[a][0];
    }

    /// <summary>
    /// 2点間の距離を求めます。
    /// 計算量はO(logV)です。
    /// </summary>
    public int GetDist(int a, int b) => _depth[a] + _depth[b] - 2 * _depth[Query(a, b)];

    /// <summary>
    /// 頂点xが2点を結ぶパス上に存在するか判定します。
    /// 計算量はO(logV)です。
    /// </summary>
    public bool IsOnPath(int a, int b, int x) => GetDist(a, x) + GetDist(x, b) == GetDist(a, b);
}

