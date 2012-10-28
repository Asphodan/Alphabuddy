#region Revision Info

// This file is part of Singular - A community driven Honorbuddy CC
// $Author: bobby53 $
// $Date: 2012-10-04 04:30:19 +0000 (Do, 04 Okt 2012) $
// $HeadURL: https://subversion.assembla.com/svn/singular_v3/trunk/Singular/Extensions.cs $
// $LastChangedBy: bobby53 $
// $LastChangedDate: 2012-10-04 04:30:19 +0000 (Do, 04 Okt 2012) $
// $LastChangedRevision: 864 $
// $Revision: 864 $

#endregion

using System.Text;
using Styx;
using Styx.CommonBot;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace Singular
{
    internal static class Extensions
    {

        public static bool Between(this double distance, double min, double max)
        {
            return distance >= min && distance <= max;
        }

        /// <summary>
        ///   A string extension method that turns a Camel-case string into a spaced string. (Example: SomeCamelString -> Some Camel String)
        /// </summary>
        /// <remarks>
        ///   Created 2/7/2011.
        /// </remarks>
        /// <param name = "str">The string to act on.</param>
        /// <returns>.</returns>
        public static string CamelToSpaced(this string str)
        {
            var sb = new StringBuilder();
            foreach (char c in str)
            {
                if (char.IsUpper(c))
                {
                    sb.Append(' ');
                }
                sb.Append(c);
            }
            return sb.ToString();
        }

        public static bool ShowPlayerNames { get; set; }

        public static string SafeName(this WoWObject obj)
        {
            if (obj.IsMe)
            {
                return "Myself";
            }

            string name;
            if (obj is WoWPlayer)
            {
                if (RaFHelper.Leader == obj)
                    return "Tank";

                name = ShowPlayerNames ? ((WoWPlayer)obj).Name : ((WoWPlayer)obj).Class.ToString();
            }
            else if (obj is WoWUnit && obj.ToUnit().IsPet)
            {
                name = "Pet";
            }
            else
            {
                name = obj.Name;
            }

            return name;
        }

        public static bool IsWanding(this LocalPlayer me)
        {
            return StyxWoW.Me.AutoRepeatingSpellId == 5019;
        }
    }
}