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
    public delegate void OnCornerMouseEnterDelegate(object sender, Cursor cursor);

    /// <summary>
    /// Interaction logic for UserControl_Round.xaml
    /// </summary>
    public partial class UserControl_Round : UserControl
    {
        // сообщение о нажатии мышью по одному из 
        public event OnCornerMouseEnterDelegate OnCornerMouseEnter;

        private static int count = 0;

        private static int last_size = 150;
        public Canvas mainCanvas { get; set; }
        public UserControl_Round()
        {
            InitializeComponent();

            this.textblock.Text = count.ToString();
            count++;

            // обработка изменения размера элемента управления
            this.SizeChanged += Round_SizeChange;
                        
            this.MouseDown += UserControl_Round_MouseDown;
            this.PreviewMouseDown += Corner_MouseDown;
            this.PreviewMouseMove += Corner_MouseMove;

            movingElement = null;
            if (current != null)
                Checked(current, false);
            current = this;
            Checked(this, false);
        }

        public static void Unchecked()
        {
            Checked(current, false);
        }
        private void Round_SizeChange(object sender, SizeChangedEventArgs e)
        {
            this.Width = last_size;
            this.Height = last_size;

            this.rectangle.Width = last_size;
            this.rectangle.Height = last_size;
            
            this.elipse.Width = last_size;
            this.elipse.Height = last_size;

            this.textblock.FontSize = (int)(30 * last_size / 50);

            int sizePoint = (int)(5 * last_size / 50);
            this.elipseTopLeft.Width = sizePoint;
            this.elipseTopLeft.Height = sizePoint;
            this.elipseTopRight.Width = sizePoint;
            this.elipseTopRight.Height = sizePoint;
            this.elipseBottomLeft.Width = sizePoint;
            this.elipseBottomLeft.Height = sizePoint;
            this.elipseBottomRight.Width = sizePoint;
            this.elipseBottomRight.Height = sizePoint;
        }

        private static UserControl_Round current;

        private void Corner_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Ellipse el)
            {
                switch (el.Tag)
                {
                    case "TopLeft":
                        el.Cursor = Cursors.ScrollSE;
                        OnCornerMouseEnter?.Invoke(this, Cursors.ScrollSE);
                        break;
                    case "TopRight":
                        el.Cursor = Cursors.ScrollSW;
                        OnCornerMouseEnter?.Invoke(this, Cursors.ScrollSW);
                        break;
                    case "BottomLeft":
                        el.Cursor = Cursors.ScrollNE;
                        OnCornerMouseEnter?.Invoke(this, Cursors.ScrollNE);
                        break;
                    case "BottomRight":
                        el.Cursor = Cursors.ScrollNW;
                        OnCornerMouseEnter?.Invoke(this, Cursors.ScrollNW);
                        break;
                    default:
                        el.Cursor = Cursors.AppStarting;
                        OnCornerMouseEnter?.Invoke(this, Cursors.AppStarting);
                        break;
                }
            }
        }
        // Координаты нажатия в передвигаемом объекте
        Point elementCoords;
        // Ссылка на передвигаемый объект
        public static Ellipse movingElement = null;
        private string corner;
        private void Corner_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(sender is Ellipse el)
            {
                if (!el.Equals(movingElement))
                {
                    switch (el.Tag)
                    {
                        case "TopLeft":
                        case "TopRight":
                        case "BottomLeft":
                        case "BottomRight":
                            corner = (string)el.Tag;
                            movingElement = el;

                            // Получить координаты мыши внутри перемещаемого объекта
                            elementCoords = e.GetPosition(mainCanvas);
                            break;
                        default:
                            movingElement = null;
                            break;
                    }
                    //                OnCornerMouseDown?.Invoke(this, (string)((Ellipse)sender).Tag);
                    e.Handled = true;
                    return;
                }
            }
            movingElement = null;
            e.Handled = false;
        }
        private void Corner_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                movingElement = null;
                if (movingElement != null)
            {
                // Текущие координаты мыши на холсте
                Point coords = e.GetPosition(mainCanvas);
                int deltaX = (int)(coords.X - elementCoords.X);
                int deltaY = (int)(coords.Y - elementCoords.Y);

                switch (corner)
                {
                    case "TopLeft":
                        last_size += Math.Abs(deltaX) > Math.Abs(deltaY) ? -deltaX : -deltaY;
                        break;
                    case "TopRight":
                        last_size += Math.Abs(deltaX) > Math.Abs(deltaY) ? deltaX : -deltaY;
                        break;
                    case "BottomLeft":
                        last_size += Math.Abs(deltaX) > Math.Abs(deltaY) ? -deltaX : deltaY;
                        break;
                    case "BottomRight":
                        last_size += Math.Abs(deltaX) > Math.Abs(deltaY) ? deltaX : deltaY;
                        break;
                }
//                last_size += Math.Abs(deltaX)> Math.Abs(deltaY) ? deltaX:deltaY;

                if (last_size < 50)
                    last_size = 50;
                elementCoords = coords;
                Round_SizeChange(sender, null);
            }
        }
        private void UserControl_Round_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is UserControl_Round round)
            {
                last_size = (int)round.rectangle.Width;
                if (current == null)
                {
                    current = round;
                }
                else if (!round.Equals(current))
                {
                    Checked(current, false);
                    current = round;
                }

                Checked(current, true);

            }    
        }

        private static void Checked(UserControl_Round current, bool v)
        {
            Visibility visibility = Visibility.Visible;
            if (v)
            {
                current.elipseTopLeft.Visibility = visibility;
                current.elipseTopRight.Visibility = visibility;
                current.elipseBottomLeft.Visibility = visibility;
                current.elipseBottomRight.Visibility = visibility;
                current.rectangle.Visibility = visibility;
            }
            else
            {
                visibility = Visibility.Hidden;
                current.elipseTopLeft.Visibility = visibility;
                current.elipseTopRight.Visibility = visibility;
                current.elipseBottomLeft.Visibility = visibility;
                current.elipseBottomRight.Visibility = visibility;
                current.rectangle.Visibility = visibility;
            }
        }

    }
}
