using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

/// <summary>
/// UnionFindです。
/// </summary>
class UnionFind
{
    int[] parents;
    public int TreeCount { get; private set; }
    public UnionFind(int n)
    {
        parents = new int[n];
        for (int i = 0; i < n; i++)
        {
            parents[i] = -1;
        }
        TreeCount = n;
    }

    /// <summary>
    /// 所属している集合の代表要素を返します。
    /// </summary>
    public int Find(int x) => parents[x] < 0 ? x : parents[x] = Find(parents[x]);

    /// <summary>
    /// 所属している集合の要素数を返します。
    /// </summary>
    public int Size(int x) => -parents[Find(x)];

    /// <summary>
    /// 2つの要素が同一の集合に属しているか判定します。
    /// </summary>
    public bool Same(int x, int y) => Find(x) == Find(y);

    /// <summary>
    /// 2つの要素が所属している集合をマージする。
    /// マージしたかどうかを真偽値で返す。
    /// </summary>
    public bool Merge(int x, int y)
    {
        x = Find(x);
        y = Find(y);
        if (x == y) return false;

        if (Size(x) < Size(y)) (x, y) = (y, x);
        parents[x] += parents[y];
        parents[y] = x;
        TreeCount--;
        return true;
    }
}

/// <summary>
/// 抽象化UnionFind
/// </summary>
/// <typeparam name="T"></typeparam>
class UnionFind<T>
{
    int[] parents;
    public int TreeCount { get; private set; }
    T[] values;
    Func<T, T, T> merge;
    public UnionFind(int n, Func<T, T, T> merge, Func<int, T> init)
    {
        parents = new int[n];
        values = new T[n];
        this.merge = merge;
        for (int i = 0; i < n; i++)
        {
            parents[i] = -1;
            values[i] = init(i);
        }
        TreeCount = n;
    }

    public int Find(int x)
    {
        if (parents[x] < 0) return x;
        return parents[x] = Find(parents[x]);
    }

    public int Size(int x)
    {
        return -parents[Find(x)];
    }

    public bool Same(int x, int y)
    {
        return Find(x) == Find(y);
    }

    public bool Merge(int x, int y)
    {
        x = Find(x);
        y = Find(y);
        if (x == y) return false;

        //サイズの大きい方をxとして、xにyをマージする
        if (Size(x) < Size(y))
        {
            var tmp = x;
            x = y;
            y = tmp;
        }
        parents[x] += parents[y];
        parents[y] = x;
        values[x] = merge(values[x], values[y]);
        TreeCount--;
        return true;
    }

    public T Get(int x) => values[Find(x)];
}

//keyがlong keyが最小のものを返す
class PriorityQueue<T>
{
    /// <summary>
    /// 空の優先度付きキューを生成します。
    /// </summary>
    public PriorityQueue()
    {
        _keys = new List<long>();
        _elements = new List<T>();
    }
    List<long> _keys;
    List<T> _elements;

    /// <summary>
    /// 優先度付きキューに要素を追加します。
    /// 計算量は O(log(要素数)) です。
    /// </summary>
    public void Enqueue(long key, T elem)
    {
        var n = _elements.Count;
        _keys.Add(key);
        _elements.Add(elem);
        while (n != 0)
        {
            var i = (n - 1) / 2;
            if (_keys[n] < _keys[i])
            {
                (_keys[n], _keys[i]) = (_keys[i], _keys[n]);
                (_elements[n], _elements[i]) = (_elements[i], _elements[n]);
            }
            n = i;
        }
    }

    /// <summary>
    /// 頂点要素を返し、削除します。
    /// 計算量は O(log(要素数)) です。
    /// </summary>
    public (long, T) Dequeue()
    {
        var t = Peek;
        Pop();
        return t;
    }

    void Pop()
    {
        var n = _elements.Count - 1;
        _elements[0] = _elements[n];
        _elements.RemoveAt(n);
        _keys[0] = _keys[n];
        _keys.RemoveAt(n);
        for (int i = 0, j; (j = 2 * i + 1) < n;)
        {
            //左の子と右の子で右の子の方が優先度が高いなら右の子を処理したい
            if ((j != n - 1) && _keys[j] > _keys[j + 1]) j++;
            //親より子が優先度が高いなら親子を入れ替える
            if (_keys[i] > _keys[j])
            {
                (_keys[i], _keys[j]) = (_keys[j], _keys[i]);
                (_elements[i], _elements[j]) = (_elements[j], _elements[i]);
            }
            i = j;
        }
    }


