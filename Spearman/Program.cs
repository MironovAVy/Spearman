using System;
using System.Globalization;
using System.IO;
using System.Linq;
using MathNet.Numerics.Statistics;

string path = @"C:\Users\Professional\Desktop\FortsData\sber-SRM4\SpearmanSBRF.txt";

if (!File.Exists(path))
{
    Console.WriteLine("Файл не найден.");
    return;
}

string[] lines = File.ReadAllLines(path);
string[] dataLines = lines.ToArray();

var result = new Grapf[dataLines.Length];

for (var i = 0; i < dataLines.Length; i++)
{
    string[] columns = dataLines[i].Split('\t');

    if (columns.Length != 2)
    {
        Console.WriteLine($"Некорректное количество колонок в строке {i + 2}: {dataLines[i]}");
        continue;
    }

    if (double.TryParse(columns[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double volatility) &&
        int.TryParse(columns[1], NumberStyles.Any, CultureInfo.InvariantCulture, out int highLow))
    {
        result[i] = new Grapf
        {
            Volatility = volatility,
            High_Low = highLow,
        };
    }
    else
    {
        Console.WriteLine($"Ошибка преобразования данных в строке {i + 2}: {dataLines[i]}");
    }
}

var VolatilityData = result.Where(x => x != null).Select(x => x.Volatility).ToArray();
var High_LowData = result.Where(x => x != null).Select(x => x.High_Low).Select(Convert.ToDouble).ToArray();

if (VolatilityData.Length > 0 && High_LowData.Length > 0)
{
    double correlation = Correlation.Spearman(VolatilityData, High_LowData);
    Console.WriteLine($"Коэффициент корреляции Спирмена: {correlation}");
}
else
{
    Console.WriteLine("Недостаточно данных для расчета корреляции.");
}


Array.Sort(VolatilityData);

double Q1 = GetQuantile(VolatilityData, 0.20); // 20-й перцентиль
double Q2 = GetQuantile(VolatilityData, 0.40); // 40-й перцентиль
double Q3 = GetQuantile(VolatilityData, 0.60); // 60-й перцентиль
double Q4 = GetQuantile(VolatilityData, 0.80); // 80-й перцентиль

Console.WriteLine($"Q1 (20-й перцентиль): {Q1}");
Console.WriteLine($"Q2 (40-й перцентиль): {Q2}");
Console.WriteLine($"Q3 (60-й перцентиль): {Q3}");
Console.WriteLine($"Q4 (80-й перцентиль): {Q4}");


static double GetQuantile(double[] sortedValues, double quantile)
{
    int n = sortedValues.Length;
    double position = (n - 1) * quantile;
    int leftIndex = (int)Math.Floor(position);
    int rightIndex = (int)Math.Ceiling(position);
    if (leftIndex == rightIndex)
    {
        return sortedValues[leftIndex];
    }
    else
    {
        double leftValue = sortedValues[leftIndex];
        double rightValue = sortedValues[rightIndex];
        return leftValue + (rightValue - leftValue) * (position - leftIndex);
    }
}

public class Grapf
{
    public double Volatility { get; set; }
    public int High_Low { get; set; }
    public override string ToString() => $"Volatility: {Volatility}, High_Low: {High_Low}";
}