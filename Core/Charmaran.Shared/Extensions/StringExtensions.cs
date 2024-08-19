using System.Text.RegularExpressions;

namespace Charmaran.Shared.Extensions
{
    public static class StringExtensions
    {
        public static string ToUpperFirstLetter(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.Empty;
            }

            source = source.ToLower();
            char[] letters = source.ToCharArray();
            letters[0] = char.ToUpper(letters[0]);
			
            return new string(letters);
        }

        public static bool HasFirstAndLastName(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return false;
            }

            string[] words = source.Split(' ');

            if (words.Length == 2)
            {
                return true;
            }

            return false;
        }

        public static bool ContainsLettersOnly(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return false;
            }

            return Regex.IsMatch(source, @"^[a-zA-Z]+\s+[a-zA-Z]+$");
        }

        public static string CleanName(this string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }

            string[] names = name.Split(' ');
            return $"{names[0].ToUpperFirstLetter()} {names[1].ToUpperFirstLetter()}";
        }
    }
}