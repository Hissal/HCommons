using HCommons.Reflection;

namespace HCommons.Tests;

[GenerateRuntimeTypeCache]
public interface IGeneratedAttributeMarker;

public abstract class GeneratedAttributeMarkerBase : IGeneratedAttributeMarker;

internal sealed class GeneratedAttributeMarkerImplementation : GeneratedAttributeMarkerBase;

public interface IGeneratedCallMarker;

internal sealed class GeneratedCallMarkerImplementation : IGeneratedCallMarker;

public interface IGeneratedWrapperCallMarker;

internal sealed class GeneratedWrapperCallMarkerImplementation : IGeneratedWrapperCallMarker;
