using DynamicData.Binding;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Threading.Tasks;

namespace ReactiveSandbox.Blazor.Pages;

public partial class Index
{
    public Index()
    {
        _ = this.WhenActivated(disposable =>
        {
            _ = ViewModel!.InboundTracks
                .ObserveCollectionChanges()
                .Subscribe(async _ => await InvokeAsync(StateHasChanged))
                .DisposeWith(disposable);
        });
    }

    public async Task SubmitAsync() => _ = await ViewModel!.SubmitCommand.Execute().ToTask();
}
