using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Views;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace HelloMauiToolkits;

class TapGamePage : BasePage<TapGameViewModel>
{
	readonly TapCountService tapCountService;
	
	public TapGamePage(TapGameViewModel viewModel, TapCountService tapCountService) : base(viewModel)
	{
		this.tapCountService = tapCountService;
		
		Title = "Tap Game";

		viewModel.GameEnded += HandleGameEnded;

		Content = new Grid
		{
			RowDefinitions = Rows.Define(
				(Row.HighScore, 44),
				(Row.Description, 24),
				(Row.TapButton, Star),
				(Row.TapCounter, 32),
				(Row.Timer, 32)),
			
			Children =
			{
				new Label()
					.Row(Row.HighScore)
					.Font(size: 36)
					.TextCenter()
					.Center()
					.Bind(Label.TextProperty, 
							static (TapGameViewModel vm) => vm.HighScore, 
							mode: BindingMode.OneWay, 
							convert: number => $"High Score: {number}"),
				
				new Label()
					.Row(Row.Description)
					.Text("Start the game, then tap the button as many times as you can in 5 seconds!")
					.Font(size: 24, italic: true)
					.TextCenter()
					.Center(),
				
				new Button()
					.Row(Row.TapButton)
					.Center()
					.Bind(Button.TextProperty, 
							static (TapGameViewModel vm) => vm.GameButtonText, 
							mode: BindingMode.OneWay)
					.BindCommand(
							static (TapGameViewModel vm) => vm.GameButtonTappedCommand, 
							commandBindingMode: BindingMode.OneTime, 
							parameterGetter: static (TapGameViewModel vm) => vm.GameButtonText, 
							parameterBindingMode: BindingMode.OneWay),
				
				new Label()
					.Row(Row.TapCounter)
					.Center()
					.TextCenter()
					.Font(size: 24, bold: true)
					.Bind(Label.TextProperty, 
							static (TapGameViewModel vm) => vm.TapCount),
				
				new Label()
					.Row(Row.Timer)
					.Center()
					.TextCenter()
					.Bind(Label.TextProperty, 
							static (TapGameViewModel vm) => vm.TimerSecondsRemaining,
							mode: BindingMode.OneWay,
							convert: seconds => $"Seconds Remaining: {seconds}")
			}
		};
	}
	
	async void HandleGameEnded(object? sender, GameEndedEventArgs e)
	{
		Popup popup = e.FinalScore switch
		{
			var score when (score > tapCountService.TapCountHighScore) => new GameEndedPopup("New High Score!", score.ToString()),
			_ => new GameEndedPopup("Game Over", $"You scored {e.FinalScore} points!")
		};
		
		await this.ShowPopupAsync(popup);
	}

	enum Row { HighScore, Description, TapButton, TapCounter, Timer }
}