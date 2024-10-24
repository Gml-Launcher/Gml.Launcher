using System;

namespace Gml.Launcher.Core.Exceptions;

public class MinecraftException(string message) : Exception(message);
