using System;
using System.Threading.Tasks;
using Kontur.Logging;
using Kontur.Net.Http;
using Kontur.Utilities.Convertions.Time;

namespace Kontur.Echelon
{
    internal class EchelonTaskHandle : IEchelonTaskHandle
    {
        private readonly IClusterClientsForShardsResolver clientResolver;
        private readonly EchelonClientRequestFactory requestFactory;
        private readonly ILog log;

        public EchelonTaskHandle(EchelonTask task, IClusterClient shardClient, EchelonClientRequestFactory requestFactory, ILog log, EchelonTaskMeta meta = null, EchelonTaskOptions options = null)
        {
            Task = task;
            Meta = meta;
            Options = options;
            this.requestFactory = requestFactory;
            this.log = log;

            clientResolver = new OneClusterClientResolver(shardClient);
        }

        public EchelonTask Task { get; }
        public EchelonTaskOptions Options { get; }
        public EchelonTaskMeta Meta { get; }

        public Task<EchelonAcknowledgeResult> AcknowledgeAsync(TimeSpan timeout)
        {
            return new AcknowledgeOperation(clientResolver, requestFactory, log).ExecuteAsync(new[] {Task.Id}, timeout);
        }

        public Task<EchelonProlongResult> ProlongExecutionAsync(TimeSpan duration)
        {
            return ProlongExecutionAsync(duration as TimeSpan?);
        }

        public Task<EchelonProlongResult> ProlongExecutionAsync()
        {
            return ProlongExecutionAsync(null);
        }

        public Task<EchelonPostponeResult> PostponeAsync(TimeSpan duration)
        {
            return PostponeAsync(duration as TimeSpan?);
        }

        public Task<EchelonPostponeResult> PostponeAsync()
        {
            return PostponeAsync(null);
        }

        private Task<EchelonProlongResult> ProlongExecutionAsync(TimeSpan? duration)
        {
            var request = new ProlongExecutionRequest(Task.Id, duration);
            var operation = new ProlongOperation(clientResolver, requestFactory, log);
            return operation.ExecuteAsync(request, 30.Seconds());
        }

        private Task<EchelonPostponeResult> PostponeAsync(TimeSpan? duration)
        {
            var request = new PostponeRequest(Task.Id, duration);
            var operation = new PostponeOperation(clientResolver, requestFactory, log);
            return operation.ExecuteAsync(request, 30.Seconds());
        }
    }
}