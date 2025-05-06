using System.IO;
using System.Windows.Threading;
using System.Windows;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Components;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Interfaces;
using Autofac;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Entities;

namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh;

// ReSharper disable once UnusedMember.Global
public partial class TlharghWindow : IDisposable {
    private DispatcherTimer? _DispatcherTimer;
    private SynchronizationContext? UiSynchronizationContext { get; }
    private DateTime _UiThreadLastActiveAt;
    private readonly IContainer _Container;
    private IChangedArborFoldersRepository? _ChangedArborFoldersRepository;
    private readonly List<FileSystemWatcher> _FileSystemWatchers = [];
    private int _Counter;

    private const int _timerIntervalInSeconds = 10, _workerMaxTimeInSeconds = 7;

    public TlharghWindow() {
        InitializeComponent();
        UiSynchronizationContext = SynchronizationContext.Current;
        UpdateUiThreadLastActiveAt();
        _Container = new ContainerBuilder().UseTlharghDvinAndPegh().Build();
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void Dispose(bool disposing) {
        if (!disposing) { return;}

        _DispatcherTimer?.Stop();

        foreach (FileSystemWatcher watcher in _FileSystemWatchers) {
            watcher.Dispose();
        }

        if (_ChangedArborFoldersRepository == null) { return; }

        _ChangedArborFoldersRepository.OnChangedFolderAdded -= OnChangedFolderAdded;
        _ChangedArborFoldersRepository.OnChangedFolderRemoved -= OnChangedFolderRemoved;
    }

    private void OnCloseButtonClickAsync(object sender, RoutedEventArgs e) {
        Environment.Exit(0);
    }

    // ReSharper disable once AsyncVoidMethod
    private async void OnTlharghWindowLoadedAsync(object sender, RoutedEventArgs e) {
        IArborFoldersSource source = _Container.Resolve<IArborFoldersSource>();
        var errorsAndInfos = new ErrorsAndInfos();
        ArborFolders arborFolders = await source.GetResolvedArborFoldersAsync(errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            MessageBox.Show(string.Join("\r\n", errorsAndInfos.Errors), Properties.Resources.CouldNotRetrieveArborFolders, MessageBoxButton.OK, MessageBoxImage.Error);
            Close();
        }

        IFolderResolver resolver = _Container.Resolve<IFolderResolver>();
        IFolder? workingFolder = await resolver.ResolveAsync(@"$(GitHub)\TlharghBin\Work", errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            MessageBox.Show(string.Join("\r\n", errorsAndInfos.Errors), Properties.Resources.CouldNotDetermineWorkingFolder, MessageBoxButton.OK, MessageBoxImage.Error);
            Close();
        }

        _ChangedArborFoldersRepository = _Container.Resolve<IChangedArborFoldersRepository>();
        _ChangedArborFoldersRepository.SetWorkingFolder(workingFolder);
        _ChangedArborFoldersRepository.OnChangedFolderAdded += OnChangedFolderAdded;
        _ChangedArborFoldersRepository.OnChangedFolderRemoved += OnChangedFolderRemoved;

        foreach (ArborFolder arborFolder in arborFolders) {
            var factory = new ArborFolderWatcherFactory(_ChangedArborFoldersRepository, arborFolder);
            _FileSystemWatchers.Add(factory.Create());
        }
        CreateAndStartTimer();
    }

    private void CreateAndStartTimer() {
        _DispatcherTimer = new DispatcherTimer();
        _DispatcherTimer.Tick += TlharghWindow_Tick;
        _DispatcherTimer.Interval = TimeSpan.FromSeconds(_timerIntervalInSeconds);
        _DispatcherTimer.Start();
    }

    private void TlharghWindow_Tick(object? sender, EventArgs e) {
        UiSynchronizationContext!.Send(_ => UpdateUiThreadLastActiveAt(), null);

        ITlharghWorker worker = _Container.Resolve<ITlharghWorkerFactory>().Create();
        worker.DoWork(++ _Counter, DateTime.Now.AddSeconds(_workerMaxTimeInSeconds));
    }

    private void UpdateUiThreadLastActiveAt() {
        _UiThreadLastActiveAt = DateTime.Now;
        UiThreadLastActiveAt.Text = _UiThreadLastActiveAt.ToLongTimeString();
    }

    private void OnChangedFolderAdded(object? sender, ChangedFolder changedFolder) {
        UiSynchronizationContext!.Send(_ => UpdateMonitorWithChangedFolder(changedFolder, false), null);
    }

    private void OnChangedFolderRemoved(object? sender, ChangedFolder changedFolder) {
        UiSynchronizationContext!.Send(_ => UpdateMonitorWithChangedFolder(changedFolder, true), null);
    }

    private void UpdateMonitorWithChangedFolder(ChangedFolder changedFolder, bool removed) {
        MonitorBox.Text = MonitorBox.Text + (string.IsNullOrWhiteSpace(MonitorBox.Text) ? "" : "\r\n")
            + (removed ? "✅" : "❎") + ' ' + changedFolder;
    }
}