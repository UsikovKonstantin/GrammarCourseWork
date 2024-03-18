using System.Data;
using System.Text;

namespace GrammarLibrary;

/// <summary>
/// Класс, представляющий собой КС-грамматику и методы для работы с ней.
/// </summary>
public class ContextFreeGrammar
{
	#region Поля
	/// <summary>
	/// Символ, с которого начинается составление цепочек.
	/// </summary>
	public char StartNonTerminal { get; private set; } = 'S'; 
	
	/// <summary>
	/// Правила грамматики.
	/// </summary>
    public Dictionary<char, HashSet<string>> Rules { get; private set; } = new Dictionary<char, HashSet<string>>();

	/// <summary>
	/// Объект для логирования.
	/// </summary>
	public ILogger? Logger { get; private set; }

	/// <summary>
	/// Список допустимых нетерминалов грамматики.
	/// </summary>
	public static List<char> AcceptableNonTerminals { get; private set; } = new List<char>() { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'À', 'Á', 'Â', 'Ã', 'Ä', 'Å', 'Ç', 'È', 'É', 'Ê', 'Ë', 'Ì', 'Í', 'Î', 'Ï', 'Ñ', 'Ò', 'Ó', 'Ô', 'Õ', 'Ö', 'Ù', 'Ú', 'Û', 'Ü', 'Ý', 'Ā', 'Ă', 'Ą', 'Ć', 'Ĉ', 'Ċ', 'Č', 'Ď', 'Đ', 'Ē', 'Ĕ', 'Ė', 'Ę', 'Ě', 'Ĝ', 'Ğ', 'Ġ', 'Ģ', 'Ĥ', 'Ĩ', 'Ī', 'Ĭ', 'Į', 'İ', 'Ĵ', 'Ķ', 'Ĺ', 'Ļ', 'Ľ', 'Ŀ', 'Ł', 'Ń', 'Ņ', 'Ň', 'Ŋ', 'Ō', 'Ŏ', 'Ő', 'Ŕ', 'Ŗ', 'Ř', 'Ś', 'Ŝ', 'Ş', 'Š', 'Ţ', 'Ť', 'Ŧ', 'Ũ', 'Ū', 'Ŭ', 'Ů', 'Ű', 'Ų', 'Ŵ', 'Ŷ', 'Ÿ', 'Ź', 'Ż', 'Ž', 'Ɔ', 'Ə', 'Ɣ', 'Ɲ', 'Ơ', 'Ư', 'Ʊ', 'Ʋ', 'Ǎ', 'Ǥ', 'Ǧ', 'Ǫ', 'Ǻ', 'Ș', 'Ț', 'Ȟ', 'Ȳ', 'Þ', 'Ɗ', 'Ƌ', 'Ƒ', 'Ɠ', 'Ɨ', 'Ƙ', 'Ɯ', 'Ɵ', 'Ƣ', 'Ƥ', 'Ʀ', 'Ƨ', 'Ƭ', 'Ʈ', 'Ƴ', 'Ƶ', 'Ƹ', 'Ǐ', 'Ǒ', 'Ǔ', 'Ǖ', 'Ǘ', 'Ǚ', 'Ǜ', 'Ǟ', 'Ǡ', 'Ǩ', 'Ǭ', 'Ǯ', 'Ǵ', 'Ƿ', 'Ǹ', 'Ȁ', 'Ȃ', 'Ȅ', 'Ȇ', 'Ȉ', 'Ȋ', 'Ȍ', 'Ȏ', 'Ȑ', 'Ȓ', 'Ȕ', 'Ȗ', 'Ƞ', 'Ȥ', 'Ȧ', 'Ȩ', 'Ȫ', 'Ȭ', 'Ȯ', 'Ȱ', 'Ɂ', 'Ƀ', 'Ʉ', 'Ɉ', 'Ɋ', 'Ɍ', '$' };

	/// <summary>
	/// Список допустимых терминалов грамматики.
	/// </summary>
	public static List<char> AcceptableTerminals { get; private set; } = new List<char>() { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '!', '@', '%', '^', '&', '*', '(', ')', '-', '=', '+', '[', ']', '{', '}', '\\', ';', ':', '<', '.', '>', '/', '?', ',' };

	/// <summary>
	/// Список текущих нетерминалов грамматики.
	/// </summary>
	public HashSet<char> NonTerminals { get; private set; } = new HashSet<char>();

	/// <summary>
	/// Список текущих терминалов грамматики.
	/// </summary>
	public HashSet<char> Terminals { get; private set; } = new HashSet<char>();
	#endregion

	#region Конструктор
	/// <summary>
	/// Конструктор грамматики.
	/// </summary>
	/// <param name="startNonTerminal"> символ, с которого начинается составление цепочек </param>
	/// <param name="rules"> правила грамматики </param>
	/// <param name="logger"> объект для логирования </param>
	/// <exception cref="Exception"> исключение, возникающее при неверно заданных данных </exception>
	public ContextFreeGrammar(char startNonTerminal, Dictionary<char, HashSet<string>> rules, ILogger? logger = null)
	{
		StartNonTerminal = startNonTerminal;
		Rules = rules.ToDictionary(entry => entry.Key, entry => new HashSet<string>(entry.Value));
		Logger = logger;

		if (!AcceptableNonTerminals.Contains(startNonTerminal))
			throw new Exception($"Недопустимый нетерминал: {startNonTerminal}.");

		foreach (char key in rules.Keys)
		{
			if (!AcceptableNonTerminals.Contains(key))
				throw new Exception($"Недопустимый нетерминал: {key}.");

			HashSet<string> values = rules[key];
			foreach (string value in values)
				foreach (char c in value)
					if (!AcceptableNonTerminals.Contains(c) && !AcceptableTerminals.Contains(c))
						throw new Exception($"Недопустимый символ: {key}.");
		}

		UpdateTerminalLists();
	}

	/// <summary>
	/// Обновить списки текущих терминалов и нетерминалов грамматики.
	/// </summary>
	private void UpdateTerminalLists()
	{
		NonTerminals = new HashSet<char>();
		Terminals = new HashSet<char>();

		if (AcceptableNonTerminals.Contains(StartNonTerminal))
			NonTerminals.Add(StartNonTerminal);

		foreach (char key in Rules.Keys)
		{
			if (AcceptableNonTerminals.Contains(key))
				NonTerminals.Add(key);

			HashSet<string> values = Rules[key];
			foreach (string value in values)
			{
				foreach (char c in value)
					if (AcceptableNonTerminals.Contains(c))
						NonTerminals.Add(c);
					else if (AcceptableTerminals.Contains(c))
						Terminals.Add(c);
			}
		}
	}
	#endregion

	#region ToString()
	public override string ToString()
	{
		string arrow = " -> ";
		string separator = " | ";

		string res = $"start: {StartNonTerminal}\n";
		foreach (char key in Rules.Keys)
		{
			string rowRes = "";
			foreach (string value in Rules[key])
				rowRes += (value == "" ? "ε" : value) + separator;
			if (rowRes != "")
				rowRes = rowRes.Substring(0, rowRes.Length - separator.Length);
			res += key + arrow + rowRes + "\n";
		}
		return res;
	}
	#endregion