    /// <summary>
    /// 頂点要素を返します。
    /// 計算量は O(1) です。
    /// </summary>
    public (long, T) Peek => (_keys[0], _elements[0]);

    /// <summary>
    /// 優先度付きキューに格納されている要素の数を返します。
    /// 計算量は O(1) です。
    /// </summary>
    public int Count => _elements.Count;
}

/// <summary>
/// 優先度付きキューです。
/// </summary>
class PriorityQueueGeneric<T>
{
    /// <summary>
    /// 空の優先度付きキューを生成します。
    /// </summary>
    /// <param name="func">(first,second)の優先度を決定する関数</param>
    public PriorityQueueGeneric(Func<T, T, bool> func)
    {
        elements = new List<T>();
        compare = func;
    }

    Func<T, T, bool> compare;
    List<T> elements;

    /// <summary>
    /// 優先度付きキューに要素を追加します。
    /// 計算量は O(log(要素数)) です。
    /// </summary>
    public void Enqueue(T elem)
    {
        int n = elements.Count;
        elements.Add(elem);
        while (n != 0)
        {
            int i = (n - 1) / 2;
            //T1がT2より優先度が高ければ交換
            if (compare(elements[n], elements[i]))
            {
                T temp = elements[n];
                elements[n] = elements[i];
                elements[i] = temp;
            }
            n = i;
        }
    }

    /// <summary>
    /// 頂点要素を返し、削除します。
    /// 計算量は O(log(要素数)) です。
    /// </summary>
    public T Dequeue()
    {
        var t = Peek;
        Pop();
        return t;
    }

    void Pop()
    {
        int n = elements.Count - 1;
        elements[0] = elements[n];
        elements.RemoveAt(n);
        for (int i = 0, j; (j = 2 * i + 1) < n;)
        {
            //左の子と右の子で右の子の方が優先度が高いなら右の子を処理したい
            if ((j != n - 1) && compare(elements[j], elements[j + 1]) == false) { j++; }
            //親より子が優先度が高いなら親子を入れ替える
            if (compare(elements[i], elements[j]) == false)
            {
                T temp = elements[j];
                elements[j] = elements[i];
                elements[i] = temp;
            }
            i = j;
        }
    }


    /// <summary>
    /// 頂点要素を返します。
    /// 計算量は O(1) です。
    /// </summary>
    public T Peek => elements[0];

    /// <summary>
    /// 優先度付きキューに格納されている要素の数を返します。
    /// 計算量は O(1) です。
    /// </summary>
    public int Count => elements.Count;
}

/// <summary>
/// セグメントツリーです。
/// </summary>
class SegmentTree<T>
{
    /// <summary>
    /// セグ木のコンストラクタ
    /// </summary>
    /// <param name="length">列の長さです。</param>
    /// <param name="funcMerge">モノイドが満たす二項演算です。</param>
    /// <param name="funcUpdate">要素を更新する演算です。(元の値,作用させる値)=>更新後の値</param>
    /// <param name="unit">モノイドの単位元です。</param>
    public SegmentTree(int length, Func<T, T, T> funcMerge, Func<T, T, T> funcUpdate, T unit)
    {
        int tmp = 1;
        while (length > tmp) tmp <<= 1; ;
        length = tmp;
        _node = new T[length << 1];
        _node.AsSpan().Fill(unit);
        _length = length;
        _funcMerge = funcMerge;
        _funcUpdate = funcUpdate;
        _unit = unit;
    }

    /// <summary>
    /// セグ木のコンストラクタ
    /// </summary>
    /// <param name="ls">セグ木に初期値としてコピーする配列です。</param>
    /// <param name="funcMerge">モノイドが満たす二項演算です。</param>
    /// <param name="funcUpdate">要素を更新する演算です。(元の値,作用させる値)=>更新後の値</param>
    /// <param name="unit">モノイドの単位元です。</param>
    public SegmentTree(IList<T> ls, Func<T, T, T> funcMerge, Func<T, T, T> funcUpdate, T unit)
    {
        var length = ls.Count;
        int tmp = 1;
        while (length > tmp) tmp <<= 1;
        length = tmp;
        _node = new T[length << 1];
        _node.AsSpan().Fill(unit);
        for (int i = 0; i < ls.Count; i++) _node[length + i] = ls[i];
        _length = length;
        _funcMerge = funcMerge;
        _funcUpdate = funcUpdate;
        _unit = unit;
    }

