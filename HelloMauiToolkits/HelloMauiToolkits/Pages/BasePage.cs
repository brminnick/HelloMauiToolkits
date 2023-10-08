using System.Diagnostics;

namespace HelloMauiToolkits;

abstract class BasePage<TViewModel> : BasePage where TViewModel : BaseViewModel
{
	protected BasePage(TViewModel viewModel) : base(viewModel)
	{
	}

	public new TViewModel BindingContext => (TViewModel)base.BindingContext;
}

abstract class BasePage : ContentPage
{
	protected BasePage(object? viewModel = null)
	{
		BindingContext = viewModel;
		Padding = 12;

		if (string.IsNullOrWhiteSpace(Title))
		{
			Title = GetType().Name;
		}
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();

		Debug.WriteLine($"OnAppearing: {Title}");
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();

		Debug.WriteLine($"OnDisappearing: {Title}");
	}
}