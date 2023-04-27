namespace ChimpSolution.Abstractions.Services;

public interface IFileManager
{
    string BasePath { get; }
    void SaveFile(string path, byte[] file, string fileName);
    void RemoveFile(string path, string fileName);
    void RemoveBaseDirectory(string path);
}