
using ERPApp.Shared.Abstractions.Formatting;

namespace ERApp.Shared.Services.Formatting;

public class FileSizeFormatter : IFileSizeFormatter
{
    public string BytesToReadableString(long byteCount)
    {
        string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
        if (byteCount == 0) return "0B";

        long absBytes = Math.Abs(byteCount);
        int place = Convert.ToInt32(Math.Floor(Math.Log(absBytes, 1024)));
        double num = Math.Round(absBytes / Math.Pow(1024, place), 1);
        return (Math.Sign(byteCount) * num).ToString() + suf[place];
    }
}