	#region Устранение непорождающих нетерминалов
	/// <summary>
	/// Устранить непорождающие нетерминалы.
	/// </summary>
	/// <returns> новая грамматика </returns>
	public ContextFreeGrammar RemoveNonGenerativeNonTerminals()
	{
		ContextFreeGrammar copyGrammar = new ContextFreeGrammar(StartNonTerminal, Rules, Logger);
		copyGrammar.RemoveNonGenerativeNonTerminalsInCurrentGrammar();
		return copyGrammar;
	}

	/// <summary>
	/// Устранить непорождающие нетерминалы в текущей грамматике.
	/// </summary>
	private void RemoveNonGenerativeNonTerminalsInCurrentGrammar()
	{
		Logger?.LogLine("УСТРАНЕНИЕ НЕПОРОЖДАЮЩИХ НЕТЕРМИНАЛОВ");

		HashSet<char> generativeNonTerminals = GetGenerativeNonTerminals();
		if (generativeNonTerminals.Count > 0)
		{
			Logger?.Log("Множество порождающих нетерминалов: ");
			Logger?.SetColor(ConsoleColor.Green);
			foreach (char item in generativeNonTerminals)
				Logger?.Log(item + " ");
			Logger?.SetColor(ConsoleColor.Gray);
			Logger?.LogLine();
		}
		else
		{
			Logger?.LogLine("Порождающие нетерминалы не найдены.");
		}

		HashSet<char> nonGenerativeNonTerminals = GetNonGenerativeNonTerminals();
		if (nonGenerativeNonTerminals.Count > 0)
		{
			Logger?.Log("Множество непорождающих нетерминалов: ");
			Logger?.SetColor(ConsoleColor.Red);
			foreach (char item in nonGenerativeNonTerminals)
				Logger?.Log(item + " ");
			Logger?.SetColor(ConsoleColor.Gray);
			Logger?.LogLine();
		}
		else
		{
			Logger?.LogLine("Непорождающие нетерминалы не найдены. Ничего удалять не нужно.\n");
			return;
		}

		Logger?.LogLine("Удалим правила, содержащие непорождающие нетерминалы");
		RemoveRulesContainingNonGenerativeNonTerminals(nonGenerativeNonTerminals);

		Logger?.LogLine("РЕЗУЛЬТАТ");
		Logger?.LogLine(this.ToString());
	}

	/// <summary>
	/// Получить набор порождающих нетерминалов.
	/// </summary>
	/// <returns> набор порождающих нетерминалов </returns>
	private HashSet<char> GetGenerativeNonTerminals()
	{
		HashSet<char> generativeNonTerminals = new HashSet<char>();
		int countPrev;
		do
		{
			countPrev = generativeNonTerminals.Count;
			foreach (char key in Rules.Keys)
			{
				foreach (string value in Rules[key])
				{
					bool f = true;
					foreach (char item in value)
					{
						if (NonTerminals.Contains(item) && !generativeNonTerminals.Contains(item))
						{
							f = false;
							break;
						}
					}
					if (f)
					{
						generativeNonTerminals.Add(key);
					}
				}
			}
		} while (generativeNonTerminals.Count != countPrev);

		return generativeNonTerminals;
	}

	/// <summary>
	/// Получить набор непорождающих нетерминалов.
	/// </summary>
	/// <returns> набор непорождающих нетерминалов </returns>
	private HashSet<char> GetNonGenerativeNonTerminals()
	{
		HashSet<char> generativeNonTerminals = GetGenerativeNonTerminals();

		HashSet<char> nonGenerativeNonTerminals = new HashSet<char>();
		foreach (char nonTerminal in NonTerminals)
			if (!generativeNonTerminals.Contains(nonTerminal))
				nonGenerativeNonTerminals.Add(nonTerminal);

		return nonGenerativeNonTerminals;
	}

	/// <summary>
	/// Удалить правила, содержащие непорождающие нетерминалы.
	/// </summary>
	/// <param name="nonGenerativeNonTerminals"> набор непорождающих нетерминалов </param>
	private void RemoveRulesContainingNonGenerativeNonTerminals(HashSet<char> nonGenerativeNonTerminals)
	{
		foreach (char key in Rules.Keys)
		{
			bool rulesWereDeleted = false;
			foreach (string value in Rules[key])
			{
				bool ruleToDelete = false;
				foreach (char c in value)
				{
					if (nonGenerativeNonTerminals.Contains(c))
					{
						ruleToDelete = true;
						break;
					}
				}

				if (ruleToDelete)
				{
					if (!rulesWereDeleted)
					{
						if (nonGenerativeNonTerminals.Contains(key))
							Logger?.SetColor(ConsoleColor.Red);

						Logger?.Log(key);
						Logger?.SetColor(ConsoleColor.Gray);
						Logger?.Log(" -> ");

						rulesWereDeleted = true;
					}
					else
					{
						Logger?.Log(" | ");
					}

					foreach (char c in value)
					{
						if (nonGenerativeNonTerminals.Contains(c))
							Logger?.SetColor(ConsoleColor.Red);

						Logger?.Log($"{c}");
						Logger?.SetColor(ConsoleColor.Gray);
					}

					Rules[key].Remove(value);
					if (Rules[key].Count == 0)
						Rules.Remove(key);
				}
			}

			if (rulesWereDeleted)
				Logger?.LogLine();
		}

		UpdateTerminalLists();
	}
	#endregion

	#region Устранение недостижимых нетерминалов
	/// <summary>
	/// Устранить недостижимые нетерминалы.
	/// </summary>
	/// <returns> новая грамматика </returns>
	public ContextFreeGrammar RemoveUnreachableNonTerminals()
	{
		ContextFreeGrammar copyGrammar = new ContextFreeGrammar(StartNonTerminal, Rules, Logger);
		copyGrammar.RemoveUnreachableNonTerminalsInCurrentGrammar();
		return copyGrammar;
	}

	/// <summary>
	/// Устранить недостижимые нетерминалы в текущей грамматике.
	/// </summary>
	private void RemoveUnreachableNonTerminalsInCurrentGrammar()
	{
		Logger?.LogLine("УСТРАНЕНИЕ НЕДОСТИЖИМЫХ НЕТЕРМИНАЛОВ");

		HashSet<char> reachableNonTerminals = GetReachableNonTerminals();
		Logger?.Log("Множество достижимых нетерминалов: ");
		Logger?.SetColor(ConsoleColor.Green);
		foreach (char item in reachableNonTerminals)
			Logger?.Log(item + " ");
		Logger?.SetColor(ConsoleColor.Gray);
		Logger?.LogLine();

		HashSet<char> nonReachableNonTerminals = GetNonReachableNonTerminals();
		if (nonReachableNonTerminals.Count > 0)
		{
			Logger?.Log("Множество недостижимых нетерминалов: ");
			Logger?.SetColor(ConsoleColor.Red);
			foreach (char item in nonReachableNonTerminals)
				Logger?.Log(item + " ");
			Logger?.SetColor(ConsoleColor.Gray);
			Logger?.LogLine();
		}
		else
		{
			Logger?.LogLine("Недостижимые нетерминалы не найдены. Ничего удалять не нужно.\n");
			return;
		}

		Logger?.LogLine("Удалим правила, содержащие недостижимые нетерминалы в левой части");
		RemoveRulesContainingNonReachableNonTerminals(nonReachableNonTerminals);

		Logger?.LogLine("РЕЗУЛЬТАТ");
		Logger?.LogLine(this.ToString());
	}

