using DesktopKit.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesktopKit.Services;

public interface INavigationService
{
    event Action<Type, object> Navigated;

    XamlRoot XamlRoot { get; set; }
    Type NotFoundPage { get; set; }
    Dictionary<string, Type> PageTypes { get; set; }
    List<JournalEntry> Journal { get; }
    JournalEntry CurrentEntry { get; }

    void NavigateTo(string pageId, object parameter = null);
    void NavigateTo(Type page, object parameter = null);
    void NavigateBack();
    void NavigateForward();
    bool CanNavigateBack();
    bool CanNavigateForward();

    Task ShowDialogAsync(ContentDialog dialog);
}

public class NavigationService : INavigationService
{
    public event Action<Type, object> Navigated;

    public XamlRoot XamlRoot { get; set; }
    public Type NotFoundPage { get; set; }
    public Dictionary<string, Type> PageTypes { get; set; }
    public List<JournalEntry> Journal { get; protected set; } = new();
    public JournalEntry CurrentEntry { get; protected set; }

    public void NavigateTo(string pageId, object parameter = null) =>
        NavigateTo(ResolvePageType(pageId), parameter);

    public void NavigateTo(Type page, object parameter = null)
    {
        if (Journal.Any())
        {
            if (Journal.Count != CurrentEntry.JournalIndex + 1)
                Journal.RemoveRange(
                    CurrentEntry.JournalIndex + 1,
                    Journal.Count - CurrentEntry.JournalIndex - 1);

            Journal.Add(new JournalEntry(CurrentEntry.JournalIndex + 1, page, parameter));
        }
        else
            Journal.Add(new JournalEntry(0, page, parameter));

        CurrentEntry = Journal.Last();
        Navigated?.Invoke(page, parameter);
    }

    public void NavigateBack()
    {
        if (CanNavigateBack())
            NavigateThroughTheJournal(Journal[CurrentEntry.JournalIndex - 1]);
    }

    public void NavigateForward()
    {
        if (CanNavigateForward())
            NavigateThroughTheJournal(Journal[CurrentEntry.JournalIndex + 1]);
    }

    public bool CanNavigateBack() =>
        CurrentEntry != null &&
        CurrentEntry.JournalIndex > 0;

    public bool CanNavigateForward() =>
        CurrentEntry != null &&
        CurrentEntry.JournalIndex + 1 < Journal.Count;

    public async Task ShowDialogAsync(ContentDialog dialog)
    {
        dialog.XamlRoot = XamlRoot;
        await dialog.ShowAsync();
    }

    protected void NavigateThroughTheJournal(JournalEntry entry)
    {
        CurrentEntry = entry;
        Navigated?.Invoke(entry.PageType, entry.Parameter);
    }

    protected Type ResolvePageType(string pageId) =>
        !string.IsNullOrEmpty(pageId) ?
            PageTypes.ContainsKey(pageId) ? PageTypes[pageId] : NotFoundPage
        : NotFoundPage;
}
