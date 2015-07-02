namespace SIM.Abstract.IO
{
  using JetBrains.Annotations;

  public interface IFileSystem
  { 
    [NotNull]
    IPathHelper Path { get; }

    [NotNull]
    IFolder ParseFolder([NotNull] string path);

    [NotNull]
    IFile ParseFile([NotNull] string path);                 
  }
}