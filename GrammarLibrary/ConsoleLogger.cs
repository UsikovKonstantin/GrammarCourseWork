using System.Text;

namespace GrammarLibrary;

/// <summary>
/// Класс для логирования в консоль.
/// </summary>
public class ConsoleLogger : ILogger
{
	public ConsoleLogger()
    {
		Console.OutputEncoding = Encoding.Unicode;
	}

    public void Log(object? message = null)
	{
		Console.Write(message);
	}

	public void LogLine(object? message = null)
	{
		Console.WriteLine(message);
	}

	public void SetColor(object color)
	{
		if (color is ConsoleColor consoleColor)
			Console.ForegroundColor = consoleColor;
	}

	public void ResetColor()
	{
		Console.ResetColor();
	}

	public void LogWithColor(object color, object? message = null)
	{
		ConsoleColor prevColor = Console.ForegroundColor;
		if (color is ConsoleColor consoleColor)
			Console.ForegroundColor = consoleColor;
		Log(message);
		Console.ForegroundColor = prevColor;
	}

	public void LogLineWithColor(object color, object? message = null)
	{
		ConsoleColor prevColor = Console.ForegroundColor;
		if (color is ConsoleColor consoleColor)
			Console.ForegroundColor = consoleColor;
		LogLine(message);
		Console.ForegroundColor = prevColor;
	}
}
