using System.Collections.ObjectModel;
using Ssdp;

namespace UpnpExplorer.Views;

public class MainPage : ContentPage
{
    readonly ObservableCollection<Models.Device> devices = [];
    private ListView deviceListView;

    public MainPage()
    {
        ToolbarItem item = new ToolbarItem
        {
            Text = "Scan",
            IconImageSource = ImageSource.FromFile("refresh_black_24dp.png"),
            Command = new Command(async () => await Refresh())
        };
        ToolbarItems.Add(item);

        deviceListView = new ListView
        {
            VerticalScrollBarVisibility = ScrollBarVisibility.Always,
            IsPullToRefreshEnabled = true,
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
                    },
                    ColumnDefinitions = { new ColumnDefinition() },
                    Padding = 10,
                };
                grid.Add(nameLabel, 0, 0);
                grid.Add(locationLabel, 0, 1);

                return new ViewCell { View = grid };
            }),
            ItemsSource = devices
        };
        deviceListView.RefreshCommand = new Command(async () => await Refresh());

        Content = deviceListView;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Refresh();
    }

    private async Task Refresh()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            deviceListView.IsRefreshing = true;
            devices.Clear();
        });
        await Scan();
        MainThread.BeginInvokeOnMainThread(() => deviceListView.IsRefreshing = false);
    }

    private async Task Scan()
    {
        var networkInterfaces = new NetworkInterfaces();
        var addresses = networkInterfaces.GetConnectedIPAddresses();

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
