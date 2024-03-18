using GrammarLibrary;

// Выберите нужное действие:
// MakeWords - составление слов по известным правилам (из input.txt, для отмены можно нажать Enter)
// MakeWordsDetailed - составление слов по известным правилам с полным процессом составления цепочек (из input.txt, для отмены можно нажать Enter)
// SimplifyGrammar - приведение грамматики к приведенной форме (из input.txt) 
// ConvertToChomskyNormalForm - приведение грамматики к нормальной форме Хомского (из input.txt)  
// FindShortestWords - поиск самых коротких слов в грамматике (из input.txt) 
ActionType actionType = ActionType.FindShortestWords;


if (actionType == ActionType.MakeWords)
{
	ContextFreeGrammar solver = new GrammarParser(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt")).GetGrammar(new ConsoleLogger());
	Console.WriteLine("Нажмите Enter для отмены...");
	CancellationTokenSource cts = new CancellationTokenSource();
	Task solverTask = Task.Run(() => solver.MakeWords(cts.Token, int.MaxValue));
	Task consoleTask = Task.Run(() =>
	{
        Console.ReadLine();
		cts.Cancel();
	});
	await solverTask;
}
else if (actionType == ActionType.MakeWordsDetailed)
{
	ContextFreeGrammar solver = new GrammarParser(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt")).GetGrammar(new ConsoleLogger());
	Console.WriteLine("Нажмите Enter для отмены...");
	CancellationTokenSource cts = new CancellationTokenSource();
	Task solverTask = Task.Run(() => solver.MakeWordsDetailed(cts.Token, int.MaxValue));
	Task consoleTask = Task.Run(() =>
	{
		Console.ReadLine();
		cts.Cancel();
	});
	await solverTask;
}
else if (actionType == ActionType.SimplifyGrammar)
{
	ContextFreeGrammar solver = new GrammarParser(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt")).GetGrammar(new ConsoleLogger());
	Console.WriteLine("НАЧАЛЬНОЕ СОСТОЯНИЕ");
	Console.WriteLine(solver.ToString());

	solver = solver.SimplifyGrammar();

	File.WriteAllText("output.txt", solver.ToString());
}
else if (actionType == ActionType.ConvertToChomskyNormalForm)
{
	ContextFreeGrammar solver = new GrammarParser(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt")).GetGrammar(new ConsoleLogger());
    Console.WriteLine("НАЧАЛЬНОЕ СОСТОЯНИЕ");
    Console.WriteLine(solver.ToString());

	solver = solver.ConvertGrammarToChomskyNormalForm();

	File.WriteAllText("output.txt", solver.ToString());
}
else if (actionType == ActionType.FindShortestWords)
{
	ContextFreeGrammar solver = new GrammarParser(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt")).GetGrammar(new ConsoleLogger());
	Console.WriteLine("НАЧАЛЬНОЕ СОСТОЯНИЕ");
	Console.WriteLine(solver.ToString());

	ContextFreeGrammar solver2 = solver.ConvertGrammarToChomskyNormalForm();

	Console.WriteLine("ОТВЕТ");
	List<string> words = solver2.GetShortestWords().OrderBy(w => w.Length).ThenBy(w => w).ToList();
	string result = "";
	if (words.Count == 0)
	{
        Console.WriteLine("Грамматика пустая");
	}
	else if (words.Count == 1)
	{
		result += $"Самое короткое слово: {(words[0] != "" ? words[0] : "ε")}\n";
		result += "Вывод: ";
		List<string> chain = solver.MakeWordDetailed(words[0]);
		result += chain[0];
		for (int i = 1; i < chain.Count; i++)
			result += " -> " + (chain[i] != "" ? chain[i] : "ε");
		result += "\n";
	}
	else
	{
		result += "Самые короткие слова: ";
		result += words[0] != "" ? words[0] : "ε";
		for (int i = 1; i < words.Count; i++)
			result += " | " + (words[i] != "" ? words[i] : "ε");
		result += "\n";
		result += "Выводы:\n";
		foreach (string word in words)
		{
			List<string> chain = solver.MakeWordDetailed(word);
			result += chain[0];
			for (int i = 1; i < chain.Count; i++)
				result += " -> " + (chain[i] != "" ? chain[i] : "ε");
			result += "\n";
		}
	}

    Console.WriteLine(result);
    File.WriteAllText("output.txt", result);
}

Console.ReadLine();
