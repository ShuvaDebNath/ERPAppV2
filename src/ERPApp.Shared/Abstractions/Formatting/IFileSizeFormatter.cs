namespace ERPApp.Shared.Abstractions.Formatting;

public interface IFileSizeFormatter
{
    string BytesToReadableString(long byteCount);
}
