using System;
using System.Collections.Generic;

namespace EventSourcing.Lib {
    public static class TypeMap {
        static readonly Dictionary<string, Type> ReverseMap = new();
        static readonly Dictionary<Type, string> Map        = new();

        public static string GetTypeName<T>() => Map[typeof(T)];
        
        public static string GetTypeName(object o) => Map[o.GetType()];

        public static Type GetType(string typeName) => ReverseMap[typeName];

        public static void AddType<T>(string name) {
            ReverseMap[name] = typeof(T);
            Map[typeof(T)]   = name;
        }
    }        
}
