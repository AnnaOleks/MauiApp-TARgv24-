using System;                       // Директива using: подключаем базовое пространство имён .NET (типы вроде String, Int32, EventArgs и т.п.)
using System.ComponentModel;        // using: компоненты/атрибуты (в данном файле напрямую не используем, но допустимо оставлять)
using System.Threading;             // using: для многопоточности и отмены задач (CancellationToken, CancellationTokenSource)
using System.Threading.Tasks;       // using: для асинхронности (Task, async/await)

namespace MauiApp_TARgv24_;         // Объявление пространства имён (логическая «папка» кода). Всё внутри относится к этому проекту/неймспейсу.

public partial class ValgusfoorPage : ContentPage  // Объявление класса страницы MAUI. partial — класс может быть разделён по нескольким файлам. Наследуемся от ContentPage (экран).
{
    public List<string> tekstnupp = new() { "ON", "OFF" };               // Публичное поле: список строк с текстами кнопок. new() — сокращённая инициализация List<string>.
    public List<string> tekstring = new() { "punane", "kollane", "roheline" }; // Публичное поле: подписи для трёх ламп (эстон.: красный/жёлтый/зелёный).

    public List<BoxView> tuled = new();   // Публичный список трёх «кругов»-ламп (BoxView). Индексы: 0=красная, 1=жёлтая, 2=зелёная.

    // --- РЕСУРСЫ КАРТИНОК ДЛЯ ЛАМП ---
    readonly string[] pildifailid = { "stop1.png", "wait.png", "go1.png" }; // Приватный массив имён файлов картинок (в Resources/Images). readonly — нельзя заменить ссылку на массив.
    readonly List<Image> pildid = new();                                   // Приватный список Image — фактические элементы UI для картинок (по одной на лампу).
    readonly List<Label> sildid = new();                                    // Приватный список Label — текстовые подписи внутри кружков (по одной на лампу).

    // --- ПОЛЯ ДЛЯ ЭЛЕМЕНТОВ ИНТЕРФЕЙСА (будем заполнять в конструкторе) ---
    BoxView ringid;                     // Текущая создаваемая лампа-круг (локальная переменная цикла, но поле для удобства).
    BoxView korpus;                     // «Корпус» светофора — большой чёрный BoxView с закруглениями.
    Grid gridring;                      // Внутренняя Grid для каждой лампы: слой круга + слой текста + слой картинки.
    Grid konteiner;                     // Общий контейнер Grid для корпуса и вертикального стека ламп.
    Label labelvarv;                    // Подпись цвета (текст внутри круга).
    VerticalStackLayout vsl;            // Вертикальный стек: три лампы расположены сверху вниз.
    HorizontalStackLayout hsl;          // Горизонтальный стек: в нём кнопки ON/OFF.
    VerticalStackLayout root;           // Корневой вертикальный контейнер страницы (содержит светофор и панель кнопок).
    ScrollView sv;                      // Обёртка-прокрутка на случай маленького экрана.

    enum TuledeStaatus { Off, Red, RedYellow, Green, Yellow } // Перечисление (enum) — возможные состояния светофора.

    // --- ПАЛИТРА ЦВЕТОВ ДЛЯ ЛАМП ---
    readonly Color Hall = Color.FromRgb(155, 166, 166); // «Холостой» серый — как в начальной отрисовке.
    readonly Color RedOn = Colors.Red;                   // Яркий красный (горит).
    readonly Color RedOff = Color.FromRgb(64, 0, 0);      // Приглушённый красный (не горит, но виден).
    readonly Color YellowOn = Colors.Yellow;                // Яркий жёлтый.
    readonly Color YellowOff = Color.FromRgb(64, 64, 0);     // Приглушённый жёлтый.
    readonly Color GreenOn = Colors.LimeGreen;             // Яркий зелёный.
    readonly Color GreenOff = Color.FromRgb(0, 64, 0);      // Приглушённый зелёный.

    // --- ТЕКУЩЕЕ СОСТОЯНИЕ И ПОЛЯ ДЛЯ АСИНХРОННОГО ЦИКЛА ---
    TuledeStaatus status = TuledeStaatus.Off;                // Поле: текущее состояние светофора. Стартуем в Off.
    CancellationTokenSource? katkestus;                      // Источник отмены (может быть null): «рубильник» для остановки цикла.
    bool kaib = false;                                       // Флаг — запущен ли цикл (true/false).