	/// <summary>
	/// Получить набор достижимых нетерминалов.
	/// </summary>
	/// <returns> набор достижимых нетерминалов </returns>
	private HashSet<char> GetReachableNonTerminals()
	{
		HashSet<char> reachableNonTerminals = new HashSet<char>() { StartNonTerminal };

		int countPrev;
		do
		{
			countPrev = reachableNonTerminals.Count;
			foreach (char key in Rules.Keys)
			{
				if (!reachableNonTerminals.Contains(key))
					continue;

				foreach (string value in Rules[key])
					foreach (char c in value)
						if (NonTerminals.Contains(c) && !reachableNonTerminals.Contains(c))
							reachableNonTerminals.Add(c);
			}
		} while (reachableNonTerminals.Count != countPrev);

		return reachableNonTerminals;
	}

	/// <summary>
	/// Получить набор недостижимых нетерминалов.
	/// </summary>
	/// <returns> набор недостижимых нетерминалов </returns>
	private HashSet<char> GetNonReachableNonTerminals()
	{
		HashSet<char> reachableNonTerminals = GetReachableNonTerminals();

		HashSet<char> nonReachableNonTerminals = new HashSet<char>();
		foreach (char nonTerminal in NonTerminals)
			if (!reachableNonTerminals.Contains(nonTerminal))
				nonReachableNonTerminals.Add(nonTerminal);

		return nonReachableNonTerminals;
	}

	/// <summary>
	/// Удалить правила, содержащие недостижимые нетерминалы.
	/// </summary>
	/// <param name="nonReachableNonTerminals"> набор недостижимых нетерминалов </param>
	private void RemoveRulesContainingNonReachableNonTerminals(HashSet<char> nonReachableNonTerminals)
	{
		Logger?.SetColor(ConsoleColor.Red);
		foreach (char key in Rules.Keys)
		{
			if (nonReachableNonTerminals.Contains(key))
			{
				Logger?.Log($"{key} -> ");
				bool firstPrinted = false;
				foreach (string value in Rules[key])
					if (!firstPrinted)
					{
						Logger?.Log($"{value} ");
						firstPrinted = true;
					}
					else
					{
						Logger?.Log($"| {value} ");
					}
				Logger?.LogLine();
				Rules.Remove(key);
			}
		}
		Logger?.SetColor(ConsoleColor.Gray);

		UpdateTerminalLists();
	}
	#endregion

	#region Устранение бесполезных нетерминалов
	/// <summary>
	/// Устранить бесполезные нетерминалы.
	/// </summary>
	/// <returns> новая грамматика </returns>
	public ContextFreeGrammar RemoveUselessNonTerminals()
	{
		ContextFreeGrammar copyGrammar = new ContextFreeGrammar(StartNonTerminal, Rules, Logger);
		copyGrammar.RemoveUselessNonTerminalsInCurrentGrammar();
		return copyGrammar;
	}

	/// <summary>
	/// Устранить бесполезные нетерминалы в текущей грамматике.
	/// </summary>
	private void RemoveUselessNonTerminalsInCurrentGrammar()
	{
		RemoveNonGenerativeNonTerminalsInCurrentGrammar();
		RemoveUnreachableNonTerminalsInCurrentGrammar();
	}
	#endregion

	#region Устранение ε-правил
	/// <summary>
	/// Преобразовать в грамматику без ε-правил.
	/// </summary>
	/// <returns> новая грамматика </returns>
	public ContextFreeGrammar RemoveEmptyRules()
	{
		ContextFreeGrammar copyGrammar = new ContextFreeGrammar(StartNonTerminal, Rules, Logger);
		copyGrammar.RemoveEmptyRulesInCurrentGrammar();
		return copyGrammar;
	}

	/// <summary>
	/// Преобразовать текущую грамматику в грамматику без ε-правил.
	/// </summary>
	private void RemoveEmptyRulesInCurrentGrammar()
	{
		Logger?.LogLine("УСТРАНЕНИЕ ε-ПРАВИЛ");

		HashSet<char> emptyGenerativeNonTerminals = GetEmptyGenerativeNonTerminals();
		if (emptyGenerativeNonTerminals.Count > 0)
		{
			Logger?.Log("ε-порождающие нетерминалы: ");
			Logger?.SetColor(ConsoleColor.Red);
			foreach (char item in emptyGenerativeNonTerminals)
				Logger?.Log(item + " ");
			Logger?.SetColor(ConsoleColor.Gray);
			Logger?.LogLine();
		}
		else
		{
			Logger?.LogLine("ε-порождающие нетерминалы не найдены. Ничего удалять не нужно.\n");
			return;
		}

		AddNewRulesForEmptyGenerativeNonTerminals(emptyGenerativeNonTerminals);

		RemoveAllEmptyRules();

		Logger?.LogLine("РЕЗУЛЬТАТ");
		Logger?.LogLine(this.ToString());
	}

	/// <summary>
	/// Получить набор ε-порождающих нетерминалов.
	/// </summary>
	/// <returns> набор ε-порождающих нетерминалов </returns>
	private HashSet<char> GetEmptyGenerativeNonTerminals()
	{
		HashSet<char> emptyGenerativeNonTerminals = new HashSet<char>();
		int countPrev;
		do
		{
			countPrev = emptyGenerativeNonTerminals.Count;
			foreach (char key in Rules.Keys)
			{
				foreach (string value in Rules[key])
				{
					bool f = true;
					foreach (char item in value)
					{
						if (!emptyGenerativeNonTerminals.Contains(item))
						{
							f = false;
							break;
						}
					}
					if (f)
					{
						emptyGenerativeNonTerminals.Add(key);
					}
				}
			}
		} while (emptyGenerativeNonTerminals.Count != countPrev);

		return emptyGenerativeNonTerminals;
	}

