using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kontur.Echelon
{
    public static class EchelonExtensions
    {
        
        public static bool IsSuccessful(this EchelonPutResult result)
        {
            return result == EchelonPutResult.Success;
        }

        public static void EnsureSuccess(this EchelonPutResult result)
        {
            if (!IsSuccessful(result))
                throw new EchelonException(result);
        }

        public static bool IsSuccessful(this EchelonAcknowledgeResult result)
        {
            return result == EchelonAcknowledgeResult.Success;
        }

        public static void EnsureSuccess(this EchelonAcknowledgeResult result)
        {
            if (!IsSuccessful(result))
                throw new EchelonException(result);
        }

        public static bool IsSuccessful(this EchelonProlongResult result)
        {
            return result == EchelonProlongResult.Success;
        }

        public static void EnsureSuccess(this EchelonProlongResult result)
        {
            if (!IsSuccessful(result))
                throw new EchelonException(result);
        }

        public static bool IsSuccessful(this EchelonPostponeResult result)
        {
            return result == EchelonPostponeResult.Success;
        }

        public static void EnsureSuccess(this EchelonPostponeResult result)
        {
            if (!IsSuccessful(result))
                throw new EchelonException(result);
        }

        public static EchelonAcknowledgeResult Acknowledge(this IEchelonTaskHandle handle, TimeSpan timeout)
        {
            return handle.AcknowledgeAsync(timeout).GetAwaiter().GetResult();
        }

        public static EchelonProlongResult ProlongExecution(this IEchelonTaskHandle handle, TimeSpan duration)
        {
            return handle.ProlongExecutionAsync(duration).GetAwaiter().GetResult();
        }

        public static EchelonProlongResult ProlongExecution(this IEchelonTaskHandle handle)
        {
            return handle.ProlongExecutionAsync().GetAwaiter().GetResult();
        }

        public static EchelonPostponeResult Postpone(this IEchelonTaskHandle handle, TimeSpan duration)
        {
            return handle.PostponeAsync(duration).GetAwaiter().GetResult();
        }

        public static EchelonPostponeResult Postpone(this IEchelonTaskHandle handle)
        {
            return handle.PostponeAsync().GetAwaiter().GetResult();
        }
    }
}