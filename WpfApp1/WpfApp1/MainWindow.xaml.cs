using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Threading;
using System.IO;
using System.Text.RegularExpressions;

namespace WpfApp1
{

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        
        
        //Деревянный ящик
        //Image woodenbox_obj = new Image { Name = "woodbox", Margin = new Thickness(0, 0, 0, -14) };
        //Игрок
        Canvas player = new Canvas();
        Image player_obj = new Image { Name = "dotnet" };
        //NPC     

        //Кнопка "Начать игру" в главном меню 
        Button play = new Button() { Name = "play", Height=45 };
        Button authors = new Button { Name = "authors", Height = 45 };
        //Счётик боеприпасов для оружия
        Label weapon_table = new Label { Name = "weapon_table" };
        //Панель сообщений
        Label chat = new Label { Name = "chat" };
        //Пуля
        Image bullet_obj = new Image { Name = "bullet" };
        //Оружие в руке
        Image weapon_obj = new Image { Name = "weapon" };
        Label authors_title = new Label { Name = "authors_title" };
        List<Canvas> NPC_mo_models = new List<Canvas>();
        List<Image> s_models = new List<Image>();
        List<Image> NPC_models = new List<Image>();
        List<Weapon> weapons = new List<Weapon>();
        List<Image> NPC_weapons = new List<Image>();
        List<NPC> NPCs = new List<NPC>();
        //public Weapon colt_45 = new Weapon();
        //public Weapon m40a1 = new Weapon();

        

