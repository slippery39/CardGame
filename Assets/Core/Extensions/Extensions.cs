using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class Extensions
{
    public static bool IsEmpty (this string source)
    {
        return (source == null || source.Trim().Length == 0);
    }

    public static List<T> GetOfType<T>(this IEnumerable<object> source)
    {
        return source.Where(o => o is T).Cast<T>().ToList();
    }

    public static IEnumerable<T> Randomize<T>(this IEnumerable<T> source)
    {
        Random rnd = new Random();
        return source.OrderBy((item) => rnd.Next());
    }

    public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
    {
        return source == null || !source.Any();
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

    public static string ToManaString(this List<CardColor> colors)
    {
        var manaString = "";

        foreach (var color in colors)
        {
            if (color == CardColor.White)
            {
                manaString += "W";
            }
            else if (color == CardColor.Blue)
            {
                manaString += "U";
            }
            else if (color == CardColor.Black)
            {
                manaString += "B";
            }
            else if (color == CardColor.Red)
            {
                manaString += "R";
            }
            else if (color == CardColor.Green)
            {
                manaString += "G";
            }
        }

        return manaString;
    }

}