    readonly int _length;
    T[] _node;
    readonly Func<T, T, T> _funcMerge;
    readonly Func<T, T, T> _funcUpdate;
    readonly T _unit;

    /// <summary>
    /// セグ木の葉に要素をセットします。
    /// </summary>
    /// <param name="i">位置</param>
    /// <param name="x">セットする要素</param>
    public void Set(int i, T x) { _node[i + _length] = x; }

    /// <summary>
    /// セグ木を構築します。
    /// </summary>
    public void Build()
    {
        for (int k = _length - 1; k > 0; k--)
        {
            _node[k] = _funcMerge(_node[k << 1], _node[k << 1 | 1]);
        }
    }

    /// <summary>
    /// 1点に対して演算を行い、セグ木の更新を行います。
    /// </summary>
    /// <param name="i">演算を行う位置</param>
    /// <param name="x">作用させる要素</param>
    public void Update(int i, T x)
    {
        i += _length;
        _node[i] = _funcUpdate(_node[i], x);
        while (i > 1)
        {
            i >>= 1;
            _node[i] = _funcMerge(_node[i << 1], _node[(i << 1) | 1]);
        }
    }

    /// <summary>
    /// 区間[l,r)でマージを実行した結果を返します。
    /// </summary>
    public T Range(int l, int r)
    {
        var tl = _unit;
        var tr = _unit;
        l += _length; r += _length;
        while (l < r)
        {
            if ((l & 1) == 1) tl = _funcMerge(tl, _node[l++]);
            if ((r & 1) == 1) tr = _funcMerge(_node[--r], tr);
            l >>= 1; r >>= 1;
        }
        return _funcMerge(tl, tr);
    }

    /// <summary>
    /// 全要素をマージした結果を返します。
    /// </summary>
    public T All() => _node[1];

    /// <summary>
    /// i番目の要素を取得します。
    /// </summary>
    public T this[int i] => _node[i + _length];

    /// <summary>
    /// [l,r)で要素が制約を満たす最小のインデックスを返します。
    /// 制約を満たす要素が存在しない場合rを返します。
    /// </summary>
    public int MinLeft(int l, int r, Func<T, bool> constraint)
    {
        //指定区間に制約を満たす要素が存在しないとき,rを返す
        if (constraint(Range(l, r)) == false) return r;
        //指定区間左から制約を満たすまでマージしていく
        l += _length;
        var now = _unit;
        while (true)
        {
            //見ているノードが制約を満たすとき
            if (constraint(_funcMerge(now, _node[l])))
            {
                //葉ならインデックスを返す
                if (l >= _length) return l - _length;
                //そうでないなら左の子を見る
                else l <<= 1; ;
            }
            //見ているノードが制約を満たさないとき
            else
            {
                //制約を満たしていないのでマージ
                now = _funcMerge(now, _node[l]);
                //左のノードなら右へ
                if ((l & 1) == 0) l++;
                //右のノードなら右の親へ
                else l = (l >> 1) + 1;
            }
        }
    }

    /// <summary>
    /// (l,r]で要素が制約を満たす最大のインデックスを返します。
    /// 制約を満たす要素が存在しない場合lを返します。
    /// </summary>
    public int MaxRight(int l, int r, Func<T, bool> constraint)
    {
        //指定区間に制約を満たす要素が存在しないとき,lを返す
        if (constraint(Range(l + 1, r + 1)) == false) return l;
        //指定区間左から制約を満たすまでマージしていく
        r += _length;
        var now = _unit;
        while (true)
        {
            //見ているノードが制約を満たすとき
            if (constraint(_node[r]))
            {
                //葉ならインデックスを返す
                if (r >= _length) return r - _length;
                //そうでないなら右の子を見る
                else r = (r << 1) + 1;
            }
            //見ているノードが制約を満たさないとき
            else
            {
                //制約を満たしていないのでマージ
                now = _funcMerge(_node[r], now);
                //右のノードなら左へ
                if ((r & 1) == 1) r--;
                //左のノードなら左の親へ
                else r = (r >> 1) - 1;
            }
        }
    }

    public void Debug()
    {
        Console.Error.WriteLine();
        for (int i = _length; i < _length * 2; i++)
        {
            var tmp = i * 2;
            while (tmp % 2 == 0)
            {
                tmp /= 2;
                Console.Error.Write(_node[tmp] + " ");
            }
            Console.Error.WriteLine();
        }
        Console.Error.WriteLine();
    }
}

