namespace SIM.IO
{
  using SIM.Abstract.IO;

  public abstract class MockFileSystemEntry : IFileSystemEntry
  {
    protected MockFileSystemEntry(IFileSystem fileSystem, string fullPath)
    {
      SIM.Assert.ArgumentNotNull(fileSystem, nameof(fileSystem));
      SIM.Assert.ArgumentNotNullOrEmpty(fullPath, nameof(fullPath));

      FileSystem = fileSystem;
      FullPath = fullPath;
    }

    public IFileSystem FileSystem { get; }

    public string FullPath { get; }
  }
}