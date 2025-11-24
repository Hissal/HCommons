# HCommons

A C# commons library providing utility types and helper functions for .NET applications.

## Features

- **Buffers**: Memory-efficient pooled array types for high-performance scenarios
- **Math**: Mathematical helper methods and extensions for common operations
- **Monads**: Functional programming utilities for safe and expressive error handling

## Installation

```bash
dotnet add package HCommons
```

## Buffers

The `HCommons.Buffers` namespace provides memory-efficient array types that use `ArrayPool<T>` for reduced allocations and improved performance.

### PooledArray<T>

A disposable wrapper around a pooled array that must be disposed to return the array to the pool.

#### Key Features

- Automatically rents arrays from `ArrayPool<T>.Shared`
- Must be disposed to avoid memory leaks
- Supports indexing, spans, and memory operations
- Provides slicing and copying capabilities

#### Basic Usage

```csharp
using HCommons.Buffers;

// Rent a pooled array
using var pooled = PooledArray<int>.Rent(100);

// Access elements
pooled[0] = 42;
Console.WriteLine(pooled[0]); // 42

// Get a span for efficient operations
Span<int> span = pooled.Span;
span.Fill(1);

// Get memory
Memory<int> memory = pooled.Memory;
```

#### Ownership Semantics

`PooledArray<T>` is a struct. Copying it transfers ownership of the underlying array:

```csharp
using var original = PooledArray<int>.Rent(10);
var copy = original; // Transfers ownership

// Only dispose once - either original or copy, not both
// Using statement will dispose 'original', so don't dispose 'copy'
```

#### Prefer Passing Spans

Best practice: Pass `Span<T>` to methods instead of `PooledArray<T>` to avoid ownership confusion:

```csharp
void ProcessData(Span<int> data) {
    // Work with data
}

using var pooled = PooledArray<int>.Rent(100);
ProcessData(pooled.Span); // Recommended approach
```

#### Creating Copies

Use `RentCopy()` when you need independent copies:

```csharp
using var original = PooledArray<int>.Rent(10);
original.Span.Fill(42);

using var copy = original.RentCopy(); // Independent copy
copy[0] = 1; // Doesn't affect original
```

#### Slicing

```csharp
using var original = PooledArray<int>.Rent(100);

using var slice = original.RentSlice(10, 20); // 20 elements starting at index 10
```

#### Returning to Pool

```csharp
var pooled = PooledArray<int>.Rent(100);
PooledArray<int>.Return(ref pooled); // Explicitly return and dispose
```

### PooledArrayBuilder<T>

A builder for constructing pooled arrays incrementally, similar to `List<T>` but using pooled memory.

```csharp
using HCommons.Buffers;

// Create a builder
using var builder = new PooledArrayBuilder<int>();

// Add elements
builder.Add(1);
builder.Add(2);
builder.Add(3);

// Build and transfer ownership
using var result = builder.BuildAndDispose();
Console.WriteLine(result.Length); // 3

// Or create a copy while keeping builder valid
using var builder2 = new PooledArrayBuilder<int>();
builder2.Add(1);
using var copy = builder2.BuildCopy();
builder2.Add(2); // Builder still valid
```

## Math

The `HCommons.Math` namespace provides mathematical helper methods and extensions for common operations.

### HMath

Static utility methods for mathematical operations.

#### ClampToFloat

Clamps a double value to the float range and converts it to a float.

```csharp
using HCommons.Math;

double largeValue = 1e100;
float clamped = HMath.ClampToFloat(largeValue); // Returns float.MaxValue

double smallValue = -1e100;
float clampedSmall = HMath.ClampToFloat(smallValue); // Returns float.MinValue

// With custom range
double value = 150.0;
float inRange = HMath.ClampToFloat(value, 0.0f, 100.0f); // Returns 100.0f
```

#### Map

Maps values from one range to another range. Available for `int`, `float`, and `double` types.

```csharp
using HCommons.Math;

// Map integer from 0-100 to 0-255
int brightness = 75;
int byteValue = HMath.Map(brightness, 0, 100, 0, 255); // 191

// Map float from 0-1 to -1 to 1
float normalized = 0.75f;
float scaled = HMath.Map(normalized, 0f, 1f, -1f, 1f); // 0.5

// Map double for precise calculations
double temperature = 20.0;
double fahrenheit = HMath.Map(temperature, 0.0, 100.0, 32.0, 212.0);
```

### HMathExtensions

Extension methods that provide the same functionality as `HMath` but in a fluent style.

```csharp
using HCommons.Math;

// ClampToFloat as extension
double value = 1e100;
float clamped = value.ClampToFloat(); // Returns float.MaxValue

// Map as extension
int brightness = 75;
int byteValue = brightness.Map(0, 100, 0, 255); // 191

float normalized = 0.75f;
float scaled = normalized.Map(0f, 1f, -1f, 1f); // 0.5
```

## Monads

The `HCommons.Monads` namespace provides functional programming utilities for expressive and safe error handling.

