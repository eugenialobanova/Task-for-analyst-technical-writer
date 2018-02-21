namespace Kontur.Echelon
{
    public enum EchelonPutResult
    {
        Success = 0,
        ConflictingId = 1,
        IncorrectArguments = 2,
        ServiceUnavailable = 3,
        Unauthenticated = 4,
        UnknownError = 5
    }
}