using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kontur.Logging;
using Kontur.Net.Http;
using Kontur.Net.Http.ClusterConfigSupport;
using Kontur.Net.Http.Factories;
using Portal.Common;

namespace Kontur.Echelon
{
    public class EchelonClient : IEchelonClient
    {
        internal readonly IClusterClientsForShardsResolver clusterClientsResolver;
        private readonly string topologyName;

        public EchelonClient(string topologyName, IAuthenticationProvider authProvider, ILog log)
        {
            Log = log;

            var topologyType = EchelonTopologyHelper.GetTopologyType(topologyName);

            IClusterClientFactory clusterClientFactory;
            switch (topologyType)
            {
                case EchelonTopologyType.Profile:
                    clusterClientsResolver = new FromTopologyClusterClientsResolver(topologyName, log);
                    this.topologyName = topologyName;
                    break;
                case EchelonTopologyType.Shard:
                    clusterClientFactory = new ClusterClientForTopologyFactory(topologyName, log).EchelonConfigure();
                    clusterClientsResolver = new FromFactoriesClusterClientsResolver(clusterClientFactory, log);
                    break;
                case EchelonTopologyType.ProxyUrl:
                    clusterClientFactory = new EndpointOnlyClusterClientFactory(new HttpUrl(topologyName));
                    clusterClientsResolver = new OneClusterClientResolver(clusterClientFactory.CreateClient(log));
                    break;
                default:
                    throw new ArgumentException($"ToplogyName '{topologyName}' expected to be parsed as Profile of Shard topology but was {topologyType}", nameof(topologyName));
            }

            RequestFactory = new EchelonClientRequestFactory(authProvider);
        }

        public EchelonClient(string topologyName, ILog log)
            : this(topologyName, null, log)
        {
        }

        internal EchelonClientRequestFactory RequestFactory { get; }

        internal ILog Log { get; }

        public Task<EchelonPutResult> PutAsync(IList<EchelonTask> tasks, EchelonTaskOptions options, TimeSpan timeout)
        {
            var request = new PutTasksRequest(tasks, options);
            if (!RequestsValidator.ValidatePutTasksRequest(request, Log) ||
                !ArgumentsValidator.ValidateTimeout(timeout, Log))
                return Task.FromResult(EchelonPutResult.IncorrectArguments);
            var operation = new PutOperation(clusterClientsResolver, RequestFactory, Log, topologyName);

            return operation.ExecuteAsync(request, timeout);
        }

        public Task<EchelonTakeResult> TakeAsync(int count, IList<string> taskTypes, TimeSpan timeout, bool includeMeta = false)
        {
            return TakeAsync(count, taskTypes, false, timeout, includeMeta);
        }

        public Task<EchelonTakeResult> TakeAsync(int count, IList<string> taskTypes, bool onlyFromCurrentTopology, TimeSpan timeout, bool includeMeta = false)
        {
            var request = new TakeTasksRequest(count, taskTypes, onlyFromCurrentTopology ? topologyName : null, includeMeta);
            if (!RequestsValidator.ValidateTakeTasksRequest(request, Log) ||
                !ArgumentsValidator.ValidateTimeout(timeout, Log))
                return Task.FromResult(new EchelonTakeResult(EchelonTakeStatus.IncorrectArguments, null));
            var operation = new TakeOperation(clusterClientsResolver, RequestFactory, Log, topologyName);

            return operation.ExecuteAsync(request, timeout);
        }
    }
}