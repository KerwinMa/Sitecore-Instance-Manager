namespace SIM.IO
{
  using JetBrains.Annotations;
  using SIM.Abstract.IO;

  public class LocalFileSystemEntry : IFileSystemEntry
  {
    public LocalFileSystemEntry([NotNull] IFileSystem fileSystem, [NotNull] string path)
    {
      Assert.ArgumentNotNull(fileSystem, nameof(fileSystem));
      Assert.ArgumentNotNullOrEmpty(path, nameof(path));

      FileSystem = fileSystem;
      FullPath = System.IO.Path.GetFullPath(path);
    }

    public IFileSystem FileSystem { get; }

    public string FullPath { get; }
  }
}