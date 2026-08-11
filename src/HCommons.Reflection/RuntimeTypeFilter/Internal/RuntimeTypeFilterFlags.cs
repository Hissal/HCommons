namespace HCommons.Reflection;

[Flags]
internal enum RuntimeTypeFilterFlags : byte {
    None = 0,
    Concrete = 1 << 0,
    Public = 1 << 1,
    Closed = 1 << 2,
    PublicParameterlessConstructor = 1 << 3,
}
