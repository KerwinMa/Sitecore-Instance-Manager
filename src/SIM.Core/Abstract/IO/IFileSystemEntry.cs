namespace SIM.Abstract.IO
{
  using JetBrains.Annotations;

  public interface IFileSystemEntry
  {             
    [NotNull]
    IFileSystem FileSystem { get; }

    [NotNull]
    string FullPath { get; }
  }
}