    public ValgusfoorPage()           // Конструктор страницы — собираем интерфейс и задаём начальное состояние.
    {
        Title = "Valgusfoor";          // Заголовок страницы (отображается в навигации/Shell).

        vsl = new VerticalStackLayout   // Создаём вертикальный стек для ламп.
        {
            HorizontalOptions = LayoutOptions.Center, // Выравниваем по центру по горизонтали.
            VerticalOptions = LayoutOptions.Center  // И по центру по вертикали.
        };

        korpus = new BoxView            // Создаём «корпус» светофора — чёрный прямоугольник с округлениями.
        {
            Color = Colors.Black,                       // Цвет заливки — чёрный.
            WidthRequest = 180,                         // Желаемая ширина (MAUI постарается ей соответствовать).
            HeightRequest = 520,                        // Желаемая высота.
            CornerRadius = 90,                          // Радиус скругления углов — даёт форму «пилюли».
            BackgroundColor = Color.FromRgba(0, 0, 0, 0), // Прозрачный фон элемента (на случай наложений).
        };

        for (int i = 0; i < tekstring.Count; i++) // Цикл на 3 итерации — создаём каждый блок лампы (круг+текст+картинка).
        {
            gridring = new Grid            // Внутренняя Grid под одну лампу — позволяет «сложить» круг, текст и картинку слоями.
            {
                WidthRequest = 160,        // Размер «ячейки лампы» — ширина.
                HeightRequest = 160,       // Высота.
                Margin = 5                 // Внешний отступ вокруг лампы.
            };

            ringid = new BoxView           // Сам круг-лампа (BoxView со скруглением делает круг).
            {
                Color = Hall,              // Начальный цвет — нужный серый (как в OFF).
                WidthRequest = 160,        // Круг впишется в квадрат 160x160.
                HeightRequest = 160,
                CornerRadius = 90,         // 90 > половины стороны → получается круг.
                BackgroundColor = Color.FromRgba(0, 0, 0, 0), // Прозрачный фон.
                Margin = 5                 // Небольшой внутренний отступ.
            };

            labelvarv = new Label          // Текстовая метка внутри круга с названием цвета (для OFF и «негорящих»).
            {
                Text = tekstring[i],       // Устанавливаем подпись по индексу: "punane"/"kollane"/"roheline".
                FontSize = 20,             // Размер шрифта.
                FontFamily = "StoryScript-Regular", // Шрифт (если подключён в проект).
                TextColor = Colors.Black,  // Цвет текста.
                HorizontalOptions = LayoutOptions.Center, // Центрируем по горизонтали в Grid.
                VerticalOptions = LayoutOptions.Center,   // Центрируем по вертикали в Grid.
                IsVisible = true           // По умолчанию текст виден (OFF-режим).
            };

            var pilt = new Image           // Элемент Image — картинка поверх круга (показываем только на «горящих» лампах).
            {
                Source = pildifailid[i],   // Файл картинки для этой лампы (stop.png/wait.png/go.png).
                Aspect = Aspect.AspectFit, // Масштабирование по содержимому без обрезки, с сохранением пропорций.
                IsVisible = false,         // По умолчанию картинка скрыта (виден текст).
                HorizontalOptions = LayoutOptions.Center, // Центр по горизонтали.
                VerticalOptions = LayoutOptions.Center,   // Центр по вертикали.
                HeightRequest = 110,       // Размер картинки (чуть меньше круга).
                WidthRequest = 110,
                InputTransparent = true    // Картинка не перехватывает нажатия/жесты (проходят вниз при необходимости).
            };

            gridring.Add(ringid);          // Добавляем в Grid нижний слой — круг.
            gridring.Add(labelvarv);       // Слой с текстом поверх круга.
            gridring.Add(pilt);            // Слой с картинкой поверх текста (но пока скрыта).
            vsl.Add(gridring);             // Кладём готовый блок лампы в вертикальный стек.

            tuled.Add(ringid);             // Сохраняем ссылку на круг (для смены цвета).
            sildid.Add(labelvarv);         // Сохраняем ссылку на подпись (для переключения видимости).
            pildid.Add(pilt);              // Сохраняем ссылку на картинку (для показа/скрытия).
        }

        hsl = new HorizontalStackLayout { HorizontalOptions = LayoutOptions.Center }; // Горизонтальная панель под кнопки, по центру.

        for (int i = 0; i < tekstnupp.Count; i++) // Цикл создания кнопок ON и OFF.
        {
            Button nupp = new Button
            {
                Text = tekstnupp[i],                    // Текст на кнопке (ON/OFF).
                FontSize = 20,                          // Размер шрифта.
                BackgroundColor = Colors.White,         // Цвет фона кнопки.
                TextColor = Color.FromRgb(4, 48, 61),   // Цвет текста (тёмно-бирюзовый).
                CornerRadius = 5,                       // Небольшое скругление.
                BorderColor = Color.FromRgb(37, 186, 199), // Цвет рамки.
                BorderWidth = 2,                        // Толщина рамки.
                Margin = 10,                            // Внешние отступы.
                FontFamily = "StoryScript-Regular",     // Шрифт.
                ZIndex = i,                             // Порядок по оси Z (не критично здесь).
            };
            nupp.WidthRequest = (int)(DeviceDisplay.MainDisplayInfo.Width / 8); // Желаемая ширина ≈ 1/8 ширины экрана (простая адаптация).
            nupp.Clicked += ButtonClicked;             // Подписываемся на событие нажатия: обработчик ButtonClicked.
            hsl.Add(nupp);                              // Добавляем кнопку в горизонтальный контейнер.
        }

        konteiner = new Grid
        {
            HorizontalOptions = LayoutOptions.Center,   // Центр по горизонтали.
            VerticalOptions = LayoutOptions.Center    // Центр по вертикали.
        };
        konteiner.Add(korpus);                          // Сначала кладём корпус (нижний слой).
        konteiner.Add(vsl);                             // Затем стек ламп (верхний слой) — видны «на» корпусе.

        root = new VerticalStackLayout { Children = { konteiner, hsl } }; // Корневой столбец: сверху светофор, снизу кнопки.
        sv = new ScrollView { Content = root, HorizontalOptions = LayoutOptions.Center }; // Прокрутка на случай малой высоты.
        Content = sv;                                   // Назначаем содержимое страницы.

        StaatusePanek(TuledeStaatus.Off);               // Начальное состояние: OFF → серые круги + текст, картинки скрыты.
    }

