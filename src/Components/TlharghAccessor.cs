using System.Net.Http;
using System.Net.Http.Json;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh.Components;

public class TlharghAccessor(ITlharghSettingsSource tlharghSettingsSource) : ITlharghAccessor {
    private const string _processChangedFolderCommand = "ProcessChangedFolder";

    private readonly HttpClient _HttpClient = new();

    public async Task ProcessChangedFolderAsync(ChangedFolder changedFolder, ErrorsAndInfos errorsAndInfos) {
        if (errorsAndInfos.AnyErrors()) { return; }

        TlharghSettings tlharghSettings = await tlharghSettingsSource.GetTlharghSettingsAsync(errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) { return; }

        string tlharghBackendUrl = tlharghSettings.TlharghBackendUrl;
        var parms = new TlharghProcessorParms {
            Command = _processChangedFolderCommand,
            ArborPathVar = changedFolder.ArborFolder.ArborSourcePathVar,
            ArborSubFolder = changedFolder.ArborSubFolder
        };

        HttpResponseMessage result = await _HttpClient.PostAsync(tlharghBackendUrl, new FormUrlEncodedContent(parms.ToDictionary()));
        if (!result.IsSuccessStatusCode) {
            errorsAndInfos.Errors.Add($"Error accessing {tlharghBackendUrl}");
            return;
        }

        var processorResult = await result.Content.ReadFromJsonAsync<TlharghProcessorResult>();
        if (processorResult == null) {
            errorsAndInfos.Errors.Add($"Error reading result from {tlharghBackendUrl}");
        } else if (!processorResult.Accepted) {
            errorsAndInfos.Errors.Add($"Request not accepted by {tlharghBackendUrl}");
        } else if (!processorResult.Success) {
            errorsAndInfos.Errors.Add($"Request could not be processed by {tlharghBackendUrl}");
        }
    }
}
