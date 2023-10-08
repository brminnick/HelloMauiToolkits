using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HelloMauiToolkits;

partial class TapGameViewModel(TapCountService tapCountService, IDispatcher dispatcher) : BaseViewModel
{
	const string tapButtonText_Start = "Start", tapButtonText_Tap = "Tap!";
	
	[ObservableProperty]
	int tapCount, highScore = tapCountService.TapCountHighScore, timerSecondsRemaining = 5;

	[ObservableProperty]
	string tapButtonText = tapButtonText_Start;

	public event EventHandler<GameEndedEventArgs>? GameEnded;

	[RelayCommand]
	void StartButtonTapped()
	{
		if (TapButtonText is tapButtonText_Start)
		{
			TapButtonText = tapButtonText_Tap;
			StartGame();
		}
		else if (TapButtonText is tapButtonText_Tap)
		{
			TapCount++;
		}
		else
		{
			throw new NotSupportedException("Invalid Game State");
		}
	}

	void StartGame()
	{
		var timer = dispatcher.CreateTimer();
		timer.Interval = TimeSpan.FromSeconds(1);

		timer.Tick += HandleTimerTicked;
		
		TapCount = 0;
		
		timer.Start();
	}

	void EndGame(int score)
	{
		TimerSecondsRemaining = 5;

		TapButtonText = tapButtonText_Start;
		
		GameEnded?.Invoke(this, new GameEndedEventArgs(score));

		if (score > tapCountService.TapCountHighScore)
		{
			HighScore = tapCountService.TapCountHighScore = score;
		}
	}
	
	
	void HandleTimerTicked(object? sender, EventArgs e)
	{
		TimerSecondsRemaining--;

		if (TimerSecondsRemaining == 0 && sender is IDispatcherTimer timer)
		{
			timer.Stop();
			EndGame(TapCount);
		}
	}
}