    // --- ПОМОЩНИК: включить режим «везде текст, без картинок» (используется для OFF) ---
    void TekstReziimKoigil(bool tekstNaha)
    {
        for (int i = 0; i < sildid.Count; i++)          // Проходим все подписи (Label).
        {
            sildid[i].IsVisible = tekstNaha;            // Показываем/скрываем текст (true = показать).
        }
        for (int i = 0; i < pildid.Count; i++)          // Проходим все картинки (Image).
        {
            pildid[i].IsVisible = false;                // Картинки скрываем в любом случае (OFF-режим).
        }
    }

    // --- ПОМОЩНИК: показать картинки только на перечисленных индексах, текст отключить везде ---
    void PildiReziimAinult(params int[] indeksid)
    {
        // Сначала у всех скрываем и текст, и картинки (готовим чистую базу).
        for (int i = 0; i < sildid.Count; i++) sildid[i].IsVisible = false; // В on-режиме текст не нужен.
        for (int i = 0; i < pildid.Count; i++) pildid[i].IsVisible = false; // Спрятать все картинки…

        // …а теперь включить только те картинки, чьи индексы переданы (например 0 — красная, 1 — жёлтая, 2 — зелёная).
        foreach (var idx in indeksid)
            if (idx >= 0 && idx < pildid.Count) pildid[idx].IsVisible = true; // Проверяем границы и включаем нужные.
    }

    // --- ГЛАВНЫЙ МЕТОД: применить визуальное состояние светофора ---
    void StaatusePanek(TuledeStaatus ts)
    {
        status = ts;                              // Запоминаем состояние (может пригодиться для доп. логики).
        if (tuled.Count < 3) return;              // Защита: должно быть ровно 3 круга.

        if (ts == TuledeStaatus.Off)              // OFF — особый режим «как изначально».
        {
            tuled[0].Color = Hall;                // Все три круга серые…
            tuled[1].Color = Hall;
            tuled[2].Color = Hall;
            TekstReziimKoigil(true);              // …и у всех виден текст, картинки скрыты.
            return;                                // Завершаем: OFF обработан.
        }

        // Если НЕ OFF, готовим базу: делаем все три «приглушёнными» (как бы выключены).
        tuled[0].Color = RedOff;                  // Красный — тёмный.
        tuled[1].Color = YellowOff;               // Жёлтый — тёмный.
        tuled[2].Color = GreenOff;                // Зелёный — тёмный.

        // Теперь по текущему состоянию включаем нужные и показываем картинки ТОЛЬКО там, где «горит».
        switch (ts)                               // Инструкция выбора по enum TuledeStaatus.
        {
            case TuledeStaatus.Red:               // Случай: горит ТОЛЬКО красный.
                tuled[0].Color = RedOn;           // Красный круг делаем ярким.
                PildiReziimAinult(0);             // Показываем картинку только на красной лампе (индекс 0).
                break;                             // Выходим из switch.

            case TuledeStaatus.RedYellow:         // Случай: горят красный + жёлтый (переход к зелёному).
                tuled[0].Color = RedOn;           // Красный — яркий.
                tuled[1].Color = YellowOn;        // Жёлтый — яркий.
                PildiReziimAinult(0, 1);          // Показываем 2 картинки: на красной и на жёлтой.
                break;

            case TuledeStaatus.Green:             // Случай: горит ТОЛЬКО зелёный.
                tuled[2].Color = GreenOn;         // Зелёный — яркий.
                PildiReziimAinult(2);             // Показываем картинку только на зелёной лампе (индекс 2).
                break;

            case TuledeStaatus.Yellow:            // Случай: горит ТОЛЬКО жёлтый.
                tuled[1].Color = YellowOn;        // Жёлтый — яркий.
                PildiReziimAinult(1);             // Показываем картинку только на жёлтой лампе (индекс 1).
                break;
        }
    }

