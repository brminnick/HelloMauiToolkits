namespace HelloMauiToolkits;

class TapGamePage : BasePage<TapGameViewModel>
{
	public TapGamePage(TapGameViewModel viewModel) : base(viewModel)
	{
		Title = "Tap Game";
		
		Content = new 
	}
	
	enum Row { HighScore, Description, TapButton, TapCounter }
}