namespace BookReader.Mobile.Pages;

public partial class AuthPage : ContentPage
{
    public AuthPage()
    {
        InitializeComponent();
    }

    private async void OnOpenLibraryClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(LibraryPage));
    }
}
