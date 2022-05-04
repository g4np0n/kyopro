using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

class MathLibrary
{
    //最大公約数
    //O(logN)
    long GCD(long a, long b)
    {
        while (true)
        {
            if (b == 0) return a;
            (a, b) = (b, a % b);
        }
    }

    //最小公倍数
    long LCM(long a, long b) { return a / GCD(a, b) * b; }

    /// <summary>
    /// 拡張ユークリッドの互除法です。
    /// ax+by=gcd(a,b)を満たすx,yを求めます。
    /// a,b>0である必要があります。
    /// </summary>
    /// <returns>gcd(a,b)</returns>
    long ExtGCD(long a, long b, ref long x, ref long y)
    {
        if (b == 0)
        {
            x = 1;
            y = 0;
            return a;
        }
        long d = ExtGCD(b, a % b, ref y, ref x);
        y -= a / b * x;
        return d;
    }

    /// <summary>
    /// 中国剰余定理より x%m1=b1 かつ x%m2=b2 を満たすような最小のxを求めます。
    /// x%m=rを満たす(r,m)を返します。
    /// </summary>
    /// <param name="b1"></param>
    /// <param name="m1"></param>
    /// <param name="b2"></param>
    /// <param name="m2"></param>
    /// <returns></returns>
    (long r,long m) CRT(long b1,long m1,long b2,long m2)
    {
        long p=0, q=0;
        long d = ExtGCD(m1, m2, ref p,ref q);
        if ((b2 - b1) % d != 0) return (0, -1);
        long m = m1 * (m2 / d);
        long tmp = (b2 - b1) / d * p % (m2 / d);
        long r = ((b1 + m1 * tmp) % m + m) % m;
        return (r, m);
    }


    //階乗
    long Factorial(long a) { long n = 1; for (int i = 1; i <= a; i++) { n *= i; } return n; }
    long Factorial(long a, long mod) { long n = 1; for (int i = 1; i <= a; i++) { n = n * i % mod; } return n; }

    //素数判定
    //O(√N)
    bool IsPrime(long N)
    {
        if (N == 1) return false;
        if (N == 2) return true;
        if (N % 2 == 0) return false;
        for (long i = 3; i * i <= N; i += 2)
        {
            if (N % i == 0) return false;
        }
        return true;
    }

    //約数列挙
    //O(√N)
    IEnumerable<long> GetDevisor(long N)
    {
        for (long i = 1; i * i <= N; i++)
        {
            if (N % i == 0)
            {
                yield return i;
                if (N / i != i) yield return N / i;
            }
        }
    }

    //素因数分解
    //O(√N)
    IEnumerable<long> PrimeFactorization(long N)
    {
        if (N == 1)
        {
            yield return 1;
            yield break;
        }
        if (N % 2 == 0)
        {
            while (N % 2 == 0)
            {
                N /= 2;
                yield return 2;
            }
        }
        for (long i = 3; i * i <= N; i += 2)
        {
            if (N % i != 0) continue;
            while (N % i == 0)
            {
                N /= i;
                yield return i;
            }
        }
        if (N != 1) yield return N;
    }

    //aの逆元を取得
    //modが素数のとき
    long GetInv(long a, long mod)
    {
        return Pow(a, mod - 2, mod);
    }

    //累乗二分法
    //O(logN)
    long Pow(long a, long n)
    {
        long res = 1;
        while (n > 0)
        {
            if ((n & 1) == 1) { res *= a; }
            a *= a;
            n >>= 1;
        }
        return res;
    }

    //mod下での累乗二分法
    //a^n modを求める
    //O(logN)
    long Pow(long a, long n, long mod)
    {
        long res = 1;
        while (n > 0)
        {
            if ((n & 1) == 1) { res = res * a % mod; }
            a = a * a % mod; ;
            n >>= 1;
        }
        return res;
    }

