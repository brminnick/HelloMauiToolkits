using System.Diagnostics;

namespace HelloMauiToolkits;

abstract class BasePage<TViewModel>(TViewModel viewModel) : BasePage(viewModel) where TViewModel : BaseViewModel
{
	public new TViewModel BindingContext => (TViewModel)base.BindingContext;
}

abstract class BasePage : ContentPage
{
	protected BasePage(object? viewModel = null)
	{
		BindingContext = viewModel;
		Padding = 12;
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