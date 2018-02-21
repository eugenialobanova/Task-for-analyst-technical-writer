using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kontur.Echelon
{
    public interface IEchelonClient
    {
        Task<EchelonPutResult> PutAsync(IList<EchelonTask> tasks, EchelonTaskOptions options, TimeSpan timeout);

        Task<EchelonTakeResult> TakeAsync(int count, IList<string> taskTypes, TimeSpan timeout, bool includeMeta = false);

    }
}
