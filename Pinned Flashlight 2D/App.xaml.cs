using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Pinned_Flashlight_2D
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Border_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Border border = sender as Border;
            double x = e.GetPosition(border).X / border.Width;
            double y = e.GetPosition(border).Y / border.Height;
            AttachEffect(border, x, y);
        }

        private void Border_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Border border = sender as Border;
            double x = e.GetPosition(border).X / border.Width;
            double y = e.GetPosition(border).Y / border.Height;
            AttachEffect(border, x, y);
        }

        private void Border_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Border border = sender as Border;
            DetachEffect(border);
        }

        private void AttachEffect(Border border, double x, double y)
        {
            #region Calculating period
            int period = 0;
            if (x < 0.5 && y < 0.5)
                period = 1;
            else if (x > 0.5 && y < 0.5)
                period = 2;
            else if (x > 0.5 && y > 0.5)
                period = 3;
            else if (x < 0.5 && y > 0.5)
                period = 4;
            #endregion

            #region Calculating distance from center
            double distanceFromCenter = Math.Sqrt(Math.Pow((x - 0.5), 2) + Math.Pow((y - 0.5), 2)) * border.Width;
            #endregion

            #region Making gradient for background
            GradientStopCollection backgroundGradientStopCollection = new GradientStopCollection();
            backgroundGradientStopCollection.Add(new GradientStop(Colors.LightGreen, 0));
            backgroundGradientStopCollection.Add(new GradientStop(Colors.Green, 3.8));

            RadialGradientBrush backgroundRadialGradientBrush = new RadialGradientBrush(backgroundGradientStopCollection);
            backgroundRadialGradientBrush.GradientOrigin = new Point(x, y);
            backgroundRadialGradientBrush.Center = new Point(x, y);

            border.Background = backgroundRadialGradientBrush;
            #endregion

            #region Making gradient for border
            GradientStopCollection borderGradientStopCollection = new GradientStopCollection();
            borderGradientStopCollection.Add(new GradientStop(Colors.White, 0.38));
            borderGradientStopCollection.Add(new GradientStop(Colors.Green, 0.62));

            RadialGradientBrush borderRadialGradientBrush = new RadialGradientBrush(borderGradientStopCollection);
            borderRadialGradientBrush.GradientOrigin = new Point(x, y);
            borderRadialGradientBrush.Center = new Point(x, y);

            border.BorderBrush = borderRadialGradientBrush;
            #endregion

            #region Calculating radius for each corner
            double[] distancesFromCorners = new double[4];
            distancesFromCorners[0] = Math.Sqrt(Math.Pow((x - 0), 2) + Math.Pow((y - 0), 2)) * border.Width;
            distancesFromCorners[1] = Math.Sqrt(Math.Pow((x - 1), 2) + Math.Pow((y - 0), 2)) * border.Width;
            distancesFromCorners[2] = Math.Sqrt(Math.Pow((x - 1), 2) + Math.Pow((y - 1), 2)) * border.Width;
            distancesFromCorners[3] = Math.Sqrt(Math.Pow((x - 0), 2) + Math.Pow((y - 1), 2)) * border.Width;

            double[] cornerRadiuses = new double[4];
            for (int i = 0; i < 4; i++)
            {
                cornerRadiuses[i] = border.Width * Math.Sqrt(2) - distancesFromCorners[i];
                cornerRadiuses[i] = 38 * cornerRadiuses[i] / 100;
                if (cornerRadiuses[i] < 19)
                    cornerRadiuses[i] = 19;
            }

            CornerRadius cornerRadius = new CornerRadius(cornerRadiuses[0], cornerRadiuses[1], cornerRadiuses[2], cornerRadiuses[3]);
            border.CornerRadius = cornerRadius;
            #endregion

            #region Calculating DropShadowEffect direction
            Point vector1Point = new Point(1 - 0.5, 0.5 - 0.5);
            Point vector2Point = new Point(x - 0.5, y - 0.5);
            double scalar = vector1Point.X * vector2Point.X + vector1Point.Y * vector2Point.Y;
            double vector1Absolute = Math.Sqrt(Math.Pow(vector1Point.X, 2) + Math.Pow(vector1Point.Y, 2));
            double vector2Absolute = Math.Sqrt(Math.Pow(vector2Point.X, 2) + Math.Pow(vector2Point.Y, 2));
            double angle = Math.Acos(scalar / (vector1Absolute * vector2Absolute)) / Math.PI * 180;
            if (period > 2)
                angle = 360 - angle;
            #endregion

            #region Making DropShadowEffect for Border
            DropShadowEffect borderDropShadowEffect = new DropShadowEffect();
            borderDropShadowEffect.Color = Colors.Black;
            borderDropShadowEffect.Direction = angle;
            borderDropShadowEffect.ShadowDepth = distanceFromCenter;
            borderDropShadowEffect.Opacity = 0.62;
            borderDropShadowEffect.BlurRadius = 19;
            //border.Effect = borderDropShadowEffect;
            #endregion

            #region Making DropShadowEffect for ContentPresenter
            DropShadowEffect contentDropShadowEffect = new DropShadowEffect();
            contentDropShadowEffect.Color = Colors.Black;
            contentDropShadowEffect.Direction = angle;
            contentDropShadowEffect.ShadowDepth = distanceFromCenter;
            contentDropShadowEffect.Opacity = 0.62;
            contentDropShadowEffect.BlurRadius = 11.78;
            (border.Child as ContentPresenter).Effect = contentDropShadowEffect;
            #endregion
        }

        private void DetachEffect(Border border)
        {
            #region Making default gradient for background
            GradientStopCollection backgroundGradientStopCollection = new GradientStopCollection();
            backgroundGradientStopCollection.Add(new GradientStop(Colors.LightGreen, 0));
            backgroundGradientStopCollection.Add(new GradientStop(Colors.Green, 3.8));

            RadialGradientBrush backgroundRadialGradientBrush = new RadialGradientBrush(backgroundGradientStopCollection);
            backgroundRadialGradientBrush.GradientOrigin = new Point(0.5, 0.5);
            backgroundRadialGradientBrush.Center = new Point(0.5, 0.5);

            border.Background = backgroundRadialGradientBrush;
            #endregion

            #region Making default gradient for border
            GradientStopCollection borderGradientStopCollection = new GradientStopCollection();
            borderGradientStopCollection.Add(new GradientStop(Colors.White, 0.38));
            borderGradientStopCollection.Add(new GradientStop(Colors.Green, 0.62));

            RadialGradientBrush borderRadialGradientBrush = new RadialGradientBrush(borderGradientStopCollection);
            borderRadialGradientBrush.GradientOrigin = new Point(0.5, 0.5);
            borderRadialGradientBrush.Center = new Point(0.5, 0.5);
            
            border.BorderBrush = borderRadialGradientBrush;
            #endregion

            #region Making default radius for each corner
            border.CornerRadius = new CornerRadius(19, 19, 19, 19);
            #endregion

            #region Making default DropShadowEffect for Border
            DropShadowEffect borderDropShadowEffect = new DropShadowEffect();
            borderDropShadowEffect.Color = Colors.Black;
            borderDropShadowEffect.Direction = 0;
            borderDropShadowEffect.ShadowDepth = 0;
            borderDropShadowEffect.Opacity = 0.62;
            borderDropShadowEffect.BlurRadius = 19;
            border.Effect = borderDropShadowEffect;
            #endregion

            #region Making default DropShadowEffect for ContentPresenter
            DropShadowEffect contentDropShadowEffect = new DropShadowEffect();
            contentDropShadowEffect.Color = Colors.Black;
            contentDropShadowEffect.Direction = 0;
            contentDropShadowEffect.ShadowDepth = 0;
            contentDropShadowEffect.Opacity = 0.62;
            contentDropShadowEffect.BlurRadius = 11.78;
            (border.Child as ContentPresenter).Effect = contentDropShadowEffect;
            #endregion
        }
    }
}
