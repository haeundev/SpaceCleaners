using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveLarson.Util
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string source)
        {
            return string.IsNullOrEmpty(source);
        }

        private static string[] pluralExceptions = { "s", "sh", "ch", "x", "z" };

        public static string ToPlural(this string text)
        {
            if (pluralExceptions.Any(p => text.EndsWith(p)))
                return text + "es";
            return text + "s";
        }

        public static string ToUppers(this string text)
        {
            var builder = new StringBuilder(text);
            for (var i = 0; i < builder.Length; ++i)
                builder[i] = char.ToUpper(builder[i]);
            return builder.ToString();
        }

        public static string ToLowers(this string text)
        {
            var builder = new StringBuilder(text);
            for (var i = 0; i < builder.Length; ++i)
                builder[i] = char.ToLower(builder[i]);
            return builder.ToString();
        }

        public static IEnumerable<int> AllIndexesOf(this string source, char character, int startIndex = 0)
        {
            for (var index = startIndex;; ++index)
            {
                index = source.IndexOf(character, index);
                if (index == -1)
                    yield break;
                yield return index;
            }
        }
        
        public static string[] SplitIntoTwo(this string sentence)
        {
            // Find the middle point index of the sentence
            var middleIndex = sentence.Length / 2;
            // Find the nearest space to the middle index from the left
            var leftSplitIndex = sentence.LastIndexOf(' ', middleIndex);
            // Find the nearest space to the middle index from the right
            var rightSplitIndex = sentence.IndexOf(' ', middleIndex);
            // Check which space is closer to the middle point and choose that as the split index
            var splitIndex = (middleIndex - leftSplitIndex) <= (rightSplitIndex - middleIndex)
                ? leftSplitIndex
                : rightSplitIndex;
            // Adjust the split point to avoid breaking a word
            if (splitIndex == -1 || splitIndex == 0 || splitIndex == sentence.Length - 1)
            {
                // If there are no spaces or the split point is at the beginning/end of the sentence
                // then simply split at the middle point
                splitIndex = middleIndex;
            }
            // Split the sentence into two parts
            var sentence1 = sentence.Substring(0, splitIndex).Trim();
            var sentence2 = sentence.Substring(splitIndex).Trim();

            // Return the result as an array
            return new string[] { sentence1, sentence2 };
        }
    }
}