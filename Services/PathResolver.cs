using dotpath.Models;

namespace dotpath.Services;

public static class PathResolver
{
    public static string Resolve(string value, List<SavedPath> savedPaths)
    {
        var trimmed = value.Trim().Trim('"');
        if (string.IsNullOrWhiteSpace(trimmed))
        {
            throw new ArgumentException("No path was supplied.");
        }

        if (trimmed.StartsWith('$'))
        {
            var variableName = SavedPath.NormalizeName(trimmed);
            var saved = savedPaths.FirstOrDefault(item => item.Name.Equals(variableName, StringComparison.OrdinalIgnoreCase));
            if (saved is null)
            {
                throw new InvalidOperationException($"Variable '{trimmed}' was not found.");
            }

            return saved.Path.Trim().Trim('"');
        }

        return trimmed;
    }

    public static string NormalizeName(string value)
    {
        return SavedPath.NormalizeName(value);
    }
}
