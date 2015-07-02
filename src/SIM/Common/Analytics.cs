namespace SIM.Common
{
  using System;
  using JetBrains.Annotations;
  using Microsoft.ApplicationInsights;
  using Microsoft.ApplicationInsights.Channel;
  using Microsoft.ApplicationInsights.Extensibility;

  public static class Analytics
  {
    private static TelemetryClient telemetryClient;

    public static void Start()
    {
      if (CoreApp.DoNotTrack())
      {
        return;
      }

      // TODO: Replace with new logging engine; Log.Debug("Insights - starting");

      try
      {
        var configuration = TelemetryConfiguration.Active;
        Assert.IsNotNull(configuration, nameof(configuration));

        configuration.TelemetryChannel = new PersistenceChannel("Sitecore Instance Manager");
        configuration.InstrumentationKey = "1447f72f-2d39-401b-91ac-4d5c502e3359";

        var client = new TelemetryClient(configuration)
        {
          InstrumentationKey = "1447f72f-2d39-401b-91ac-4d5c502e3359"
        };

        telemetryClient = client;
        try
        {
          // ReSharper disable PossibleNullReferenceException
          client.Context.Component.Version = string.IsNullOrEmpty(ApplicationManager.AppVersion) ? "0.0.0.0" : ApplicationManager.AppVersion;
          client.Context.Session.Id = Guid.NewGuid().ToString();
          client.Context.User.Id = Environment.MachineName + "\\" + Environment.UserName;
          client.Context.User.AccountId = CoreApp.GetCookie();
          client.Context.Device.OperatingSystem = Environment.OSVersion.ToString();
          // ReSharper restore PossibleNullReferenceException
          client.TrackEvent("Start");
          client.Flush();
        }
        catch (Exception ex)
        {
          client.TrackException(ex);
          // TODO: Replace with new logging engine; Log.Error(ex, "Error in app insights");
        }
      }
      catch (Exception ex)
      {
        // TODO: Replace with new logging engine; Log.Error(ex, "Error in app insights");
      }

      // TODO: Replace with new logging engine; Log.Debug("Insights - started");
    }

    public static void TrackEvent([NotNull] string eventName)
    {
      Assert.ArgumentNotNull(eventName, nameof(eventName));

      var tc = telemetryClient;
      if (tc == null)
      {
        return;
      }

      try
      {
        tc.TrackEvent(eventName);
      }
      catch (Exception ex)
      {
        // TODO: Replace with new logging engine; Log.Error(ex, "Error during event tracking: {0}", eventName);
      }
    }

    public static void Flush()
    {
      var tc = telemetryClient;
      if (tc == null)
      {
        return;
      }

      try
      {
        tc.TrackEvent("Exit");

        tc.Flush();
      }
      catch (Exception ex)
      {
        // TODO: Replace with new logging engine; Log.Error(ex, "Error during flushing");
      }
    }
  }
}