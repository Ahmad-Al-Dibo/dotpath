namespace dotpath.Services;

public static class CommandParser
{
    public static (string Command, string Argument) Parse(string input)
    {
        var trimmed = input.Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
        {
            return (string.Empty, string.Empty);
        }

        var index = trimmed.IndexOfAny([' ', '\t']);
        if (index < 0)
        {
            return (trimmed.ToLowerInvariant(), string.Empty);
        }

        var command = trimmed[..index];
        var argument = trimmed[(index + 1)..].Trim();
        return (command.ToLowerInvariant(), argument);
    }
}