/// <summary>
/// 遅延評価セグメントツリーです。
/// </summary>
class LazySegmentTree<Tx, Tl>
{
    /// <summary>
    /// 遅延セグ木のコンストラクタ
    /// </summary>
    /// <param name="length">列の長さです。</param>
    /// <param name="funcDataMerge">モノイドが満たす二項演算です。</param>
    /// <param name="funcUpdate">要素を更新する演算です。(元の値,作用させる値,区間の長さ,区間の左端の添え字)=>更新後の値</param>
    /// <param name="funcLazyMerge">遅延要素が満たす二項演算です。</param>
    /// <param name="dataUnit">要素の単位元です。</param>
    /// <param name="lazyUnit">遅延要素の単位元です。</param>
    public LazySegmentTree(int length, Func<Tx, Tx, Tx> funcDataMerge, Func<Tx, Tl, int, int, Tx> funcUpdate, Func<Tl, Tl, Tl> funcLazyMerge, Tx dataUnit, Tl lazyUnit)
    {
        int tmp = 1;
        while (length > tmp) tmp <<= 1;
        length = tmp;
        _data = new Tx[length << 1];
        _data.AsSpan().Fill(dataUnit);
        _lazy = new Tl[length << 1];
        _lazy.AsSpan().Fill(lazyUnit);
        _existLazy = new bool[length << 1];
        _range = new (int l, int r)[length << 1];
        _range[1] = (0, length);
        for (int i = 2; i < 2 * length; i++)
        {
            if ((i & 1) == 0) _range[i] = (_range[i >> 1].l, (_range[i >> 1].r + _range[i >> 1].l) >> 1);
            else _range[i] = ((_range[i >> 1].r + _range[i >> 1].l) >> 1, _range[i >> 1].r);
        }
        _length = length;
        _funcDataMerge = funcDataMerge;
        _funcUpdate = funcUpdate;
        _funcLazyMerge = funcLazyMerge;
        _dataUnit = dataUnit;
        _lazyUnit = lazyUnit;
    }

    /// <summary>
    /// 遅延セグ木のコンストラクタ
    /// </summary>
    /// <param name="length">列の長さです。</param>
    /// <param name="funcDataMerge">モノイドが満たす二項演算です。</param>
    /// <param name="funcUpdate">要素を更新する演算です。(元の値,作用させる値,区間の長さ,区間の左端)=>更新後の値</param>
    /// <param name="funcLazyMerge">遅延要素が満たす二項演算です。</param>
    /// <param name="dataUnit">要素の単位元です。</param>
    /// <param name="lazyUnit">遅延要素の単位元です。</param>
    public LazySegmentTree(IList<Tx> ls, Func<Tx, Tx, Tx> funcDataMerge, Func<Tx, Tl, int, int, Tx> funcUpdate, Func<Tl, Tl, Tl> funcLazyMerge, Tx dataUnit, Tl lazyUnit)
    {
        var length = ls.Count;
        int tmp = 1;
        while (length > tmp) tmp <<= 1;
        length = tmp;
        _data = new Tx[length << 1];
        _data.AsSpan().Fill(dataUnit);
        for (int i = 0; i < ls.Count; i++) _data[length + i] = ls[i];
        _lazy = new Tl[length << 1];
        _lazy.AsSpan().Fill(lazyUnit);
        _existLazy = new bool[length << 1];
        _range = new (int l, int r)[length << 1];
        _range[1] = (0, length);
        for (int i = 2; i < 2 * length; i++)
        {
            if ((i & 1) == 0) _range[i] = (_range[i >> 1].l, (_range[i >> 1].r + _range[i >> 1].l) >> 1);
            else _range[i] = ((_range[i >> 1].r + _range[i >> 1].l) >> 1, _range[i >> 1].r);
        }
        _length = length;
        _funcDataMerge = funcDataMerge;
        _funcUpdate = funcUpdate;
        _funcLazyMerge = funcLazyMerge;
        _dataUnit = dataUnit;
        _lazyUnit = lazyUnit;
    }

    readonly int _length;
    Tx[] _data;
    Tl[] _lazy;
    bool[] _existLazy; //遅延要素が存在するかどうか
    (int l, int r)[] _range; //ノードiは区間[l,r)の値を持つ
    readonly Func<Tx, Tx, Tx> _funcDataMerge;
    readonly Func<Tx, Tl, int, int, Tx> _funcUpdate;
    readonly Func<Tl, Tl, Tl> _funcLazyMerge;
    readonly Tx _dataUnit;
    readonly Tl _lazyUnit;


