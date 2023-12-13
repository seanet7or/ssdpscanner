namespace UpnpExplorer
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

#if WINDOWS
        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = base.CreateWindow(activationState);

            var displayInfo = DeviceDisplay.Current.MainDisplayInfo;

            const int newWidth = 600;
            var newHeight = Math.Min(1000, displayInfo.Height / displayInfo.Density);

            window.Width = newWidth;
            window.Height = newHeight;

            return window;
        }
#endif
    }
}
