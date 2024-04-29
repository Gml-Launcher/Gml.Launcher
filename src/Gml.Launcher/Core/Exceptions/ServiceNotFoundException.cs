using System;

namespace Gml.Launcher.Core.Exceptions;

public class ServiceNotFoundException : Exception
{
    public Type NotFoundedService { get;}

    public ServiceNotFoundException(Type eType)
    {
        NotFoundedService = eType;
    }
}
