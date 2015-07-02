using System;

namespace SIM.Extensions
{
 public static class DisposableExtensions
  {
    public static TR ActAndDispose<TD, TR>(this TD that, Func<TD, TR> func) 
      where TD : IDisposable 
      where TR : class
    {
      if (that == null)
      {
        return null;
      }

      using (that)
      {
        return func?.Invoke(that);
      }
    }
  }
}