	/// <summary>
	/// Добавить новые правила, которые получатся после удаления ε-порождающих нетерминалов.
	/// </summary>
	/// <param name="emptyGenerativeNonTerminals"> набор ε-порождающих нетерминалов </param>
	private void AddNewRulesForEmptyGenerativeNonTerminals(HashSet<char> emptyGenerativeNonTerminals)
	{
		bool addStartEmptyRule = false;
		bool addedAtLeastOneRule = false;
		foreach (char key in Rules.Keys)
		{
			for (int i = 0; i < Rules[key].Count; i++)
			{
				string value = Rules[key].ToList()[i];

				HashSet<string> toAdd = GetPermutations(emptyGenerativeNonTerminals, value);
				if (key == StartNonTerminal && value == "")
					toAdd.Add("");

				bool newRowAdded = false;
				foreach (string ruleToAdd in toAdd)
				{
					if (ruleToAdd == "" && key == StartNonTerminal)
						addStartEmptyRule = true;
					else if (ruleToAdd != "" && key.ToString() != ruleToAdd && !Rules[key].Contains(ruleToAdd))
					{
						if (!addedAtLeastOneRule)
						{
							Logger?.LogLine("Добавим новые правила");
							addedAtLeastOneRule = true;
						}

						if (!newRowAdded)
						{
							Logger?.LogWithColor(ConsoleColor.Green, $"{key} -> ");
							Logger?.LogWithColor(ConsoleColor.Green, $"{ruleToAdd} ");
							newRowAdded = true;
						}
						else
						{
							Logger?.LogWithColor(ConsoleColor.Green, $"| {ruleToAdd} ");
						}

						Rules[key].Add(ruleToAdd);
					}
				}

				if (newRowAdded)
				{
					Logger?.Log($"(для правила {key} -> ");
					foreach (char c in value)
						if (emptyGenerativeNonTerminals.Contains(c))
							Logger?.LogWithColor(ConsoleColor.Red, c);
						else
							Logger?.LogWithColor(ConsoleColor.Gray, c);
					Logger?.LogLine(")");
				}
			}
		}

		if (addStartEmptyRule)
		{
			if (!addedAtLeastOneRule)
				Logger?.LogLine("Добавим новые правила");

			Logger?.LogWithColor(ConsoleColor.Green, $"$ -> ε {StartNonTerminal} ");
			Logger?.LogLine("(т.к. в исходной грамматике выводилось ε)");

			Rules.Add('$', new HashSet<string>() { "", StartNonTerminal.ToString() });
			StartNonTerminal = '$';
			NonTerminals.Add('$');
		}

		UpdateTerminalLists();
	}

	/// <summary>
	/// Получить набор правил, которые получатся после удаления из него ε-порождающих нетерминалов.
	/// </summary>
	/// <param name="emptyGenerativeCharacters"> набор ε-порождающих нетерминалов </param>
	/// <param name="rule"> правило </param>
	/// <returns> набор новых правил </returns>
	private HashSet<string> GetPermutations(HashSet<char> emptyGenerativeCharacters, string rule) 
	{
		HashSet<string> result = new HashSet<string>();

		List<int> indexes = new List<int>();
		for (int i = 0; i < rule.Length; i++)
			if (emptyGenerativeCharacters.Contains(rule[i]))
				indexes.Add(i);

		if (indexes.Count > 0)
		{
			List<List<bool>> boolLists = GenerateBooleanLists(indexes.Count);
			foreach (List<bool> boolList in boolLists)
			{
				string newRuleToAdd = new string(rule);
				for (int i = boolList.Count - 1; i >= 0; i--)
					if (boolList[i])
						newRuleToAdd = newRuleToAdd.Substring(0, indexes[i]) + "" + newRuleToAdd.Substring(indexes[i] + 1);
				result.Add(newRuleToAdd);
			}
		}

		return result;
	}

	/// <summary>
	/// Получить все возможные булевые списки длины n, кроме всех false.
	/// </summary>
	/// <param name="n"> длина списка </param>
	/// <returns> булевые списки длины n </returns>
	private static List<List<bool>> GenerateBooleanLists(int n)
	{
		List<List<bool>> result = new List<List<bool>>();
		GenerateBooleanListsHelper(n, new List<bool>(), result);
		return result;
	}

	/// <summary>
	/// Рекурсивный метод генерации булевых списков.
	/// </summary>
	/// <param name="n"> длина списка </param>
	/// <param name="currentList"> текущий список </param>
	/// <param name="result"> результирующий набор </param>
	private static void GenerateBooleanListsHelper(int n, List<bool> currentList, List<List<bool>> result)
	{
		if (currentList.Count == n)
		{
			if (!currentList.All(b => !b))
			{
				result.Add(new List<bool>(currentList));
			}
			return;
		}

		currentList.Add(true);
		GenerateBooleanListsHelper(n, currentList, result);
		currentList.RemoveAt(currentList.Count - 1);

		currentList.Add(false);
		GenerateBooleanListsHelper(n, currentList, result);
		currentList.RemoveAt(currentList.Count - 1);
	}

	/// <summary>
	/// Удалить из грамматики все правила вида A -> ε, где A - нетерминал (кроме начального)
	/// </summary>
	private void RemoveAllEmptyRules()
	{
		Logger?.LogLine("Удалим ε-правила");
		Logger?.SetColor(ConsoleColor.Red);
		foreach (char key in Rules.Keys)
		{
			if (key != StartNonTerminal && Rules[key].Contains(""))
			{
				Logger?.LogLine($"{key} -> ε");
				Rules[key].Remove("");
				if (Rules[key].Count == 0)
					Rules.Remove(key);
			}
		}
		Logger?.SetColor(ConsoleColor.Gray);

		UpdateTerminalLists();
	}
	#endregion

	#region Устранение цепных правил
	/// <summary>
	/// Устранить цепные правила.
	/// </summary>
	/// <returns> новая грамматика </returns>
	public ContextFreeGrammar RemoveChainRules()
	{
		ContextFreeGrammar copyGrammar = new ContextFreeGrammar(StartNonTerminal, Rules, Logger);
		copyGrammar.RemoveChainRulesInCurrentGrammar();
		return copyGrammar;
	}

	/// <summary>
	/// Устранить цепные правила в текущей грамматике.
	/// </summary>
	private void RemoveChainRulesInCurrentGrammar()
	{
		Logger?.LogLine("УСТРАНЕНИЕ ЦЕПНЫХ ПРАВИЛ");

		Dictionary<char, HashSet<char>> chainRules = GetChainRules();
		if (chainRules.Keys.Count > 0)
		{
			Logger?.LogLine("Цепные правивила, имеющиеся непосредственно в грамматике (без базисов)");
			Logger?.SetColor(ConsoleColor.Green);
			foreach (char key in chainRules.Keys)
			{
				Logger?.Log($"{key} -> ");
				bool firstPrinted = false;
				foreach (char value in chainRules[key])
					if (!firstPrinted)
					{
						Logger?.Log($"{value} ");
						firstPrinted = true;
					}
					else
					{
						Logger?.Log($"| {value} ");
					}
				Logger?.LogLine();
			}
			Logger?.SetColor(ConsoleColor.Gray);
		}
		else
		{
			Logger?.LogLine("Цепные правила не найдены. Ничего удалять не нужно.\n");
			return;
		}

		Dictionary<char, HashSet<char>> allChainRules = new Dictionary<char, HashSet<char>>();
		foreach (char nonTerminal in chainRules.Keys)
			allChainRules[nonTerminal] = GetReachableVertices(chainRules, nonTerminal);

		Logger?.LogLine("Полное множество цепных правил (без базисов)");
		Logger?.SetColor(ConsoleColor.Green);
		foreach (char key in allChainRules.Keys)
		{
			Logger?.Log($"{key} -> ");
			bool firstPrinted = false;
			foreach (char value in allChainRules[key])
				if (!firstPrinted)
				{
					Logger?.Log($"{value} ");
					firstPrinted = true;
				}
				else
				{
					Logger?.Log($"| {value} ");
				}
			Logger?.LogLine();
		}
		Logger?.SetColor(ConsoleColor.Gray);

		Dictionary<(char, char), HashSet<string>> rulesToAdd = GetRulesToAddForChainRules(allChainRules);
		AddNewRulesForChainRules(rulesToAdd);

		Logger?.LogLine("Удалим цепные правила");
		RemoveAllChainRules(chainRules);

		Logger?.LogLine("РЕЗУЛЬТАТ");
		Logger?.LogLine(this.ToString());
	}

