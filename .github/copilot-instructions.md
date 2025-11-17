# Copilot Instructions for HCommons

## Project Overview
HCommons is a C# commons library that provides utility types and helper functions. It targets multiple frameworks: net9.0, net8.0, netstandard2.0, and netstandard2.1.

## Project Structure
- `src/HCommons/` - Main library source code
- `src/HCommons.Tests/` - xUnit test project
- `HCommons.sln` - Solution file

## Build & Test Commands
```bash
# Build the solution
dotnet build HCommons.sln

# Run tests (net8.0 and net9.0 available in CI)
dotnet test HCommons.sln

# Build specific framework
dotnet build src/HCommons/HCommons.csproj -f net9.0
```

## Language & Framework
- **Language**: C# with LangVersion 13
- **Target Frameworks**: net9.0, net8.0, netstandard2.0, netstandard2.1
- **Features**: ImplicitUsings enabled, Nullable enabled
- **Test Framework**: xUnit v3 with Shouldly assertions
- **Mocking**: NSubstitute

## Testing Guidelines
- Use xUnit v3 with Shouldly for assertions
- Global usings include `Xunit`, `Shouldly`
- Tests target multiple frameworks: net6.0, net8.0, net9.0, net48
- Follow AAA (Arrange, Act, Assert) pattern in tests
  - If each part is not clearly separated, use comments to delineate sections
- Use descriptive names for test methods
- Mock dependencies using NSubstitute
- Each test class should be in its own file
- Each test should test a single behavior and assert only applicable conditions
- Avoid asserting the same condition multiple times in different tests unless it's a precondition
  - If asserting a precondition, add a message explaining that a precondition failed and not the actual test
  - In this case there should be another test that tests the precondition only
- Only assert what is being tested in each test

## Code Style
- Use descriptive variable and method names
- Enable nullable reference types
- Use implicit usings where available
- Follow C# naming conventions (PascalCase for public members, camelCase for private)
- Use modern C# features (pattern matching, records, etc.)

## Dependencies
- **Runtime Dependencies**: JetBrains.Annotations
- **netstandard2.0 Dependencies**: PolySharp, System.Buffers, System.Memory
- **netstandard2.1 Dependencies**: PolySharp
- **Test Dependencies**: xUnit v3, Shouldly, NSubstitute

## Important Notes
- The project uses polyfills for older framework targets (netstandard2.0, netstandard2.1)
- Warning CS0436 about RuntimeHelpers conflicts is expected for netstandard2.0 builds
- Tests may not run on net6.0 if the runtime is not available, but this is expected
- Internal polyfills are in `src/HCommons/Internal/Polyfills/`

## File Organization
- Keep each test class in its own file
- Match test file names to the class being tested (e.g., `PooledArray.cs` → `PooledArrayTest.cs`)
- Group related functionality in appropriate namespaces
