using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HelloMauiToolkits;

partial class TapGameViewModel(TapCountService tapCountService, IDispatcher dispatcher) : BaseViewModel
{
	const string tapButtonText_Start = "Start", tapButtonText_Tap = "Tap!";

	readonly WeakEventManager weakEventManager = new();
	
	[ObservableProperty]
	int tapCount, highScore = tapCountService.TapCountHighScore, timerSeconds;

	[ObservableProperty]
	string tapButtonText = tapButtonText_Start;

	public event EventHandler<GameEndedEventArgs> GameEnded
	{
		add => weakEventManager.AddEventHandler(value);
		remove => weakEventManager.RemoveEventHandler(value);
	}

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
	}

	void EndGame(int score)
	{
		TimerSeconds = 0;
		
		OnGameEnded(score);

		if (score > tapCountService.TapCountHighScore)
		{
			HighScore = tapCountService.TapCountHighScore = score;
		}
	}
	
	
	void HandleTimerTicked(object? sender, EventArgs e)
	{
		TimerSeconds++;

		if (TimerSeconds >= 5)
		{
			EndGame(TapCount);
		}
	}

	void OnGameEnded(int score) => weakEventManager.HandleEvent(this, new GameEndedEventArgs(score), nameof(GameEndedEventArgs));
}