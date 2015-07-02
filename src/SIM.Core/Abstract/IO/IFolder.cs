namespace SIM.Abstract.IO
{
  public interface IFolder : IFileSystemEntry
  {
    bool TryCreate();

    bool Exists { get; }
  }
}