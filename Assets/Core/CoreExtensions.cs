using System.Collections.Generic;
using System.Linq;

namespace Assets.Core
{
    internal static class CoreExtensions
    {
        /// <summary>
        /// Returns if a list of effects would need to have targets selected for at least one of the effects.
        /// </summary>
        /// <param name="effects"></param>
        /// <returns></returns>
        public static bool NeedsTargets(this List<Effect> effects)
        {
            var targetInfo = effects.Where(e => e.NeedsTargets());
            return targetInfo.Any();
        }

        /// <summary>
        /// Capitalizes the first character of a string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string UpperFirst(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // Convert the first letter to uppercase and concatenate the rest of the string
            string result = char.ToUpper(input[0]) + input.Substring(1).ToLower();
            return result;
        }

        public static string LowerFirst(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // Convert the first letter to uppercase and concatenate the rest of the string
            string result = char.ToLower(input[0]) + input.Substring(1).ToLower();
            return result;
        }
    }
}
