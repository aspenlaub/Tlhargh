using System.IO;
using System.Windows.Threading;
using System.Windows;
using Aspenlaub.Net.GitHub.CSharp.Dvin.Components;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Components;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Interfaces;
using Autofac;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;

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
        _ChangedArborFoldersRepository.OnFolderSurvived += OnFolderSurvived;

        foreach (ArborFolder arborFolder in arborFolders) {
            var factory = new ArborFolderWatcherFactory(_ChangedArborFoldersRepository, arborFolder);
            _FileSystemWatchers.Add(factory.Create());
        }
        CreateAndStartTimer();
    }

    private void CreateAndStartTimer() {
        _DispatcherTimer = new DispatcherTimer();
        _DispatcherTimer.Tick += TlharghWindow_TickAsync;
        _DispatcherTimer.Interval = TimeSpan.FromSeconds(_timerIntervalInSeconds);
        _DispatcherTimer.Start();
    }

    private async void TlharghWindow_TickAsync(object? sender, EventArgs e) {
        try {
            UiSynchronizationContext!.Send(_ => UpdateUiThreadLastActiveAt(), null);

            ITlharghWorker worker = _Container.Resolve<ITlharghWorkerFactory>().Create();
            await worker.DoWorkAsync(++ _Counter, DateTime.Now.AddSeconds(_workerMaxTimeInSeconds));
        } catch (Exception exception) {
            UiSynchronizationContext!.Send(_ => UpdateMonitorWithException(exception), null);
            IFolder? exceptionLogFolder = new Folder(Path.GetTempPath()).SubFolder("AspenlaubExceptions");
            ExceptionSaver.SaveUnhandledException(exceptionLogFolder, exception, Constants.TlharghAppId, _ => { });
        }
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

    private void OnFolderSurvived(object? sender, EventArgs e) {
        UiSynchronizationContext!.Send(_ => UpdateMonitorWithInconvenience(Properties.Resources.FolderSurvivedRemovalAfterProcessing), null);
    }

    private void UpdateMonitorWithChangedFolder(ChangedFolder changedFolder, bool removed) {
        MonitorBox.Text = MonitorBox.Text + (string.IsNullOrWhiteSpace(MonitorBox.Text) ? "" : "\r\n")
            + (removed ? "✅" : "❎") + ' ' + changedFolder;
    }

    private void UpdateMonitorWithException(Exception exception) {
        MonitorBox.Text = MonitorBox.Text + (string.IsNullOrWhiteSpace(MonitorBox.Text) ? "" : "\r\n")
            + "❎ Exception: " + exception.Message;
    }

    private void UpdateMonitorWithInconvenience(string inconvenience) {
        MonitorBox.Text = MonitorBox.Text + (string.IsNullOrWhiteSpace(MonitorBox.Text) ? "" : "\r\n")
            + "❎ OOPS: " + inconvenience;
    }
}