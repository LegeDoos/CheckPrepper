# .NET 9.0 Upgrade Report

## Project target framework modifications

| Project name   | Old Target Framework | New Target Framework | Commits  |
|:----------------------|:--------------------:|:--------------------:|----------|
| CheckPrepper.csproj   | net8.0           | net9.0               | 59dcf4a2 |

## All commits

| Commit ID | Description   |
|:----------|:--------------------------------------------------------------------------------|
| a2ae0c1d  | Commit upgrade plan    |
| 59dcf4a2  | Update CheckPrepper.csproj to target net9.0    |

## Summary

The upgrade to .NET 9.0 was completed successfully. The target framework in CheckPrepper.csproj was changed from net8.0 to net9.0. No other project settings or NuGet packages required modifications.

## Next steps

- Test the application thoroughly to ensure all functionality works correctly with .NET 9.0
- Review and update any documentation that references .NET 8.0
- Consider reviewing the [.NET 9.0 release notes](https://learn.microsoft.com/dotnet/core/whats-new/dotnet-9/overview) for new features and improvements you might want to leverage
