using System;
using System.Collections.Generic;
using UclaExt.Common.Interfaces;

namespace UclaExt.Common.Extensions
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Prints a sequence of items to the 
        /// console using the ToString method.
        /// </summary>
        /// <param name="items">IEnumerable sequence of items</param>
        public static void PrintToConsole(this IEnumerable<object> items)
        {
            foreach (IDisplayable idisp in items)
            {
                Console.WriteLine(idisp.ToString());
            }
        }


        /// <summary>
        /// Prints a sequence of IDisplayable items to the 
        /// console using the Display method.
        /// </summary>
        /// <param name="items">IEnumerable sequence of IDisplayable items</param>
        public static void PrintToConsole(this IEnumerable<IDisplayable> items)
        {
            foreach (IDisplayable idisp in items)
            {
                Console.WriteLine(idisp.Display());
            }
        }

    }
}