    // --- АВТОЦИКЛ: крутит состояния по таймингам, пока не отменят токеном ---
    async Task TsukkelAsync(CancellationToken ct) // async — есть await внутри; возвращает Task. ct — токен отмены.
    {
        while (!ct.IsCancellationRequested)       // Бесконечный цикл, пока не запросили отмену.
        {
            StaatusePanek(TuledeStaatus.Red);     // Красный
            await Task.Delay(3000, ct);           // Ждём 3 секунды (ожидание отменяемо ct).

            StaatusePanek(TuledeStaatus.RedYellow); // Красный+Жёлтый
            await Task.Delay(1000, ct);             // Ждём 1 секунду.

            StaatusePanek(TuledeStaatus.Green);   // Зелёный
            await Task.Delay(3000, ct);           // Ждём 3 секунды.

            StaatusePanek(TuledeStaatus.Yellow);  // Жёлтый
            await Task.Delay(1000, ct);           // Ждём 1 секунду.
        }
    }

    // --- ОБРАБОТЧИК КНОПОК ON/OFF ---
    private async void ButtonClicked(object? sender, EventArgs e) // Сигнатура стандартного обработчика событий. async void допустимо для UI-событий.
    {
        if (sender is not Button nupp) return;     // Защита: если событие пришло не от Button, просто выходим.
        var tekst = (nupp.Text ?? "").Trim().ToUpperInvariant(); // Берём текст кнопки, убираем пробелы, приводим к верхнему регистру (удобно для сравнения).

        if (tekst == "ON")                         // Если нажали ON —
        {
            if (kaib) return;                      // — и цикл уже идёт, то ничего не делаем (защита от двойного старта).
            katkestus = new CancellationTokenSource(); // Создаём источник отмены (будет нужен для остановки).
            kaib = true;                           // Флаг «цикл запущен».

            try
            {
                await TsukkelAsync(katkestus.Token); // Запускаем автоцикл и ждём его (он завершится только по отмене).
            }
            catch (OperationCanceledException)
            {
                // Нормальная отмена ожидания (Task.Delay с ct был прерван) — можно игнорировать.
            }
            finally
            {
                kaib = false;                      // После выхода из цикла сбрасываем флаг.
                StaatusePanek(TuledeStaatus.Off);  // Возвращаем OFF: серые круги + текст, картинки скрыты.
            }
        }
        else if (tekst == "OFF")                   // Если нажали OFF —
        {
            if (kaib)                              // — и цикл шёл,
            {
                katkestus?.Cancel();               // просим отмену через источник (если не null).
                kaib = false;                      // Сразу сбрасываем флаг (даже если отмена прилетит чуть позже).
            }
            StaatusePanek(TuledeStaatus.Off);      // OFF-режим: серые + текст.
        }
        // Прочие тексты игнорируем (в текущей конфигурации их нет).
    }

    protected override void OnDisappearing()       // Переопределение жизненного цикла страницы — вызывается при уходе со страницы.
    {
        base.OnDisappearing();                     // Вызов базовой реализации.
        if (kaib)                                  // Если цикл в этот момент идёт —
        {
            katkestus?.Cancel();                   // — запрашиваем отмену,
            kaib = false;                          // — сбрасываем флаг.
        }
        StaatusePanek(TuledeStaatus.Off);          // И гарантированно возвращаем OFF: серые круги + текст.
    }
}
