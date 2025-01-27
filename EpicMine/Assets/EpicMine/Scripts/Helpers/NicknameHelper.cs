using System.Collections.Generic;

namespace BlackTemple.EpicMine
{
    public static class NicknameHelper
    {
        private static readonly Dictionary<char, string[]> _charsTable = new Dictionary<char, string[]>
        {
            { 'а', new [] { "a" } },
            { 'б', new [] { "b", "6" } },
            { 'в', new [] { "v", "b" } },
            { 'г', new [] { "g", "r" } },
            { 'д', new [] { "d", "g" } },
            { 'е', new [] { "e" } },
            { 'ё', new [] { "e" } },
            { 'ж', new [] { "zh" } },
            { 'з', new [] { "z", "3" } },
            { 'и', new [] { "i", "u" } },
            { 'й', new [] { "y", "i", "u" } },
            { 'к', new [] { "k" } },
            { 'л', new [] { "l" } },
            { 'м', new [] { "m" } },
            { 'н', new [] { "n", "h" } },
            { 'о', new [] { "o", "0" } },
            { 'п', new [] { "p", "n" } },
            { 'р', new [] { "r", "p" } },
            { 'с', new [] { "s", "c" } },
            { 'т', new [] { "t", "m" } },
            { 'у', new [] { "u", "y" } },
            { 'ф', new [] { "f" } },
            { 'х', new [] { "h", "x" } },
            { 'ц', new [] { "c", "u" } },
            { 'ч', new [] { "ch" } },
            { 'ш', new [] { "sh" } },
            { 'щ', new [] { "sch" } },
            { 'ь', new [] { "b" } },
            { 'ъ', new [] { "" } },
            { 'ы', new [] { "y", "bi" } },
            { 'э', new [] { "e" } },
            { 'ю', new [] { "yu" } },
            { 'я', new [] { "ya" } },
            { ' ', new [] { "_" } },
            { '-', new [] { "_" } },
            { '_', new [] { "_" } },
            { 'a', new [] { "a" } },
            { 'b', new [] { "b" } },
            { 'c', new [] { "c" } },
            { 'd', new [] { "d" } },
            { 'e', new [] { "e" } },
            { 'f', new [] { "f" } },
            { 'g', new [] { "g" } },
            { 'h', new [] { "h" } },
            { 'i', new [] { "i" } },
            { 'j', new [] { "j" } },
            { 'k', new [] { "k" } },
            { 'l', new [] { "l" } },
            { 'm', new [] { "m" } },
            { 'n', new [] { "n" } },
            { 'o', new [] { "o" } },
            { 'p', new [] { "p" } },
            { 'q', new [] { "q" } },
            { 'r', new [] { "r" } },
            { 's', new [] { "s" } },
            { 't', new [] { "t" } },
            { 'v', new [] { "v" } },
            { 'u', new [] { "u" } },
            { 'w', new [] { "w" } },
            { 'x', new [] { "x" } },
            { 'y', new [] { "y" } },
            { 'z', new [] { "z" } },
            { '0', new [] { "0" } },
            { '1', new [] { "1" } },
            { '2', new [] { "2" } },
            { '3', new [] { "3" } },
            { '4', new [] { "4" } },
            { '5', new [] { "5" } },
            { '6', new [] { "6" } },
            { '7', new [] { "7" } },
            { '8', new [] { "8" } },
            { '9', new [] { "9" } },
        };

        public static List<string> GetWordTranslitVariants(string word)
        {
            var wordCharsVariants = new List<string[]>();

            foreach (var wordChar in word)
            {
                foreach (var replacedCharVariants in _charsTable)
                {
                    if (replacedCharVariants.Key == wordChar)
                        wordCharsVariants.Add(replacedCharVariants.Value);
                }
            }

            var wordVariants = new List<string>();
            var wordLength = word.Length;
            var wordCharsVariantsCurrentIndexes = new int[wordLength];

            while (wordCharsVariantsCurrentIndexes[0] < wordCharsVariants[0].Length)
            {
                var wordVariant = string.Empty;

                for (var i = 0; i < wordLength; i++)
                {
                    var charVariants = wordCharsVariants[i];
                    var charCurrentVariant = charVariants[wordCharsVariantsCurrentIndexes[i]];
                    wordVariant += charCurrentVariant;
                }

                if (!wordVariants.Contains(wordVariant))
                    wordVariants.Add(wordVariant);

                wordCharsVariantsCurrentIndexes[wordLength - 1]++;

                for (var i = wordLength - 1; i > 0; i--)
                {
                    var charCurrentVariantIndex = wordCharsVariantsCurrentIndexes[i];
                    if (charCurrentVariantIndex >= wordCharsVariants[i].Length)
                    {
                        wordCharsVariantsCurrentIndexes[i - 1]++;
                        wordCharsVariantsCurrentIndexes[i] = 0;
                    }
                }
            }

            return wordVariants;
        }
    }
}