### Optional<T>

Represents a value that may or may not be present, eliminating null reference issues.

#### Creating Optionals

```csharp
using HCommons.Monads;

// Some value
Optional<int> some = Optional<int>.Some(42);
Optional<int> someImplicit = 42; // Implicit conversion

// No value
Optional<int> none = Optional<int>.None();
```

#### Checking and Accessing Values

```csharp
Optional<int> opt = 42;

if (opt.HasValue) {
    Console.WriteLine(opt.Value); // 42
}

// Try pattern
if (opt.TryGetValue(out var value)) {
    Console.WriteLine(value);
}

// Get value or default
int val = opt.GetValueOrDefault(0);
```

#### Pattern Matching

```csharp
var message = opt.Match(
    onValue: v => $"Value is {v}",
    onNone: () => "No value"
);

// With actions
opt.Switch(
    onValue: v => Console.WriteLine($"Got {v}"),
    onNone: () => Console.WriteLine("Nothing")
);
```

#### Transformations

```csharp
Optional<int> opt = 42;

// Select (map)
Optional<string> str = opt.Select(x => x.ToString());

// Bind (flatMap)
Optional<int> doubled = opt.Bind(x => Optional<int>.Some(x * 2));

// Where (filter)
Optional<int> filtered = opt.Where(x => x > 10);

// SelectMany (LINQ query syntax)
var result = from x in opt
             from y in Optional<int>.Some(10)
             select x + y;
```

#### Stateful Transformations

All transformation methods (`Select`, `Bind`, `Where`, `Match`, `Switch`, and `SelectMany`) have overloads that accept a state parameter, allowing you to pass additional context without closures:

```csharp
Optional<int> opt = 42;

// Select with state
var multiplier = 2;
Optional<int> doubled = opt.Select(multiplier, (state, x) => x * state);

// Bind with state
var options = new Dictionary<int, string>();
Optional<string> lookup = opt.Bind(options, (dict, key) => 
    dict.TryGetValue(key, out var val) ? Optional<string>.Some(val) : Optional<string>.None());

// Where with state
var threshold = 10;
Optional<int> filtered = opt.Where(threshold, (limit, x) => x > limit);

// Match with state
var context = "Result";
string message = opt.Match(context, 
    (ctx, v) => $"{ctx}: {v}", 
    ctx => $"{ctx}: None");

// Switch with state
var logger = new Logger();
opt.Switch(logger,
    (log, v) => log.Info($"Got {v}"),
    log => log.Info("No value"));
```

### Either<TLeft, TRight>

Represents a value that is one of two possible types, commonly used for success/error scenarios.

```csharp
using HCommons.Monads;

// Create Either
Either<string, int> left = "Error occurred";
Either<string, int> right = 42;

// Check which side
if (right.IsRight) {
    Console.WriteLine(right.Right); // 42
}

// Pattern matching
var message = right.Match(
    leftFunc: err => $"Error: {err}",
    rightFunc: val => $"Success: {val}"
);

// Switch with actions
right.Switch(
    leftAction: err => Console.WriteLine($"Failed: {err}"),
    rightAction: val => Console.WriteLine($"Got: {val}")
);
```

#### Either Extensions

The `Either` type includes powerful extension methods for transformations and conversions:

```csharp
using HCommons.Monads;

Either<string, int> either = 42;

// Swap left and right
Either<int, string> swapped = either.Swap(); // Left("42") becomes Right("42")

// Convert to Optional
Optional<int> rightOpt = either.AsRightOptional(); // Some(42)
Optional<string> leftOpt = either.AsLeftOptional(); // None

// Map both sides
Either<int, string> mapped = either.Map(
    leftMapper: err => err.Length,
    rightMapper: val => val.ToString()
);

// Map only one side
Either<string, string> rightMapped = either.MapRight(x => x.ToString());
Either<int, int> leftMapped = either.MapLeft(err => err.Length);
```

#### Stateful Either Transformations

`Either` also supports stateful transformations for `Map`, `MapLeft`, `MapRight`, `Match`, and `Switch`:

```csharp
Either<string, int> either = 42;

// Map with state
var formatter = new NumberFormatter();
Either<string, string> formatted = either.Map(formatter,
    (fmt, err) => fmt.FormatError(err),
    (fmt, val) => fmt.FormatValue(val));

// Match with state
var logger = new Logger();
var result = either.Match(logger,
    (log, err) => { log.Error(err); return -1; },
    (log, val) => { log.Info($"Got {val}"); return val; });
```

### Result and Result<TValue>

Represents the outcome of an operation that can succeed or fail with an error.

#### Result (no value)

```csharp
using HCommons.Monads;

Result DoOperation() {
    try {
        // Some operation
        return Result.Success();
    } catch (Exception ex) {
        return Result.Failure(Error.FromException(ex));
    }
}

var result = DoOperation();

if (result.IsSuccess) {
    Console.WriteLine("Operation succeeded");
} else {
    Console.WriteLine($"Operation failed: {result.Error}");
}
```

