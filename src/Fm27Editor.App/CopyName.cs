using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace Fm27Editor.App;

internal static class CopyName
{
    public static void Attach<T>(DataGrid grid, Func<T, string> name, Action<string> status) where T : class
    {
        async Task CopyAsync()
        {
            if (grid.SelectedItem is not T row || TopLevel.GetTopLevel(grid)?.Clipboard is not { } clipboard) return;
            try
            {
                await clipboard.SetTextAsync(name(row));
                status(AppLanguage.T("Name copied"));
            }
            catch (Exception ex) { status($"Could not copy name: {ex.Message}"); }
        }

        var item = new MenuItem { Header = AppLanguage.T("Copy name") };
        item.Click += async (_, _) => await CopyAsync();
        grid.ContextMenu = new ContextMenu { Items = { item } };
        grid.AddHandler(InputElement.PointerPressedEvent, (_, e) =>
        {
            if (e.GetCurrentPoint(grid).Properties.IsRightButtonPressed && e.Source is Avalonia.Visual source &&
                source.FindAncestorOfType<DataGridRow>(includeSelf: true)?.DataContext is T row)
                grid.SelectedItem = row;
        }, RoutingStrategies.Tunnel);
        grid.AddHandler(InputElement.KeyDownEvent, async (_, e) =>
        {
            if (e.Key == Key.C && e.KeyModifiers == KeyModifiers.Control && grid.SelectedItem is T)
            {
                e.Handled = true;
                await CopyAsync();
            }
        }, RoutingStrategies.Tunnel);
    }
}
