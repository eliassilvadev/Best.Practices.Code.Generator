namespace BestPracticesCodeGenerator.Extensions
{
    public static class StringExtension
    {
        public static string GetWordWithFirstLetterDown(this string word)
        {
            return $"{word.ToLower()[0]}{word.Substring(1)}";
        }

        public static string ReplaceFirstOccurrence(this string texto, string oldValue, string newValue)
        {
            int index = texto.IndexOf(oldValue);
            if (index < 0)
            {
                return texto;
            }
            return texto.Substring(0, index) + newValue + texto.Substring(index + oldValue.Length);
        }
    }
}