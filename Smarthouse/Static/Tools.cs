using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smarthouse.Repositories.Tools
{
    public static class ToolsAndUtilities
    {
        public static char GetLastChar(string stringToCheck)
        {
            char lastChar = stringToCheck.Last();
            return lastChar;
        }

        public static string CutLastChars(string stringToCut, int howManyChars)
        {
            // jeżeli podana liczba jest większa niż długość wyrazu
            if (howManyChars > stringToCut.Length) howManyChars = stringToCut.Length - 1;
            return stringToCut.Remove(stringToCut.Length - howManyChars);
        }

    }
}
