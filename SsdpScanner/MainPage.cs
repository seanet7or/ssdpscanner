using Ssdp;
using System.Collections.ObjectModel;

namespace SsdpScanner;

public class MainPage : ContentPage
{
    private Label logLb;

    ObservableCollection<Device> devices = new ObservableCollection<Device>();

    public MainPage()
    {
        logLb = new Label
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        var scanBn = new Button { Text = "Scan" };
        scanBn.Clicked += async (object sender, EventArgs e) => await Scan();

        var deviceListView = new ListView
        {
            VerticalScrollBarVisibility = ScrollBarVisibility.Always,
            IsPullToRefreshEnabled = true,
            ItemTemplate = new DataTemplate(() =>
            {
                // Create views with bindings for displaying each property.
                Label nameLabel = new Label();
                nameLabel.SetBinding(Label.TextProperty, "DisplayName");

                // Return an assembled ViewCell.
                return new ViewCell
                {
                    View = new StackLayout
                    {
                        Padding = new Thickness(0, 5),
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                            new StackLayout
                            {
                                VerticalOptions = LayoutOptions.Center,
                                Spacing = 0,
                                Children = { nameLabel, }
                            }
                        }
                    }
                };
            })
        };
        deviceListView.ItemsSource = devices;
        deviceListView.RefreshCommand = new Command(async () =>
        {
            await this.Scan();
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

        using (var deviceDiscovery = new DeviceDiscovery(addresses))
        {
            deviceDiscovery.DeviceDiscovered += OnDeviceDiscovered;
            await deviceDiscovery.SearchAsync(5);
        }
    }

    void OnDeviceDiscovered(object sender, DeviceDiscoveredEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            logLb.Text += e.SearchResponse.Header;
            devices.Add(new Device { DisplayName = e.SearchResponse.Usn });
        });
    }
}
