using System.Windows.Threading;
using System.Windows;

namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh;

// ReSharper disable once UnusedMember.Global
public partial class TlharghWindow : IDisposable {
    private DispatcherTimer? _DispatcherTimer;
    private SynchronizationContext? UiSynchronizationContext { get; }
    private DateTime _UiThreadLastActiveAt;

    public TlharghWindow() {
        InitializeComponent();
        UiSynchronizationContext = SynchronizationContext.Current;
        UpdateUiThreadLastActiveAt();
    }

    public void Dispose() {
        _DispatcherTimer?.Stop();
    }

    private void OnCloseButtonClickAsync(object sender, RoutedEventArgs e) {
        Environment.Exit(0);
    }

    private void OnTlharghWindowLoaded(object sender, RoutedEventArgs e) {
        CreateAndStartTimer();
    }

    private void CreateAndStartTimer() {
        _DispatcherTimer = new DispatcherTimer();
        _DispatcherTimer.Tick += TlharghWindow_Tick;
        _DispatcherTimer.Interval = TimeSpan.FromSeconds(7);
        _DispatcherTimer.Start();
    }

    private void TlharghWindow_Tick(object? sender, EventArgs e) {
        UiSynchronizationContext!.Send(_ => UpdateUiThreadLastActiveAt(), null);
    }

    private void UpdateUiThreadLastActiveAt() {
        _UiThreadLastActiveAt = DateTime.Now;
        UiThreadLastActiveAt.Text = _UiThreadLastActiveAt.ToLongTimeString();
    }
}