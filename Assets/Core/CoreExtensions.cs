using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
