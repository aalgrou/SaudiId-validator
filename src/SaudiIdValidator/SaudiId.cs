using System.Globalization;
using System.Text;

namespace SaudiIdValidator
{
    /// <summary>
    /// Validates Saudi national ID, Iqama and border numbers.
    /// </summary>
    /// <remarks>
    /// A valid ID is 10 digits long, starts with 1, 2, 3 or 4, and passes the Luhn check.
    /// </remarks>
    public static class SaudiId
    {
        /// <summary>
        /// Validates a Saudi ID number given as a string.
        /// Surrounding whitespace is ignored and Arabic-Indic digits (٠-٩, ۰-۹) are accepted.
        /// </summary>
        /// <param name="input">The ID number to validate. <c>null</c> is treated as invalid.</param>
        /// <returns>The validation result.</returns>
        public static SaudiIdResult Validate(string? input)
        {
            var id = Normalize(input);

            // Check if the ID is in a valid format
            if (id.Length != 10 || !IsAsciiDigits(id))
            {
                return Result(id, SaudiIdCode.InvalidFormat);
            }

            // Check if the ID starts with 1, 2, 3, or 4
            if (id[0] < '1' || id[0] > '4')
            {
                return Result(id, SaudiIdCode.InvalidPrefix);
            }

            // Check if the ID is valid based on the Luhn sum
            if (LuhnSum(id) % 10 != 0)
            {
                return Result(id, SaudiIdCode.InvalidChecksum);
            }

            switch (id[0])
            {
                case '1':
                    return Result(id, SaudiIdCode.NationalId);
                case '2':
                    return Result(id, SaudiIdCode.Iqama);
                default:
                    return Result(id, SaudiIdCode.BorderNumber);
            }
        }

        /// <summary>
        /// Validates a Saudi ID number given as a number.
        /// </summary>
        /// <param name="input">The ID number to validate.</param>
        /// <returns>The validation result.</returns>
        public static SaudiIdResult Validate(long input) =>
            Validate(input.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Returns <c>true</c> when <paramref name="input"/> is a valid Saudi ID number.
        /// </summary>
        /// <param name="input">The ID number to validate.</param>
        public static bool IsValid(string? input) => Validate(input).IsValid;

        /// <summary>
        /// Returns <c>true</c> when <paramref name="input"/> is a valid Saudi ID number.
        /// </summary>
        /// <param name="input">The ID number to validate.</param>
        public static bool IsValid(long input) => Validate(input).IsValid;

        /// <summary>
        /// Returns the default English message for a code, e.g. as a fallback when
        /// your application has no localized text for it.
        /// </summary>
        /// <param name="code">The result code.</param>
        public static string GetDefaultMessage(SaudiIdCode code)
        {
            switch (code)
            {
                case SaudiIdCode.NationalId:
                    return "National ID";
                case SaudiIdCode.Iqama:
                    return "Iqama ID";
                case SaudiIdCode.BorderNumber:
                    return "Border Number";
                case SaudiIdCode.InvalidFormat:
                    return "ID should be 10 digits long";
                case SaudiIdCode.InvalidPrefix:
                    return "ID should start with 1, 2, 3, or 4";
                case SaudiIdCode.InvalidChecksum:
                    return "ID failed Luhn algorithm";
                default:
                    return code.ToString();
            }
        }

        private static SaudiIdResult Result(string id, SaudiIdCode code) =>
            new SaudiIdResult(id, code, GetDefaultMessage(code));

        // Doubles digits at even (0-based) positions, subtracting 9 when the result exceeds 9.
        private static int LuhnSum(string id)
        {
            var sum = 0;
            for (var i = 0; i < id.Length; i++)
            {
                var digit = id[i] - '0';
                if (i % 2 == 0)
                {
                    digit *= 2;
                    if (digit > 9)
                    {
                        digit -= 9;
                    }
                }

                sum += digit;
            }

            return sum;
        }

        private static bool IsAsciiDigits(string value)
        {
            foreach (var c in value)
            {
                if (c < '0' || c > '9')
                {
                    return false;
                }
            }

            return true;
        }

        // Trims whitespace and converts Arabic-Indic (U+0660-0669) and
        // Extended Arabic-Indic (U+06F0-06F9) digits to ASCII digits.
        private static string Normalize(string? input)
        {
            if (input == null)
            {
                return string.Empty;
            }

            var trimmed = input.Trim();
            var builder = new StringBuilder(trimmed.Length);
            foreach (var c in trimmed)
            {
                if (c >= '٠' && c <= '٩')
                {
                    builder.Append((char)('0' + (c - '٠')));
                }
                else if (c >= '۰' && c <= '۹')
                {
                    builder.Append((char)('0' + (c - '۰')));
                }
                else
                {
                    builder.Append(c);
                }
            }

            return builder.ToString();
        }
    }
}
