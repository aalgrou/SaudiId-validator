namespace SaudiIdValidator
{
    /// <summary>
    /// The outcome of validating a Saudi ID number.
    /// </summary>
    public sealed class SaudiIdResult
    {
        internal SaudiIdResult(string id, SaudiIdCode code, string message)
        {
            Id = id;
            Code = code;
            Message = message;
        }

        /// <summary><c>true</c> when the ID passed all checks.</summary>
        public bool IsValid => (int)Code < 100;

        /// <summary>The normalized ID that was validated (trimmed, Arabic-Indic digits converted to ASCII).</summary>
        public string Id { get; }

        /// <summary>
        /// The ID type when valid, or the error code when invalid.
        /// Use this to show your own (localized) messages.
        /// </summary>
        public SaudiIdCode Code { get; }

        /// <summary>
        /// Default English description: the ID type when valid (e.g. "National ID"),
        /// or the failure reason when invalid (e.g. "ID failed Luhn algorithm").
        /// </summary>
        public string Message { get; }

        /// <inheritdoc />
        public override string ToString() => $"{Id}: {Code} ({Message})";
    }
}
