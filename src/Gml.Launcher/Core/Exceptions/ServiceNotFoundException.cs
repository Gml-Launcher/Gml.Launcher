using System;

namespace Gml.Launcher.Core.Exceptions;

public class ServiceNotFoundException(Type eType) : Exception
{
    public Type NotFoundedService { get; } = eType;
}
