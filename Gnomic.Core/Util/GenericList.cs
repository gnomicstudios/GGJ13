using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gnomic.Util
{
    public static class GenericList
    {
        public static int RemoveAll<T>(List<T> list, Predicate<T> match)
        {
            int count = 0;
            for (int i = list.Count - 1; i >= 0; --i)
            {
                if (match(list[i]))
                {
                    list.RemoveAt(i);
                    count++;
                }

            }
            return count;
        }
        public static bool Exists<T>(List<T> list, Predicate<T> match)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (match(list[i]))
                {
                    return true;
                }
            }
            return false;
        }
        public static T Find<T>(List<T> list, Predicate<T> match)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                T tmp = list[i];
                if (match(tmp))
                {
                    return tmp;
                }
            }
            return default(T);
        }
    }
}
