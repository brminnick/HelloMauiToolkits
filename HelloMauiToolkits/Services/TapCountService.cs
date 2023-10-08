namespace HelloMauiToolkits;

class TapCountService(IPreferences preferences)
{
	public int TapCountHighScore
	{
		get => preferences.Get(nameof(TapCountHighScore), 0);
		set
		{
			if (value < 0)
				throw new ArgumentOutOfRangeException(nameof(value), "High Score Cannot Be A Negative Number");

			preferences.Set(nameof(TapCountHighScore), value);
		}
	}
}