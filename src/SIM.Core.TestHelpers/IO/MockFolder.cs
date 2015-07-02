namespace SIM.IO
{
  using SIM.Abstract.IO;

  public class MockFolder : MockFileSystemEntry, IFolder
  {
    public MockFolder(IFileSystem fileSystem, string fullPath) : base(fileSystem, fullPath)
    {
    }

    public bool Exists { get; private set; }

    public bool TryCreate()
    {
      var existedBefore = Exists;

      Exists = true;

      return !existedBefore;
    }
  }
}