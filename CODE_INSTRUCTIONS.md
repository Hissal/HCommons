# Testing Guidelines
- Use xUnit v3 with Shouldly for assertions.
- Global usings include `Xunit`, `Shouldly`.
- Tests target multiple frameworks; net6.0, net8.0, net9.0, net48.
- Follow AAA (Arrange, Act, Assert) pattern in tests. (If each part is not clearly separated, use comments to delineate sections)
- Use descriptive names for test methods.
- Mock dependencies using NSubstitute.
- Each test class should be in its own file.
- Each test should test a single behavior and assert only applicable conditions.
- Avoid asserting the same condition multiple times in different tests unless its a precondition in which case add a message explaining that a precondition failed and not the actual test, in this case there should be another test that tests the precondition only. (Only assert what is beaing tested in each test)
