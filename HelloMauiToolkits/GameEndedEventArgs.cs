namespace HelloMauiToolkits;

class GameEndedEventArgs(int score) : EventArgs
{
	public int FinalScore { get; } = score;
}