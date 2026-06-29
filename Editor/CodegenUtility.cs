using System.Text;

namespace Whaledevelop
{
    public static class CodegenUtility
    {
        public static string ToPrivateFieldName(string name)
        {
            var builder = new StringBuilder();
            foreach (var character in name)
            {
                if (char.IsLetterOrDigit(character))
                {
                    builder.Append(character);

                    continue;
                }

                builder.Append('_');
            }
            return builder.Length == 0 ? string.Empty : builder.ToString();
        }
    }
}