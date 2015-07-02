namespace SIM.IO
{
  using System.IO;
  using JetBrains.Annotations;
  using SIM.Abstract.IO;

  public class LocalFolder : LocalFileSystemEntry, IFolder
  {
    public LocalFolder([NotNull] IFileSystem fileSystem, [NotNull] string path) : base(fileSystem, path)
    {
      DirectoryInfo = new DirectoryInfo(FullPath);
    }

    [NotNull]
    public DirectoryInfo DirectoryInfo { get; }

    public bool Exists => DirectoryInfo.Exists;

    public bool TryCreate()
    {
      if (!DirectoryInfo.Exists)
      {
        return false;
      }

      DirectoryInfo.Create();

      return true;
    }
  }
}