    /// <summary>
    /// セグ木の葉に要素をセットします。
    /// </summary>
    /// <param name="i">位置</param>
    /// <param name="x">セットする要素</param>
    public void Set(int i, Tx x) { _data[i + _length] = x; }

    /// <summary>
    /// セグ木を構築します。
    /// </summary>
    public void Build()
    {
        for (int k = _length - 1; k > 0; k--)
        {
            _data[k] = _funcDataMerge(_data[k << 1], _data[(k << 1) | 1]);
        }
    }

    //ノードiに遅延要素があるなら評価を実行し、遅延要素を子に伝播する。
    void Eval(int i)
    {
        if (_existLazy[i] == false) return;
        _data[i] = _funcUpdate(_data[i], _lazy[i], _range[i].r - _range[i].l, _range[i].l);
        //ノードiが葉かどうかチェック
        if (i < _length)
        {
            //葉でないなら、遅延要素を子に伝播
            _lazy[i << 1] = _funcLazyMerge(_lazy[i << 1], _lazy[i]);
            _lazy[(i << 1) | 1] = _funcLazyMerge(_lazy[(i << 1) | 1], _lazy[i]);
            _existLazy[i << 1] = true;
            _existLazy[(i << 1) | 1] = true;
        }
        _lazy[i] = _lazyUnit;
        _existLazy[i] = false;
    }

    //遅延要素を子に伝搬させる必要があるノードのインデックスを降順に列挙したい
    IEnumerable<int> PropagateNodesIndex(int l, int r)
    {
        //後に評価を実行するノードは遅延しなくてよい
        var lm = (l / (l & -l)) >> 1;
        var rm = (r / (r & -r)) >> 1;
        while (l < r)
        {
            if (r <= rm) yield return r;
            if (l <= lm) yield return l;
            l >>= 1; r >>= 1;
        }
        while (l > 0)
        {
            yield return l;
            l >>= 1;
        }
    }

    /// <summary>
    /// 区間[l,r)に演算を行いセグ木を更新します。
    /// </summary>
    /// <param name="x">作用させる要素</param>
    public void Update(int l, int r, Tl x)
    {
        l += _length;
        r += _length;
        var idxs = PropagateNodesIndex(l, r).ToArray();
        //トップダウンで遅延要素を伝播させる
        foreach (var idx in idxs.Reverse()) Eval(idx);
        //区間[l,r)の要素を更新
        while (l < r)
        {
            if ((l & 1) == 1)
            {
                _lazy[l] = _funcLazyMerge(_lazy[l], x);
                _existLazy[l] = true;
                Eval(l);
                l++;
            }
            if ((r & 1) == 1)
            {
                r--;
                _lazy[r] = _funcLazyMerge(_lazy[r], x);
                _existLazy[r] = true;
                Eval(r);
            }
            l >>= 1; r >>= 1;
        }
        //ボトムアップでノードを更新する
        foreach (var idx in idxs)
        {
            Eval(idx << 1);
            Eval((idx << 1) | 1);
            _data[idx] = _funcDataMerge(_data[idx << 1], _data[(idx << 1) | 1]);
        }
    }

    /// <summary>
    /// 区間[l,r)でマージを実行した結果を返します。
    /// </summary>
    public Tx Range(int l, int r)
    {
        var tl = _dataUnit;
        var tr = _dataUnit;
        l += _length; r += _length;
        var idxs = PropagateNodesIndex(l, r).ToArray();
        //トップダウンで遅延要素を伝播させる
        foreach (var idx in idxs.Reverse()) Eval(idx);
        while (l < r)
        {
            if ((l & 1) == 1)
            {
                Eval(l);
                tl = _funcDataMerge(tl, _data[l]);
                l++;
            }
            if ((r & 1) == 1)
            {
                r--;
                Eval(r);
                tr = _funcDataMerge(_data[r], tr);
            }
            l >>= 1; r >>= 1;
        }
        return _funcDataMerge(tl, tr);
    }

    /// <summary>
    /// 全要素をマージした結果を返します。
    /// </summary>
    /// <returns></returns>
    public Tx All() => Range(0, _length);

