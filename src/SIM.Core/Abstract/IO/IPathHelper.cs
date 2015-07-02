namespace SIM.Abstract.IO
{
  using JetBrains.Annotations;

  public interface IPathHelper
  {
    [NotNull]
    string GetFullPath([NotNull] string path);

    [NotNull]
    string Combine([NotNull] string path1, [NotNull] string path2);
  }
}