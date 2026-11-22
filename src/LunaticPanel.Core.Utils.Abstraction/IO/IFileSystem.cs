using System;

namespace LunaticPanel.Core.Utils.Abstraction.IO;

public interface IFileSystem
{
    string[] GetFiles(string path, string searchPattern, SearchOption searchOption);
}
