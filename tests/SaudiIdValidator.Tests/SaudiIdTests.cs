namespace SaudiIdValidator.Tests;

public class SaudiIdTests
{
    // Builds a valid 10-digit ID from a 9-digit prefix by computing the Luhn check digit.
    private static string WithCheckDigit(string prefix)
    {
        var sum = 0;
        for (var i = 0; i < prefix.Length; i++)
        {
            var digit = prefix[i] - '0';
            if (i % 2 == 0)
            {
                digit *= 2;
                if (digit > 9) digit -= 9;
            }
            sum += digit;
        }

        return prefix + (10 - sum % 10) % 10;
    }

    private static string FlipLastDigit(string id) =>
        id[..9] + (char)('0' + (id[9] - '0' + 1) % 10);

    [Theory]
    [InlineData("100000000", SaudiIdCode.NationalId, "National ID")]
    [InlineData("108765432", SaudiIdCode.NationalId, "National ID")]
    [InlineData("200000000", SaudiIdCode.Iqama, "Iqama ID")]
    [InlineData("234567890", SaudiIdCode.Iqama, "Iqama ID")]
    [InlineData("312345678", SaudiIdCode.BorderNumber, "Border Number")]
    [InlineData("498765432", SaudiIdCode.BorderNumber, "Border Number")]
    public void Valid_ids_return_type_code(string prefix, SaudiIdCode code, string message)
    {
        var id = WithCheckDigit(prefix);

        var result = SaudiId.Validate(id);

        Assert.True(result.IsValid);
        Assert.Equal(id, result.Id);
        Assert.Equal(code, result.Code);
        Assert.Equal(message, result.Message);
    }

    [Theory]
    [InlineData("100000000")]
    [InlineData("234567890")]
    [InlineData("498765432")]
    public void Wrong_check_digit_fails_luhn(string prefix)
    {
        var id = FlipLastDigit(WithCheckDigit(prefix));

        var result = SaudiId.Validate(id);

        Assert.False(result.IsValid);
        Assert.Equal(SaudiIdCode.InvalidChecksum, result.Code);
        Assert.Equal("ID failed Luhn algorithm", result.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("123456789")]
    [InlineData("12345678901")]
    [InlineData("12345a7890")]
    [InlineData("1234-67890")]
    [InlineData("1234 67890")]
    [InlineData("١٢٣٤٥٦٧٨٩")]
    public void Wrong_format_fails_length_check(string input)
    {
        var result = SaudiId.Validate(input);

        Assert.False(result.IsValid);
        Assert.Equal(SaudiIdCode.InvalidFormat, result.Code);
        Assert.Equal("ID should be 10 digits long", result.Message);
    }

    [Fact]
    public void Null_is_invalid_and_does_not_throw()
    {
        var result = SaudiId.Validate((string?)null);

        Assert.False(result.IsValid);
        Assert.Equal(string.Empty, result.Id);
        Assert.Equal(SaudiIdCode.InvalidFormat, result.Code);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("5")]
    [InlineData("9")]
    public void Wrong_first_digit_fails_prefix_check(string first)
    {
        // Luhn-valid, so only the prefix rule can reject it
        var id = WithCheckDigit(first + "12345678");

        var result = SaudiId.Validate(id);

        Assert.False(result.IsValid);
        Assert.Equal(SaudiIdCode.InvalidPrefix, result.Code);
        Assert.Equal("ID should start with 1, 2, 3, or 4", result.Message);
    }

    [Fact]
    public void Arabic_indic_and_persian_digits_are_normalized()
    {
        var id = WithCheckDigit("108765432");
        var arabic = new string(id.Select(c => (char)('٠' + (c - '0'))).ToArray());
        var persian = new string(id.Select(c => (char)('۰' + (c - '0'))).ToArray());

        Assert.Equal(id, SaudiId.Validate(arabic).Id);
        Assert.True(SaudiId.Validate(arabic).IsValid);
        Assert.True(SaudiId.Validate(persian).IsValid);
    }

    [Fact]
    public void Surrounding_whitespace_is_trimmed()
    {
        var id = WithCheckDigit("234567890");

        var result = SaudiId.Validate($"  {id}\t\n");

        Assert.True(result.IsValid);
        Assert.Equal(id, result.Id);
    }

    [Fact]
    public void Other_unicode_digits_are_rejected()
    {
        // Devanagari digits are char.IsDigit == true but must not be accepted
        var id = WithCheckDigit("108765432");
        var devanagari = new string(id.Select(c => (char)('०' + (c - '0'))).ToArray());

        Assert.False(SaudiId.IsValid(devanagari));
    }

    [Fact]
    public void Long_overload_matches_string_overload()
    {
        var id = WithCheckDigit("108765432");

        Assert.True(SaudiId.IsValid(long.Parse(id)));
        Assert.Equal(SaudiIdCode.NationalId, SaudiId.Validate(long.Parse(id)).Code);
        Assert.False(SaudiId.IsValid(long.Parse(FlipLastDigit(id))));
        Assert.False(SaudiId.IsValid(-1L));
    }

    [Fact]
    public void Code_numeric_values_are_stable()
    {
        // Consumers may persist these or use them as localization keys; never renumber.
        Assert.Equal(1, (int)SaudiIdCode.NationalId);
        Assert.Equal(2, (int)SaudiIdCode.Iqama);
        Assert.Equal(3, (int)SaudiIdCode.BorderNumber);
        Assert.Equal(101, (int)SaudiIdCode.InvalidFormat);
        Assert.Equal(102, (int)SaudiIdCode.InvalidPrefix);
        Assert.Equal(103, (int)SaudiIdCode.InvalidChecksum);
    }

    [Fact]
    public void Every_code_has_a_default_message()
    {
        foreach (var code in Enum.GetValues<SaudiIdCode>())
        {
            Assert.NotEqual(code.ToString(), SaudiId.GetDefaultMessage(code));
        }
    }
}
