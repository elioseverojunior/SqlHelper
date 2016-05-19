using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlHelper.Extensions
{
    public static class StringExtensions
    {
        public static string Capitalize(this string text)
        {
            return string.IsNullOrEmpty(text) ? "" : (text.Substring(0, 1).ToUpper()) + (text.Substring(1).ToLower());
        }

        public static string Capitalize(this string text, char splitter = ' ')
        {
            List<string> Resultado = new List<string>();
            if (string.IsNullOrEmpty(text))
                return "";

            foreach (string s in text.Split(splitter).ToList())
            {
                Resultado.Add(s.Capitalize());
            }

            return string.Join(splitter.ToString(), Resultado);
        }
    }
}
