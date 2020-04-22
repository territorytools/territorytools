/*
 * ShortURL (https://github.com/delight-im/ShortURL)
 * Copyright (c) delight.im (https://www.delight.im/)
 * Licensed under the MIT License (https://opensource.org/licenses/MIT)
 */

using System.Linq;
using System.Text;

namespace TerritoryTools.Entities
{
     // Vowels and unclear letters I, l, 1, 0, O removed
    public class AlphaNumberId
    {
        const string Characters = 
            "23456789bcdfghjkmnpqrstvwxyzBCDFGHJKLMNPQRSTVWXYZ";

        static readonly int CharactersLength = Characters.Length;

        public static string ToAlphaNumberId(int id)
        {
            var builder = new StringBuilder();
            while (id > 0)
            {
                builder.Insert(0, Characters.ElementAt(id % CharactersLength));
                id = id / CharactersLength;
            }

            return builder.ToString();
        }

        public static int ToIntegerId(string alphaNumberId)
        {
            int id = 0;
            for (var i = 0; i < alphaNumberId.Length; i++)
            {
                id = id * CharactersLength 
                    + Characters.IndexOf(alphaNumberId.ElementAt(i));
            }

            return id;
        }
    }
}
