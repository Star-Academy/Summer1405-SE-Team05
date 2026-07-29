using System;
using System.Collections.Generic;

namespace SearchApp
{
    public static class LanguageDictionary
    {
       
        public static readonly Dictionary<string, CommandType> Commands = new(StringComparer.OrdinalIgnoreCase)
        {
            // انگلیسی
            { "exit", CommandType.Exit },
            { "search", CommandType.Search },
            { "back", CommandType.Back },
            { "forward", CommandType.Forward },
            { "current", CommandType.Current },
            { "stats", CommandType.Stats },
            { "unique", CommandType.Unique },

            
            { "خروج", CommandType.Exit },
            { "جستجو", CommandType.Search },
            { "عقب", CommandType.Back },
            { "جلو", CommandType.Forward },
            { "حاضر", CommandType.Forward },
            { "برترین", CommandType.Stats },
            { "خاص", CommandType.Unique },




        };
    }
}