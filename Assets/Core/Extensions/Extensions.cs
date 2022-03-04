using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class Extensions
{
    public static IEnumerable<T> Randomize<T>(this IEnumerable<T> source)
    {
        Random rnd = new Random();
        return source.OrderBy((item) => rnd.Next());
    }
}

