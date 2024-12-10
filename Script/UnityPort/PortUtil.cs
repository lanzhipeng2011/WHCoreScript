using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityPort
{
    public class PortUtil
    {
        public static Dictionary<K, V> HashtableToDictionary<K, V>(Hashtable table)
        {
            return table
            .Cast<DictionaryEntry>()
            .ToDictionary(kvp => (K)kvp.Key, kvp => (V)kvp.Value);
        }


        public static byte[] StringToASCII(string s)
        {
            byte[] retVal = new byte[s.Length];

            for ( int i = 0; i < s.Length; ++i )
            {
                char ch = s[i];
                if ( ch <= 0x7f )
                {
                    retVal[i] = (byte)ch;
                }
                else
                {
                    retVal[i] = (byte)'?';
                }
            }

            return retVal;
        }
    }

}