    /// <summary>
    /// [l,r)で要素が制約を満たす最小のインデックスを返します。
    /// 制約を満たす要素が存在しない場合rを返します。
    /// </summary>
    public int MinLeft(int l, int r, Func<Tx, bool> constraint)
    {
        //指定区間に制約を満たす要素が存在しないとき,rを返す
        if (constraint(Range(l, r)) == false) return r;
        //指定区間左から制約を満たすまでマージしていく
        l += _length;
        var now = _dataUnit;
        while (true)
        {
            Eval(l);
            //見ているノードが制約を満たすとき
            if (constraint(_funcDataMerge(now, _data[l])))
            {
                //葉ならインデックスを返す
                if (l >= _length) return l - _length;
                //そうでないなら左の子を見る
                else l <<= 1; ;
            }
            //見ているノードが制約を満たさないとき
            else
            {
                //制約を満たしていないのでマージ
                now = _funcDataMerge(now, _data[l]);
                //左のノードなら右へ
                if ((l & 1) == 0) l++;
                //右のノードなら右の親へ
                else l = (l >> 1) + 1;
            }
        }
    }

    /// <summary>
    /// (l,r]で要素が制約を満たす最大のインデックスを返します。
    /// 制約を満たす要素が存在しない場合lを返します。
    /// </summary>
    public int MaxRight(int l, int r, Func<Tx, bool> constraint)
    {
        //指定区間に制約を満たす要素が存在しないとき,lを返す
        if (constraint(Range(l + 1, r + 1)) == false) return l;
        //指定区間左から制約を満たすまでマージしていく
        r += _length;
        var now = _dataUnit;
        while (true)
        {
            Eval(r);
            //見ているノードが制約を満たすとき
            if (constraint(_data[r]))
            {
                //葉ならインデックスを返す
                if (r >= _length) return r - _length;
                //そうでないなら右の子を見る
                else r = (r << 1) + 1;
            }
            //見ているノードが制約を満たさないとき
            else
            {
                //制約を満たしていないのでマージ
                now = _funcDataMerge(_data[r], now);
                //右のノードなら左へ
                if ((r & 1) == 1) r--;
                //左のノードなら左の親へ
                else r = (r >> 1) - 1;
            }
        }
    }

    public void Debug()
    {
        Console.Error.WriteLine();
        for (int i = _length; i < _length * 2; i++)
        {
            var tmp = i * 2;
            while (tmp % 2 == 0)
            {
                tmp /= 2;
                Console.Error.Write(_data[tmp] + "/" + _lazy[tmp] + " ");
            }
            Console.Error.WriteLine();
        }
        Console.Error.WriteLine();
    }
}

/// <summary>
/// ダブリングを抽象化したクラスです。
/// </summary>
/// <typeparam name="T">モノイドの条件を満たす必要があります。次の添え字を示す要素を持ちます。</typeparam>
public class Doubling<T>
{
    int logN = 0;
    T[][] next;
    Func<T, int> index;
    Func<T, T, T> funcT;
    /// <summary>
    /// ダブリングテーブルを構築します。
    /// 計算量はO(MlogN)です。
    /// </summary>
    /// <param name="N">最大で何個先まで見るかを表す整数です。</param>
    /// <param name="M">添え字の取り得る種類です。</param>
    /// <param name="gen">モノイドの単位元です。</param>
    /// <param name="init">添え字iの1個次の添え字を求める関数です。</param>
    /// <param name="index">モノイドの持つ添え字の要素を返す関数です。</param>
    /// <param name="funcT">モノイドの二項演算です。(Tnow,Tnext)=>T </param>
    public Doubling(long N, int M, T gen, Func<int, T> init, Func<T, int> index, Func<T, T, T> funcT)
    {
        while ((1L << (logN + 1)) <= N) logN++;
        next = new T[logN + 1][];
        this.index = index;
        this.funcT = funcT;
        for (int i = 0; i < logN + 1; i++) next[i] = Enumerable.Repeat(gen, M).ToArray();
        for (int i = 0; i < M; i++) next[0][i] = init(i);
        for (int k = 0; k < logN; k++)
            for (int i = 0; i < M; i++)
                next[k + 1][i] = funcT(next[k][i], next[k][index(next[k][i])]);
    }

    /// <summary>
    /// 状態pからq回以下の遷移で移動できる状態の集合に対する集計クエリを実行します。
    /// </summary>
    /// <param name="p">始点の状態です。</param>
    /// <param name="q">遷移可能数です。</param>
    /// <returns></returns>
    public T Query(T p, long q)
    {
        for (int k = logN; k >= 0; k--)
        {
            if (((q >> k) & 1) == 1) p = funcT(p, next[k][index(p)]);
        }
        return p;
    }
}

