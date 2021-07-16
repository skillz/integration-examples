using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#if !DOXYGEN_SHOULD_SKIP_THIS
namespace SkillzSDK
{
  namespace Extensions {

    /// <summary>
    /// Skillz helper extensions. Internal only, do not rely on for game code
    /// </summary>
    public static class SkillzExtensions
    {
      public static bool? SafeGetBoolValue(
          this Dictionary<string, object> dict,
          string key)
      {
        string val = dict.SafeGetStringValue(key);
        return Helpers.SafeBoolParse(val);
      }

      public static double? SafeGetDoubleValue(
          this Dictionary<string, object> dict,
          string key)
      {
        string val = dict.SafeGetStringValue(key);
        return Helpers.SafeDoubleParse(val);
      }

      public static DateTime? SafeGetUnixDateTimeValue(
          this Dictionary<string, object> dict,
          string key)
      {
        double? val = dict.SafeGetDoubleValue(key);
        return Helpers.SafeParseUnixTime(val);
      }

      public static int? SafeGetIntValue(
          this Dictionary<string, object> dict,
          string key)
      {
        string val = dict.SafeGetStringValue(key);
        return Helpers.SafeIntParse(val);
      }

      public static uint? SafeGetUintValue(
          this Dictionary<string, object> dict,
          string key)
      {
        string val = dict.SafeGetStringValue(key);
        return Helpers.SafeUintParse(val);
      }

      public static ulong? SafeGetUlongValue(
        this Dictionary<string, object> dict,
        string key)
      {
        return Helpers.SafeUlongParse(dict.SafeGetStringValue(key));
      }

      public static string SafeGetStringValue(
          this Dictionary<string, object> dict,
          string key)
      {
        object val = dict.SafeGetValue(key);
        return val != null ? val.ToString() : null;
      }

      public static object SafeGetValue(
          this Dictionary<string, object> dict,
          string key)
      {
        return dict.ContainsKey(key) ? dict[key] : null;
      }

      // put specific type parsing in here?
    }
  } // namespace Extensions

  /// <summary>
  /// Skillz helper methods. Internal only, do not rely on for game code
  /// </summary>
  public static class Helpers
  {
    public static bool? SafeBoolParse(
        string str,
        string trueStr = "True",
        string falseStr = "False",
        string trueInt = "1",
        string falseInt = "0")
    {
      if (str == trueStr || str == trueInt)
      {
        return true;
      }
      if (str == falseStr || str == falseInt)
      {
        return false;
      }
      return null;
    }

    public static DateTime? SafeParseUnixTime(double? unixTime)
    {
      if (unixTime == null)
      {
        return null;
      }

      try
      {
        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return epoch.AddSeconds((double) unixTime);
      }
      catch (Exception)
      {
        return null;
      }
    }

    public static double? SafeDoubleParse(string str)
    {
      double result;
      bool success = double.TryParse(str, out result);
      return success ? result : (double?) null;
    }

    public static int? SafeIntParse(string str)
    {
      int result;
      bool success = int.TryParse(str, out result);
      return success ? result : (int?) null;
    }

    public static uint? SafeUintParse(string str)
    {
      uint result;
      bool success = uint.TryParse(str, out result);
      return success ? result : (uint?) null;
    }

    public static ulong? SafeUlongParse(string str)
    {
      return ulong.TryParse(str, out var result) ? result : (ulong?)null;
    }
  }

  /// <summary>
  /// Skillz helper methods. Internal only, do not rely on for game code
  /// </summary>
  struct UnmanagedArray : IDisposable
  {
    public UnmanagedArray (byte[] data)
    {
      dataLength = (UInt64)data.Length;
      dataIntPtr = Marshal.AllocHGlobal (Marshal.SizeOf (data [0]) * data.Length);
      Marshal.Copy (data, 0, dataIntPtr, data.Length);
    }

    public IntPtr IntPtr {
      get { return dataIntPtr; }
    }

    public UInt64 Length {
      get { return dataLength; }
    }

    private IntPtr dataIntPtr;
    private UInt64 dataLength;

    void IDisposable.Dispose ()
    {
      Marshal.FreeHGlobal (dataIntPtr);
    }
  }
}
#endif
