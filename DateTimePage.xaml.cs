using Microsoft.Maui.Layouts;

namespace MauiApp_TARgv24_;

public partial class DateTimePage : ContentPage
{
	Label mis_on_valitud;
	DatePicker datepicker;
	TimePicker timePicker;
	Picker picker;
	AbsoluteLayout al;

	public DateTimePage()
	{
		mis_on_valitud = new Label
		{
			Text = "Siin kuvatakse valitud kuupäev või kellaaeg",
			FontSize = 20,
			TextColor = Colors.Black,
			FontFamily = "Felipa-Regular"
        };

		datepicker = new DatePicker
		{
			FontSize = 20,
			Background = Color.FromRgb(200, 200, 100),
			TextColor = Colors.Black,
			FontFamily = "Felipa-Regular",
			MinimumDate = DateTime.Now.AddDays(-7), //new Datetime (1900,1,1)
			MaximumDate = new DateTime(2100, 12, 31),
			Date = DateTime.Now,
			Format = "D"
		};
        datepicker.DateSelected += Kuupäeva_valimine;

		timePicker = new TimePicker
		{
			FontSize = 20,
			Background = Color.FromRgb(200,200,100),
			TextColor = Colors.Black,
			FontFamily = "",
			Time = new TimeSpan (12,0,0),
			Format = "t"
		};

		timePicker.PropertyChanged += (s, e) =>
		{
			if (e.PropertyName == TimePicker.TimeProperty.PropertyName)
			{
				mis_on_valitud.Text = $"Valisite kellaaja: {timePicker.Time}";
			}
		};

		picker = new Picker
		{
			Title = "Vali uks",
			FontSize = 20,
			Background = Color.FromRgb(200, 200, 100),
			TextColor = Colors.Black,
			FontFamily = "",
			ItemsSource = new List<string> { "Üks", "Kaks", "Kolm", "Neli", "Viis" }
		};
		picker.Items.Add("Kuus");
		picker.SelectedIndexChanged += (s, e) =>
		{
			if (picker.SelectedIndex != -1)
			{
				mis_on_valitud.Text = $"Valisite: {picker.Items[picker.SelectedIndex]}";
			}
		};

		al = new AbsoluteLayout { Children = { mis_on_valitud, datepicker, timePicker } };
		AbsoluteLayout.SetLayoutBounds(mis_on_valitud, new Rect(0.5, 0.2, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
		AbsoluteLayout.SetLayoutFlags(mis_on_valitud, AbsoluteLayoutFlags.All);
        AbsoluteLayout.SetLayoutBounds(datepicker, new Rect(0.5, 0.5, 0.8, 0.2));
        AbsoluteLayout.SetLayoutFlags(datepicker, AbsoluteLayoutFlags.All);
        AbsoluteLayout.SetLayoutBounds(timePicker, new Rect(0.5, 0.7, 0.9, 0.2));
        AbsoluteLayout.SetLayoutFlags(timePicker, AbsoluteLayoutFlags.All);
        Content = al;
    }

    private void Kuupäeva_valimine(object? sender, DateChangedEventArgs e)
    {
		mis_on_valitud.Text = $"Valisite kuupäeva: {e.NewDate:D}";
    }
}