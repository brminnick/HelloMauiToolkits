namespace HelloMauiToolkits;

sealed class App(AppShell appShell) : Application
{
	readonly AppShell _appShell = appShell;

	protected override Window CreateWindow(IActivationState? activationState) => new(_appShell);
}