namespace MauiApp_TARgv24_;

public partial class TekstPage : ContentPage
{
    Label lblTekst;
    Editor editorTekst;
    HorizontalStackLayout hsl;
    Button btn;
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
        btn = new Button
        {
            Text = "Loe tekst"
        };
        btn.Clicked += Btn_Clicked;
        hsl = new HorizontalStackLayout
        {
            BackgroundColor = Color.FromRgb(120, 30, 50),
            Children = { lblTekst, editorTekst, btn },
            HorizontalOptions = LayoutOptions.Center
        };
        Content = hsl;

    }
    private async void Btn_Clicked(object? sender, EventArgs e)
    {
        IEnumerable<Locale> locales = await TextToSpeech.Default.GetLocalesAsync();

        SpeechOptions options = new SpeechOptions()
        {
            Pitch = 1.5f,   // 0.0 - 2.0
            Volume = 0.75f, // 0.0 - 1.0
            Locale = locales.FirstOrDefault((l => l.Language == "et-EE"))
        };
        var text = editorTekst.Text;
        if (string.IsNullOrWhiteSpace(text))
        {
            await DisplayAlert("Viga", "Palun sisesta tekst", "OK");
            return;
        }
        try
        {
            await TextToSpeech.SpeakAsync(text, options);
        }
        catch (Exception ex)
        {
            await DisplayAlert("TTS viga", ex.Message, "OK");
        }
    }

    private void EditorTekst_TextChanged(object? sender, TextChangedEventArgs e)
    {
        lblTekst.Text = editorTekst.Text;
    }
}