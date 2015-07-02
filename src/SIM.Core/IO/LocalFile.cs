namespace SIM.IO
{
  using System.IO;
  using JetBrains.Annotations;
  using Newtonsoft.Json;
  using SIM.Abstract.IO;

  public class LocalFile : LocalFileSystemEntry, IFile
  {
    public LocalFile([NotNull] string path) : this(LocalFileSystem.Default, path)
    { 
      FileInfo = new FileInfo(FullPath);
    }

    public LocalFile([NotNull] IFileSystem fileSystem, [NotNull] string path) : base(fileSystem, path)
    {
      FileInfo = new FileInfo(FullPath);
      Folder = new LocalFolder(fileSystem, FullPath);
    }

    [NotNull]
    [JsonIgnore]
    public FileInfo FileInfo { get; }

    [NotNull]
    [JsonIgnore]
    public IFolder Folder { get; }

    public Stream OpenRead()
    {
      return FileInfo.OpenRead();
    }

    public Stream OpenWrite()
    {
      return FileInfo.OpenWrite();
    }
  }
}