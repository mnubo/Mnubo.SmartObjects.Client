using System;
using System.Linq;

namespace Mnubo.SmartObjects.Client.Test
{
    public static class StringGenerator
    {
        private const string Chars = "qwertyuiopasdfghjklzxcvbnm";
        private static readonly Random RandomSeed = new Random();
        
        public static string RandomString(int size = 8)
        {
            return new (Enumerable.Repeat(Chars, size).Select(s => s[RandomSeed.Next(s.Length)]).ToArray());
        }
    }
}