	/// <summary>
	/// Получить цепные правила в грамматике.
	/// </summary>
	/// <returns> цепные правила в грамматике </returns>
	private Dictionary<char, HashSet<char>> GetChainRules()
	{
		Dictionary<char, HashSet<char>> chainRules = new Dictionary<char, HashSet<char>>();
		foreach (char key in Rules.Keys)
			foreach (string value in Rules[key])
				if (value.Length == 1 && NonTerminals.Contains(value[0]))
					if (!chainRules.ContainsKey(key))
						chainRules[key] = new HashSet<char>() { value[0] };
					else
						chainRules[key].Add(value[0]);
		return chainRules;
	}

	/// <summary>
	/// Получить все нетерминалы, которые можно получить из заданного нетерминала (цепные правила).
	/// </summary>
	/// <param name="chainRules"> цепные правила </param>
	/// <param name="startVertex"> начальный нетерминал </param>
	/// <returns> все нетерминалы, которые можно получить из заданного нетерминала </returns>
	private static HashSet<char> GetReachableVertices(Dictionary<char, HashSet<char>> chainRules, char startVertex)
	{
		HashSet<char> visited = new HashSet<char>();
		Stack<char> stack = new Stack<char>();
		stack.Push(startVertex);

		while (stack.Count > 0)
		{
			char currentVertex = stack.Pop();
			if (!visited.Contains(currentVertex))
			{
				visited.Add(currentVertex);
				if (chainRules.ContainsKey(currentVertex))
					foreach (char neighbor in chainRules[currentVertex])
						stack.Push(neighbor);
			}
		}

		visited.Remove(startVertex);
		return visited;
	}

	/// <summary>
	/// Получить все правила, которые нужно добавить для преобразования грамматики в грамматику без цепных правил.
	/// </summary>
	/// <param name="allChainRules"> цепные правила </param>
	/// <returns> правила, которые нужно добавить для преобразования грамматики в грамматику без цепных правил </returns>
	private Dictionary<(char, char), HashSet<string>> GetRulesToAddForChainRules(Dictionary<char, HashSet<char>> allChainRules)
	{
		Dictionary<(char, char), HashSet<string>> rulesToAdd = new Dictionary<(char, char), HashSet<string>>();
		foreach (char from in allChainRules.Keys)
			foreach (char to in allChainRules[from])
			{
				if (!Rules.ContainsKey(to))
					continue;

				foreach (string value in Rules[to])
				{
					if (value.Length == 1 && NonTerminals.Contains(value[0]) && allChainRules[from].Contains(value[0]) &&
						!Rules[from].Contains(value) || from.ToString() == value)
						continue;

					if (rulesToAdd.ContainsKey((from, to)))
						rulesToAdd[(from, to)].Add(value);
					else
						rulesToAdd[(from, to)] = new HashSet<string>() { value };
				}
			}
		return rulesToAdd;
	}

	/// <summary>
	/// Добавить новые правила для преобразования грамматики в грамматику без цепных правил.
	/// </summary>
	/// <param name="rulesToAdd"> правила, которые нужно добавить </param>
	private void AddNewRulesForChainRules(Dictionary<(char, char), HashSet<string>> rulesToAdd)
	{
		bool addedAtLeastOneRule = false;
		foreach ((char, char) key in rulesToAdd.Keys)
		{
			bool newRowAdded = false;
			foreach (var value in rulesToAdd[key])
			{
				if (!Rules.ContainsKey(key.Item1))
				{
					if (!addedAtLeastOneRule)
					{
						Logger?.LogLine("Добавим новые правила");
						addedAtLeastOneRule = true;
					}

					if (!newRowAdded)
					{
						Logger?.LogWithColor(ConsoleColor.Green, $"{key.Item1} -> ");
						Logger?.LogWithColor(ConsoleColor.Green, $"{value} ");
						newRowAdded = true;
					}
					else
					{
						Logger?.LogWithColor(ConsoleColor.Green, $"| {value} ");
					}

					Rules[key.Item1] = new HashSet<string>() { value };
				}
				else if (!Rules[key.Item1].Contains(value))
				{
					if (!addedAtLeastOneRule)
					{
						Logger?.LogLine("Добавим новые правила");
						addedAtLeastOneRule = true;
					}

					if (!newRowAdded)
					{
						Logger?.LogWithColor(ConsoleColor.Green, $"{key.Item1} -> ");
						Logger?.LogWithColor(ConsoleColor.Green, $"{value} ");
						newRowAdded = true;
					}
					else
					{
						Logger?.LogWithColor(ConsoleColor.Green, $"| {value} ");
					}

					Rules[key.Item1].Add(value);
				}
			}
			Logger?.LogLine($"(для правила {key.Item1} -> {key.Item2})");
		}

		UpdateTerminalLists();
	}

	/// <summary>
	/// Удалить цепные правила из грамматики.
	/// </summary>
	/// <param name="chainRules"> цепные правила </param>
	private void RemoveAllChainRules(Dictionary<char, HashSet<char>> chainRules)
	{
		Logger?.SetColor(ConsoleColor.Red);
		foreach (char key in chainRules.Keys)
		{
			bool firstPrinted = false;
			Logger?.Log($"{key} -> ");
			foreach (char value in chainRules[key])
			{
				if (!firstPrinted)
				{
					Logger?.Log($"{value} ");
					firstPrinted = true;
				}
				else
				{
					Logger?.Log($"| {value} ");
				}

				Rules[key].Remove(value.ToString());
				if (Rules[key].Count == 0)
					Rules.Remove(key);
			}
			Logger?.LogLine();
		}
		Logger?.SetColor(ConsoleColor.Gray);

		UpdateTerminalLists();
	}
	#endregion

	#region Устранение длинных правил
	/// <summary>
	/// Устранить длинные правила.
	/// </summary>
	/// <returns> новая грамматика </returns>
	public ContextFreeGrammar RemoveLongRules()
	{
		ContextFreeGrammar copyGrammar = new ContextFreeGrammar(StartNonTerminal, Rules, Logger);
		copyGrammar.RemoveLongRulesInCurrentGrammar();
		return copyGrammar;
	}

