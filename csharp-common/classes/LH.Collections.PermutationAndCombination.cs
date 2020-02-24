﻿/*
 * Copyright
 *
 * https://github.com/LokiHonoo/development-resources
 * Copyright (C) LH.Studio 2015. All rights reserved.
 *
 * This code page is published under the terms of the MIT license.
 */

using System;
using System.Collections.Generic;
using System.Numerics;

namespace LH.Collections
{
    /// <summary>
    /// 求数组组合。
    /// </summary>
    /// <typeparam name="T">元素泛型。</typeparam>
    public sealed class Combination<T>
    {
        #region 成员

        private readonly ArraySegment<T> _array;
        private readonly int _m;

        #endregion 成员

        #region 委托

        /// <summary>
        /// 组合完成一组元素后的回调函数。
        /// </summary>
        /// <param name="result">组合完成后的一组元素集合。</param>
        /// <param name="userState">传递用户参数。</param>
        public delegate void CreatedCallback(T[] result, object userState);

        #endregion 委托

        /// <summary>
        /// 求数组 m 个元素的组合。
        /// </summary>
        /// <param name="array">元素数组。</param>
        /// <param name="m">指定选择的元素数量。</param>
        public Combination(T[] array, int m)
        {
            if (array.Length == 0)
            {
                throw new Exception("元素数组不能是空数组。");
            }
            if (m < 1 || m > array.Length)
            {
                throw new Exception("m 的值不能小于 1 或大于元素数组的最大长度。");
            }
            _array = new ArraySegment<T>(array);
            _m = m;
        }

        /// <summary>
        /// 求数组 m 个元素的组合。
        /// </summary>
        /// <param name="array">元素数组切片。</param>
        /// <param name="m">指定选择的元素数量。</param>
        public Combination(ArraySegment<T> array, int m)
        {
            if (array.Count == 0)
            {
                throw new Exception("元素数组切片不能是空数组切片。");
            }
            if (m < 1 || m > array.Count)
            {
                throw new Exception("m 的不能小于 1 或大于元素数组切片的最大长度。");
            }
            _array = array;
            _m = m;
        }

        /// <summary>
        /// 计算可组合数量。
        /// </summary>
        /// <returns></returns>
        public BigInteger GetCount()
        {
            return C(_array.Count, _m);
        }

        /// <summary>
        /// 输出组合的集合。
        /// </summary>
        /// <returns></returns>
        public List<T[]> Output()
        {
            BigInteger count = GetCount();
            if (count > int.MaxValue)
            {
                throw new Exception("可计算的组合数量超出了容器容量 (Int32)。");
            }
            List<T[]> result = new List<T[]>((int)count);
            Output((r, s) => { result.Add(r); }, null);
            return result;
        }

        /// <summary>
        /// 每生成一组集合后调用回调函数，输出组合的集合。
        /// </summary>
        /// <param name="created">组合完成一组元素后的回调函数。</param>
        /// <param name="userState">传递用户参数。</param>
        public void Output(CreatedCallback created, object userState)
        {
            Combine(_array, _m, 0, _m, created, userState);
        }

        /// <summary>
        /// 指定 n、 m 值求可组合的数量。
        /// </summary>
        /// <param name="n">指定元素总数。</param>
        /// <param name="m">指定选择的元素数量。</param>
        /// <returns></returns>
        private BigInteger C(int n, int m)
        {
            BigInteger numerator = BigInteger.One;
            BigInteger denominator = BigInteger.One;
            for (int i = 0; i < m; i++)
            {
                numerator *= n;
                n--;
            }
            for (int i = m; i >= 1; i--)
            {
                denominator *= i;
            }
            return numerator / denominator;
        }

        /// <summary>
        /// 组合递归方法。
        /// </summary>
        /// <param name="array">元素数组切片。</param>
        /// <param name="m">指定选择的元素数量。</param>
        /// <param name="ii">循环到的主要分段元素数组切片的索引。</param>
        /// <param name="jj">循环到的盈余分段元素数组切片的索引。</param>
        /// <param name="created">组合完成一组元素后的回调函数。</param>
        /// <param name="userState">传递用户参数。</param>
        private void Combine(ArraySegment<T> array, int m, int ii, int jj, CreatedCallback created, object userState)
        {
            for (int i = ii; i < m; i++)
            {
                for (int j = jj; j < array.Count; j++)
                {
                    Swap(array, i, j);
                    Combine(array, m, i + 1, j + 1, created, userState);
                    Swap(array, i, j);
                }
            }
            T[] result = new T[m];
            Buffer.BlockCopy(array.Array, array.Offset, result, 0, result.Length);
            created?.Invoke(result, userState);
        }

        /// <summary>
        /// 交换元素数组切片中的两个元素。
        /// </summary>
        /// <param name="array">元素数组切片。</param>
        /// <param name="index1">要交换的第一个元素的索引。</param>
        /// <param name="index2">要交换的第二个元素的索引。</param>
        private void Swap(ArraySegment<T> array, int index1, int index2)
        {
            index1 = array.Offset + index1;
            index2 = array.Offset + index2;
            T tmp = array.Array[index1];
            array.Array[index1] = array.Array[index2];
            array.Array[index2] = tmp;
        }
    }

