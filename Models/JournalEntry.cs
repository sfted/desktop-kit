namespace DesktopKit.Models;

using System;

public class JournalEntry
{
    public JournalEntry(
        int journalIndex,
        Type pageType, 
        object parameter = null)
    {
        JournalIndex = journalIndex;
        PageType = pageType;
        Parameter = parameter;
    }

    public int JournalIndex { get; set; }
    public Type PageType { get; set; }
    public object Parameter { get; set; }
}
