using Ssdp;
using System.Net;

namespace SsdpScanner;

public partial class MainPage : ContentPage
{
    int count = 0;

    public MainPage()
    {
        InitializeComponent();
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        count++;

        if (count == 1)
            CounterBtn.Text = $"Clicked {count} time";
        else
            CounterBtn.Text = $"Clicked {count} times";

        SemanticScreenReader.Announce(CounterBtn.Text);
    }

    private async void OnScanClicked(object sender, EventArgs e)
    {
        var networkInterfaces = new NetworkInterfaces();
        var addresses = networkInterfaces.GetConnectedIPAddresses();

        LogLb.Text += string.Join(", ", addresses.Select(a => a.ToString()));

        using (var deviceDiscovery = new DeviceDiscovery(addresses))
        {
            deviceDiscovery.DeviceDiscovered += OnDeviceDiscovered;
            await deviceDiscovery.SearchAsync(5);
        }
    }

    void OnDeviceDiscovered(object sender, DeviceDiscoveredEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() => LogLb.Text += e.SearchResponse.Header);
    }
}