    /// <summary>
    /// 求数组排列。
    /// </summary>
    /// <typeparam name="T">元素泛型。</typeparam>
    public sealed class Permutation<T>
    {
        #region 成员

        private readonly ArraySegment<T> _array;
        private readonly int _m;

        #endregion 成员

        #region 委托

        /// <summary>
        /// 排列完成一组元素后的回调函数。
        /// </summary>
        /// <param name="result">排列完成后的一组元素集合。</param>
        /// <param name="userState">传递用户参数。</param>
        public delegate void CreatedCallback(T[] result, object userState);

        #endregion 委托

        /// <summary>
        /// 求数组 m 个元素的排列。
        /// </summary>
        /// <param name="array">元素数组。</param>
        /// <param name="m">指定选择的元素数量。</param>
        public Permutation(T[] array, int m)
        {
            if (array.Length == 0)
            {
                throw new Exception("元素数组不能是空数组。");
            }
            if (m < 1 || m > array.Length)
            {
                throw new Exception("m 的不能小于 1 或大于元素数组的最大长度。");
            }
            _array = new ArraySegment<T>(array);
            _m = m;
        }

        /// <summary>
        /// 求数组 m 个元素的排列。
        /// </summary>
        /// <param name="array">元素数组切片。</param>
        /// <param name="m">指定选择的元素数量。</param>
        public Permutation(ArraySegment<T> array, int m)
        {
            if (array.Count == 0)
            {
                throw new Exception("元素数组切片不能是空数组切片。");
            }
            if (m < 1 || m > array.Count)
            {
                throw new Exception("m 的值不能小于 1 或大于元素数组切片的最大长度。");
            }
            _array = array;
            _m = m;
        }

        /// <summary>
        /// 计算可排列数量。
        /// </summary>
        /// <returns></returns>
        public BigInteger GetCount()
        {
            return P(_array.Count, _m);
        }

        /// <summary>
        /// 输出排列的集合。
        /// </summary>
        /// <returns></returns>
        public List<T[]> Output()
        {
            BigInteger count = GetCount();
            if (count > int.MaxValue)
            {
                throw new Exception("可计算的排列数量超出了容器容量 (Int32)。");
            }
            List<T[]> result = new List<T[]>((int)count);
            Output((r, s) => { result.Add(r); }, null);
            return result;
        }

        /// <summary>
        /// 每生成一组集合后调用回调函数，输出排列的集合。
        /// </summary>
        /// <param name="created">排列完成一组元素后的回调函数。</param>
        /// <param name="userState">传递用户参数。</param>
        public void Output(CreatedCallback created, object userState)
        {
            if (_m == _array.Count)
            {
                Permutate(_array, 0, created, userState);
            }
            else
            {
                Combination<T> combination = new Combination<T>(_array, _m);
                combination.Output((cr, cs) =>
                {
                    Permutate(new ArraySegment<T>(cr), 0, created, userState);
                }, null);
            }
        }

        /// <summary>
        /// 指定 n、 m 值求可排列的数量。
        /// </summary>
        /// <param name="n">指定元素总数。</param>
        /// <param name="m">指定选择的元素数量。</param>
        /// <returns></returns>
        private BigInteger P(int n, int m)
        {
            BigInteger integer = BigInteger.One;
            for (int i = 0; i < m; i++)
            {
                integer *= n;
                n--;
            }
            return integer;
        }

        /// <summary>
        /// 排列递归方法。
        /// </summary>
        /// <param name="array">元素数组切片。</param>
        /// <param name="ii">已循环到的元素数组切片的索引。</param>
        /// <param name="created">排列完成一组元素后的回调函数。</param>
        /// <param name="userState">传递用户参数。</param>
        private void Permutate(ArraySegment<T> array, int ii, CreatedCallback created, object userState)
        {
            if (ii == array.Count - 1)
            {
                T[] result = new T[array.Count];
                Buffer.BlockCopy(array.Array, array.Offset, result, 0, result.Length);
                created?.Invoke(result, userState);
            }
            else
            {
                for (int i = ii; i < array.Count; i++)
                {
                    if (i == ii)
                    {
                        Permutate(array, ii + 1, created, userState);
                    }
                    else
                    {
                        Swap(array, i, ii);
                        Permutate(array, ii + 1, created, userState);
                        Swap(array, i, ii);
                    }
                }
            }
        }

        /// <summary>
        /// 交换元素数组切片中的两个元素。
        /// </summary>
        /// <param name="array">元素数组切片。</param>
        /// <param name="index1">要交换的第一个元素的索引。</param>
        /// <param name="index2">要交换的第二个元素的索引。</param>
        private void Swap(ArraySegment<T> array, int index1, int index2)
        {
            index1 = array.Offset + index1;
            index2 = array.Offset + index2;
            T tmp = array.Array[index1];
            array.Array[index1] = array.Array[index2];
            array.Array[index2] = tmp;
        }
    }
}