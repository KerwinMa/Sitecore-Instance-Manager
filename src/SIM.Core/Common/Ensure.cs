namespace SIM.Common
{
  using JetBrains.Annotations;

  public static class Ensure
  {
    [ContractAnnotation("obj:null=>stop")]        
    public static void IsNotNull(object obj, string message)
    {
      Assert.ArgumentNotNullOrEmpty(message, nameof(message));

      if (obj != null)
      {
        return;
      }       

      throw new MessageException(message);
    }

    [ContractAnnotation("str:null=>stop")]       
    public static void IsNotNullOrEmpty(string str, string message)
    {
      Assert.ArgumentNotNullOrEmpty(message, nameof(message));

      if (!string.IsNullOrEmpty(str))
      {
        return;
      }             

      throw new MessageException(message);
    }

    [ContractAnnotation("condition:false=>stop")]      
    public static void IsTrue(bool condition, string message)
    {
      Assert.ArgumentNotNullOrEmpty(message, nameof(message));

      if (condition)
      {
        return;
      }                  

      throw new MessageException(message);
    }
  }
}