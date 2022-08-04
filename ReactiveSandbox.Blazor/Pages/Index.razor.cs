using DynamicData.Binding;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Threading.Tasks;

namespace ReactiveSandbox.Blazor.Pages;

public partial class Index
{
    [Inject]
    public IDialogService DialogService { get; set; }

    public Index()
    {
        _ = this.WhenActivated(disposable =>
        {
            _ = ViewModel!.InboundTracks
                .ObserveCollectionChanges()
                .Subscribe(async _ => await InvokeAsync(StateHasChanged))
                .DisposeWith(disposable);

            _ = ViewModel.ConfirmClean.RegisterHandler(
                async context =>
                {
                    var result = await DialogService!.ShowMessageBox("Clear", "Clear all tracks?", cancelText: "Cancel");
                    context.SetOutput(result.Value);
                })
                .DisposeWith(disposable);

        });
    }

    public async Task SubmitAsync() => _ = await ViewModel!.SubmitCommand.Execute().ToTask();
    public async Task ClearAsync() => _ = await ViewModel!.CleanCommand.Execute().ToTask();
}