	/// <summary>
	/// Устранить длинные правила в текущей грамматике.
	/// </summary>
	/// <exception cref="Exception"> исключение, возникающее, если у грамматики закончились допустимые нетерминальные символы </exception>
	private void RemoveLongRulesInCurrentGrammar()
	{
		Logger?.LogLine("УСТРАНЕНИЕ ДЛИННЫХ ПРАВИЛ");

		bool foundAtLeastOneLongRule = false;
		Dictionary<char, HashSet<string>> rulesToAdd = new Dictionary<char, HashSet<string>>();
		Dictionary<string, char> shortcutRuleFromNonTerminal = new Dictionary<string, char>();
		foreach (char key in Rules.Keys)
		{
			foreach (string value in Rules[key])
			{
				if (value.Length <= 2)
					continue;
				
				if (!foundAtLeastOneLongRule)
				{
					Logger?.LogLine("Будем удалять длинные правила и для них добавлять короткие правила");
					foundAtLeastOneLongRule = true;
				}

				Rules[key].Remove(value);
				Logger?.LogWithColor(ConsoleColor.Red, $"{key} -> {value}  ");

				string[] longRuleParts = new string[value.Length - 1];
				for (int i = 0; i < longRuleParts.Length; i++)
					longRuleParts[i] = value[i].ToString();
				longRuleParts[longRuleParts.Length - 1] += value[value.Length - 1];

				char prevNonTerminal = key;
				char? tempNonTerminal = GetUnusedNonTerminal();
				if (tempNonTerminal == null)
					throw new Exception("Закончились нетерминалы");
				char currNonTerminal = tempNonTerminal.Value;

				string longRuleRemain = value;
				bool shortcutFound = false;
				for (int i = 0; i < longRuleParts.Length - 1; i++)
				{
					longRuleRemain = longRuleRemain.Substring(1, longRuleRemain.Length - 1);
					char? shortcutNonTerminal = null;
					foreach (char nonTerminal in Rules.Keys)
						if (Rules[nonTerminal].Count == 1 && Rules[nonTerminal].First() == longRuleRemain)
						{
							shortcutNonTerminal = nonTerminal;
							break;
						}

					if (shortcutRuleFromNonTerminal.ContainsKey(longRuleRemain))
						shortcutNonTerminal = shortcutRuleFromNonTerminal[longRuleRemain];

					if (shortcutNonTerminal != null)
					{
						if (!rulesToAdd.ContainsKey(prevNonTerminal))
							rulesToAdd[prevNonTerminal] = new HashSet<string>() { longRuleParts[i] + shortcutNonTerminal };
						else
							rulesToAdd[prevNonTerminal].Add(longRuleParts[i] + shortcutNonTerminal);

						Logger?.LogWithColor(ConsoleColor.Green, $"{prevNonTerminal} -> {longRuleParts[i] + shortcutNonTerminal}  ");
						shortcutFound = true;
						break;
					}

					if (!rulesToAdd.ContainsKey(prevNonTerminal))
						rulesToAdd[prevNonTerminal] = new HashSet<string>() { longRuleParts[i] + currNonTerminal };
					else
						rulesToAdd[prevNonTerminal].Add(longRuleParts[i] + currNonTerminal);

					shortcutRuleFromNonTerminal[longRuleRemain] = currNonTerminal;
					Logger?.LogWithColor(ConsoleColor.Green, $"{prevNonTerminal} -> {longRuleParts[i] + currNonTerminal}  ");
					NonTerminals.Add(currNonTerminal);

					prevNonTerminal = currNonTerminal;
					tempNonTerminal = GetUnusedNonTerminal();
					if (tempNonTerminal == null)
						throw new Exception("Закончились нетерминалы");
					currNonTerminal = tempNonTerminal.Value;
				}

				if (!shortcutFound)
				{
					if (!rulesToAdd.ContainsKey(prevNonTerminal))
						rulesToAdd[prevNonTerminal] = new HashSet<string>() { longRuleParts[longRuleParts.Length - 1] };
					else
						rulesToAdd[prevNonTerminal].Add(longRuleParts[longRuleParts.Length - 1]);
					Logger?.LogLineWithColor(ConsoleColor.Green, $"{prevNonTerminal} -> {longRuleParts[longRuleParts.Length - 1]}");
				}
				else
				{
					Logger?.LogLine();
				}
			}
		}

		if (!foundAtLeastOneLongRule)
		{
			Logger?.LogLine("Длинные правила не найдены. Ничего удалять не нужно.\n");
			return;
		}

		foreach (char key in rulesToAdd.Keys)
			foreach (var value in rulesToAdd[key])
				if (!Rules.ContainsKey(key))
					Rules[key] = new HashSet<string>() { value };
				else
					Rules[key].Add(value);

		UpdateTerminalLists();

		Logger?.LogLine("РЕЗУЛЬТАТ");
		Logger?.LogLine(this.ToString());
	}

	/// <summary>
	/// Получить новый доступный нетерминал.
	/// </summary>
	/// <returns> нетерминал </returns>
	private char? GetUnusedNonTerminal()
	{
		foreach (char c in AcceptableNonTerminals)
			if (c != '$' && !NonTerminals.Contains(c))
				return c;
		return null;
	}
	#endregion

	#region Избавление от нескольких терминалов в правиле
	/// <summary>
	/// Избавиться от нескольких терминалов в правиле.
	/// </summary>
	/// <returns> новая грамматика </returns>
	public ContextFreeGrammar RemoveMultipleTerminals()
	{
		ContextFreeGrammar copyGrammar = new ContextFreeGrammar(StartNonTerminal, Rules, Logger);
		copyGrammar.RemoveMultipleTerminalsInCurrentGrammar();
		return copyGrammar;
	}

	/// <summary>
	/// Избавиться от нескольких терминалов в правиле в текущей грамматике.
	/// </summary>
	private void RemoveMultipleTerminalsInCurrentGrammar()
	{
		Logger?.LogLine("ИЗБАВЛЕНИЕ ОТ НЕСКОЛЬКИХ ТЕРМИНАЛОВ В ПРАВИЛЕ");

		bool foundAtLeastOneRule = false;
		Dictionary<char, char> replaceFromTerminalToNonTerminal = new Dictionary<char, char>();
		List<(char, string)> rulesToAdd = new List<(char, string)>();
		foreach (char key in Rules.Keys)
			foreach (string value in Rules[key])
			{
				if (value.Length != 2)
					continue;

				bool ruleWasDeleted = false;
				List<(char, char)> rulesToAddFromNonTerminalToTerminal = new List<(char, char)>();
				string twoNonTerminals = "";
				for (int i = 0; i < 2; i++)
				{
					char currentSumbol = value[i];
					if (Terminals.Contains(currentSumbol))
					{
						if (!foundAtLeastOneRule)
						{
							Logger?.LogLine("Будем удалять правила c несколькими терминалами и для них добавлять новые правила");
							foundAtLeastOneRule = true;
						}

						if (!ruleWasDeleted)
						{
							Logger?.LogWithColor(ConsoleColor.Red, $"{key} -> {value}  ");
							ruleWasDeleted = true;
						}

						(char nonTerminal, List<(char, char)> rulesToAddFromTerminalToNonTerminal) tuple = ProcessTerminal(currentSumbol, replaceFromTerminalToNonTerminal, rulesToAdd);
						twoNonTerminals += tuple.nonTerminal;
						rulesToAddFromNonTerminalToTerminal.AddRange(tuple.rulesToAddFromTerminalToNonTerminal);
					}
					else
					{
						twoNonTerminals += currentSumbol;
					}
				}

				if (value != twoNonTerminals)
				{
					Rules[key].Remove(value);
					if (Rules[key].Count == 0)
						Rules.Remove(key);
					rulesToAdd.Add((key, twoNonTerminals));

					Logger?.SetColor(ConsoleColor.Green);
					Logger?.Log($"{key} -> {twoNonTerminals}  ");
					foreach ((char, char) item in rulesToAddFromNonTerminalToTerminal)
						Logger?.Log($"{item.Item1} -> {item.Item2}  ");
					Logger?.LogLine();
					Logger?.SetColor(ConsoleColor.Gray);
				}
			}

		if (!foundAtLeastOneRule)
		{
			Logger?.LogLine("Правила с несколькими терминалами не найдены. Ничего удалять не нужно.\n");
			return;
		}

		foreach ((char, string) ruleParts in rulesToAdd)
			if (!Rules.ContainsKey(ruleParts.Item1))
				Rules[ruleParts.Item1] = new HashSet<string>() { ruleParts.Item2 };
			else
				Rules[ruleParts.Item1].Add(ruleParts.Item2);

		UpdateTerminalLists();

		Console.WriteLine("РЕЗУЛЬТАТ");
		Console.WriteLine(this.ToString());
	}

