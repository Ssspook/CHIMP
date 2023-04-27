using ChimpSolution.Abstractions.Services;

namespace ChimpSolution.Sdk.Services;

public class FileSystemFileManager : IFileManager
{
    public FileSystemFileManager(string basePath)
    {
        if (string.IsNullOrWhiteSpace(basePath))
            throw new ArgumentNullException(string.Empty, "Wrong path provided");

        BasePath = basePath;
    }

    public string BasePath { get; }

    public void SaveFile(string path, byte[] file, string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentNullException(string.Empty, "Empty file name provided");

        var outputDirectory = string.Concat(BasePath, path);
        if (!Directory.Exists(outputDirectory))
            Directory.CreateDirectory(outputDirectory);

        File.WriteAllBytes(Path.Combine(outputDirectory, fileName), file);
    }

    public void RemoveFile(string path, string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentNullException(string.Empty, "Empty file name provided");

        var outputDirectory = string.Concat(BasePath, path);
        if (!Directory.Exists(outputDirectory))
            throw new FormatException("Invalid path provided");

        File.Delete(Path.Combine(outputDirectory, fileName));
    }

    public void RemoveBaseDirectory(string path)
    {
        if (!Directory.Exists(Path.Combine(BasePath, path)))
            throw new DirectoryNotFoundException("Base directory not found");

        Directory.Delete(Path.Combine(BasePath, path), true);
    }
}