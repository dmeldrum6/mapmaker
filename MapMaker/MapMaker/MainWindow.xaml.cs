using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace MapMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // global opened image
        BitmapImage imgSelect = new BitmapImage(new Uri("pack://application:,,,/MapMaker;component/tiles.png"));
        public MainWindow()
        {
            InitializeComponent();

            Image_Process(imgSelect);
        }

        private void Rec_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Rectangle rClick = (Rectangle)e.Source;
                rClick.Fill = rectSelect.Fill;
            }
            catch (Exception x)
            {
                Rectangle rClick = (Rectangle)e.Source;
                MessageBox.Show("Error Filling Target - " + rClick.Name + " : " + x.Message);
            }
        }

        private void ButPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Grid control = this.gridMap;
                RenderTargetBitmap rtb = new RenderTargetBitmap((int)control.ActualWidth, (int)control.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                Rect bounds = VisualTreeHelper.GetDescendantBounds(control);
                DrawingVisual dv = new DrawingVisual();
                using (DrawingContext ctx = dv.RenderOpen())
                {
                    VisualBrush vb = new VisualBrush(control);
                    ctx.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
                }

                rtb.Render(dv);
                PngBitmapEncoder png = new PngBitmapEncoder();
                png.Frames.Add(BitmapFrame.Create(rtb));

                SaveFileDialog dlg = new SaveFileDialog();
                dlg.FileName = "MapImage_" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString();
                dlg.DefaultExt = ".png";
                dlg.Filter = "Portable Network Graphic (*.png)|*.png";
                Nullable<bool> result = dlg.ShowDialog();

                if (result == true)
                {
                    using (Stream fileStream = new FileStream(dlg.FileName, FileMode.Create))
                    {
                        png.Save(fileStream);
                    }
                }
            }
            catch (Exception x)
            {
                MessageBox.Show("Error opening print dialog: " + x.Message);
            }
        }

        private void Image_Process(BitmapImage img)
        {
            try
            {
                lbImages.Items.Clear();

                int wid = img.PixelWidth;
                int hgt = img.PixelHeight;

                for (int i = 0; i < hgt; i++)
                {
                    for (int j = 0; j < wid; j++)
                    {
                        Rectangle rectLoop = new Rectangle();
                        rectLoop.Tag = j.ToString() + "x" + i.ToString();
                        rectLoop.Width = 32;
                        rectLoop.Height = 32;

                        ImageBrush ib = new ImageBrush();
                        ib.ImageSource = img;
                        ib.Stretch = Stretch.None;
                        ib.ViewboxUnits = BrushMappingMode.Absolute;
                        ib.Viewbox = new Rect(new Point((double)j, (double)i), new Size(32, 32));
                        rectLoop.Fill = ib;
                        rectLoop.MouseDown += Rec_ListClick;
                        lbImages.Items.Add(rectLoop);

                        j = j + 31;
                    }

                    i = i + 31;
                }
            }
            catch (Exception x)
            {
                MessageBox.Show("Error Processing Image: " + x.Message);
            }
        }

        private void Rec_ListClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Rectangle rClick = (Rectangle)e.Source;
                string rBack;
                if (rClick.Tag == null) { rBack = ""; } else { rBack = rClick.Tag.ToString(); }
                int swd = Convert.ToInt32(rBack.Split('x')[0]);
                int sht = Convert.ToInt32(rBack.Split('x')[1]);

                ImageBrush ib = new ImageBrush();
                ib.ImageSource = imgSelect;
                ib.Stretch = Stretch.None;
                ib.ViewboxUnits = BrushMappingMode.Absolute;
                ib.Viewbox = new Rect(new Point(swd, sht), new Size(32, 32));
                rectSelect.Fill = ib;
            }
            catch (Exception x)
            {
                Rectangle rClick = (Rectangle)e.Source;
                MessageBox.Show("Error Filling Target - " + rClick.Name + " : " + x.Message);
            }
        }

        private void ButLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select an image file";
            ofd.Filter = "All supported graphics|*.jpg;*.jpeg;*.png;*.bmp;*.gif|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png|" +
              "Bitmap Image (*.bmp)|*.bmp|" +
              "GIF (*.gif)|*.gif";

            if (ofd.ShowDialog() == true)
            {
                imgSelect = new BitmapImage(new Uri(ofd.FileName));
            }
            Image_Process(imgSelect);
        }

        private void ButClear_Click(object sender, RoutedEventArgs e)
        {
            // Clear Background Rectangles
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = imgSelect;
            ib.Stretch = Stretch.None;
            ib.ViewboxUnits = BrushMappingMode.Absolute;
            ib.Viewbox = new Rect(new Point(0, 0), new Size(32, 32));

            foreach (Rectangle rect in gridMap.Children)
            {
                rect.Fill = ib;
            }

            Image_Process(imgSelect);
        }

    }
}