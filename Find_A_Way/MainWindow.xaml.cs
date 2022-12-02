using Find_A_Way.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Find_A_Way
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Thread? th;
        CancellationTokenSource cts = new CancellationTokenSource();
        delegate void OneArgDelegate(Cell cell1, CancellationToken cancellationToken);
        Algorithm? algorithm;
        Board board;
        Rectangle finish;
        Rectangle hero;
        Rectangle visual;
        int cell = 20;
        int canvasWidth = 1080;
        int canvasHeight = 660;
        bool radio1 = false, radio2 = false;
        int startX = 100, startY = 100;
        int finishX = 100, finishY = 100;

        public MainWindow()
        {
            board = new Board(canvasHeight / cell, canvasWidth / cell);
            canvas = new Canvas();
            hero = new Rectangle();
            visual = new Rectangle();
            StartRadioButton = new RadioButton();
            FinishButton = new RadioButton();
            finish = new Rectangle();
            InitializeComponent();
        }

        private void paint(int x, int y)
        {
            var action = (Rectangle r, SolidColorBrush b) =>
            {
                r.Fill = b;
                r.Width = cell;
                r.Height = cell;
                Canvas.SetLeft(r, x * cell);
                Canvas.SetTop(r, y * cell);
                canvas.Children.Add(r);
            };

            if (radio1 == true && board.map[y][x].state == State.START)
            {
                action.Invoke(hero, Brushes.Blue);
            }
            else if (radio2 == true && board.map[y][x].state == State.FINISH)
            {
                action.Invoke(finish, Brushes.Red);
            }
            else if (board.map[y][x].state == State.WALL)
            {
                Rectangle wall = new Rectangle();
                action.Invoke(wall, Brushes.Green);
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (startX != 100 && startY != 100 && finishX != 100 && finishY != 100)
            {
                cts.Dispose();
                cts = new CancellationTokenSource();
                algorithm = new Algorithm(startX, startY, finishX, finishY, board.map);
                algorithm.Run();
                th = new Thread(printPath);
                th.Start();
                StartButton.IsEnabled = false;
            }
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (StartButton.IsEnabled)
            {
                Point point = e.GetPosition(canvas);
                int x = Convert.ToInt32(point.X) / cell;
                int y = Convert.ToInt32(point.Y) / cell;
                if (e.LeftButton == MouseButtonState.Released)
                {
                    canvas.Children.Remove(visual);
                    visual.Stroke = Brushes.Black;
                    visual.Width = cell;
                    visual.Height = cell;
                    Canvas.SetLeft(visual, x * cell);
                    Canvas.SetTop(visual, y * cell);
                    canvas.Children.Add(visual);
                }
                else if (WallButton.IsChecked == true && e.LeftButton == MouseButtonState.Pressed)
                {
                    try
                    {
                        board.map[y][x].state = State.WALL;
                        paint(x, y);
                    }
                    catch (IndexOutOfRangeException d)
                    {
                        Console.WriteLine(d.Message);
                    }
                }
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            board.Clear();
            canvas.Children.Clear();
            cts.Cancel();
            StartButton.IsEnabled = true;
        }

        private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (StartButton.IsEnabled)
            {
                Point point = e.GetPosition(canvas);
                int x = Convert.ToInt32(point.X) / cell;
                int y = Convert.ToInt32(point.Y) / cell;
                var action = (Rectangle r, int x1, int y1, State s, out bool radio) =>
                {
                    canvas.Children.Remove(r);
                    try
                    {
                        board.map[y1][x1].state = State.NOTVISITED;
                        board.map[y][x].state = s;
                    }
                    catch(IndexOutOfRangeException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    radio = true;
                };

                if (StartRadioButton.IsChecked == true)
                {
                    action.Invoke(hero, startX, startY, State.START, out radio1);
                    startX = x;
                    startY = y;
                }
                else if (FinishButton.IsChecked == true)
                {
                    action.Invoke(finish, finishX, finishY, State.FINISH, out radio2);
                    finishX = x;
                    finishY = y;
                }
                paint(x, y);
                radio1 = radio2 = false;
            }
        }

        private void printPath()
        {
            List<Cell> path = new List<Cell>();
#pragma warning disable CS8602
            Cell current = algorithm.finish;
#pragma warning restore CS8602

            if(current.parent != null)
                path.Add(current);
            while (current.parent != algorithm.start && current.parent != null)
            {
                path.Add(current);
                current = current.parent;
            }
            path.Add(current);

            for (int i = path.Count - 1; i > 0; i--)
            {
                    Thread.Sleep(100);
                    canvas.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                                                  new OneArgDelegate(printHero), path[i], cts.Token);
            }
        }

        private void printHero(Cell cell1, CancellationToken cancellationToken)
        {
            if(cancellationToken.IsCancellationRequested)
                return;
            canvas.Children.Remove(hero);
            hero.Fill = Brushes.Blue;
            hero.Width = cell;
            hero.Height = cell;
            Canvas.SetLeft(hero, cell1.x * cell);
            Canvas.SetTop(hero, cell1.y * cell);
            canvas.Children.Add(hero);
        }
    }
}
