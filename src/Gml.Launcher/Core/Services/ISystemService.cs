using System.Collections.Generic;
using Gml.Launcher.Models;
using Gml.Web.Api.Domains.System;

namespace Gml.Launcher.Core.Services;

public interface ISystemService
{
    string GetApplicationFolder();
    string GetGameFolder(string additionalPath, bool needCreate);
    ulong GetMaxRam();
    OsType GetOsType();
    IEnumerable<Language> GetAvailableLanguages();
}
