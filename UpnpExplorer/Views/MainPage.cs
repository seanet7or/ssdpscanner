using System.Collections.ObjectModel;
using Ssdp;

namespace UpnpExplorer.Views;

public class MainPage : ContentPage
{
    private readonly Label logLb;

    readonly ObservableCollection<Models.Device> devices = [];

    public MainPage()
    {
        logLb = new Label
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        var scanBn = new Button { Text = "Scan" };
        scanBn.Clicked += async (sender, e) => await Scan();

        var deviceListView = new ListView
        {
            VerticalScrollBarVisibility = ScrollBarVisibility.Always,
            IsPullToRefreshEnabled = true,
            RowHeight = 100,
            ItemTemplate = new DataTemplate(() =>
            {
                var nameLabel = new Label();
                nameLabel.SetBinding(Label.TextProperty, "DisplayName");

                var locationLabel = new Label();
                locationLabel.SetBinding(Label.TextProperty, "Location");

                var grid = new Grid
                {
                    RowDefinitions =
                    {
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    },
                    ColumnDefinitions = { new ColumnDefinition() }
                };

                grid.Add(nameLabel, 0, 1);
                grid.Add(locationLabel, 0, 2);

                return new ViewCell { View = grid };
            }),
            ItemsSource = devices
        };
        deviceListView.RefreshCommand = new Command(async () =>
        {
            await Scan();
            deviceListView.IsRefreshing = false;
        });

        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = new GridLength(100) },
                new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
            },
            ColumnDefinitions = { new ColumnDefinition() }
        };

        var logView = new ScrollView { Content = logLb };
        grid.Add(scanBn, 0, 0);
        grid.Add(deviceListView, 0, 1);
        grid.Add(logView, 0, 2);

        Content = grid;
    }

    private async Task Scan()
    {
        var networkInterfaces = new NetworkInterfaces();
        var addresses = networkInterfaces.GetConnectedIPAddresses();

        logLb.Text += string.Join(", ", addresses.Select(a => a.ToString()));

        using var deviceDiscovery = new DeviceDiscovery(addresses);
        deviceDiscovery.DeviceDiscovered += OnDeviceDiscovered;
        await deviceDiscovery.SearchAsync(5);
    }

    void OnDeviceDiscovered(object? sender, DeviceDiscoveredEventArgs e)
    {
        var location = e.SearchResponse.Location;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (!devices.Any((d) => d.Location == location))
            {
                logLb.Text += e.SearchResponse.Header;
                devices.Add(
                    new Models.Device
                    {
                        DisplayName = (e.SearchResponse.Server ?? "") + e.SearchResponse.Usn,
                        Location = e.SearchResponse.Location
                    }
                );
            }
        });
    }
}
