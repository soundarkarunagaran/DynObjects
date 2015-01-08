// Copyright 2010 Stefan Dragnev.
// The library and all its files are distributed under the MIT license.
// See the full text of the license in the accompanying LICENSE.txt file or at http://www.opensource.org/licenses/mit-license.php

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dyn.Utilities
{
    public struct Pair<T, U>
    {
        public T First;
        public U Second;

        public Pair(T first, U second)
        {
            First = first;
            Second = second;
        }

        public override int GetHashCode()
        {
            return First.GetHashCode() ^ Second.GetHashCode();
        }
    }

    public static class Util
    {
        public static Pair<T, U> MakePair<T, U>(T t, U u)
        {
            return new Pair<T, U>(t, u);
        }

        public static T GetAttribute<T>(this ICustomAttributeProvider objType) where T : Attribute
        {
            return (from attr in objType.GetCustomAttributes(typeof(T), false) select attr).SingleOrDefault() as T;
        }

        public static bool ImplementsInterface(this Type type, Type interfaceType)
        {
            return type.GetInterfaces().Contains(interfaceType);
        }

        public struct EnumerationPair<T>
        {
            public T Value;
            public int Index;
        }

        public static IEnumerable<EnumerationPair<T>> Enumerate<T>(this IEnumerable<T> first)
        {
            var enumerator = first.GetEnumerator();
            var index = 0;
            while (enumerator.MoveNext())
            {
                yield return new EnumerationPair<T> { Value = enumerator.Current, Index = index };
                index++;
            }
        }

        /// <summary>
        /// Reimplementation of Delegate.Combine(Delegate[] delgs) because it's missing in Silverlight
        /// </summary>
        /// <param name="delgs"></param>
        /// <returns></returns>
        public static Delegate CombineDelegates(Delegate[] delgs)
        {
            switch (delgs.Length)
            {
                case 0:
                    return null;
                case 1:
                    return delgs[0];
                default:
                    {
                        var result = Delegate.Combine(delgs[0], delgs[1]);
                        for (int i = 2; i < delgs.Length; ++i)
                        {
                            result = Delegate.Combine(result, delgs[i]);
                        }
                        return result;
                    }
            }
        }

        public static IEnumerable<T> FlattenOnce<T>(this IEnumerable<T[]> collection)
        {
            return collection.SelectMany(enumerable => enumerable);
        }
    }

    public static class CoreExtensions
    {
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue)
        {
            TValue val;
            return dict.TryGetValue(key, out val) ? val : defaultValue;
        }
    }
}
