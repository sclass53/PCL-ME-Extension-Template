// ============================================================================
//  PCL-ME plugin template
// ============================================================================
//
//  This file is a starting point, not a finished plugin. The project builds and
//  installs as-is, but everything is deliberately generic: ids use the
//  "your.plugin" prefix and the UI is a small placeholder.
//
//  Before publishing, do these steps:
//
//   1. Rename the project/assembly/namespace (PluginTemplate -> MyPlugin).
//      If you rename the assembly, update `entryAssembly` and `entryType` in
//      extension.json to match.
//   2. Change `id`, `name`, `description`, `author` and `version` in extension.json.
//      Keep the id lowercase with dots/hyphens (for example `me.myplugin`).
//   3. Rename every UI contribution id below (they must start with your
//      extension id plus a dot) and update the ids in the manifest commands.
//   4. Replace the TODO sections with your own UI and behaviour.
//   5. Delete the parts you do not need (instance tab, action, search, worker,
//      settings) from both the code and extension.json.
//
//  The full guide is in README.md.
// ============================================================================

using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using PCL.Extensions.Avalonia;
using PCL.Extensions.Contracts;
using PCL.UI.Commands;
using PCL.UI.Controls;

namespace PluginTemplate;

public sealed class PluginTemplateExtension : IExtension
{
    // TODO: namespace your contribution ids with your extension id.
    // The host validates that every UI id starts with "<manifest id>.".
    private const string PageId = "your.plugin.page";

    public ValueTask ActivateAsync(IExtensionContext context, CancellationToken cancellationToken)
    {
        var ui = context.GetService<IExtensionUi>();

        // ------------------------------------------------------------------
        // PAGE
        // A Page view appears in the Extensions menu and can be opened by a
        // command, search result or toolbar action.
        // ------------------------------------------------------------------
        ui.RegisterView(new ExtensionView
        {
            Id = PageId,
            // TODO: replace the page title (and its locale key) with yours.
            Title = "Plugin Template",
            TitleKey = "title"
        }, _ =>
        {
            // TODO: build your own page here. Any native Avalonia control works:
            // PclCard, PclButton, PclSearchBox, custom controls, XAML, etc.
            var panel = new StackPanel { Margin = new Thickness(24), Spacing = 16 };
            panel.Children.Add(new TextBlock
            {
                // TODO: read your own setting. Keys are declared in extension.json.
                // Settings are string/boolean/number values stored per extension.
                Text = context.Settings.Get("greeting", "Hello from my plugin!"),
                FontSize = 24,
                TextWrapping = TextWrapping.Wrap
            });
            panel.Children.Add(new TextBlock
            {
                Text = context.Text.Get("description"),
                Opacity = 0.7,
                TextWrapping = TextWrapping.Wrap
            });
            panel.Children.Add(new PclButton
            {
                Text = context.Text.Get("notify"),
                // TODO: replace this with your action. `context.Launcher.Notify`
                // shows a small launcher notification.
                Command = new ActionCommand(() => context.Launcher.Notify(context.Text.Get("message")))
            });
            return new PclCard
            {
                Header = context.Text.Get("title"),
                CardContent = panel,
                Margin = new Thickness(24)
            };
        });

        // ------------------------------------------------------------------
        // INSTANCE TAB (optional)
        // The view receives the selected instance id. Remove this block if
        // your extension does not need an instance-specific tab.
        // ------------------------------------------------------------------
        ui.RegisterView(new ExtensionView
        {
            Id = "your.plugin.instance",
            Title = "Plugin instance information",
            Location = ExtensionViewLocation.InstanceTab
        }, view => new TextBlock
        {
            // TODO: show instance-specific information here.
            Text = string.IsNullOrWhiteSpace(view.InstanceId)
                ? context.Text.Get("no_instance")
                : view.InstanceId,
            Margin = new Thickness(16),
            TextWrapping = TextWrapping.Wrap
        });

        // ------------------------------------------------------------------
        // COMMAND
        // Commands are declared in extension.json. Register the handler here;
        // the same command can be used by the Extensions menu, the global
        // search and toolbar/instance-menu actions.
        // ------------------------------------------------------------------
        var command = context.Manifest.Commands.Single();
        context.Commands.Register(command, token =>
            ui.ShowViewAsync(PageId, cancellationToken: token));
        // TODO: if you have more commands, register each manifest command and
        // keep the ids in extension.json in sync.

        // ------------------------------------------------------------------
        // INSTANCE CONTEXT-MENU ACTION (optional)
        // Remove this if you do not need an entry in the instance context menu.
        // ------------------------------------------------------------------
        ui.RegisterAction(new ExtensionAction(
            "your.plugin.instance-menu",
            "Plugin Template",
            "your.plugin.open",
            ExtensionActionLocation.InstanceMenu,
            "title"));

        // ------------------------------------------------------------------
        // GLOBAL SEARCH PROVIDER (optional)
        // Return results for queries your extension can answer. Keep them fast;
        // they run on a background thread with a short deadline.
        // ------------------------------------------------------------------
        context.Search.Register("your.plugin.search", (query, _) =>
        {
            // TODO: replace this keyword matching with your own search.
            var matches = query.Text.Length == 0
                          || query.Text.Contains("plugin", StringComparison.OrdinalIgnoreCase)
                          || query.Text.Contains("template", StringComparison.OrdinalIgnoreCase);
            return ValueTask.FromResult<IReadOnlyList<ExtensionSearchResult>>(matches
                ?
                [
                    new ExtensionSearchResult
                    {
                        Id = "your.plugin",
                        Kind = ExtensionSearchKind.Page,
                        Score = 0.8,
                        Title = context.Text.Get("title"),
                        Description = context.Text.Get("description"),
                        CommandId = "your.plugin.open"
                    }
                ]
                : []);
        });

        // ------------------------------------------------------------------
        // LAUNCHER EVENT (optional)
        // Subscribe to host events with context.Events.Subscribe("topic", ...).
        // Common topics include launcher.ready and launcher.instance.changed.
        // ------------------------------------------------------------------
        context.Events.Subscribe("launcher.instance.changed",
            message => context.Log(ExtensionLogLevel.Information, "Selected instance: " + message.Data));

        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        // TODO: unsubscribe from events, dispose windows/workers and release
        // other resources your extension owns. Registrations returned by
        // RegisterView/RegisterAction/Search.Register are cleaned up by the
        // host, but anything you create yourself must be disposed here.
        return ValueTask.CompletedTask;
    }
}

// ============================================================================
//  OPTIONAL WORKER
// ============================================================================
//
//  A worker runs in a separate process, so it can do long-running or risky
//  work without blocking the launcher. This template does not start it.
//
//  To use it:
//    var worker = context.StartWorker(new ExtensionWorkerOptions(
//        "PluginTemplate.dll", "PluginTemplate.TemplateWorker"));
//    var result = await worker.RequestAsync("delay", JsonSerializer.SerializeToElement(500));
//
//  The worker type must be public, have a public parameterless constructor
//  and implement IExtensionWorkerHandler.
// ============================================================================
public sealed class TemplateWorker : IExtensionWorkerHandler
{
    public async ValueTask<JsonElement> HandleAsync(string method, JsonElement payload, CancellationToken cancellationToken)
    {
        // TODO: implement worker methods. Return JSON for the caller.
        if (method == "delay")
        {
            await Task.Delay(payload.GetInt32(), cancellationToken);
        }

        return JsonSerializer.SerializeToElement(new { method, payload });
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}