using System.IO;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh.Components;

public class ArborFolderWatcherFactory(IChangedArborFoldersRepository changedArborFoldersRepository, ArborFolder arborFolder) {
    public FileSystemWatcher Create() {
        var watcher = new FileSystemWatcher(arborFolder.FullFolder) {
            NotifyFilter =
                NotifyFilters.DirectoryName
                | NotifyFilters.FileName
                | NotifyFilters.CreationTime
                | NotifyFilters.LastWrite
        };
        watcher.Changed += OnChanged;
        watcher.Created += OnCreated;
        watcher.Deleted += OnDeleted;
        watcher.Renamed += OnRenamed;
        watcher.IncludeSubdirectories = true;
        watcher.EnableRaisingEvents = true;

        return watcher;
    }

    private static Folder FolderFromFullPath(string fullPath) {
        int pos = fullPath.LastIndexOf('\\');
        return new Folder(fullPath.Substring(pos + 1).Contains('.')
            ? fullPath[..pos]
            : fullPath);
    }

    private void OnChanged(object sender, FileSystemEventArgs e) {
        if (e.ChangeType != WatcherChangeTypes.Changed) { return; }

        changedArborFoldersRepository.RegisterChangeInFolder(arborFolder, FolderFromFullPath(e.FullPath));
    }

    private void OnCreated(object sender, FileSystemEventArgs e) {
        changedArborFoldersRepository.RegisterChangeInFolder(arborFolder, FolderFromFullPath(e.FullPath));
    }

    private void OnDeleted(object sender, FileSystemEventArgs e) {
        changedArborFoldersRepository.RegisterChangeInFolder(arborFolder, FolderFromFullPath(e.FullPath));
    }

    private void OnRenamed(object sender, RenamedEventArgs e) {
        changedArborFoldersRepository.RegisterChangeInFolder(arborFolder, FolderFromFullPath(e.OldFullPath));
        changedArborFoldersRepository.RegisterChangeInFolder(arborFolder, FolderFromFullPath(e.FullPath));
    }
}