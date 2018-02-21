using System.Collections.Generic;

namespace Kontur.Echelon
{
    public class EchelonTakeResult
    {
        private static readonly IList<IEchelonTaskHandle> emptyHandles = new IEchelonTaskHandle[] {};

        private readonly IList<IEchelonTaskHandle> taskHandles;

        public EchelonTakeResult(EchelonTakeStatus status, IList<IEchelonTaskHandle> taskHandles)
        {
            Status = status;
            this.taskHandles = taskHandles;
        }

        public EchelonTakeStatus Status { get; }

        public IList<IEchelonTaskHandle> TaskHandles
        {
            get
            {
                switch (Status)
                {
                    case EchelonTakeStatus.Success:
                        return taskHandles;
                    case EchelonTakeStatus.NotFound:
                        return emptyHandles;
                    default:
                        throw new EchelonException(Status);
                }
            }
        }

        public bool IsSuccessful()
        {
            return Status == EchelonTakeStatus.Success || Status == EchelonTakeStatus.NotFound;
        }

        public EchelonTakeResult EnsureSuccess()
        {
            if (!IsSuccessful())
                throw new EchelonException(Status);
            return this;
        }
    }
}