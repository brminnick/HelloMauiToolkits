namespace HelloMauiToolkits;

static class GameConstants
{
	public const string GameButtonText_Tap = "Tap!";
	public const string GameButtonText_Start = "Start";
	
	public static TimeSpan GameDuration { get; } = TimeSpan.FromSeconds(5);
	public static TimeSpan GameEndPopupDisplayTime { get; } = TimeSpan.FromSeconds(3);
	
	public static string GetScoreEmoji(double score, double highScore) => (score/highScore) switch
	{
		< 0 => throw new ArgumentOutOfRangeException(nameof(score), "Score Cannot Be Negative"),
		< 0.25 => "🥹",
		< 0.75 => "😃",
		> 1 => "🚀",
		_ => throw new NotSupportedException($"Input value not supported")
	};
		
}