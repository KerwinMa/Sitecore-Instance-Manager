namespace SIM.Common
{
  using System;

  public class CommandResult
  {
    public bool Success { get; set; }

    public string Message { get; set; }

    public TimeSpan Elapsed { get; set; }

    public CustomException Error { get; set; }
  }

  public class CommandResult<TResult> : CommandResult
  {
    public TResult Data { get; set; }
  }
}