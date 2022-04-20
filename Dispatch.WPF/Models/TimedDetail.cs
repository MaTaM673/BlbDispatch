using System;

namespace Dispatch.WPF.Models;
public class TimedDetail
{
    public TimedDetail(string detail, DateTime? time = null)
    {
        time ??= DateTime.Now;
        Time = time.Value;
        Detail = detail;
    }

    public DateTime Time { get; set; }
    public string Detail { get; set; }

}
