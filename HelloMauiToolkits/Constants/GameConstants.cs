namespace HelloMauiToolkits;

static class GameConstants
{
	public const string GameButtonText_Tap = "Tap!";
	public const string GameButtonText_Start = "Start";
	public static TimeSpan GameDuration { get; } = TimeSpan.FromSeconds(5);
	public static TimeSpan GameEndPopupDisplayTime { get; } = TimeSpan.FromSeconds(3);
}