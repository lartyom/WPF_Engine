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


namespace WpfApp1
{

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        NPC bot = new NPC();
        //Деревянный ящик
        Image woodenbox_obj = new Image { Name = "woodbox", Source = new BitmapImage(new Uri(@"pack://application:,,,/objects/wooden_box.png")), Margin = new Thickness(0, 0, 0, -14) };
        //Игрок
        Canvas player = new Canvas();
       
        Canvas player_npc = new Canvas();
        //Тело NPC
        Image player_npc_obj = new Image { Name = "bot", Source = new BitmapImage(new Uri(@"pack://application:,,,/player_skin/player_armored.png")), Height = 283, Width = 122 };
        //Кнопка "Начать игру" в главном меню 
        Button play = new Button() { Name = "play", Content = "Начать игру" };
        //Счётик боеприпасов для оружия
        Label weapon_table = new Label { Name = "weapon_table", Content = "", Foreground = Brushes.White, Background = Brushes.Black, FontFamily = new FontFamily("Consolas") };
        //Панель сообщений
        Label chat = new Label { Name = "chat", Content = "",  FontFamily = new FontFamily("Consolas") };
        //Пуля
        Image bullet_obj = new Image { Name = "bullet", Height = 73, Width = 78, Source = new BitmapImage(new Uri(@"pack://application:,,,/weapons/bullet.png")) };

       

        public MainWindow()
        {
            InitializeComponent();
           
           
            
            //Расположение игрока
            Grid.SetColumnSpan(player, 2);
            Grid.SetRowSpan(player, 3);
            //Тело игрока
            Image player_obj = new Image { Name = "dotnet", Source = new BitmapImage(new Uri(@"pack://application:,,,/player_skin/pixil-frame-0.png")), Height = 283, Width = 122 };          

            //его расположение
            Canvas.SetTop(player_obj, 129);
            Grid.SetColumnSpan(player_obj, 2);
            Grid.SetRowSpan(player_obj, 3);

            Canvas.SetTop(player_npc_obj, 129);
            Canvas.SetLeft(player_npc_obj, 660);
            Grid.SetColumnSpan(player_npc_obj, 2);
            Grid.SetRowSpan(player_obj, 3);
            
            //его "спавн"
            player.Children.Add(player_obj);
            
            player_npc.Children.Add(player_npc_obj);


            //Оружие в руках игрока
            Image weapon_obj = new Image { Name = "weapon", Height = 73, Width = 78 };
            //его расположение
            Canvas.SetTop(weapon_obj, 194);
            Canvas.SetLeft(weapon_obj, 82);
            Grid.SetColumnSpan(weapon_obj, 2);
            Grid.SetRowSpan(weapon_obj, 3);
            //его "cпавн"
            player.Children.Add(weapon_obj);

            //Расположение счётика БП
            Grid.SetColumn(weapon_table, 5);
            Grid.SetRow(weapon_table, 4);

            Canvas.SetTop(bullet_obj, 194);
            Canvas.SetLeft(bullet_obj, 82);
            Grid.SetColumnSpan(bullet_obj, 2);
            Grid.SetRowSpan(bullet_obj, 3);

            //Расположение панели сообщений
            Grid.SetColumnSpan(chat, 2);
            Grid.SetRowSpan(chat, 6);
            //его появление на экране
            gridok.Children.Add(chat);
                              
            //Расположение деревянного ящика
            Grid.SetColumn(woodenbox_obj, 3);
            Grid.SetColumnSpan(woodenbox_obj, 1);
            Grid.SetRow(woodenbox_obj, 4);
            Grid.SetRowSpan(woodenbox_obj, 4);
                      
            Player.general = player_obj; //Присваеваем тело игрока (его изображение) к классу Player
            Player.weapon = weapon_obj; //Присваеваем изображение орудия к классу игрока
            Player.weapon_table = weapon_table; //Присваеваем табло счётика БП к игроку (чтоб был виден классу weapon)
           
            
            bot.general = player_npc_obj;
            bot.chat = chat;
           

            //Расположение кнопки "Начать игру" в главном меню
            Grid.SetColumn(play, 1);
            Grid.SetRow(play, 4);
            //её создание
            gridok.Children.Add(play);         
            play.Click += Play_Click; //Событие клика по ней

            //Пистолет (Кольт M1911) 
            colt_45.Name = "Colt .45"; //Название оружия
            colt_45.cartridge_name = ".45 ACP"; //калибр боеприпасов к нему
            //Скин оружия
            colt_45.Skin = new BitmapImage[3] { new BitmapImage(new Uri("pack://application:,,,/weapons/colt_45.png")), new BitmapImage(new Uri("pack://application:,,,/weapons/colt_45_fire.png")), new BitmapImage(new Uri("pack://application:,,,/weapons/colt_45.png")) };
            //Звук выстрела
            colt_45.sound = new Uri($@"{System.IO.Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}weapons\sounds\colt1911_fire.ogg");
            colt_45.in_magazine = 10; //Вместимость магазина
            colt_45.in_magazine_count = 0; //Кол-во "маслят" в магазе
            colt_45.cartridge_count = 30; //Кол-во боеприпасов к оружию

            //Винтовка (Ремингтон M40A1)
            m40a1.Name = "Remington M40A1";
            m40a1.cartridge_name = "7,62x51 NATO";
            m40a1.Skin = new BitmapImage[1] { new BitmapImage(new Uri("pack://application:,,,/weapons/m40a1.png")) };
            m40a1.sound = new Uri($@"{System.IO.Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}weapons\sounds\r700_fire.ogg");
            m40a1.in_magazine = 5;
            m40a1.in_magazine_count = 0;
            m40a1.cartridge_count = 15;

            this.KeyDown += Dotnet_KeyDown; //Присваевыем управление игре

            chat.Content += "WPF__Engine v0.1";

        }

       
        string location = "level"; 
        int count = 0; //Кол-во хода
        int jump_count = 0; //Кол-во (высота) прыжка
        bool space_pressed = false; //Зажата ли Space
        //bool shift_pressed = false;

       
        TextBox console = new TextBox(); //Консолька
        Button console_button = new Button(); //Кнопка исполнения команды консоли

