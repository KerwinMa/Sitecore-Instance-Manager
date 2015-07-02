namespace SIM.IO
{
  using SIM.Abstract.IO;

  public static class LocalFolderExtensions
  {  
    public static IFile GetChildFile(this IFolder folder, string fileName)
    {
      return new LocalFile(folder.FileSystem, folder.FileSystem.Path.Combine(folder.FullPath, fileName));
    }
  }
}