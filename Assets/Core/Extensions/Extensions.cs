using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// <a href="https://en.wikipedia.org/wiki/Curiously_recurring_template_pattern">CRTP</a>-based
/// interface to implement for objects that can create deep clones of themselves,
/// but can be abused if TSelf is not specified as the same type as the
/// implementing class.
/// </summary>
/// <typeparam name="TSelf"></typeparam>
public interface IDeepCloneable<TSelf> where TSelf : IDeepCloneable<TSelf>
{
    public TSelf DeepClone(CardGame cardGame);
}

public static class DeepCloneExtensions
{
    /// <summary>
    /// Produces another list with the same objects deeply cloned using
    /// their implementation of <see cref="IDeepCloneable{TSelf}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <returns></returns>
    public static IEnumerable<T> DeepClone<T>(this IEnumerable<T> collection, CardGame cardGame)
            where T : IDeepCloneable<T>
    {
        return collection.Select(item => item.DeepClone(cardGame)).ToList();
    }
}

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

    //Quick Clone Method using memberwise clone
    public static T Clone<T>(this T obj)
    {
        var inst = obj.GetType().GetMethod("MemberwiseClone", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        return (T)inst?.Invoke(obj, null);
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

