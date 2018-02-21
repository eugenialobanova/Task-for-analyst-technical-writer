namespace Kontur.Echelon
{
    public enum EchelonProlongResult
    {
        Success = 0,
        TaskNotFound = 1,
        TaskIsNotWaiting = 2,
        ServiceUnavailable = 3,
        Unauthenticated = 4,
        UnknownError = 5
    }
}