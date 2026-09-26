# <img src="https://raw.githubusercontent.com/aalgrou/SaudiId-validator/main/assets/icon.png" alt="" width="40" align="top"> SaudiIdValidator

[![NuGet](https://img.shields.io/nuget/v/SaudiIdValidator.svg)](https://www.nuget.org/packages/SaudiIdValidator)
[![NuGet downloads](https://img.shields.io/nuget/dt/SaudiIdValidator.svg)](https://www.nuget.org/packages/SaudiIdValidator)
[![Build](https://github.com/aalgrou/SaudiId-validator/actions/workflows/publish.yml/badge.svg)](https://github.com/aalgrou/SaudiId-validator/actions/workflows/publish.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/aalgrou/SaudiId-validator/blob/main/LICENSE)

Validates Saudi identification numbers — **National ID**, **Iqama** and **Border numbers**.

Every result carries a stable **code** (the ID type when valid, an error code when invalid) plus a default English message, so you can show your own localized text in any language.

A number is valid when it:

1. is exactly 10 digits,
2. starts with `1`, `2`, `3` or `4`,
3. passes the Luhn checksum.

Input is trimmed and Arabic-Indic digits (`٠-٩`, `۰-۹`) are converted to ASCII digits before validating.

Targets `netstandard2.0` and `net8.0`, so it works on .NET Framework 4.6.1+, .NET Core, and .NET 5+.

## Install

```sh
dotnet add package SaudiIdValidator
```

## Usage

```csharp
using SaudiIdValidator;

// Quick check
bool ok = SaudiId.IsValid("1087654321");

// Full result
SaudiIdResult result = SaudiId.Validate("1087654321");

Console.WriteLine(result.IsValid);  // True
Console.WriteLine(result.Code);     // NationalId
Console.WriteLine(result.Message);  // National ID

// Numeric input is supported too
SaudiId.IsValid(1087654321L);
```

### Result codes

| Code              | Value | Valid | Default message                      |
|-------------------|-------|-------|--------------------------------------|
| `NationalId`      | 1     | ✔     | National ID                          |
| `Iqama`           | 2     | ✔     | Iqama ID                             |
| `BorderNumber`    | 3     | ✔     | Border Number (IDs starting 3 or 4)  |
| `InvalidFormat`   | 101   | ✘     | ID should be 10 digits long          |
| `InvalidPrefix`   | 102   | ✘     | ID should start with 1, 2, 3, or 4   |
| `InvalidChecksum` | 103   | ✘     | ID failed Luhn algorithm             |

Values below 100 are valid ID types, and values of 100 and above are errors. The numeric values are stable and will not change, so it's safe to store them or use them as keys.

### Localizing messages

Map `result.Code` to your own text, for example with a `.resx` resource file or a dictionary:

```csharp
var arabic = new Dictionary<SaudiIdCode, string>
{
    [SaudiIdCode.NationalId]      = "هوية وطنية",
    [SaudiIdCode.Iqama]           = "بطاقة إقامة",
    [SaudiIdCode.BorderNumber]    = "رقم حدود",
    [SaudiIdCode.InvalidFormat]   = "رقم الهوية يجب أن يتكون من 10 أرقام",
    [SaudiIdCode.InvalidPrefix]   = "رقم الهوية يجب أن يبدأ بـ 1، 2، 3، أو 4",
    [SaudiIdCode.InvalidChecksum] = "رقم الهوية غير صحيح",
};

var result = SaudiId.Validate(input);
string text = arabic.TryGetValue(result.Code, out var t) ? t : result.Message;
```

`SaudiId.GetDefaultMessage(code)` returns the English default for any code.

### `SaudiIdResult`

| Property  | Type          | Description                                                    |
|-----------|---------------|----------------------------------------------------------------|
| `IsValid` | `bool`        | `true` when all checks pass                                    |
| `Id`      | `string`      | The normalized ID that was validated                           |
| `Code`    | `SaudiIdCode` | ID type when valid, error code when invalid                    |
| `Message` | `string`      | Default English message (type name or failure reason)          |

## Console app

The repository includes a small console app:

```sh
# validate arguments (exit code 0 if all valid, 1 otherwise)
dotnet run --project src/SaudiIdValidator.ConsoleApp -- 1087654321 2345678904

# interactive mode
dotnet run --project src/SaudiIdValidator.ConsoleApp
```

## Project structure

```
src/SaudiIdValidator/             Core library (the NuGet package)
src/SaudiIdValidator.ConsoleApp/  Console app for trying the validator
tests/SaudiIdValidator.Tests/     xUnit tests
```

## Build & test

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```sh
dotnet build
dotnet test
dotnet pack src/SaudiIdValidator -c Release -o artifacts
```

## Publishing

Pushing a tag like `v1.0.0` triggers the GitHub Actions workflow in `.github/workflows/publish.yml`, which tests, packs and pushes the package to NuGet using [Trusted Publishing](https://learn.microsoft.com/en-us/nuget/nuget-org/trusted-publishing), so no long-lived API key is stored. It requires a Trusted Publishing policy on nuget.org for this repository and workflow file (`publish.yml`), and a repository secret `NUGET_USER` containing the nuget.org username. The tag version overrides the `<Version>` in the csproj.

## Contributing

Issues and pull requests are welcome at [github.com/aalgrou/SaudiId-validator](https://github.com/aalgrou/SaudiId-validator/issues). Please add or update tests for any behavior change.

## Author

**Abdullah AlGrou** — [@aalgrou](https://github.com/aalgrou)

## License

[MIT](https://github.com/aalgrou/SaudiId-validator/blob/main/LICENSE) © Abdullah AlGrou