        public MainWindow()
        {
            InitializeComponent();
            
            IniFile config = new IniFile($"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}System.ini");
            player_obj.Height = Convert.ToInt32(config.Read("size", "player").ToString().Split(',')[0]);
            player_obj.Width = Convert.ToInt32(config.Read("size", "player").ToString().Split(',')[1]);       
            Canvas.SetTop(player_obj, Convert.ToInt32(config.Read("position", "player").ToString().Split(',')[0]));
            Canvas.SetLeft(player_obj, Convert.ToInt32(config.Read("position", "player").ToString().Split(',')[1]));
            //его "спавн"
            player.Children.Add(player_obj);
            
            //Оружие в руках игрока
            weapon_obj.Height = Convert.ToInt32(config.Read("size", "weapon_obj").ToString().Split(',')[0]);
            weapon_obj.Width = Convert.ToInt32(config.Read("size", "weapon_obj").ToString().Split(',')[1]);
            //его расположение
            Canvas.SetTop(weapon_obj, Canvas.GetTop(player_obj)+ 65);
            Canvas.SetLeft(weapon_obj, Canvas.GetLeft(player_obj) + 82);

            //его "cпавн"
            player.Children.Add(weapon_obj);

            //Расположение счётика БП
            Grid.SetColumn(weapon_table, Convert.ToInt32(config.Read("cr_position", "weapon_table").ToString().Split(',')[0]));
            Grid.SetRow(weapon_table, Convert.ToInt32(config.Read("cr_position", "weapon_table").ToString().Split(',')[1]));
            weapon_table.Content = $"Health: {Player.health}"; /*config.Read("content", "weapon_table");*/
            weapon_table.FontFamily = new FontFamily(config.Read("font", "weapon_table"));
            weapon_table.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString((config.Read("foreground", "weapon_table")));
            weapon_table.Background = (SolidColorBrush)new BrushConverter().ConvertFromString((config.Read("background", "weapon_table")));

            Canvas.SetTop(bullet_obj, Convert.ToInt32(config.Read("position", "bullet_obj").ToString().Split(',')[0]));
            Canvas.SetLeft(bullet_obj, Convert.ToInt32(config.Read("position", "bullet_obj").ToString().Split(',')[1]));
            bullet_obj.Source = new BitmapImage(new Uri($@"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}/{config.Read("visual", "bullet_obj")}", UriKind.RelativeOrAbsolute));
            //Расположение панели сообщений
            Grid.SetColumnSpan(chat, Convert.ToInt32(config.Read("cr_span", "chat").ToString().Split(',')[0]));
            Grid.SetRowSpan(chat, Convert.ToInt32(config.Read("cr_span", "chat").ToString().Split(',')[1]));
            chat.Content = config.Read("content", "chat");
            chat.FontFamily = new FontFamily(config.Read("font", "chat"));
            //chat.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString((config.Read("foreground", "chat")));
            //chat.Background = (SolidColorBrush)new BrushConverter().ConvertFromString((config.Read("background", "chat")));

            //его появление на экране
            gridok.Children.Add(chat);

            //Расположение деревянного ящика
           
            Player.general = player_obj; //Присваеваем тело игрока (его изображение) к классу Player
            Player.weapon = weapon_obj; //Присваеваем изображение орудия к классу игрока
            Player.weapon_table = weapon_table; //Присваеваем табло счётика БП к игроку (чтоб был виден классу weapon)                      
            //Расположение кнопки "Начать игру" в главном меню
            Grid.SetColumn(play, Convert.ToInt32(config.Read("cr_position", "play").ToString().Split(',')[0]));
            Grid.SetRow(play, Convert.ToInt32(config.Read("cr_position", "play").ToString().Split(',')[1]));
            play.Content = config.Read("content", "play");

            Grid.SetColumn(authors, Convert.ToInt32(config.Read("cr_position", "authors").ToString().Split(',')[0]));
            Grid.SetRow(authors, Convert.ToInt32(config.Read("cr_position", "authors").ToString().Split(',')[1]));
            
            authors.Content = config.Read("content", "authors");
            
            //её создание
            gridok.Children.Add(play);
            gridok.Children.Add(authors);
            play.Click += Play_Click; //Событие клика по ней
            authors.Click += Authors_Click;
            this.KeyDown += Dotnet_KeyDown; //Присваевыем управление игре

            int[] counts = new int[3] { 0, 0, 0 };
             
            LoadConfig("user.cfg");
        
 using (StreamReader sr = new StreamReader($"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}System.ini", System.Text.Encoding.Default))
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    var match = Regex.Matches(line, @"\[(.+?)\]")
                                        .Cast<Match>()
                                        .Select(m => m.Groups[1].Value)
                                        .ToList();

                    if (line.Contains("["))
                    {
                        string Class = config.Read("class", match[0].ToString());
                        
                        switch (Class)
                        {
                            
                            case "WEAPON":
                                BitmapImage[] weapon_skin = new BitmapImage[config.Read("skin", match[0].ToString()).Split(',').Length];
                                for (int i = 0; i < weapon_skin.Length; i++)
                                {
                                    weapon_skin[i] = new BitmapImage(new Uri($@"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}{config.Read("skin", match[0].ToString()).Split(',')[i]}"));
                                }
                                weapons.Add(new Weapon()
                                {
                                    Name = match[0].ToString(),
                                    cartridge_name = config.Read("cartridge_name", match[0].ToString()),
                                    Skin = weapon_skin,
                                    sound = new Uri($@"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}{config.Read("sound", match[0].ToString())}"),
                                    in_magazine = Convert.ToInt32(config.Read("in_magazine", match[0].ToString())),
                                    in_magazine_count = Convert.ToInt32(config.Read("in_magazine_count", match[0].ToString())),
                                    cartridge_count = Convert.ToInt32(config.Read("cartridge_count", match[0].ToString())),
                                    damage = Convert.ToInt32(config.Read("damage", match[0].ToString()))
                                    
                                });
                                break;
                            case "OBJECT":
                                s_models.Add(new Image()
                                {
                                    Name = match[0].ToString(),
                                    Source = new BitmapImage(new Uri($@"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}/{config.Read("visual", match[0].ToString())}")),

                                });
                                Grid.SetColumn(s_models[counts[1]], Convert.ToInt32(config.Read("cr_position", match[0].ToString()).ToString().Split(',')[0]));
                                Grid.SetColumnSpan(s_models[counts[1]], Convert.ToInt32(config.Read("cr_span", match[0].ToString()).ToString().Split(',')[0]));
                                Grid.SetRow(s_models[counts[1]], Convert.ToInt32(config.Read("cr_position", match[0].ToString()).ToString().Split(',')[1]));
                                Grid.SetRowSpan(s_models[counts[1]], Convert.ToInt32(config.Read("cr_span", match[0].ToString()).ToString().Split(',')[1]));
                                counts[1] += 1;
                                break;
                            //case "D_OBJECT":
                            //    items.Add(new Image()
                            //    {
                            //        Name = match[0].ToString(),
                            //        Source = new BitmapImage(new Uri($@"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}/{config.Read("visual", match[0].ToString())}")),
                            //        Height = Convert.ToInt32(config.Read("size", match[0].ToString()).ToString().Split(',')[0]),
                            //        Width = Convert.ToInt32(config.Read("size", match[0].ToString()).ToString().Split(',')[1])



                            //    });
                            //    Canvas.SetTop(items.Find(x => x.Name == match[0].ToString()), Convert.ToInt32(config.Read("position", match[0].ToString()).ToString().Split(',')[0]));
                            //    Canvas.SetLeft(items.Find(x => x.Name == match[0].ToString()), Convert.ToInt32(config.Read("position", match[0].ToString()).ToString().Split(',')[1]));
                            //    d_items.Add(new Canvas() { Name = match[0].ToString() });
                            //    break;
                            case "NPC":

                                NPC_models.Add(new Image()
                                {
                                    Name = match[0].ToString(),
                                    Source = new BitmapImage(new Uri($@"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}/{config.Read("visual", match[0].ToString())}")),
                                    Height = Convert.ToInt32(config.Read("size", match[0].ToString()).ToString().Split(',')[0]),
                                    Width = Convert.ToInt32(config.Read("size", match[0].ToString()).ToString().Split(',')[1])

                                });

                                NPC_mo_models.Add(new Canvas());
                                NPC_mo_models[counts[0]].Children.Add(NPC_models[counts[0]]);
                                Canvas.SetTop(NPC_models[counts[0]], Convert.ToInt32(config.Read("position", match[0].ToString()).ToString().Split(',')[0]));
                                Canvas.SetLeft(NPC_models[counts[0]], Convert.ToInt32(config.Read("position", match[0].ToString()).ToString().Split(',')[1]));
                                NPCs.Add(new NPC() {
                                    chat = chat,
                                    general = NPC_models[counts[0]],
                                    mo_general = NPC_mo_models[counts[0]],
                                    player_weapon = weapons[Convert.ToInt32(config.Read("weapon", match[0].ToString()))],
                                    health = Convert.ToInt32(config.Read("health", match[0].ToString())),
                                    alive = Convert.ToBoolean(config.Read("alive", match[0].ToString()))
                                });

                                counts[0] += 1;
                                break;
                            case "NPC_WEAPON":

                                NPC_weapons.Add(new Image()
                                {
                                    Name = match[0].ToString(),
                                    //Source = new BitmapImage(new Uri($@"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}/{config.Read("visual", match[0].ToString())}")),
                                    Height = Convert.ToInt32(config.Read("size", match[0].ToString()).ToString().Split(',')[0]),
                                    Width = Convert.ToInt32(config.Read("size", match[0].ToString()).ToString().Split(',')[1])

                                });
                                NPCs[Convert.ToInt32(config.Read("owner", match[0].ToString()))].mo_general.Children.Add(NPC_weapons[counts[2]]);
                                Canvas.SetTop(NPC_weapons[counts[2]], Canvas.GetTop(NPCs[Convert.ToInt32(config.Read("owner", match[0].ToString()))].general)+65);
                                Canvas.SetLeft(NPC_weapons[counts[2]], Canvas.GetLeft(NPCs[Convert.ToInt32(config.Read("owner", match[0].ToString()))].general)+82);
                                NPCs[Convert.ToInt32(config.Read("owner", match[0].ToString()))].weapon = NPC_weapons[counts[2]];
                                counts[2] += 1;
                                break;
                             
                        }
                    }
                    
                    }
                    

                }
            
        }       
       

        string location = null; 
        int count = 0; //Кол-во хода
        int jump_count = 0; //Кол-во (высота) прыжка
        bool space_pressed = false; //Зажата ли Space
        //bool shift_pressed = false;

       
        TextBox console = new TextBox(); //Консолька
        Button console_button = new Button(); //Кнопка исполнения команды консоли

        //weapon

        //public async void Pause_Click(object sender, RoutedEventArgs e)
        //{
        //    weapon_table.Opacity = 1;
        //    Player.general.Opacity = 1;
        //    Player.weapon.Opacity = 1;          
        //    foreach (var i in NPCs)
        //    {
        //        i.general.Opacity = 1;
        //        i.weapon.Opacity = 1;
        //    }
        //    foreach (var i in s_models)
        //    {
        //        i.Opacity = 1;
        //    }
        //    gridok.Background = new ImageBrush(new BitmapImage(new Uri($@"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}{location}.png"))); //Меняем бэкграунд на игровой фон 
        //    gridok.Children.Remove(play);
        //    this.KeyDown += Dotnet_KeyDown;
        //    count -= 5;


        //}

            public async void Play_Click(object sender, RoutedEventArgs e)
        {
            gridok.Background = new ImageBrush(new BitmapImage(new Uri($@"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}level.png"))); //Меняем бэкграунд на игровой фон

            //Спавним игрока           
            gridok.Children.Add(player);
            
            gridok.Children.Remove(play);
            gridok.Children.Remove(authors);//Удаляем кнопку
            gridok.Children.Add(weapon_table); //Отоброжаем счётик БП    
            location = "level";
            
        }
        public async void Authors_Click(object sender, RoutedEventArgs e)
        {
            location = "authors";
            gridok.Children.Remove(play);
            gridok.Children.Remove(authors);
            string[,] titles = new string[3, 2] { { "Программирование: ","Артём Лазарев" }, { "Графика: ", "Артём Лазарев" }, { "Идея: ", "Артём Лазарев" } };
            gridok.Background = new ImageBrush(new BitmapImage(new Uri($@"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}level_2.png"))); //Меняем бэкграунд на игровой фон
            Grid.SetColumn(authors_title, 1);
            Grid.SetRow(authors_title, 3);
            Grid.SetColumnSpan(authors_title, 3);
            Grid.SetRowSpan(authors_title, 2);
            gridok.Children.Add(authors_title);
            for (int i = 0; i < 3; i++)
            {
                
                await Task.Delay(1500);
                authors_title.Content = titles[i, 0]+"\n\n"+ titles[i, 1];
                for (double j = 1; j > 0; j-=0.1)
                {
                    await Task.Delay(500);
                    authors_title.Opacity = j;
                    
                }

            }
                
            

        }
        public void LoadConfig(string path)
        {
            using (StreamReader sr = new StreamReader($"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}{path}", System.Text.Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    console.Text = line;
                    PlayCommand_Click(null, null);

                }
            }
        }

        public async void PlayCommand_Click(object sender, RoutedEventArgs e)
        {
            chat.Content += "\n" + console.Text; //Добовляем сообщение на панель сообщение с консоли
            string[] user_command = console.Text.Split();
            switch (user_command[0])
            {
                                 
                case "clear": //Очищаем панель сообщений
                    chat.ClearValue(ContentProperty);
                
                    break;
                case "exit":
                    this.Close();
                    break;
                case "start":
                    Play_Click(null, null);
                    break;
                case "help":
                    chat.Content += "\nstart - Начать игру\nexit - Выйти\nclear - очистить консоль";
                        break;
                case "location":
                    chat.Content += $"\nLocation: {location}"; //Сообщение о смене локации
                    break;
                case "fullscreen":
                    switch (user_command[1])
                    {
                        case "on":
                            this.WindowStyle = WindowStyle.None;
                            this.WindowState = WindowState.Maximized;
                            break;
                        case "off":
                            this.WindowStyle = WindowStyle.SingleBorderWindow;
                            this.WindowState = WindowState.Normal;
                            break;
                    }  
                     
                    break;
                
                case "spawn":
                    switch (user_command[1])
                    {
                        case "s_models":
                            gridok.Children.Add(s_models[Int32.Parse(user_command[2])]);
                            break;
                        case "NPCs":
                            gridok.Children.Add(NPCs[Int32.Parse(user_command[2])].mo_general);
                            NPCs[Int32.Parse(user_command[2])].weapon.Source = NPCs[Int32.Parse(user_command[2])].player_weapon.Skin[0];
                            NPCs[Int32.Parse(user_command[2])].Flip(-1);                          
                            break;
                    }
                    break;
                case "remove":
                    switch (user_command[1])
                    {
                        case "s_models":
                            gridok.Children.Remove(s_models[Int32.Parse(user_command[2])]);
                            break;
                        case "NPCs":
                            gridok.Children.Remove(NPCs[Int32.Parse(user_command[2])].mo_general);
                            break;
                    }
                    break;
                case "background":
                    gridok.Background = new ImageBrush(new BitmapImage(new Uri($@"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}{user_command[1]}")));
                    break;
                case "tp":
                    switch (user_command[1])
                    {
                        case "s_models":
                            Grid.SetColumn(s_models[Int32.Parse(user_command[2])], Int32.Parse(user_command[3]));
                            Grid.SetRow(s_models[Int32.Parse(user_command[2])], Int32.Parse(user_command[4]));
                            break;
                        case "NPCs":
                            Canvas.SetTop(NPCs[Int32.Parse(user_command[2])].general, Int32.Parse(user_command[3]));
                            Canvas.SetLeft(NPCs[Int32.Parse(user_command[2])].general, Int32.Parse(user_command[4]));
                            break;
                        case "Player":
                            Canvas.SetTop(Player.general, Int32.Parse(user_command[2]));
                            Canvas.SetLeft(Player.general, Int32.Parse(user_command[3]));
                            count += Int32.Parse(user_command[3]);
                            break;
                    }
                    break;
                case "slot_0":
                    Player.weapons[0] = weapons[Int32.Parse(user_command[1])];                    
                    break;
                case "slot_1":
                    Player.weapons[1] = weapons[Int32.Parse(user_command[1])];
                    break;
                case "hp":
                    chat.Content += $"\n{Player.health}";
                    break;
                case "hud_draw":
                    switch (user_command[1])
                    {
                        case "on":
                            if(!gridok.Children.Contains(weapon_table))
                            gridok.Children.Add(weapon_table);
                            break;
                        case "off":
                            gridok.Children.Remove(weapon_table);
                            break;
                    }
                    break;

                    // вызываем функцию и получаем результат                  

            }
            console.Clear();
            gridok.Children.Remove(console);
            gridok.Children.Remove(console_button);
        }

        public async void Dotnet_KeyDown(object sender, KeyEventArgs e)
        {
            //Будем поворачивать игрока с его орудием, когда будет нужно
            ScaleTransform flipTrans = new ScaleTransform();         
            Player.general.RenderTransformOrigin = new Point(0.5, 0.5);
            Player.weapon.RenderTransformOrigin = new Point(0.5, 0.5);         
            Player.general.RenderTransform = flipTrans;
            Player.weapon.RenderTransform = flipTrans;           
            flipTrans.ScaleX = Player.player_flip;          


            switch (e.Key)
            {
                case Key.D: //Движение направо
                    //Поворачиваем игрока направо
                    flipTrans.ScaleX = 1;
                    Player.player_flip = 1;
                    //RotateTransform rotate = new RotateTransform(70);
                    //Player.weapon.RenderTransform = rotate;
                    Player.Walking(); //Запуск анимации хождения
                    count += Player.walk_speed; //Приписываем кол-ву хода скорсть хождения игрока
                    Canvas.SetLeft(Player.weapon, count + 82); //Движение орудия
                    Canvas.SetLeft(Player.general, count); //Движение игрока                  
                    
                  
                    //Cмена локаций
                    if (Canvas.GetLeft(Player.general) == 650 && location != "level_2")
                    { //Переход на вторую локацию с "бункером"

                        location = "level_2"; //Меняем локацию    

                        LoadConfig($"maps/{location}.cfg");
                        
                        count = 0; //Обнуляем кол-во хода
                        if (!NPCs[0].alive)
                        {
                            RotateTransform rotate = new RotateTransform(90);
                            NPCs[0].general.RenderTransform = rotate;
                            NPCs[0].walk_speed = 0;
                        }
                        NPCs[0].player_weapon.FireNPC(NPCs[0]);
                        for (int i = 0; i < 17; i++)
                        {
                            
                            await Task.Delay(500);
                            NPCs[0].player_weapon.FireNPC(NPCs[0]);
                           
                        }
                        //NPCs[0].Walk(-1, 28);
                        //await Task.Delay(1000);
                        //NPCs[0].Walk(1, 28);



                        //flipTrans.ScaleX = -1;
                        //NPCs[1].player_flip = -1;
                        //Пересоздаём табло
                        gridok.Children.Remove(weapon_table);
                        gridok.Children.Add(weapon_table);
                        //NPCs[0].Say("Здраствуй, брат!\nЕсли ты хочешь\nвыжить в этой зоне\nтебе необходима\nбоевая подготовка\nНу как берёшся?");
                        //while (true)
                        //{
                        //    await Task.Delay(1);
                        //    if (chat.Content.ToString().Contains("да") || chat.Content.ToString().Contains("OK"))
                        //    {
                        //        NPCs[1].Say("Тогда держи ствол и стрельни\nв этот ящик");                               
                        //        Player.weapons[1] = weapons[0];
                        //        Player.weapons[0] = weapons[2];
                        //        Grid.SetRow(s_models[0], 1);
                        //        break;

                        //    };
                        //} 

                    }
                    break;

                case Key.A: //Движение налево

                    flipTrans.ScaleX = -1;
                    Player.player_flip = -1;
                    Player.Walking();
                    count -= Player.walk_speed;

                    Canvas.SetLeft(Player.general, count);

                    Canvas.SetLeft(Player.weapon, count - 41);
                    if (Canvas.GetLeft(Player.general) == -5 && location != "level")
                    { //Переход обратно
                        location = "level";
                        LoadConfig($"maps/{location}.cfg");
                        gridok.Background = gridok.Background = new ImageBrush(new BitmapImage(new Uri($@"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}level.png")));
                        console.Text += "clear";
                        PlayCommand_Click(null, null);
                        
                        


                    }                   
                    break;

                case Key.Space: //Прыжок
                    flipTrans.ScaleX = Player.player_flip;

                    if (space_pressed == false)
                    {
                        space_pressed = true;
                        Canvas.SetTop(Player.general, jump_count);

                        Canvas.SetTop(Player.weapon, jump_count + 194);
                        Canvas.SetTop(bullet_obj, jump_count + 194);
                        jump_count += 50;
                        Canvas.SetTop(Player.general, jump_count);

                        Canvas.SetTop(Player.weapon, jump_count + 194);
                        Canvas.SetTop(bullet_obj, jump_count + 194);

                        for (int i = 0; i < 40; i++) //Анимация падения
                        {
                            await Task.Delay(1);
                            jump_count += 2;
                            Canvas.SetTop(Player.general, jump_count);

                            Canvas.SetTop(Player.weapon, jump_count + 70);
                            Canvas.SetTop(bullet_obj, jump_count + 70);
                        }

                        jump_count = 0;
                        space_pressed = false;
                    }

                    //x_coords.Content += "\n" + Canvas.GetTop(Player.general);

                    break;

                case Key.D2: //Клавиша "2" (переключение на пистолет)
                    flipTrans.ScaleX = Player.player_flip;                   
                    
                    Player.player_weapon = Player.weapons[1]; //Оружие игрока - пистолет
                    Player.weapon.Source = Player.player_weapon.Skin[0]; //его изображение                  
                    //Отображение счётика БП для данного орудия
                    weapon_table.Content = $"Health: {Player.health}\n{Player.player_weapon.cartridge_name}\n--------------\n{Player.player_weapon.in_magazine_count} / {Player.player_weapon.cartridge_count}";
                    if (Player.weapon.Source == null)
                    {
                        weapon_table.Content = $"Health: {Player.health}";
                    }
                    break;
                case Key.D1:  //Клавиша "2" (переключение на винтовку)
                    flipTrans.ScaleX = Player.player_flip;
                    Player.player_weapon = Player.weapons[0];
                    Player.weapon.Source = Player.player_weapon.Skin[0];                 
                    weapon_table.Content = $"Health: {Player.health}\n{Player.player_weapon.cartridge_name}\n--------------\n{Player.player_weapon.in_magazine_count} / {Player.player_weapon.cartridge_count}";
                    if (Player.weapon.Source == null)
                    {
                        weapon_table.Content = $"Health: {Player.health}";
                    }
                    break;
                case Key.Back: //Клавиша "Backspace" (оружие убрал!)
                    flipTrans.ScaleX = Player.player_flip;
                    Player.weapon.Source = null;                   
                    weapon_table.Content = $"Health: {Player.health}";
                    break;

                case Key.Enter:
                    //int side = 0;
                    flipTrans.ScaleX = Player.player_flip;
                    if (Player.weapon.Source != null) //Только если есть волына
                    {
                        Player.player_weapon.Fire(); //Огонь!
                        //Если кончились патроны стрелять не должно
                        if (Player.player_weapon.in_magazine_count != 0 && Player.player_weapon.in_magazine_count < Player.player_weapon.in_magazine) 
                        {
                            foreach (var i in NPCs)
                            {
                                if (Canvas.GetLeft(i.general) > Canvas.GetLeft(Player.general) && Player.player_flip == 1 || Canvas.GetLeft(i.general) < Canvas.GetLeft(Player.general) && Player.player_flip == -1)
                                {
                                    i.health -= Player.player_weapon.damage;
                                    if(i.health <= 0)
                                    {
                                        RotateTransform rotate = new RotateTransform(90);
                                        i.general.RenderTransform = rotate;
                                        i.walk_speed = 0;
                                        i.alive = false;
                                    }
                                }
                            }
                            //try { 
                            //player.Children.Add(bullet_obj); //Спавн маслины     
                            
                            //    for (int i = 0; i < 5000; i += 500) //её полёт (пока только в одну сторону)
                            //    {
                            //        //if(i > 1000)
                            //        //{
                            //        //    gridok.Children.Remove(woodenbox_obj);
                            //        //    NPCs[1].Say("Ого! Держи патроны");                                  
                            //        //    weapons[0].cartridge_count += 20;
                            //        //    break;
                            //        //}
                            //        await Task.Delay(1); //Прошла типа одна секунда                         
                            //        Canvas.SetLeft(bullet_obj, count + 82 + i); //И быстро полетела (кто не поймал - я не виноват)                                  
                            //    }
                            
                            //player.Children.Remove(bullet_obj); //Приземлилась
                            //}
                            //catch { }
                        }                                              
                    }
                    break;
                case Key.OemTilde:
                    gridok.Children.Add(console); //Отображаем консольку
                    gridok.Children.Add(console_button); //Кнопку исполнения
                    Grid.SetColumnSpan(console, 5); //Расположение консоли
                    Grid.SetColumn(console_button, 6); //Расположение её кнопки
                    console_button.Content = "Play"; //Играть!
                    console_button.Click += PlayCommand_Click; //Подписываем на исполнение
                    console.Background = Brushes.Black; //Делаем брутально-чёрной
                    console.FontFamily = Fonts.SystemFontFamilies.First(x => x.ToString() == "Consolas"); //Харатерно консольный шрифт
                    console.Foreground = Brushes.White; 
                    console.Focus();  //Cтавим её на фокус                 
                    break;
                case Key.Escape:
                    if (location == "authors")
                    {
                        gridok.Children.Remove(authors_title);
                        gridok.Children.Add(play);
                        gridok.Children.Add(authors);
                        gridok.Background = new ImageBrush(new BitmapImage(new Uri($@"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}Series.jpg")));
                        break;

                    }
                    //if(location != "authors" || location != null)
                    //{
                    //    weapon_table.Opacity = 0;
                    //    Player.general.Opacity = 0;
                    //    Player.weapon.Opacity = 0;
                    //    foreach (var i in NPCs)
                    //    {
                    //        i.general.Opacity = 0;
                    //        i.weapon.Opacity = 0;
                    //    }
                    //    foreach (var i in s_models)
                    //    {
                    //        i.Opacity = 0;
                    //    }
                    //    gridok.Background = new ImageBrush(new BitmapImage(new Uri($@"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}Series.jpg"))); //Меняем бэкграунд на игровой фон                      
                    //    gridok.Children.Add(play);
                    //    play.Content = "Продолжить";
                    //    play.Click -= Play_Click;
                    //    play.Click += Pause_Click;
                    //    this.KeyDown -= Dotnet_KeyDown;
                    //}
                    break;
            }
        }
    }
       class Player 
    {
        public static List<Weapon> weapons = new List<Weapon>() { new Weapon() { Skin = new BitmapImage[1] { null } }, new Weapon() { Skin = new BitmapImage[1] { null } } };
        public static Image general; //Тело игрока
        public static int health = 150;
        public static bool alive = true;
        public static Image weapon; //его орудие (изображение)
        public static Weapon player_weapon; //Оружие игрока
        public static Label weapon_table { get; set; } //Счётик БП
        public static int player_flip = 0; //Сторона в которую повёрнут игрок
        public static int walk_speed = 5; //Скорость хождения
        
        public static async void  Walking() //Анимация хождения (смена картинок через определёное время)
        {
            
            general.Source = new BitmapImage(new Uri($@"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}/player_skin/player_walking/player_coat_2.png")); 
            await Task.Delay(20);
            general.Source = new BitmapImage(new Uri($@"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}/{new IniFile($"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}System.ini").Read("visual", "player")}", UriKind.RelativeOrAbsolute));
            await Task.Delay(5);
            general.Source = new BitmapImage(new Uri($@"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}/player_skin/player_walking/player_coat_3.png"));
            await Task.Delay(20);
            general.Source = new BitmapImage(new Uri($@"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}/{new IniFile($"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}System.ini").Read("visual", "player")}", UriKind.RelativeOrAbsolute));
        }        
       
    }
    class NPC : Player
    {
        public Canvas mo_general;
        public Image general;
        public int health;
        public bool alive;
        public Image weapon;
        public Weapon player_weapon;
        public Label chat;
        public int player_flip;
        public int walk_speed = 5;       

        public void Say(string text)
        {
            chat.Content += "\n" + text;
            
        }
        public async void Walking() //Анимация хождения (смена картинок через определёное время)
        {

            general.Source = new BitmapImage(new Uri("pack://application:,,,/player_skin/player_walking/player_armored_walking_1.png"));
            await Task.Delay(20);
            general.Source = new BitmapImage(new Uri($@"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}/{new IniFile($"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}System.ini").Read("visual", "bot")}", UriKind.RelativeOrAbsolute));
            await Task.Delay(5);
            general.Source = new BitmapImage(new Uri("pack://application:,,,/player_skin/player_walking/player_armored_walking_2.png"));
            await Task.Delay(20);
            general.Source = new BitmapImage(new Uri($@"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}/{new IniFile($"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}System.ini").Read("visual", "bot")}", UriKind.RelativeOrAbsolute));
        }
        public void Flip(int side)
        {
            //Будем поворачивать игрока с его орудием, когда будет нужно
            ScaleTransform flipTrans = new ScaleTransform();
            general.RenderTransformOrigin = new Point(0.5, 0.5);
            weapon.RenderTransformOrigin = new Point(0.5, 0.5);
            general.RenderTransform = flipTrans;
            weapon.RenderTransform = flipTrans;
            flipTrans.ScaleX = player_flip;
            flipTrans.ScaleX = side;
            player_flip = side;
            switch (side)
            {
                case -1:
                    Canvas.SetLeft(weapon, Canvas.GetLeft(general) - 41);
                    break;
                case 1:
                    Canvas.SetLeft(weapon, Canvas.GetLeft(general) + 82);
                    break;
            }
        }
        public async void Walk(int side, int count)
        {
            double count_npc = Canvas.GetLeft(general); ;                         
            Flip(side);
            switch (side)
            {
                case 1:
                    for (int i = 0; i < count; i++)
                    {
                        await Task.Delay(25);
                        Walking(); //Запуск анимации хождения
                        count_npc += walk_speed; //Приписываем кол-ву хода скорсть хождения игрока
                        Canvas.SetLeft(weapon, count_npc + 82); //Движение орудия
                        Canvas.SetLeft(general, count_npc); //Движение игрока    
                    }
                    break;
                case -1:
                    for (int i = 0; i < count; i++)
                    {
                        await Task.Delay(25);
                        Walking(); //Запуск анимации хождения
                        count_npc -= walk_speed; //Приписываем кол-ву хода скорсть хождения игрока
                        Canvas.SetLeft(weapon, count_npc - 41); //Движение орудия
                        Canvas.SetLeft(general, count_npc); //Движение игрока    
                    }
                    break;
            }
            
            
        }

        }
     class Weapon
    {
        public string Name; //Название оружия
        public BitmapImage[] Skin; //Его скин
        public Uri sound; //Его звук
        public int in_magazine;
        public int in_magazine_count;
        public string cartridge_name;
        public int cartridge_count;
        public int damage;
        public void Fire() //Огонь из оружия
        {           
            if (Player.weapon.Source != null)
            {
                if (in_magazine_count == 0 && cartridge_count == 0)
                {
                    MediaPlayer sp = new MediaPlayer();
                    sp.Open(new Uri($@"{System.IO.Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}weapons\sounds\block.ogg"));
                    sp.Play();
                }
                if (in_magazine_count == 0)
                {
                    if (cartridge_count != 0)
                    {
                        cartridge_count -= in_magazine;
                        in_magazine_count += in_magazine;
                    }
                    Player.weapon_table.Content = $"Health: {Player.health}\n{cartridge_name}\n--------------\n{in_magazine_count} / {cartridge_count}";
                }
                else
                {
                    in_magazine_count -= 1;
                    Player.weapon_table.Content = $"Health: {Player.health}\n{cartridge_name}\n--------------\n{in_magazine_count} / {cartridge_count}";
                    Fire_animation();
                    MediaPlayer sp = new MediaPlayer();
                    sp.Open(sound);
                    sp.Play();
                }
            }
        }

        public void FireNPC(NPC npc) //Огонь из оружия
        {
            if (npc.weapon.Source != null)
            {
                if (in_magazine_count == 0 && cartridge_count == 0)
                {
                    MediaPlayer sp = new MediaPlayer();
                    sp.Open(new Uri($@"{System.IO.Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}weapons\sounds\block.ogg"));
                    sp.Play();
                }
                if (in_magazine_count == 0)
                {
                    if (cartridge_count != 0)
                    {
                        cartridge_count -= in_magazine;
                        in_magazine_count += in_magazine;
                    }                  
                }
                else
                {
                    in_magazine_count -= 1;
                    FireNPC_animation(npc);
                    MediaPlayer sp = new MediaPlayer();
                    sp.Open(sound);
                    sp.Play();
                    if (Canvas.GetLeft(Player.general) > Canvas.GetLeft(npc.general) && npc.player_flip == 1 || Canvas.GetLeft(Player.general) < Canvas.GetLeft(npc.general) && npc.player_flip == -1)
                    {
                        Player.health -= npc.player_weapon.damage;
                        Player.weapon_table.Content = $"Health: {Player.health}\n{cartridge_name}\n--------------\n{in_magazine_count} / {cartridge_count}";
                        Fire_animation();
                        if (Player.health <= 0)
                        {
                            RotateTransform rotate = new RotateTransform(90);
                            Player.general.RenderTransform = rotate;
                            Player.walk_speed = 0;
                            Player.alive = false;

                        }
                    }
                }
            }
        }
        public async void FireNPC_animation(NPC npc) //Анимация огня
        {
            foreach (var item in Skin)
            {
                npc.weapon.Source = item;
                await Task.Delay(50);
            }
            npc.weapon.Source = Skin[0];
        }
        public async void Fire_animation() //Анимация огня
        {
            foreach (var item in Skin)
            {
                Player.weapon.Source = item;
                await Task.Delay(50);                
            }
            Player.weapon.Source = Skin[0];
        }
    }
   
}
        


  

