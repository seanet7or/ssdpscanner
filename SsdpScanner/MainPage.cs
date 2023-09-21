using Ssdp;

namespace SsdpScanner;

public class MainPage : ContentPage
{
    private Label logLb;

    public MainPage()
    {
        this.logLb = new Label { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
        var scanBn = new Button { Text = "Scan" };
        scanBn.Clicked += OnScanClicked;
        Content = new VerticalStackLayout
        {
            Children = {
                scanBn,
                logLb
            }
        };
    }

    private async void OnScanClicked(object sender, EventArgs e)
    {
        var networkInterfaces = new NetworkInterfaces();
        var addresses = networkInterfaces.GetConnectedIPAddresses();

        logLb.Text += string.Join(", ", addresses.Select(a => a.ToString()));

        using (var deviceDiscovery = new DeviceDiscovery(addresses))
        {
            deviceDiscovery.DeviceDiscovered += OnDeviceDiscovered;
            await deviceDiscovery.SearchAsync(5);
        }
    }

    void OnDeviceDiscovered(object sender, DeviceDiscoveredEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() => logLb.Text += e.SearchResponse.Header);
    }
}