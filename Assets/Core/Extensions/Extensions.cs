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

    /// <summary>
    /// Extension method that can check if a string is numeric.
    /// Call by using "ExampleString123".IsNumeric()
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static bool IsNumeric(this string text) => double.TryParse(text, out _);

    /// <summary>
    /// Extension method that can check if a char is numeric.
    /// Call by using charVar.IsNumeric()
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static bool IsNumeric(this char character) => Int32.TryParse(character.ToString(), out _);

}