	/// <summary>
	/// Обработать терминальный символ.
	/// </summary>
	/// <param name="terminal"> терминал </param>
	/// <param name="replaceFromTerminalToNonTerminal"> словарь замен </param>
	/// <param name="rulesToAdd"> правила, которые нужно добавить </param>
	/// <returns> 1 - нетерминал, на который произошла замена, 2 - новые правила </returns>
	/// <exception cref="Exception"> исключение, возникающее, если допустимые нетерминальные символы закончились </exception>
	private (char, List<(char, char)>) ProcessTerminal(char terminal, Dictionary<char, char> replaceFromTerminalToNonTerminal, List<(char, string)> rulesToAdd)
	{
		List<(char, char)> rulesToAddFromTerminalToNonTerminal = new List<(char, char)>();
		if (!replaceFromTerminalToNonTerminal.ContainsKey(terminal))
		{
			char nonTerminal = ' ';
			foreach (char k in Rules.Keys)
				if (Rules[k].Count == 1 && Rules[k].First() == terminal.ToString())
				{
					nonTerminal = k;
					break;
				}

			if (nonTerminal == ' ')
			{
				char? temp = GetUnusedNonTerminal();
				if (temp == null)
					throw new Exception("Нетерминалы закончились");
				nonTerminal = temp.Value;
				NonTerminals.Add(nonTerminal);
			}

			replaceFromTerminalToNonTerminal[terminal] = nonTerminal;
			if (!rulesToAdd.Contains((nonTerminal, terminal.ToString())))
				rulesToAddFromTerminalToNonTerminal.Add((nonTerminal, terminal));
			rulesToAdd.Add((nonTerminal, terminal.ToString()));

			return (nonTerminal, rulesToAddFromTerminalToNonTerminal);
		}
		else
		{
			char nonTerminal = replaceFromTerminalToNonTerminal[terminal];
			if (!rulesToAdd.Contains((nonTerminal, terminal.ToString())))
				rulesToAddFromTerminalToNonTerminal.Add((nonTerminal, terminal));
			rulesToAdd.Add((nonTerminal, terminal.ToString()));

			return (nonTerminal, rulesToAddFromTerminalToNonTerminal);
		}
	}
	#endregion

	#region Упростить грамматику
	/// <summary>
	/// Упростить грамматику.
	/// </summary>
	/// <returns> новая грамматика </returns>
	public ContextFreeGrammar SimplifyGrammar()
	{
		ContextFreeGrammar copyGrammar = new ContextFreeGrammar(StartNonTerminal, Rules, Logger);
		copyGrammar = copyGrammar.RemoveEmptyRules();
		copyGrammar = copyGrammar.RemoveChainRules();
		copyGrammar = copyGrammar.RemoveNonGenerativeNonTerminals();
		copyGrammar = copyGrammar.RemoveUnreachableNonTerminals();
		return copyGrammar;
	}

	/// <summary>
	/// Привести грамматику к нормальной форме Хомского.
	/// </summary>
	/// <returns> новая грамматика </returns>
	public ContextFreeGrammar ConvertGrammarToChomskyNormalForm()
	{
		ContextFreeGrammar copyGrammar = new ContextFreeGrammar(StartNonTerminal, Rules, Logger);
		copyGrammar = copyGrammar.RemoveLongRules();
		copyGrammar = copyGrammar.RemoveEmptyRules();
		copyGrammar = copyGrammar.RemoveChainRules();
		copyGrammar = copyGrammar.RemoveNonGenerativeNonTerminals();
		copyGrammar = copyGrammar.RemoveUnreachableNonTerminals();
		copyGrammar = copyGrammar.RemoveMultipleTerminals();
		return copyGrammar;
	}
	#endregion

	#region Составление слов
	/// <summary>
	/// Получить список найденных слов в грамматике.
	/// </summary>
	/// <param name="cancellationToken"> объект для отмены метода </param>
	/// <param name="chainLengthLimit"> максимальная длина выводимой цепочки </param>
	/// <returns> список найденных слов в грамматике </returns>
	public List<string> MakeWords(CancellationToken cancellationToken, int chainLengthLimit = int.MaxValue)
	{
		bool wordFound = false;
		HashSet<string> resultWords = new HashSet<string>();
		HashSet<string> currWords = new HashSet<string>() { StartNonTerminal.ToString() };
		HashSet<string> nextWords = new HashSet<string>();

		while (true)
		{
			if (currWords.Count == 0)
				break;

			foreach (string word in currWords)
			{
				if (cancellationToken.IsCancellationRequested)
					break;

				bool canReplace = false;
				foreach (char ruleChar in Rules.Keys)
				{
					int index = word.IndexOf(ruleChar);
					if (index != -1)
					{
						foreach (string replaceWith in Rules[ruleChar])
						{
							string nextWord = word.Remove(index, 1).Insert(index, replaceWith);
							if (nextWord.Length > chainLengthLimit)
								continue;
							nextWords.Add(nextWord);
							canReplace = true;
						}
					}
				}

				if (!canReplace && word.All(c => Terminals.Contains(c)) && !resultWords.Contains(word))
				{
					if (!wordFound)
					{
						Logger?.LogLine("Найденные слова:");
						wordFound = true;
					}

					Logger?.LogLine(word != "" ? $"{ChangeStringFormat(word)}: {word}" : "ε");

					resultWords.Add(word);
				}
			}

			currWords = new HashSet<string>(nextWords);
			nextWords = new HashSet<string>();
		}

		return resultWords.OrderBy(w => w.Length).ThenBy(w => w).ToList();
	}

	/// <summary>
	/// Изменяет вид строки. Например строка "aaabbc" превратится в "3a2b1c".
	/// </summary>
	/// <param name="input"> входная строка </param>
	/// <returns> измененная строка </returns>
	private static string ChangeStringFormat(string input)
	{
		if (string.IsNullOrEmpty(input))
			return string.Empty;

		StringBuilder result = new StringBuilder();
		char currentChar = input[0];
		int count = 1;
		for (int i = 1; i < input.Length; i++)
		{
			if (input[i] == currentChar)
			{
				count++;
			}
			else
			{
				result.Append($"{count}{currentChar}");
				currentChar = input[i];
				count = 1;
			}
		}
		result.Append($"{count}{currentChar}");

		return result.ToString();
	}
	#endregion

