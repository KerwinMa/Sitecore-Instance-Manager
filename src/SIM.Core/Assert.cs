namespace SIM
{
  using System;
  using System.Diagnostics;
  using JetBrains.Annotations;

  public static class Assert
  {                                    
    [DebuggerStepThrough]
    [NotNull]
    [ContractAnnotation("objectValue:null => stop")]   
    public static T IsNotNull<T>(T objectValue, [NotNull] string message) where T : class
    {                                                                       
      var condition = objectValue == null;
      if (condition)
      {
        throw new InvalidOperationException(message);
      }

      return objectValue;
    }
                             
    [DebuggerStepThrough]
    [NotNull, ContractAnnotation("stringValue:null => stop")]
    public static string IsNotNullOrEmpty(string stringValue, [NotNull] string message)
    {
      Assert.ArgumentNotNullOrEmpty(message, nameof(message));

      var condition = string.IsNullOrEmpty(stringValue);
      if (condition)
      {
        throw new InvalidOperationException(message);
      }

      return stringValue;
    }
    [DebuggerStepThrough]
    [NotNull, ContractAnnotation("argumentValue:null => stop")]
    public static object ArgumentNotNull(object argumentValue, [NotNull] string argumentName)
    {
      Assert.ArgumentNotNullOrEmpty(argumentName, nameof(argumentName));

      var condition = argumentValue == null;
      if (condition)
      {
        throw new ArgumentNullException(argumentName);
      }

      return argumentValue;
    }
    [DebuggerStepThrough]
    [NotNull, ContractAnnotation("argumentValue:null => stop")]
    public static string ArgumentNotNullOrEmpty(string argumentValue, [NotNull] string argumentName)
    {
      if (string.IsNullOrEmpty(argumentName))
      {
        throw new ArgumentException("argumentName");
      }

      var condition = string.IsNullOrEmpty(argumentValue);
      if (condition)
      {
        throw new ArgumentException(argumentName);
      }

      return argumentValue;
    }
    [DebuggerStepThrough]
    [ContractAnnotation("argumentCondidition:false => stop")]
    public static void ArgumentCondition(bool argumentCondidition, [NotNull] string argumentName, [NotNull] string message)
    {
      Assert.ArgumentNotNullOrEmpty(argumentName, "argumentName");      
                                                                            
      var condition = !argumentCondidition;
      if (condition)
      {
        throw new ArgumentException(message, argumentName);
      }
    }
    [DebuggerStepThrough]               
    public static T ArgumentCondition<T>(T argumentValue, Func<T, bool> argumentCondidition, [NotNull] string argumentName, [NotNull] string message)
    {
      Assert.ArgumentNotNull(argumentCondidition, "argumentCondition");
      Assert.ArgumentNotNullOrEmpty(argumentName, "argumentName");
      Assert.ArgumentNotNullOrEmpty(message, nameof(message));

      var condition = !argumentCondidition(argumentValue);
      if (condition)
      {
        throw new ArgumentException(message, argumentName);
      }

      return argumentValue;
    }
    [DebuggerStepThrough]
    [ContractAnnotation("assertionCondition:false => stop")]
    public static void IsTrue(bool assertionCondition, [NotNull] string message)
    {
      Assert.ArgumentNotNullOrEmpty(message, nameof(message));

      var condition = !assertionCondition;
      if (condition)
      {
        throw new InvalidOperationException(message);
      }
    }
    [DebuggerStepThrough]
    [ContractAnnotation("assertionCondition:false => stop")]
    public static void IsTrue<T>(bool assertionCondition) where T : Exception, new()
    {
      var condition = !assertionCondition;
      if (condition)
      {
        throw new T();
      }
    }
    [DebuggerStepThrough]
    [ContractAnnotation("assertionCondition:false => stop")]
    public static void IsTrue(bool assertionCondition, [NotNull] Func<Exception> throwFunc)
    {
      Assert.IsNotNull(throwFunc, "throwFunc");

      if (assertionCondition)
      {
        return;
      }

      var exception = throwFunc();
      Assert.ArgumentCondition(exception != null, "throwFunc", "The function must not return null.");

      throw exception;
    }
  }
}