# Coding Standard

This document defines the coding style for the merged WinUI community and matchmaking application. All team members should follow these rules for new code and for files they significantly modify.

## Rules

1. Use PascalCase for classes, records, structs, enums, public properties, public methods, events, and XAML page/control names.
2. Use camelCase for local variables, method parameters, and private helper method parameters.
3. Prefix private fields with `_` and use camelCase, for example `_profileService`.
4. Prefix interfaces with `I`, for example `IUsersService` and `ICommunitiesRepository`.
5. Use clear domain names instead of abbreviations; prefer `communityId` over `cid` and `profileRepository` over `repo` outside very small scopes.
6. Keep one public type per `.cs` file, and make the file name match the type name.
7. Keep namespaces aligned with folder structure whenever practical.
8. Use `var` only when the type is obvious from the right side, such as `new ProfileRepository(...)`; otherwise write the explicit type.
9. Keep methods focused on one responsibility. If a method needs comments to explain multiple phases, consider extracting private helper methods.
10. Do not use mock data in production code paths. Test-only mock helpers must stay in test projects or be clearly isolated from runtime flow.
11. Access databases through repository/service classes, not directly from views or viewmodels, except for simple composition at application startup.
12. Do not hard-code database connection strings, credentials, local machine names, or user IDs in committed code.
13. Do not swallow exceptions silently unless the failure is genuinely optional; when catching exceptions, show a useful user message or preserve the inner exception for debugging.
14. In async UI code, use `async`/`await` rather than blocking calls such as `.Result`, `.Wait()`, or `GetAwaiter().GetResult()`.
15. View code-behind should handle UI events and navigation only; business rules belong in services or viewmodels.
16. Viewmodels should not create windows, dialogs, or controls directly. Use events, commands, or navigation services to communicate with the UI layer.
17. Use dependency injection or explicit constructor parameters for services; avoid hidden static service lookups except at application composition boundaries.
18. Keep XAML readable: group layout, resources, and controls logically, and avoid deeply nested panels when a Grid or reusable UserControl is clearer.
19. Use `ObservableProperty` and commands consistently in CommunityToolkit viewmodels, and avoid directly modifying generated backing fields outside constructors.
20. Name SQL tables and columns consistently with the existing database schema. When adding SQL scripts, make them repeatable or clearly document required execution order.
21. Use parameterized SQL commands for all database input. Never concatenate user input into SQL strings.
22. Add or update tests when changing business logic, data mapping, validation, navigation decisions, or compatibility/profile calculations.
23. Keep comments short and useful. Do not comment obvious code; explain non-obvious decisions, workarounds, or external constraints.
24. Do not commit generated folders, IDE state, local NuGet caches, personal photos, or local config files such as `.vs/`, `.dotnet-cli/`, `bin/`, `obj/`, and machine-specific appsettings.

## Formatting

Use the default Visual Studio/Rider C# formatter for this solution. Before opening a pull request, format the files you touched and build the solution.

## StyleCop

If StyleCop or another analyzer conflicts with these rules, update the analyzer configuration or discuss the rule change with the technical lead before merging.
