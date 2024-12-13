namespace HelloMauiToolkits;

sealed partial class AppShell : Shell
{
	public AppShell(TapGamePage tapGamePage)
	{
		Items.Add(tapGamePage);
	}
}