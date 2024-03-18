namespace GrammarLibrary;

/// <summary>
/// Класс для логирования.
/// </summary>
public interface ILogger
{
	/// <summary>
	/// Сделать лог сообщения.
	/// </summary>
	/// <param name="message"> сообщение </param>
	void Log(object? message = null);

	/// <summary>
	/// Сделать лог сообщения с переносом на новую строку.
	/// </summary>
	/// <param name="message"> сообщение </param>
	void LogLine(object? message = null);

	/// <summary>
	/// Установить цвет текста.
	/// </summary>
	/// <param name="color"> цвет </param>
	void SetColor(object color);

	/// <summary>
	/// Сбросить цвет текста.
	/// </summary>
	void ResetColor();

	/// <summary>
	/// Сделать лог сообщения определенного цвета.
	/// </summary>
	/// <param name="color"> цвет </param>
	/// <param name="message"> сообщение </param>
	void LogWithColor(object color, object? message = null);

	/// <summary>
	/// Сделать лог сообщения определенного цвета с переносом на новую строку.
	/// </summary>
	/// <param name="color"> цвет </param>
	/// <param name="message"> сообщение </param>
	void LogLineWithColor(object color, object? message = null);
}
