using SmartX.Api.Models;

namespace SmartX.Api.Utilities;

public static class DeploymentValidator
{
    public static List<string> Validate(
        DeploymentNode node,
        string parentPath = "")
    {
        var errors = new List<string>();

        string currentPath = string.IsNullOrWhiteSpace(parentPath)
            ? node.Name
            : $"{parentPath} -> {node.Name}";

        if (!node.IsConfigured)
        {
            errors.Add($"Invalid deployment node: {currentPath}");
        }

        foreach (var child in node.Children)
        {
            errors.AddRange(
                Validate(child, currentPath));
        }

        return errors;
    }
}