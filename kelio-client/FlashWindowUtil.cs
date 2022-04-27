using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace kelio_client
{
  [StructLayout(LayoutKind.Sequential)]
  struct FlashWinfo
  {
    public static FlashWinfo CreateFlashWinfo(IntPtr handle, uint flags, uint count, uint timeout)
    {
      FlashWinfo fi = new FlashWinfo();
      fi.cbSize = Convert.ToUInt32(Marshal.SizeOf(fi));
      fi.hwnd = handle;
      fi.dwFlags = flags;
      fi.uCount = count;
      fi.dwTimeout = timeout;
      return fi;
    }
    /// <summary>
    /// The size of the structure in bytes.
    /// </summary>
    public uint cbSize;
    /// <summary>
    /// A Handle to the Window to be Flashed. The window can be either opened or minimized.
    /// </summary>
    public IntPtr hwnd;
    /// <summary>
    /// The Flash Status.
    /// </summary>
    public uint dwFlags;
    /// <summary>
    /// The number of times to Flash the window.
    /// </summary>
    public uint uCount;
    /// <summary>
    /// The rate at which the Window is to be flashed, in milliseconds. If Zero, the function uses the default cursor blink rate.
    /// </summary>
    public uint dwTimeout;
  }

  class FlashWinfoDwFlags
  {
    /// <summary>
    /// Stop flashing. The system restores the window to its original stae.
    /// </summary>
    public const uint FLASHW_STOP = 0;

    /// <summary>
    /// Flash the window caption.
    /// </summary>
    public const uint FLASHW_CAPTION = 1;

    /// <summary>
    /// Flash the taskbar button.
    /// </summary>
    public const uint FLASHW_TRAY = 2;

    /// <summary>
    /// Flash both the window caption and taskbar button.
    /// This is equivalent to setting the FLASHW_CAPTION | FLASHW_TRAY flags.
    /// </summary>
    public const uint FLASHW_ALL = 3;

    /// <summary>
    /// Flash continuously, until the FLASHW_STOP flag is set.
    /// </summary>
    public const uint FLASHW_TIMER = 4;

    /// <summary>
    /// Flash continuously until the window comes to the foreground.
    /// </summary>
    public const uint FLASHW_TIMERNOFG = 12;
  }

  public class FlashWindowUtil
  {
    [DllImport("user32.dll", EntryPoint = "FlashWindowEx")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool FlashWindowEx(ref FlashWinfo pwfi);

    public static bool FlashWindowEx(IntPtr formHandle)
    {
      return FlashWindowEx(formHandle, uint.MaxValue);
    }

    public static bool FlashWindowEx(IntPtr formHandle, uint count)
    {
      if (System.Environment.OSVersion.Version.Major >= 5)
      {
        FlashWinfo fi = FlashWinfo.CreateFlashWinfo(formHandle, FlashWinfoDwFlags.FLASHW_ALL, count, 0);
        return FlashWindowEx(ref fi);
      }
      return false;
    }

    public static bool StopFlashWindow(IntPtr formHandle)
    {
      if (System.Environment.OSVersion.Version.Major >= 5)
      {
        FlashWinfo fi = FlashWinfo.CreateFlashWinfo(formHandle, FlashWinfoDwFlags.FLASHW_STOP, uint.MaxValue, 0);
        return FlashWindowEx(ref fi);
      }

      return false;
    }

  }
}
