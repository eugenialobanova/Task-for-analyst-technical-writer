using System;
using System.Threading.Tasks;

namespace Kontur.Echelon
{
    public interface IEchelonTaskHandle
    {
        EchelonTask Task { get; }

        EchelonTaskOptions Options { get; }

        EchelonTaskMeta Meta { get; }

        Task<EchelonAcknowledgeResult> AcknowledgeAsync(TimeSpan timeout);

        Task<EchelonProlongResult> ProlongExecutionAsync(TimeSpan duration);

        Task<EchelonProlongResult> ProlongExecutionAsync();

        Task<EchelonPostponeResult> PostponeAsync(TimeSpan duration);

        Task<EchelonPostponeResult> PostponeAsync();
    }
}