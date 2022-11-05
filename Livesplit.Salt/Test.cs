using System;
using System.Collections.Generic;
using System.Threading;

namespace LiveSplit.Salt
{
    public static class Test
    {
        public static void Main()
        {
            var state = new Model.LiveSplitState(
                new Model.Run(new Model.Comparisons.StandardComparisonGeneratorsFactory()),
                null, null, null, null);
            using SaltComponent comp = new SaltComponent(state, "mine");
            

            //while (true)
            //{
            //    Thread.Sleep(500);

            //    Console.WriteLine("===== New scan =====");
            //    comp._mem.Hook();
            //    if (!comp._mem.IsHooked)
            //    {
            //        Console.WriteLine("----- Not Hooked yet -----");
            //        continue;
            //    }

            //    if (comp._mem.GetPlayerCount() == 0)
            //    {
            //        Console.WriteLine("----- 0 players -----");
            //        continue;
            //    }

            //    var player = comp._mem.GetPlayer(0);
            //    Console.WriteLine("Scanning player " + 0 + " (" + player + ")");

            //    comp._mem.UpdateInventory(0);
            //    Console.WriteLine("- Animation: " + comp._mem.GetPlayerAnim(0));
            //    Console.WriteLine("- Inventory: " + comp._mem._playerItems[0].Keys.Count);
            //}
        }
    }
}
