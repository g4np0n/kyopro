using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

class Subsequence
{
    /// <summary>
    /// 最長増加部分列の長さを求めます。
    /// 計算量は O(|A|log|A|) です。
    /// </summary>
    /// <param name="Compare">T1がT2より大きいか？</param>
    /// <returns>与えられたAにおけるCompare関数に従って得られる最長増加部分列の長さを返します。</returns>
    int LIS<T>(IReadOnlyList<T> A, Func<T, T, bool> Compare)
    {
        int N = A.Count;
        var dp = new List<T>();
        for (int i = 0; i < N; i++)
        {
            var ok = dp.Count;
            var ng = -1;
            while (ok - ng > 1)
            {
                var mid = (ok + ng) / 2;
                if (Compare(dp[mid], A[i])) ok = mid;
                else ng = mid;
            }
            if (ok == dp.Count) dp.Add(A[i]);
            else dp[ok] = A[i];
        }
        return dp.Count;
    }

    /// <summary>
    /// 最長共通部分列を求めます。
    /// </summary>
    class LCS<T>
    {
        /// <summary>
        /// 最長共通部分列長を求めるDPテーブルを構築します。
        /// 計算量は O(|A||B|)です
        /// </summary>
        public LCS(IReadOnlyList<T> A, IReadOnlyList<T> B)
        {
            this.A = A;
            this.B = B;
            dp = new int[this.A.Count + 1][];
            for (int i = 0; i < this.A.Count + 1; i++) dp[i] = new int[this.B.Count + 1];

            for (int i = 0; i < this.A.Count; i++)
            {
                for (int j = 0; j < this.B.Count; j++)
                    dp[i + 1][j + 1] = this.A[i].Equals(this.B[j]) ? dp[i][j] + 1 : Math.Max(dp[i + 1][j], dp[i][j + 1]);
            }
        }

        IReadOnlyList<T> A, B;
        int[][] dp;

        /// <summary>
        /// 最長共通部分列長を取得します。
        /// </summary>
        public int Length => dp[A.Count][B.Count];

        /// <summary>
        /// 最長となるような共通部分列を1つ求めます。
        /// 計算量は O(|A||B|です)
        /// </summary>
        /// <returns></returns>
        public T[] Construct()
        {
            var st = new Stack<T>();
            int i = A.Count;
            int j = B.Count;
            while (i > 0 && j > 0)
            {
                if (dp[i][j] == dp[i - 1][j])
                {
                    i--;
                    continue;
                }
                if (dp[i][j] == dp[i][j - 1])
                {
                    j--;
                    continue;
                }
                st.Push(A[i - 1]);
                i--; j--;
            }
            return st.ToArray();
        }
    }

    /// <summary>
    /// s[0..n)とs[i..n)のLCP(Longest Common Prefix/最大共通接頭辞)の長さを格納した配列を返します。
    /// </summary>
    int[] Z_Algorithm(string S)
    {
        var res = new int[S.Length];
        int i = 1, j = 0;
        while (i < S.Length)
        {
            while (i + j < S.Length && S[j] == S[i + j]) j++;
            res[i] = j;
            if (j == 0) { i++; continue; }
            int k = 1;
            while (i + k < S.Length && k + res[k] < j) { res[i + k] = res[k]; k++; }
            i += k; j -= k;
        }
        return res;
    }

    /// <summary>
    /// 各文字を中心とした回文の最長半径を求めます。
    /// 偶数長回文については#a#b#b#a#のようにして、2で割って使います。
    /// 計算量は O(N) です。
    /// </summary>
    /// <returns>各文字を中心とした回文の最長半径を格納した配列を返します。</returns>
    int[] Manacher(string S)
    {
        var R = new int[S.Length];
        int i = 0, j = 0;
        while (i < S.Length)
        {
            while (i - j >= 0 && i + j < S.Length && S[i - j] == S[i + j]) j++;
            R[i] = j;
            int k = 1;
            while (i - k >= 0 && k + R[i - k] < j)
            {
                R[i + k] = R[i - k];
                k++;
            }
            i += k;
            j -= k;
        }
        return R;
    }
}

