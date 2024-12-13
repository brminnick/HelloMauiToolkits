using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HelloMauiToolkits;
		
partial class TapGameViewModel(TapCountService tapCountService, IDispatcher dispatcher) : BaseViewModel
{
	readonly WeakEventManager _gameEndedWeakEventManager = new();
	readonly TapCountService _tapCountService = tapCountService;
	readonly IDispatcher _dispatcher = dispatcher;

	public event EventHandler<GameEndedEventArgs> GameEnded
	{
		add => _gameEndedWeakEventManager.AddEventHandler(value);
		remove => _gameEndedWeakEventManager.RemoveEventHandler(value);
	}

	[ObservableProperty]
	public partial int TapCount { get; private set; }

	[ObservableProperty]
	public partial int HighScore { get; private set; }= tapCountService.TapCountHighScore;

	[ObservableProperty]
	public partial int TimerSecondsRemaining { get; private set; }= GameConstants.GameDuration.Seconds;

	[ObservableProperty]
	public partial string GameButtonText { get; private set; }= GameConstants.GameButtonText_Start;

	[ObservableProperty, NotifyCanExecuteChangedFor(nameof(GameButtonTappedCommand))]
	public partial bool CanGameButtonTappedCommandExecute { get; private set; } = true;

	[RelayCommand(CanExecute = nameof(CanGameButtonTappedCommandExecute))]
	void GameButtonTapped(string buttonText)
	{
		if (buttonText is GameConstants.GameButtonText_Start)
		{
			GameButtonText = GameConstants.GameButtonText_Tap;
			StartGame();
		}
		else if (buttonText is GameConstants.GameButtonText_Tap)
		{
			TapCount++;
		}
		else
		{
			throw new NotSupportedException("Invalid Game State");
		}
	}

	[RelayCommand]
	void UpdateHighScore(int score)
	{
		_tapCountService.TapCountHighScore = HighScore = score;
	}

	void StartGame()
	{
		var timer = _dispatcher.CreateTimer();
		timer.Interval = TimeSpan.FromSeconds(1);

		timer.Tick += HandleTimerTicked;

		TapCount = 0;

		timer.Start();
	}

	async Task EndGame(int score)
	{
		try
		{
			CanGameButtonTappedCommandExecute = false;

			OnGameEnded(new GameEndedEventArgs(score));

			TimerSecondsRemaining = GameConstants.GameDuration.Seconds;
			GameButtonText = GameConstants.GameButtonText_Start;

			await Task.Delay(TimeSpan.FromSeconds(GameConstants.GameEndPopupDisplayTime.Seconds));
		}
		finally
		{
			CanGameButtonTappedCommandExecute = true;
		}
	}

	async void HandleTimerTicked(object? sender, EventArgs e)
	{
		TimerSecondsRemaining--;

		if (TimerSecondsRemaining is 0)
		{
			ArgumentNullException.ThrowIfNull(sender);
			
			var timer = (IDispatcherTimer)sender;

			timer.Stop();

			await EndGame(TapCount);
		}
	}

	void OnGameEnded(GameEndedEventArgs eventArgs) => _gameEndedWeakEventManager.HandleEvent(this, eventArgs, nameof(GameEnded));
}