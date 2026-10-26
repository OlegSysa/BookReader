using BookReader.Mobile.Models;

namespace BookReader.Mobile.Pages;

public partial class LibraryPage : ContentPage
{
    public IReadOnlyList<LibraryBookItem> Books { get; } =
    [
        new()
        {
            Title = "Кобзар",
            Author = "Тарас Шевченко",
            Status = "Готово",
            Initial = "К",
            Progress = 0.42,
            Cover = Color.FromArgb("#8C3A2F")
        },
        new()
        {
            Title = "1984",
            Author = "Джордж Орвелл",
            Status = "Готово",
            Initial = "1",
            Progress = 0.18,
            Cover = Color.FromArgb("#3D5A4C")
        },
        new()
        {
            Title = "Маленький принц",
            Author = "Антуан де Сент-Екзюпері",
            Status = "Обробляється",
            Initial = "М",
            Progress = 0,
            Cover = Color.FromArgb("#2F4A6E")
        },
        new()
        {
            Title = "Sapiens",
            Author = "Ювал Ной Харарі",
            Status = "Готово",
            Initial = "S",
            Progress = 0.76,
            Cover = Color.FromArgb("#6E4B2A")
        }
    ];

    public LibraryPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
