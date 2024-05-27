using System;
using System.Collections.Generic;
using System.Text;

namespace Numerics5
{
    internal static class ListUtils
    {
        public static List<T> New<T>(int capacity) => new List<T>(new T[capacity]);
    }
}
