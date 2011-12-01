/*
 * JSPackager v0.1
 *
 * Copyright 2011, Fog Creek Software
 * Licensed under the MIT license.
 * See LICENSE.txt
 */


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSPackager.Extensions {
    public static class Extensions {
        public static V Get<K, V>(this IDictionary<K, V> dict, K key) {
            V value;
            if (dict.TryGetValue(key, out value)) {
                return value;
            }

            return default(V);
        }

        public static List<V> GetList<V>(this Dictionary<string, object> dict, string key) {
            var obj = dict.Get(key);
            if (obj != null) {
                return ((ArrayList)obj).Cast<V>().ToList();
            }
            return new List<V>();
        }

        public static string Fmt(this string template, params object[] args) {
            return String.Format(template, args);
        }
    }
}
