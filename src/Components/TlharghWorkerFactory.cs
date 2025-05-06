using Aspenlaub.Net.GitHub.CSharp.Tlhargh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh.Components;

public class TlharghWorkerFactory(IChangedArborFoldersRepository repository, ITlharghAccessor accessor) : ITlharghWorkerFactory {
    public ITlharghWorker Create() {
        return new TlharghWorker(repository, accessor);
    }
}
