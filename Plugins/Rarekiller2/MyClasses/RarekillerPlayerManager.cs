//=================================================================
//
//				      Rarekiller - Plugin
//						Autor: katzerle
//			Honorbuddy Plugin - www.thebuddyforum.com
//    Credits to highvoltz, bloodlove, SMcCloud, Lofi, ZapMan 
//                and all the brave Testers
//
//==================================================================

// Just copied from another Plugin ;)

using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Xml.Linq;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.Windows.Media;
using System.Net;
using System.Globalization;

using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.Helpers;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.Plugins;

namespace BadWolf
{
    class RarekillerPlayerManager
    {
        public Dictionary<WoWPlayer, double> players = new Dictionary<WoWPlayer, double>();
        private static Stopwatch Watch = new Stopwatch();
        public WoWPlayer FollowingPlayer;

        public void update()
        {
            double lastRun = Watch.Elapsed.TotalSeconds;
            Watch.Reset();
            Watch.Start();

            if (Rarekiller.Settings.DeveloperLogs)
                Logging.Write("Rarekiller: Scan for other Players");
            ObjectManager.Update();
            Dictionary<WoWPlayer, double> newplayers = new Dictionary<WoWPlayer, double>();

            foreach (WoWPlayer player in ObjectManager.GetObjectsOfType<WoWPlayer>())
            {
				if (!(Rarekiller.TrustPlayersList.Contains(player.Name)))
				{

					if (players.ContainsKey(player))
					{
						newplayers.Add(player, players[player] + lastRun);
						if (players[player] > Convert.ToInt64(Rarekiller.Settings.PlayerFollowsTime) * 0.8)
							Logging.Write(Colors.Red, "Plugin Part Security: Been followed for " + newplayers[player] + " seconds by " + player.Name);
					}
					else
					{
						newplayers.Add(player, 0);
					}
				}
            }
            players = newplayers;
        }

        public bool needExit(Int64 time)
        {
            foreach (KeyValuePair<WoWPlayer, double> pt in players)
            {
                if (pt.Value > time)
                {
                    FollowingPlayer = pt.Key;
                    return true;
                }
            }
            return false;
        }
    }
}
