using System;

namespace SIM.Extensions
{
  public static class TExtensions
  {
    public static T Apply<T>(this T that, Action<T> action)
    {
      if (that != null)
      {
        action(that);
      }

      return that;
    }
  }
}
