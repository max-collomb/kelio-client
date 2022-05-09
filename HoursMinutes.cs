using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace kelio_client
{

  class HourMinuteInterval
  {
    public bool IsNegative { get; set; } = false;
    public int Hour { get; set; } = 0;
    public int Minute { get; set; } = 0;
    public HourMinuteInterval(string hmiStr)
    {
      Match match = new Regex(@"([0-9]{1,2})[:h]([0-9]{1,2})").Match(hmiStr);
      if (match.Success)
      {
        Hour = Int32.Parse(match.Groups[1].Value);
        Minute = Int32.Parse(match.Groups[2].Value);
        if (Hour < 0)
        {
          Hour = Math.Abs(Hour);
          IsNegative = true;
        }
      }
    }
    public HourMinuteInterval(int _Minute)
    {
      Hour = 0;
      Minute = Math.Abs(_Minute);
      IsNegative = _Minute < 0;
    }
    public override string ToString()
    {
      return (IsNegative ? "-" : "+") +  Hour.ToString("00") + ":" + Minute.ToString("00");
    }
  }
  class HourMinute
  {
    public int Hour { get; set; } = 0;
    public int Minute { get; set; } = 0;
    public HourMinute(string hmStr)
    {
      Match match = new Regex(@"([0-9]{1,2})[:h]([0-9]{1,2})").Match(hmStr);
      if (match.Success)
      {
        Hour = Int32.Parse(match.Groups[1].Value);
        Minute = Int32.Parse(match.Groups[2].Value);
      }
    }
    public HourMinute(int _Hour, int _Minute)
    {
      Hour = _Hour;
      Minute = _Minute;
      NormalizeMinutes();
    }
    public HourMinute(HourMinute hm)
    {
      Hour = hm.Hour;
      Minute = hm.Minute;
    }
    public HourMinute(DateTime dt)
    {
      Hour = dt.Hour;
      Minute = dt.Minute;
    }
    public HourMinute Add(HourMinuteInterval hmi)
    {
      return new HourMinute(
        Hour + hmi.Hour * (hmi.IsNegative ? -1 : 1),
        Minute + hmi.Minute * (hmi.IsNegative ? -1 : 1)
      );
    }
    public HourMinute Remove(HourMinuteInterval hmi)
    {
      return new HourMinute(
        Hour - hmi.Hour * (hmi.IsNegative ? -1 : 1),
        Minute - hmi.Minute * (hmi.IsNegative ? -1 : 1)
      );
    }
    private void NormalizeMinutes()
    {
      while (Minute > 59)
      {
        Hour++;
        Minute -= 60;
      }
      while (Minute < 0)
      {
        Hour--;
        Minute += 60;
      }
    }
    public override string ToString()
    {
      return Hour.ToString("00") + ":" + Minute.ToString("00");
    }
  }
}
