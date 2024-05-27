using System.Collections.Generic;

namespace Numerics8
{
    internal static class ListUtils
    {
        public static List<T> New<T>(int capacity) => new List<T>(new T[capacity]);

        public static List<List<T>> ConcatinateColumns<T>(List<List<T>> columns)
        {
            var res = New<List<T>>(columns[0].Count);
            
            for (int j = 0; j < res.Count; j++)
            {
                res[j] = New<T>(columns.Count);

                for (int i = 0; i < columns.Count; i++)
                {
                    res[j][i] = columns[i][j];
                }
            }

            return res;
        }

        public static List<List<T>> ConcatinateRows<T>(List<List<T>> rows)
        {
            var res = New<List<T>>(rows.Count);

            for (int j = 0; j < res.Count; j++)
            {
                res[j] = New<T>(rows[j].Count);

                for (int i = 0; i < rows[j].Count; i++)
                {
                    res[j][i] = rows[j][i];
                }
            }

            return res;
        }

        public static T[,] To2DArray<T>(List<List<T>> list)
        {
            var res = new T[list.Count, list[0].Count];
            for (int j = 0; j < list.Count; j++)
            {
                for (int i = 0; i < list[j].Count; i++)
                {
                    res[j, i] = list[j][i];
                }
            }

            return res;
        }
    }
}
