namespace HelloMauiToolkits;

sealed class GameEndedEventArgs(int score) : EventArgs
{
	public int FinalScore { get; } = score;
}