#### Result<TValue> (with value)

```csharp
Result<int> Divide(int a, int b) {
    if (b == 0)
        return Result<int>.Failure(Error.WithMessage("Division by zero"));
    
    return Result<int>.Success(a / b);
}

var result = Divide(10, 2);

// Pattern matching
var message = result.Match(
    onSuccess: value => $"Result: {value}",
    onFailure: error => $"Error: {error.Message}"
);

// Get value or default
int value = result.GetValueOrDefault(-1);

// Try pattern
if (result.TryGetValue(out var val)) {
    Console.WriteLine(val);
}
```

#### Result Extensions

`Result` and `Result<TValue>` have extension methods for functional transformations:

```csharp
using HCommons.Monads;

Result<int> result = Result<int>.Success(42);

// Select (map) to transform success values
Result<string> strResult = result.Select(x => x.ToString());

// Bind (flatMap) to chain operations that return results
Result<int> doubled = result.Bind(x => 
    x > 0 ? Result<int>.Success(x * 2) : Result<int>.Failure("Value must be positive"));

// MapError to transform errors
Result<int> remapped = result.MapError(err => 
    Error.WithMessage($"Operation failed: {err.Message}"));

// Match for pattern matching
var message = result.Match(
    onSuccess: val => $"Got {val}",
    onFailure: err => $"Error: {err.Message}"
);
```

#### Stateful Result Transformations

All Result transformation methods support stateful overloads:

```csharp
Result<int> result = Result<int>.Success(42);

// Select with state
var multiplier = 2;
Result<int> doubled = result.Select(multiplier, (m, x) => x * m);

// Bind with state
var validator = new Validator();
Result<int> validated = result.Bind(validator, (v, x) => 
    v.IsValid(x) ? Result<int>.Success(x) : Result<int>.Failure("Invalid"));

// MapError with state
var logger = new Logger();
Result<int> logged = result.MapError(logger, (log, err) => {
    log.Error(err.Message);
    return err;
});

// Match with state
var formatter = new Formatter();
string formatted = result.Match(formatter,
    (fmt, val) => fmt.FormatSuccess(val),
    (fmt, err) => fmt.FormatError(err));
```

### OperationResult and OperationResult<TValue>

Extends `Result` with cancellation support for operations that can be cancelled.

```csharp
using HCommons.Monads;

OperationResult<int> PerformOperation(CancellationToken ct) {
    if (ct.IsCancellationRequested)
        return OperationResult<int>.Cancelled(Cancelled.Because("User cancelled"));
    
    try {
        int result = 42; // Some operation
        return OperationResult<int>.Success(result);
    } catch (Exception ex) {
        return OperationResult<int>.Failure(Error.FromException(ex));
    }
}

var result = PerformOperation(CancellationToken.None);

if (result.IsSuccess) {
    Console.WriteLine($"Success: {result.Value}");
} else if (result.IsCancelled) {
    Console.WriteLine($"Cancelled: {result.Cancellation.Reason}");
} else {
    Console.WriteLine($"Failed: {result.Error.Message}");
}
```

#### OperationResult Extensions

`OperationResult<TValue>` supports the same functional transformations as `Result<TValue>`, with cancellation awareness:

```csharp
using HCommons.Monads;

OperationResult<int> opResult = OperationResult<int>.Success(42);

// Select preserves cancellation state
OperationResult<string> strResult = opResult.Select(x => x.ToString());

// Bind chains operations
OperationResult<int> doubled = opResult.Bind(x => 
    OperationResult<int>.Success(x * 2));

// All transformation methods have stateful overloads
var multiplier = 3;
OperationResult<int> tripled = opResult.Select(multiplier, (m, x) => x * m);
```

### Error

Represents an error with a message and optional exception.

```csharp
using HCommons.Monads;

// From message
Error error1 = Error.WithMessage("Something went wrong");
Error error2 = "Something went wrong"; // Implicit conversion

// From exception
try {
    throw new InvalidOperationException("Bad operation");
} catch (Exception ex) {
    Error error3 = Error.FromException(ex);
    Error error4 = ex; // Implicit conversion
    
    Console.WriteLine(error3.Message);
    Console.WriteLine(error3.HasException); // true
}

// Error with value
Error<int> error5 = Error<int>.WithMessage(42, "Partial result available");
```

### Cancelled

Represents a cancellation with a reason and optional associated value.

```csharp
using HCommons.Monads;

// Simple cancellation
Cancelled cancellation1 = Cancelled.Because("User requested");
Cancelled cancellation2 = "User requested"; // Implicit conversion

// Cancellation with value
Cancelled<int> cancellation3 = Cancelled<int>.Because(42, "Partial result");
```

## Target Frameworks

- .NET 9.0
- .NET 8.0
- .NET Standard 2.1
- .NET Standard 2.0

## License

This project is licensed under the MIT License - see the LICENSE file for details.