/// <summary>
/// 二次元累積和クラスです。
/// </summary>
public class SquareCumulativeSum
{
    public SquareCumulativeSum(IList<IList<long>> arr)
    {
        cum = new long[arr.Count + 1][];
        for (int i = 0; i <= arr.Count; i++) cum[i] = new long[arr[i].Count + 1];
        for (int i = 0; i < arr.Count; i++)
        {
            for (int j = 0; j < arr[i].Count; j++)
            {
                cum[i + 1][j + 1] = arr[i][j];
            }
        }
    }

    long[][] cum;

    public void Build()
    {
        for (int y = 0; y < cum.Length; y++)
        {
            for (int x = 0; x < cum[0].Length - 1; x++) cum[y][x + 1] += cum[y][x];
        }
        for (int x = 0; x < cum[0].Length; x++)
        {
            for (int y = 0; y < cum.Length - 1; y++) cum[y + 1][x] += cum[y][x];
        }
    }

    /// <summary>
    /// [(sy,sx),(by,bx))の和を求める
    /// </summary>
    /// <returns></returns>
    public long GetValue(int by, int bx, int sy, int sx)
    {
        return cum[by][bx] - cum[by][sx] - cum[sy][bx] + cum[sy][sx];
    }

    public long[] this[int i]
    {
        get { return cum[i]; }
    }
}

/// <summary>
/// Self-Balancing Binary Search Tree
/// (using Randamized BST)
/// </summary>
public class SB_BinarySearchTree<T> where T : IComparable
{
    public class Node
    {
        public T Value;
        public Node LChild;
        public Node RChild;
        public int Count;     //size of the sub tree

        public Node(T v)
        {
            Value = v;
            Count = 1;
        }
    }

    static Random _rnd = new Random();

    public static int Count(Node t)
    {
        return t == null ? 0 : t.Count;
    }

    static Node Update(Node t)
    {
        t.Count = Count(t.LChild) + Count(t.RChild) + 1;
        return t;
    }

    public static Node Merge(Node l, Node r)
    {
        if (l == null || r == null) return l == null ? r : l;

        if ((double)Count(l) / (double)(Count(l) + Count(r)) > _rnd.NextDouble())
        {
            l.RChild = Merge(l.RChild, r);
            return Update(l);
        }
        else
        {
            r.LChild = Merge(l, r.LChild);
            return Update(r);
        }
    }

    /// <summary>
    /// split as [0, k), [k, n)
    /// </summary>
    public static Tuple<Node, Node> Split(Node t, int k)
    {
        if (t == null) return new Tuple<Node, Node>(null, null);
        if (k <= Count(t.LChild))
        {
            var s = Split(t.LChild, k);
            t.LChild = s.Item2;
            return new Tuple<Node, Node>(s.Item1, Update(t));
        }
        else
        {
            var s = Split(t.RChild, k - Count(t.LChild) - 1);
            t.RChild = s.Item1;
            return new Tuple<Node, Node>(Update(t), s.Item2);
        }
    }

    public static Node Remove(Node t, T v)
    {
        if (Find(t, v) == null) return t;
        return RemoveAt(t, LowerBound(t, v));
    }

    public static Node RemoveAt(Node t, int k)
    {
        var s = Split(t, k);
        var s2 = Split(s.Item2, 1);
        return Merge(s.Item1, s2.Item2);
    }

    public static bool Contains(Node t, T v)
    {
        return Find(t, v) != null;
    }

    public static Node Find(Node t, T v)
    {
        while (t != null)
        {
            var cmp = t.Value.CompareTo(v);
            if (cmp > 0) t = t.LChild;
            else if (cmp < 0) t = t.RChild;
            else break;
        }
        return t;
    }

    public static Node FindByIndex(Node t, int idx)
    {
        if (t == null) return null;

        var currentIdx = Count(t) - Count(t.RChild) - 1;
        while (t != null)
        {
            if (currentIdx == idx) return t;
            if (currentIdx > idx)
            {
                t = t.LChild;
                currentIdx -= (Count(t == null ? null : t.RChild) + 1);
            }
            else
            {
                t = t.RChild;
                currentIdx += (Count(t == null ? null : t.LChild) + 1);
            }
        }

        return null;
    }

