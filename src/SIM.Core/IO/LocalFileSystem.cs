namespace SIM.IO
{
  using SIM.Abstract.IO;

  public class LocalFileSystem : IFileSystem
  {
    public static readonly IFileSystem Default = new LocalFileSystem();

    public IFolder ParseFolder(string path)
    {
      return new LocalFolder(this, path);
    }

    public IFile ParseFile(string path)
    {
      return new LocalFile(this, path);
    }
                                       
    public IPathHelper Path { get; } = new SystemPathHelper();
  }
}