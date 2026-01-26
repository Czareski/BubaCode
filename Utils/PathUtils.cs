namespace BubaCode.Utils;

public class PathUtils
{
    public static string NormalizePath(string path)
    {
        return path.Replace('/', '\\').TrimEnd(new[] {'\\'});
    }
}