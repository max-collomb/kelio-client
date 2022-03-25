﻿using System;
using System.IO;
using System.Reflection;

namespace kelio_client
{
  public sealed class Setting
  {
    public static int creditmax { get; set; }
    public static string url { get; set; }
    public static string username { get; set; }
    public static string password { get; set; }
    static readonly string SETTINGS = "kelio-client.ini";
    static readonly Setting instance = new Setting();
    Setting() { }
    static Setting()
    {
      string property = "";
      string[] settings = File.ReadAllLines(SETTINGS);
      foreach (string s in settings)
        try
        {
          string[] split = s.Split(new char[] { ':' }, 2);
          if (split.Length != 2)
            continue;
          property = split[0].Trim();
          string value = split[1].Trim();
          PropertyInfo propInfo = instance.GetType().GetProperty(property);
          switch (propInfo.PropertyType.Name)
          {
            case "Int32":
              propInfo.SetValue(null, Convert.ToInt32(value), null);
              break;
            case "String":
              propInfo.SetValue(null, value, null);
              break;
          }
        }
        catch
        {
          throw new Exception("Invalid setting '" + property + "'");
        }
    }
  }
}
