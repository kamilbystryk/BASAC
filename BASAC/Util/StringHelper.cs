using System;
namespace BASAC.Util
{
    public static class StringHelper
    {
        public static string RemoveCharFromString(this string input, char charItem)
        {
            int Index = input.IndexOf(charItem);
            if (Index < 0)
            {
                return input;
            }
            return (RemoveCharFromString(input.Remove(Index, 1), charItem));
        }
    }
}
