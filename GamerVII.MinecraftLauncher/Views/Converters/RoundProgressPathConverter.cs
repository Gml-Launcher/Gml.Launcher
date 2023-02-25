using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;
using System.Windows.Markup;

namespace GamerVII.MinecraftLauncher.Views.Converters
{
    internal class RoundProgressPathConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter,
                              CultureInfo culture)
        {
            if (values?.Contains(DependencyProperty.UnsetValue) != false)
                return DependencyProperty.UnsetValue;

            var v = (double)values[0]; // значение слайдера
            var min = (double)values[1]; // минимальное значение
            var max = (double)values[2]; // максимальное

            var ratio = (v - min) / (max - min); // какую долю окружности закрашивать
            var isFull = ratio >= 1; // для случая полной окружности нужна особая обработка
            var angleRadians = 2 * Math.PI * ratio;
            var angleDegrees = 360 * ratio;

            // внешний радиус примем за 1, растянем в XAML'е.
            var outerR = 1;
            // как параметр передадим долю радиуса, которую занимает внутренняя часть
            var innerR =
                  System.Convert.ToDouble(parameter, CultureInfo.InvariantCulture) * outerR;
            // вспомогательные штуки: вектор направления вверх
            var vector1 = new Vector(0, -1);
            // ... и на конечную точку дуги
            var vector2 = new Vector(Math.Sin(angleRadians), -Math.Cos(angleRadians));
            var center = new Point();

            var geo = new StreamGeometry();
            geo.FillRule = FillRule.EvenOdd;

            using (var ctx = geo.Open())
            {
                Size outerSize = new Size(outerR, outerR),
                     innerSize = new Size(innerR, innerR);

                if (!isFull)
                {
                    Point p1 = center + vector1 * outerR, p2 = center + vector2 * outerR,
                          p3 = center + vector2 * innerR, p4 = center + vector1 * innerR;

                    ctx.BeginFigure(p1, isFilled: true, isClosed: true);
                    ctx.ArcTo(p2, outerSize, angleDegrees, isLargeArc: angleDegrees > 180,
                        sweepDirection: SweepDirection.Clockwise, isStroked: true,
                        isSmoothJoin: false);
                    ctx.LineTo(p3, isStroked: true, isSmoothJoin: false);
                    ctx.ArcTo(p4, innerSize, -angleDegrees, isLargeArc: angleDegrees > 180,
                        sweepDirection: SweepDirection.Counterclockwise, isStroked: true,
                        isSmoothJoin: false);

                    Point diag1 = new Point(-outerR, -outerR),
                          diag2 = new Point(outerR, outerR);
                    ctx.BeginFigure(diag1, isFilled: false, isClosed: false);
                    ctx.LineTo(diag2, isStroked: false, isSmoothJoin: false);
                }
                else
                {
                    Point p1 = center + vector1 * outerR, p2 = center - vector1 * outerR,
                          p3 = center + vector1 * innerR, p4 = center - vector1 * innerR;

                    ctx.BeginFigure(p1, isFilled: true, isClosed: true);
                    ctx.ArcTo(p2, outerSize, 180, isLargeArc: false,
                        sweepDirection: SweepDirection.Clockwise, isStroked: true,
                        isSmoothJoin: false);
                    ctx.ArcTo(p1, outerSize, 180, isLargeArc: false,
                        sweepDirection: SweepDirection.Clockwise, isStroked: true,
                        isSmoothJoin: false);
                    ctx.BeginFigure(p3, isFilled: true, isClosed: true);
                    ctx.ArcTo(p4, innerSize, 180, isLargeArc: false,
                        sweepDirection: SweepDirection.Clockwise, isStroked: true,
                        isSmoothJoin: false);
                    ctx.ArcTo(p3, innerSize, 180, isLargeArc: false,
                        sweepDirection: SweepDirection.Clockwise, isStroked: true,
                        isSmoothJoin: false);
                }
            }

            geo.Freeze();
            return geo;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter,
                                    CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}