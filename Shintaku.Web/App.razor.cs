using Avalonia.Web.Blazor;

namespace Shintaku.Web;

public partial class App
{
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        
        WebAppBuilder.Configure<Shintaku.App>()
            .SetupWithSingleViewLifetime();
    }
}