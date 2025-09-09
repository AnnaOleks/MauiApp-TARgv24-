namespace MauiApp_TARgv24_;

public partial class TekstPage : ContentPage
{
	Label lblTekst;
	Editor editorTekst;
	HorizontalStackLayout hsl;
    public TekstPage()
	{
		lblTekst = new Label
		{
			Text = "Siia tuleb tekst",
            FontSize = 20,
            FontFamily = "StoryScript-Regular",
            TextColor = Colors.Black,
        };
        editorTekst = new Editor
        {
            FontSize = 20,
            FontFamily = "StoryScript-Regular",
            TextColor = Colors.Black,
            BackgroundColor = Color.FromRgb(200, 200, 100),
            AutoSize = EditorAutoSizeOption.TextChanges,
            Placeholder = "Sisesta siia tekst",
            PlaceholderColor = Colors.Gray,
            FontAttributes = FontAttributes.Italic
        };
        editorTekst.TextChanged += EditorTekst_TextChanged;
        hsl = new HorizontalStackLayout 
        { 
            BackgroundColor = Color.FromRgb(120, 30, 50),
            Children = { lblTekst, editorTekst },
            HorizontalOptions = LayoutOptions.Center
        };
        Content = hsl;

    }

    private void EditorTekst_TextChanged(object? sender, TextChangedEventArgs e)
    {
        lblTekst.Text = editorTekst.Text;
    }
}