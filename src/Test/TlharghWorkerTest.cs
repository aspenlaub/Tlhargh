﻿using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Components;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh.Test;

[TestClass]
public class TlharghWorkerTest {
    private readonly FakeChangedArborFoldersRepository _ChangedArborFoldersRepository = new();
    private readonly FakeTlharghAccessor _FakeTlharghAccessor = new();
    private TlharghWorker _Sut = new(null, null);
    private readonly ArborFolder _ArborFolder = new();
    private readonly IFolder _Folder = new Folder(Path.GetTempPath()).SubFolder(nameof(ArborFolderWatcherTest));
    private IFolder SubFolder => _Folder.SubFolder("Sub");
    private IFolder SubFolderTwo => _Folder.SubFolder("SubTwo");

    [TestInitialize]
    public void Initialize() {
        _ChangedArborFoldersRepository.Reset();
        _ChangedArborFoldersRepository.RegisterChangeInFolder(_ArborFolder, (Folder)SubFolder);
        _ChangedArborFoldersRepository.RegisterChangeInFolder(_ArborFolder, (Folder)SubFolderTwo);
        _FakeTlharghAccessor.Reset();
        _Sut = new TlharghWorker(_ChangedArborFoldersRepository, _FakeTlharghAccessor);
    }

    [TestMethod]
    public async Task WorkerStopsWhenStopAtIsReached() {
        Assert.AreEqual(2, _ChangedArborFoldersRepository.FoldersWithChanges().Count);
        DateTime now = DateTime.Now;
        await _Sut.DoWorkAsync(0, now);
        Assert.AreEqual(1, _ChangedArborFoldersRepository.FoldersWithChanges().Count);
    }

    [TestMethod]
    public async Task WorkerDoesNothingIfProcessingFails() {
        Assert.AreEqual(2, _ChangedArborFoldersRepository.FoldersWithChanges().Count);
        DateTime now = DateTime.Now;
        _FakeTlharghAccessor.LetNextProcessingFail();
        await _Sut.DoWorkAsync(0, now);
        Assert.AreEqual(2, _ChangedArborFoldersRepository.FoldersWithChanges().Count);
    }

    [TestMethod]
    public async Task WorkerCompletesWhenStopAtIsHigh() {
        Assert.AreEqual(2, _ChangedArborFoldersRepository.FoldersWithChanges().Count);
        DateTime nowPlusOneHour = DateTime.Now.AddHours(1);
        await _Sut.DoWorkAsync(0, nowPlusOneHour);
        Assert.AreEqual(0, _ChangedArborFoldersRepository.FoldersWithChanges().Count);
    }
}
