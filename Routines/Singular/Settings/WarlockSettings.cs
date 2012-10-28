#region Revision Info

// This file is part of Singular - A community driven Honorbuddy CC
// $Author: Nesox $
// $Date: 2012-09-07 21:55:59 +0000 (Fr, 07 Sep 2012) $
// $HeadURL: https://subversion.assembla.com/svn/singular_v3/trunk/Singular/Settings/WarlockSettings.cs $
// $LastChangedBy: Nesox $
// $LastChangedDate: 2012-09-07 21:55:59 +0000 (Fr, 07 Sep 2012) $
// $LastChangedRevision: 684 $
// $Revision: 684 $

#endregion

using System.IO;

namespace Singular.Settings
{
    internal class WarlockSettings : Styx.Helpers.Settings
    {
        public WarlockSettings()
            : base(Path.Combine(SingularSettings.SettingsPath, "Warlock.xml"))
        {
        }
    }
}