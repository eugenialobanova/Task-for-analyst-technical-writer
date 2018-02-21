namespace Kontur.Echelon
{
    public enum EchelonTakeStatus
    {
        Success = 0,
        NotFound = 1,
        IncorrectArguments = 2,
        ServiceUnavailable = 3,
        Unauthenticated = 4,
        UnknownError = 5
    }
}