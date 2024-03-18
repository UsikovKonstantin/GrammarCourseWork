namespace GrammarLibrary;

/// <summary>
/// Класс для парсинга исходных данных.
/// </summary>
public class GrammarParser
{
	/// <summary>
	/// Символ, с которого начинается составление цепочек.
	/// </summary>
	public char StartCharacter { get; private set; }

	/// <summary>
	/// Правила грамматики.
	/// </summary>
	public Dictionary<char, HashSet<string>> Rules { get; private set; } = new Dictionary<char, HashSet<string>>();

	/// <summary>
	/// Конструктор по файлу.
	/// </summary>
	/// <param name="filePath"> путь до файла </param>
	/// <exception cref="Exception"> исключение, возникающее при неверно заданных данных </exception>
	public GrammarParser(string filePath)
    {
		if (!File.Exists(filePath))
			throw new Exception($"Файл не найден: {filePath}.");
		string[] lines = File.ReadAllLines(filePath);
		(StartCharacter, Rules) = ParseLines(lines);
	}

	/// <summary>
	/// Конструктор по массиву строк.
	/// </summary>
	/// <param name="lines"> массив строк </param>
	public GrammarParser(string[] lines)
	{
		(StartCharacter, Rules) = ParseLines(lines);
	}

	/// <summary>
	/// Парсинг входных строк.
	/// </summary>
	/// <param name="lines"> строки </param>
	/// <returns> начальный символ и словарь правил </returns>
	/// <exception cref="Exception"> исключение, возникающее при неверно заданных данных </exception>
	private (char, Dictionary<char, HashSet<string>>) ParseLines(string[] lines)
	{
		char startCharacter = ' ';
		Dictionary<char, HashSet<string>> rules = new Dictionary<char, HashSet<string>>();

		foreach (string lineWithComments in lines)
		{
			string[] lineParts = lineWithComments.Split('#');
			string line = lineParts.Length == 0 ? "" : lineParts[0].Trim();

			if (line == "")
				continue;

			if (line.StartsWith("start:"))
			{
				if (startCharacter != ' ')
					throw new Exception($"Ошибка: начальный нетерминал указан несколько раз.");

				string startStr = line.Remove(0, 6).Trim();
				if (startStr.Length != 1)
					throw new Exception($"Ошибка в строке: {lineWithComments}. Нетерминал (\'{startStr}\') должен состоять из 1 символа.");

				char startChar = startStr[0];
				if (!ContextFreeGrammar.AcceptableNonTerminals.Contains(startChar))
					throw new Exception($"Ошибка в строке: {lineWithComments}. Недопустимый нетерминал: {startChar}.");

				startCharacter = startChar;
			}
			else
			{
				string[] ruleParts = line.Split(new string[] { "->" }, StringSplitOptions.RemoveEmptyEntries);
				if (ruleParts.Length != 2)
					throw new Exception($"Ошибка в строке: {lineWithComments}.");

				string startStr = ruleParts[0].Trim();
				if (startStr.Length != 1)
					throw new Exception($"Ошибка в строке: {lineWithComments}. Нетерминал (\'{startStr}\') должен состоять из 1 символа.");

				char startChar = startStr[0];
				if (!ContextFreeGrammar.AcceptableNonTerminals.Contains(startChar))
					throw new Exception($"Ошибка в строке: {lineWithComments}. Недопустимый нетерминал: {startChar}.");

				string[] words = ruleParts[1].Trim().Split(new string[] { "|" }, StringSplitOptions.None);
				if (words.Length == 0)
					throw new Exception($"Ошибка в строке: {lineWithComments}.");

				if (!rules.ContainsKey(startChar))
					rules[startChar] = new HashSet<string>();

				foreach (string word in words)
				{
					string trimWord = word.Trim();
					if (trimWord == "")
						throw new Exception($"Ошибка в строке: {lineWithComments}.");

					string repWord = trimWord.Replace("_", "");
					foreach (char c in repWord)
						if (!ContextFreeGrammar.AcceptableNonTerminals.Contains(c) && !ContextFreeGrammar.AcceptableTerminals.Contains(c))
							throw new Exception($"Ошибка в строке: {lineWithComments}. Недопустимый символ: {c}.");
					rules[startChar].Add(repWord);
				}
			}
		}

		if (startCharacter == ' ')
			throw new Exception($"Ошибка: не указан начальный нетерминал.");

		return (startCharacter, rules);
	}

	/// <summary>
	/// Получить грамматику.
	/// </summary>
	/// <returns> грамматика </returns>
	public ContextFreeGrammar GetGrammar(ILogger? logger = null)
	{
		return new ContextFreeGrammar(StartCharacter, Rules, logger);
	}
}