	#region Составление цепочек
	/// <summary>
	/// Получить список найденных слов в грамматике с полными цепочками их вывода.
	/// </summary>
	/// <param name="cancellationToken"> объект для отмены метода </param>
	/// <param name="chainLengthLimit"> максимальная длина выводимой цепочки </param>
	/// <returns> список найденных слов в грамматике с полными цепочками их вывода </returns>
	public List<List<string>> MakeWordsDetailed(CancellationToken cancellationToken, int chainLengthLimit = int.MaxValue)
	{
		bool chainFound = false;
		HashSet<string> resultWords = new HashSet<string>();
		HashSet<string> currWords = new HashSet<string>() { StartNonTerminal.ToString() };
		HashSet<string> nextWords = new HashSet<string>();

		List<List<string>> resultChains = new List<List<string>>();
		List<List<string>> currChains = new List<List<string>>() { new List<string>() { StartNonTerminal.ToString() } };
		List<List<string>> nextChains = new List<List<string>>();

		while (true)
		{
			if (currChains.Count == 0)
				break;

			foreach (List<string> chain in currChains)
			{
				if (cancellationToken.IsCancellationRequested)
					break;

				string word = chain[chain.Count - 1];

				bool canReplace = false;
				foreach (char ruleString in Rules.Keys)
				{
					int index = word.IndexOf(ruleString);
					if (index != -1)
					{
						foreach (string replaceWith in Rules[ruleString])
						{
							string nextWord = word.Remove(index, 1).Insert(index, replaceWith);
							if (nextWord.Length > chainLengthLimit || nextWords.Contains(nextWord))
								continue;
							nextWords.Add(nextWord);
							List<string> nextChain = new List<string>(chain);
							nextChain.Add(nextWord);
							nextChains.Add(nextChain);
							canReplace = true;
						}
					}
				}

				if (!canReplace && word.All(c => Terminals.Contains(c)) && !resultWords.Contains(word))
				{
					if (!chainFound)
					{
						Logger?.LogLine("Найденные цепочки:");
						chainFound = true;
					}

					for (int i = 0; i < chain.Count - 1; i++)
						Logger?.Log($"{(chain[i] == "" ? "ε" : chain[i])} => ");
					Logger?.LogLineWithColor(ConsoleColor.Green, chain[chain.Count - 1] == "" ? "ε" : chain[chain.Count - 1]);

					resultChains.Add(chain);
					resultWords.Add(word);
				}
			}

			currChains = new List<List<string>>(nextChains);
			nextChains = new List<List<string>>();
			currWords = new HashSet<string>(nextWords);
			nextWords = new HashSet<string>();
		}

		return resultChains.OrderBy(x => x.Count).ToList();
	}
	#endregion

	#region Составление цепочки по конкретному слову
	/// <summary>
	/// Получить цепочку вывода конкретного слова.
	/// </summary>
	/// <param name="word"> слово </param>
	/// <returns> цепочка вывода </returns>
	public List<string> MakeWordDetailed(string word)
	{
		HashSet<string> currWords = new HashSet<string>() { StartNonTerminal.ToString() };
		HashSet<string> nextWords = new HashSet<string>();

		List<List<string>> currChains = new List<List<string>>() { new List<string>() { StartNonTerminal.ToString() } };
		List<List<string>> nextChains = new List<List<string>>();

		while (true)
		{
            foreach (List<string> chain in currChains)
			{
				string currWord = chain[chain.Count - 1];

				bool canReplace = false;
				foreach (char ruleString in Rules.Keys)
				{
					int index = currWord.IndexOf(ruleString);
					if (index != -1)
					{
						foreach (string replaceWith in Rules[ruleString])
						{
							string nextWord = currWord.Remove(index, 1).Insert(index, replaceWith);
							if (nextWord.Where(c => Terminals.Contains(c)).Count() > word.Length || nextWords.Contains(nextWord))
								continue;
							nextWords.Add(nextWord);
							List<string> nextChain = new List<string>(chain);
							nextChain.Add(nextWord);
							nextChains.Add(nextChain);
							canReplace = true;
						}
					}
				}

				if (!canReplace && currWord.All(c => Terminals.Contains(c)) && currWord == word)
				{
					return chain;
				}
			}

			currChains = new List<List<string>>(nextChains);
			nextChains = new List<List<string>>();
			currWords = new HashSet<string>(nextWords);
			nextWords = new HashSet<string>();
		}
	}
	#endregion

	#region Найти самые короткие слова в грамматике
	/// <summary>
	/// Получить самые короткие слова в грамматике.
	/// </summary>
	/// <returns> самые короткие слова в грамматике </returns>
	public List<string> GetShortestWords()
	{
		List<string> result = new List<string>();

		if (Rules.Keys.Count == 0)
			return result;

		if (Rules[StartNonTerminal].Contains(""))
		{
			result.Add("");
			return result;
		}

		List<(string, int)> currChains = new List<(string, int)>() { (StartNonTerminal.ToString(), 0) };
		List<(string, int)> nextChains = new List<(string, int)>();

		while (true)
		{
			foreach ((string, int) currChain in currChains)
			{
				List<string> words = new List<string>() { currChain.Item1 };
				int index = currChain.Item2;

				while (true)
				{
					List<char> terminalsThatCanReplaceCurrentNonTerminal = GetTerminalsThatCanReplaceNonTerminal(words[0][index]);
					if (terminalsThatCanReplaceCurrentNonTerminal.Count != 0)
					{
						List<string> newWords = new List<string>();
						foreach (string word in words)
							foreach (char replaceChar in terminalsThatCanReplaceCurrentNonTerminal)
								newWords.Add(word.Remove(index, 1).Insert(index, replaceChar.ToString()));

						words = new List<string>(newWords);
						index++;
						if (index == words[0].Length)
						{
							foreach (string word in words)
								result.Add(word);
							break;
						}
					}
					else
					{
						foreach (string value in Rules[words[0][index]])
							foreach (string word in words)
							{
								string nextWord = word.Remove(index, 1).Insert(index, value);
								nextChains.Add((nextWord, index));
							}
						break;
					}
				}
			}

			if (result.Count > 0)
				return result;

			currChains = new List<(string, int)>(nextChains);
			nextChains = new List<(string, int)>();
		}
	}

	/// <summary>
	/// Получить терминалы, которые можно получить из нетерминала.
	/// </summary>
	/// <param name="nonTerminal"> нетерминал </param>
	/// <returns> список терминалов </returns>
	public List<char> GetTerminalsThatCanReplaceNonTerminal(char nonTerminal)
	{
		List<char> terminals = new List<char>();
		if (Rules.ContainsKey(nonTerminal))
			foreach (string item in Rules[nonTerminal])
				if (item.Length == 1 && Terminals.Contains(item[0]))
					terminals.Add(item[0]);
		return terminals;
	}
	#endregion
}
