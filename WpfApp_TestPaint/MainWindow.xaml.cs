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

namespace WpfApp_TestPaint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
              
        private void element_OnCornerMouseEnter(object sender, Cursor cursor)
        {
        }

        // Ссылка на передвигаемый объект
        UserControl movingElement = null;

        // Координаты нажатия в передвигаемом объекте
        Point elementCoords;

        static int label_number = 0;

        private void mainCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (movingElement != null)
            {
                // Поместить отпущенный объект на самый нижний Z-уровень
                Canvas.SetZIndex(movingElement, 0);

                movingElement = null;
            }
        }

        private void mainCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                UserControl_Round.Unchecked();
                return;
            }
            UserControl_Round userControl_Round = new UserControl_Round();

            // Установить координаты для созданного элемента из координат мыши
            Point coords = e.GetPosition(mainCanvas);
            coords.Offset(-userControl_Round.Width / 2, -userControl_Round.Height / 2);

            Canvas.SetLeft(userControl_Round, coords.X);
            Canvas.SetTop(userControl_Round, coords.Y);

            // Поместить созданный объект на самый нижний Z-уровень
            Canvas.SetZIndex(userControl_Round, 0);

            // Зарегистрировать обработчик нажатия мышью на созданном объекте
            userControl_Round.MouseDown += Element_MouseDown;

            userControl_Round.mainCanvas = mainCanvas;

            mainCanvas.Children.Add(userControl_Round);
        }

        private void mainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (movingElement != null)
            {
                // Текущие координаты мыши на холсте
                Point coords = e.GetPosition(mainCanvas);

                // Перемещение элемента по новым координатам мыши, с учётом места нажатия на элементе
                Canvas.SetLeft(movingElement, coords.X - elementCoords.X);
                Canvas.SetTop(movingElement, coords.Y - elementCoords.Y);
                return;
            }
            if (sender is Canvas)
            {
                e.Handled = false;
                return;
            }
        }

        private void Element_MouseDown(object sender, MouseButtonEventArgs e)
        {
            movingElement = (UserControl)sender;

            // Получить координаты мыши внутри перемещаемого объекта
            elementCoords = e.GetPosition(movingElement);

            // Поместить выбранный объект на самый верхний Z-уровень
            Canvas.SetZIndex(movingElement, 10);
            e.Handled = true;
        }

    }
}
