namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh.Interfaces;

public interface ITlharghWorker {
    Task DoWorkAsync(int counter, DateTime stopAt);
}
