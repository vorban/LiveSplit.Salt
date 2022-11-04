using System;
using System.Collections.Generic;
using System.Threading;

namespace LiveSplit.Salt
{
    public static class Test
    {
        public static void Main()
        {
            var state = new Model.LiveSplitState(new Model.Run(new Model.Comparisons.StandardComparisonGeneratorsFactory()), null, null, null, null);
            using SaltComponent comp = new SaltComponent(state, "mine");
            

            while (true)
            {
                Thread.Sleep(500);

                //mem.UpdateInventory(0);
                comp.Update(null, state, 0, 0, UI.LayoutMode.Horizontal);
                //foreach (int i in mem._playerItems.Keys)
                //{
                //    Console.WriteLine("ITEM " + i + "(" + mem._playerItems[i].Count + ")");
                //    foreach (string s in mem._playerItems[i].Keys)
                //    {
                //        var item = mem._playerItems[i][s];

                //        Console.WriteLine("\t- " + s + " (" + item.count + ")");
                //    }
                //}
            }
        }
    }
}
