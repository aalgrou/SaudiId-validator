namespace SaudiIdValidator
{
    /// <summary>
    /// The result code of a validation. Values 1-99 mean the ID is valid and identify its type;
    /// values 100 and above are error codes. Numeric values are stable and safe to persist or
    /// use as localization keys.
    /// </summary>
    public enum SaudiIdCode
    {
        /// <summary>Valid Saudi citizen national ID (starts with 1).</summary>
        NationalId = 1,

        /// <summary>Valid resident Iqama ID (starts with 2).</summary>
        Iqama = 2,

        /// <summary>Valid border number (starts with 3 or 4).</summary>
        BorderNumber = 3,

        /// <summary>Error: the ID is not exactly 10 digits (includes empty or <c>null</c> input).</summary>
        InvalidFormat = 101,

        /// <summary>Error: the ID does not start with 1, 2, 3 or 4.</summary>
        InvalidPrefix = 102,

        /// <summary>Error: the ID failed the Luhn checksum.</summary>
        InvalidChecksum = 103,
    }
}
