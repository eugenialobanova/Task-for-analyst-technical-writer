namespace Kontur.Echelon
{
	public enum EchelonRetryStrategy : byte
	{
		Linear = 0,
		Exponential = 1,
        LinearBackoff = 2
	}
}