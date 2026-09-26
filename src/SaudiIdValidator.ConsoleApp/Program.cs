using System.Text;
using SaudiIdValidator;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

// Validate IDs passed as arguments, e.g. `dotnet run -- 1234567890 2345678901`
if (args.Length > 0)
{
    var allValid = true;
    foreach (var arg in args)
    {
        var result = SaudiId.Validate(arg);
        Print(result);
        allValid &= result.IsValid;
    }

    return allValid ? 0 : 1;
}

// Otherwise run interactively
Console.WriteLine("Saudi ID Validator — enter an ID to validate (empty line to exit).");
while (true)
{
    Console.Write("> ");
    var line = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(line))
    {
        return 0;
    }

    Print(SaudiId.Validate(line));
}

static void Print(SaudiIdResult result)
{
    Console.ForegroundColor = result.IsValid ? ConsoleColor.Green : ConsoleColor.Red;
    Console.WriteLine($"{result.Id}: {(result.IsValid ? "VALID" : "INVALID")}");
    Console.ResetColor();
    Console.WriteLine($"  Code:    {(int)result.Code} ({result.Code})");
    Console.WriteLine($"  Message: {result.Message}");
}