    ModInt[][] Pow(ModInt[][] a,long n)
    {
        var size = a.Length;
        var res = new ModInt[size][];
        for(int i = 0; i < size; i++)
        {
            res[i] = new ModInt[size];
            res[i].AsSpan().Fill(new ModInt(0));
            res[i][i] = 1;
        }

        while (n > 0)
        {
            ModInt[][] tmp;
            if ((n & 1) == 1)
            {
                tmp = new ModInt[size][];
                for (int i = 0; i < size; i++) tmp[i] = new ModInt[size];
                for (int i = 0; i < size; i++)
                {
                    for(int j = 0; j < size; j++)
                    {
                        for(int k = 0; k < size; k++)
                        {
                            tmp[i][j] += res[i][k] * a[k][j];
                        }
                    }
                }
                res = tmp;
            }
            tmp = new ModInt[size][];
            for (int i = 0; i < size; i++) tmp[i] = new ModInt[size];
            for(int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    for (int k = 0; k < size; k++)
                    {
                        tmp[i][j] += a[i][k] * a[k][j];
                    }
                }
            }
            a = tmp;
            n >>= 1;
        }
        return res;
    }

    bool NextPermutation(int[] array)
    {
        var size = array.Length;
        var ok = false;

        //array[i]<array[i+1]を満たす最大のiを求める
        int i = size - 2;
        for (; 0 <= i; i--)
        {
            if (array[i] < array[i + 1])
            {
                ok = true;
                break;
            }
        }

        //全ての要素が降順の場合、NextPermutationは存在しない
        if (ok == false) return false;

        //array[i]<array[j]を満たす最大のjを求める
        int j = size - 1;
        for (; ; j--)
        {
            if (array[i] < array[j]) break;
        }

        //iとjの要素をswapする
        var tmp = array[i];
        array[i] = array[j];
        array[j] = tmp;

        //i以降の要素を反転させる
        Array.Reverse(array, i + 1, size - (i + 1));

        return true;
    }

    bool NextPermutation<T>(T[] array) where T : IComparable
    {
        var size = array.Length;
        var ok = false;

        //array[i]<array[i+1]を満たす最大のiを求める
        int i = size - 2;
        for (; 0 <= i; i--)
        {
            if (array[i].CompareTo(array[i + 1]) < 0)
            {
                ok = true;
                break;
            }
        }

        //全ての要素が降順の場合、NextPermutationは存在しない
        if (ok == false) return false;

        //array[i]<array[j]を満たす最大のjを求める
        int j = size - 1;
        for (; ; j--)
        {
            if (array[i].CompareTo(array[j]) < 0) break;
        }

        //iとjの要素をswapする
        var tmp = array[i];
        array[i] = array[j];
        array[j] = tmp;

        //i以降の要素を反転させる
        Array.Reverse(array, i + 1, size - (i + 1));

        return true;
    }
}

class ModLib
{
    long mod;
    static int max = 1000000;

    public long[] fib_ac = new long[max];
    public long[] fib_inv = new long[max];
    public long[] inv = new long[max];

    public ModLib(long mod)
    {
        this.mod = mod;
        fib_ac[0] = fib_ac[1] = 1;
        fib_inv[0] = fib_inv[1] = 1;
        inv[1] = 1;
        for (int i = 2; i < max; i++)
        {
            fib_ac[i] = fib_ac[i - 1] * i % mod;
            inv[i] = mod - inv[mod % i] * (mod / i) % mod;
            fib_inv[i] = fib_inv[i - 1] * inv[i] % mod;
        }
    }

    public long Combination(int n, int m)
    {
        if (n < m) return 0;
        if (n < 0 || m < 0) return 0;
        return fib_ac[n] * (fib_inv[m] * fib_inv[n - m] % mod) % mod;
    }
}

struct ModInt
{
    const int mod = 1000000007;
    long value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ModInt(long value) { if ((this.value = value % mod) < 0) this.value += mod; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator long(ModInt modInt) => modInt.value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ModInt(long value) => new ModInt(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ModInt operator +(ModInt a, ModInt b)
    {
        long res = a.value + b.value;
        return new ModInt() { value = res >= mod ? res - mod : res };
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ModInt operator +(ModInt a, long b) => a.value + b;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ModInt operator -(ModInt a, ModInt b)
    {
        long res = a.value - b.value;
        return new ModInt() { value = res >= 0 ? res : res + mod };
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ModInt operator -(ModInt a, long b) => a.value - b;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ModInt operator *(ModInt a, ModInt b)
    {
        long res = a.value * b.value;
        return new ModInt() { value = res >= mod ? res % mod : res };
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ModInt operator *(ModInt a, long b) => a.value * b;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ModInt operator /(ModInt a, ModInt b) => a.value * GetInverse(b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static long GetInverse(long a)
    {
        long div;
        long p = mod;
        long x1 = 1, y1 = 0, x2 = 0, y2 = 1;
        while (true)
        {
            if (p == 1) return x2;
            div = a / p;
            x1 -= x2 * div;
            y1 -= y2 * div;
            a %= p;
            if (a == 1) return x1;
            div = p / a;
            x2 -= x1 * div;
            y2 -= y1 * div;
            p %= a;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => value.ToString();
}

class Eratosthenes
{
    readonly int n;
    readonly int[] minPrime;
    public Eratosthenes(int N)
    {
        n = N;
        minPrime = Enumerable.Range(0, n + 1).Select(s => s % 2 == 0 && s > 2 ? 2 : s).ToArray();
        for (int i = 3; i * i <= n; i += 2)
        {
            if (IsPrime(i) == false) continue;
            for (int j = i * i; j <= n; j += i * 2)
            {
                if (IsPrime(j)) minPrime[j] = i;
            }
        }
    }

    public bool IsPrime(int N) => N > 1 && minPrime[N] == N;
    public IEnumerable<int> PrimeFactors(int N)
    {
        while (N != 1)
        {
            yield return minPrime[N];
            N /= minPrime[N];
        }
    }

}