        //weapon

        public Weapon colt_45 = new Weapon();
        public Weapon m40a1 = new Weapon();

        private void Rendering(object o, EventArgs args)
        {
            FrameDurations.Add(DateTime.Now.Ticks - PreviousFrameTime);
            PreviousFrameTime = DateTime.Now.Ticks;
        }

        public async void Play_Click(object sender, RoutedEventArgs e)
        {
            gridok.Background = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/level.png"))); //Меняем бэкграунд на игровой фон

            //Спавним игрока           
            gridok.Children.Add(player);
            gridok.Children.Remove(play); //Удаляем кнопку
            gridok.Children.Add(weapon_table); //Отоброжаем счётик БП      
           
        }
        

            public async void PlayCommand_Click(object sender, RoutedEventArgs e)
        {
            chat.Content += "\n" + console.Text; //Добовляем сообщение на панель сообщение с консоли
            switch (console.Text)
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
                case "fullscreen on":
                    this.WindowStyle = WindowStyle.None;
                    this.WindowState = WindowState.Maximized;     
                    break;
                case "fullscreen off":
                    this.WindowStyle = WindowStyle.SingleBorderWindow;
                    this.WindowState = WindowState.Normal;
                    break;
                
            }
            console.Clear();
            gridok.Children.Remove(console);
            gridok.Children.Remove(console_button);
        }

        public async void Dotnet_KeyDown(object sender, KeyEventArgs e)
        {
            //Будем поворачивать игрока с его орудием, когда будет нужно
            ScaleTransform flipTrans = new ScaleTransform();
            ScaleTransform flipTrans2 = new ScaleTransform();
            Player.general.RenderTransformOrigin = new Point(0.5, 0.5);
            Player.weapon.RenderTransformOrigin = new Point(0.5, 0.5);
            bot.general.RenderTransformOrigin = new Point(0.5, 0.5);
            Player.general.RenderTransform = flipTrans;
            Player.weapon.RenderTransform = flipTrans;
            bot.general.RenderTransform = flipTrans2;
            flipTrans.ScaleX = Player.player_flip;
            flipTrans2.ScaleX = bot.player_flip;


            switch (e.Key)
            {
                case Key.D: //Движение направо
                    //Поворачиваем игрока направо
                    flipTrans.ScaleX = 1;
                    Player.player_flip = 1;

                    Player.Walking(); //Запуск анимации хождения
                    count += Player.walk_speed; //Приписываем кол-ву хода скорсть хождения игрока
                    Canvas.SetLeft(Player.weapon, count + 82); //Движение орудия
                    Canvas.SetLeft(Player.general, count); //Движение игрока                  

                    //Cмена локаций
                    if (Canvas.GetLeft(Player.general) == 650 && location != "level_2")
                    { //Переход на вторую локацию с "бункером"

                        location = "level_2"; //Меняем локацию    
                        console.Text += "location";
                        PlayCommand_Click(null, null);
                        gridok.Background = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/level_2.png")));  //Отображаем другой бэкграунд
                        Canvas.SetLeft(Player.general, 0); //Устанавливаем позицию игрока на место
                        count = 0; //Обнуляем кол-во хода
                        gridok.Children.Add(woodenbox_obj); //Cпавним ящик
                        
                        gridok.Children.Add(player_npc);
                       
                        flipTrans.ScaleX = -1;
                        bot.player_flip = -1;
                        //Пересоздаём табло
                        gridok.Children.Remove(weapon_table); 
                        gridok.Children.Add(weapon_table);
                        bool walk = true;
                        bot.Say("Здраствуй, брат");
                                                                        
                        while (walk)
                        {
                            await Task.Delay(1);
                            bot.Walking();
                            if (chat.Content.ToString().Contains("Успокойся"))
                            {
                                bot.Say("Хорошо");
                                break;
                            }
                        }
                        

                    }
                    if (Canvas.GetLeft(Player.general) == -5 && location != "level")
                    { //Переход обратно
                        location = "level";
                       
                        gridok.Background = gridok.Background = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/level.png")));
                        Canvas.SetLeft(Player.general, 650);
                        count = 650;
                        gridok.Children.Remove(woodenbox_obj);
                        gridok.Children.Remove(player_npc);
                    }
                    break;

                case Key.A: //Движение налево

                    flipTrans.ScaleX = -1;
                    Player.player_flip = -1;
                    Player.Walking();
                    count -= Player.walk_speed;

                    Canvas.SetLeft(Player.general, count);

                    Canvas.SetLeft(Player.weapon, count - 41);
                    
                    break;

                case Key.Space: //Прыжок
                    flipTrans.ScaleX = Player.player_flip;

                    if (space_pressed == false)
                    {
                        space_pressed = true;
                        Canvas.SetTop(Player.general, jump_count);

                        Canvas.SetTop(Player.weapon, jump_count + 194);
                        jump_count += 50;
                        Canvas.SetTop(Player.general, jump_count);

                        Canvas.SetTop(Player.weapon, jump_count + 194);

                        for (int i = 0; i < 40; i++) //Анимация падения
                        {
                            await Task.Delay(1);
                            jump_count += 2;
                            Canvas.SetTop(Player.general, jump_count);

                            Canvas.SetTop(Player.weapon, jump_count + 70);
                        }

                        jump_count = 0;
                        space_pressed = false;
                    }

                    //x_coords.Content += "\n" + Canvas.GetTop(Player.general);

                    break;

                case Key.D2: //Клавиша "2" (переключение на пистолет)
                    flipTrans.ScaleX = Player.player_flip;
                    Player.player_weapon = colt_45; //Оружие игрока - пистолет
                    Player.weapon.Source = Player.player_weapon.Skin[0]; //его изображение
                    //Отображение счётика БП для данного орудия
                    weapon_table.Content = $"{Player.player_weapon.cartridge_name}\n--------------\n{Player.player_weapon.in_magazine_count} / {Player.player_weapon.cartridge_count}"; 
                    break;
                case Key.D1:  //Клавиша "2" (переключение на винтовку)
                    flipTrans.ScaleX = Player.player_flip;
                    Player.player_weapon = m40a1;
                    Player.weapon.Source = Player.player_weapon.Skin[0];
                    weapon_table.Content = $"{Player.player_weapon.cartridge_name}\n--------------\n{Player.player_weapon.in_magazine_count} / {Player.player_weapon.cartridge_count}";
                    break;
                case Key.Back: //Клавиша "Backspace" (оружие убрал!)
                    flipTrans.ScaleX = Player.player_flip;
                    Player.weapon.Source = null;
                    weapon_table.Content = "";
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
                            player.Children.Add(bullet_obj); //Спавн маслины                        
                                for (int i = 0; i < 5000; i += 500) //её полёт (пока только в одну сторону)
                                {
                                    await Task.Delay(1); //Прошла типа одна секунда                         
                                    Canvas.SetLeft(bullet_obj, count + 82 + i); //И быстро полетела (кто не поймал - я не виноват)
                                }
                            player.Children.Remove(bullet_obj); //Приземлилась
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
            }
        }
    }
       class Player 
    {
        public static Image general; //Тело игрока
        public static Image weapon; //его орудие (изображение)
        public static Weapon player_weapon; //Оружие игрока
        public static Label weapon_table { get; set; } //Счётик БП
        public static int player_flip = 0; //Сторона в которую повёрнут игрок
        public static int walk_speed = 5; //Скорость хождения

        public static async void  Walking() //Анимация хождения (смена картинок через определёное время)
        {
            
            general.Source = new BitmapImage(new Uri("pack://application:,,,/player_skin/player_walking/player_walking_1.png")); 
            await Task.Delay(20);
            general.Source = new BitmapImage(new Uri("pack://application:,,,/player_skin/pixil-frame-0.png"));
            await Task.Delay(5);
            general.Source = new BitmapImage(new Uri("pack://application:,,,/player_skin/player_walking/player_walking_2.png"));
            await Task.Delay(20);
            general.Source = new BitmapImage(new Uri("pack://application:,,,/player_skin/pixil-frame-0.png"));
        }       
    }
    class NPC : Player
    {
        public Image general;
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
            general.Source = new BitmapImage(new Uri("pack://application:,,,/player_skin/player_armored.png"));
            await Task.Delay(5);
            general.Source = new BitmapImage(new Uri("pack://application:,,,/player_skin/player_walking/player_armored_walking_2.png"));
            await Task.Delay(20);
            general.Source = new BitmapImage(new Uri("pack://application:,,,/player_skin/player_armored.png"));
        }

    }
   public class Weapon
    {
        public string Name; //Название оружия
        public BitmapImage[] Skin; //Его скин
        public Uri sound; //Его звук
        public int in_magazine;
        public int in_magazine_count;
        public string cartridge_name;
        public int cartridge_count;
        public  void Fire() //Огонь из оружия
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
                    Player.weapon_table.Content = $"{cartridge_name}\n--------------\n{in_magazine_count} / {cartridge_count}";
                }
                else
                {
                    in_magazine_count -= 1;
                    Player.weapon_table.Content = $"{cartridge_name}\n--------------\n{in_magazine_count} / {cartridge_count}";
                    Fire_animation();
                    MediaPlayer sp = new MediaPlayer();
                    sp.Open(sound);
                    sp.Play();
                }
            }
        }
        public async void Fire_animation() //Анимация огня
        {
            foreach (var item in Skin)
            {
                Player.weapon.Source = item;
                await Task.Delay(50);
            }
        }
    }
   
    }
}

  

