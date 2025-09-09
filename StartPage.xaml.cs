namespace MauiApp_TARgv24_;

public partial class StartPage : ContentPage
{
	public List<ContentPage> lehed = new List<ContentPage>() { new TekstPage(), new FigurePage(), new TimerPage(), new ValgusfoorPage() };
	public List<string> tekstid = new List<string> () {"Tee lahti leht tekstiga", "Tee lahti figure leht", "Tee lahti timer leht", "Tee lahti valgusfoor" };
	ScrollView sv;
    VerticalStackLayout vsl;
	public StartPage()
	{
		//InitializeComponent();
		Title = "Avaleht";
		vsl = new VerticalStackLayout { BackgroundColor = Color.FromRgb(4, 48, 61) };
		for (int i = 0; i < lehed.Count; i++)
		{
			Button nupp = new Button
			{
				Text = tekstid[i],
				FontSize = 20,
				BackgroundColor = Colors.White,
				TextColor = Color.FromRgb(4, 48, 61),
				CornerRadius = 5,
				BorderColor= Color.FromRgb(37, 186, 199),
				BorderWidth=2,
				Margin=10,
                FontFamily = "StoryScript-Regular",
				ZIndex=i
			};
			vsl.Add(nupp);
            nupp.Clicked += Nupp_Clicked;
        }
		sv = new ScrollView { Content = vsl };
		Content = sv;
    }

    private async void Nupp_Clicked(object? sender, EventArgs e)
    {
        Button nupp = (Button)sender;
		await Navigation.PushAsync(lehed[nupp.ZIndex]);
    }
}