    public static int UpperBound(Node t, T v)
    {
        var torg = t;
        if (t == null) return -1;

        var ret = Int32.MaxValue;
        var idx = Count(t) - Count(t.RChild) - 1;
        while (t != null)
        {
            var cmp = t.Value.CompareTo(v);

            if (cmp > 0)
            {
                ret = Math.Min(ret, idx);
                t = t.LChild;
                idx -= (Count(t == null ? null : t.RChild) + 1);
            }
            else if (cmp <= 0)
            {
                t = t.RChild;
                idx += (Count(t == null ? null : t.LChild) + 1);
            }
        }
        return ret == Int32.MaxValue ? Count(torg) : ret;
    }

    public static int LowerBound(Node t, T v)
    {
        var torg = t;
        if (t == null) return -1;

        var idx = Count(t) - Count(t.RChild) - 1;
        var ret = Int32.MaxValue;
        while (t != null)
        {
            var cmp = t.Value.CompareTo(v);
            if (cmp >= 0)
            {
                if (cmp == 0) ret = Math.Min(ret, idx);
                t = t.LChild;
                if (t == null) ret = Math.Min(ret, idx);
                idx -= t == null ? 0 : (Count(t.RChild) + 1);
            }
            else if (cmp < 0)
            {
                t = t.RChild;
                idx += (Count(t == null ? null : t.LChild) + 1);
                if (t == null) return idx;
            }
        }
        return ret == Int32.MaxValue ? Count(torg) : ret;
    }

    public static Node Insert(Node t, T v)
    {
        var ub = LowerBound(t, v);
        return InsertByIdx(t, ub, v);
    }

    static Node InsertByIdx(Node t, int k, T v)
    {
        var s = Split(t, k);
        return Merge(Merge(s.Item1, new Node(v)), s.Item2);
    }

    public static IEnumerable<T> Enumerate(Node t)
    {
        var ret = new List<T>();
        Enumerate(t, ret);
        return ret;
    }

    static void Enumerate(Node t, List<T> ret)
    {
        if (t == null) return;
        Enumerate(t.LChild, ret);
        ret.Add(t.Value);
        Enumerate(t.RChild, ret);
    }
}

/// <summary>
/// C-like set
/// </summary>
public class Set<T> where T : IComparable
{
    protected SB_BinarySearchTree<T>.Node _root;

    public T this[int idx] { get { return ElementAt(idx); } }

    public int Count()
    {
        return SB_BinarySearchTree<T>.Count(_root);
    }

    public virtual void Insert(T v)
    {
        if (_root == null) _root = new SB_BinarySearchTree<T>.Node(v);
        else
        {
            if (SB_BinarySearchTree<T>.Find(_root, v) != null) return;
            _root = SB_BinarySearchTree<T>.Insert(_root, v);
        }
    }

    public void Clear()
    {
        _root = null;
    }

    public void Remove(T v)
    {
        _root = SB_BinarySearchTree<T>.Remove(_root, v);
    }

    public bool Contains(T v)
    {
        return SB_BinarySearchTree<T>.Contains(_root, v);
    }

    public T ElementAt(int k)
    {
        var node = SB_BinarySearchTree<T>.FindByIndex(_root, k);
        if (node == null) throw new IndexOutOfRangeException();
        return node.Value;
    }

    public int Count(T v)
    {
        return SB_BinarySearchTree<T>.UpperBound(_root, v) - SB_BinarySearchTree<T>.LowerBound(_root, v);
    }

    public int LowerBound(T v)
    {
        return SB_BinarySearchTree<T>.LowerBound(_root, v);
    }

    public int UpperBound(T v)
    {
        return SB_BinarySearchTree<T>.UpperBound(_root, v);
    }

    public Tuple<int, int> EqualRange(T v)
    {
        if (!Contains(v)) return new Tuple<int, int>(-1, -1);
        return new Tuple<int, int>(SB_BinarySearchTree<T>.LowerBound(_root, v), SB_BinarySearchTree<T>.UpperBound(_root, v) - 1);
    }

    public List<T> ToList()
    {
        return new List<T>(SB_BinarySearchTree<T>.Enumerate(_root));
    }
}

/// <summary>
/// C-like multiset
/// </summary>
public class MultiSet<T> : Set<T> where T : IComparable
{
    public override void Insert(T v)
    {
        if (_root == null) _root = new SB_BinarySearchTree<T>.Node(v);
        else _root = SB_BinarySearchTree<T>.Insert(_root, v);
    }
}

