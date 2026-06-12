using BlockChain.Models;
using BlockChain.Service;

var displayService = new DisplayService();
var blockChainService = new BlockChainService(1);
var explorer = new BlockchainExplorer(blockChainService);
var pendingTransactions = new List<Transaction>();

Console.OutputEncoding = System.Text.Encoding.UTF8;

bool exitApp = false;
while (!exitApp)
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine();
    Console.WriteLine("==================================================");
    Console.WriteLine("                БЛОКЧЕЙН МЕНЮ                     ");
    Console.WriteLine("==================================================");
    Console.ResetColor();
    Console.WriteLine($" [1] Додати транзакцію (у черзі: {pendingTransactions.Count})");
    Console.WriteLine(" [2] Змайнити блок");
    Console.WriteLine(" [3] Показати блокчейн");
    Console.WriteLine(" [4] Перевірити валідність");
    Console.WriteLine(" [5] Аналітика (Blockchain Explorer)");
    Console.WriteLine(" [0] Вихід");
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("==================================================");
    Console.ResetColor();
    Console.Write("Виберіть опцію: ");

    string? choice = Console.ReadLine();
    Console.WriteLine();

    switch (choice)
    {
        case "1":
            AddTransactionMenu(pendingTransactions);
            break;
        case "2":
            MineBlockMenu(blockChainService, pendingTransactions);
            break;
        case "3":
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("--- Вміст Блокчейну ---");
            Console.ResetColor();
            displayService.PrintChain(blockChainService.Chain);
            break;
        case "4":
            VerifyValidityMenu(blockChainService);
            break;
        case "5":
            AnalyticsMenu(explorer);
            break;
        case "0":
            exitApp = true;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Дякуємо за використання Блокчейну! Бувай!");
            Console.ResetColor();
            break;
        default:
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Некоректна опція. Спробуйте ще раз.");
            Console.ResetColor();
            break;
    }
}

void AddTransactionMenu(List<Transaction> pending)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("--- ДОДАВАННЯ ТРАНЗАКЦІЇ ---");
    Console.ResetColor();

    Console.Write("Введіть відправника (From): ");
    string? from = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(from))
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Помилка: адреса відправника не може бути порожньою.");
        Console.ResetColor();
        return;
    }

    Console.Write("Введіть отримувача (To): ");
    string? to = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(to))
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Помилка: адреса отримувача не може бути порожньою.");
        Console.ResetColor();
        return;
    }

    Console.Write("Введіть суму переказу (Amount): ");
    string? amountStr = Console.ReadLine()?.Trim();
    if (!decimal.TryParse(amountStr, out decimal amount) || amount <= 0)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Помилка: сума має бути числом більше нуля.");
        Console.ResetColor();
        return;
    }

    try
    {
        var tx = TransactionService.CreateTransaction(from, to, amount);
        pending.Add(tx);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Транзакцію успішно додано до черги!");
        Console.ResetColor();
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Помилка створення транзакції: {ex.Message}");
        Console.ResetColor();
    }
}

void MineBlockMenu(BlockChainService blockchain, List<Transaction> pending)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("--- МАЙНИНГ НОВОГО БЛОКУ ---");
    Console.ResetColor();

    if (pending.Count == 0)
    {
        Console.Write("Черга транзакцій порожня. Бажаєте змайнити порожній блок? (y/n): ");
        string? response = Console.ReadLine()?.Trim().ToLower();
        if (response != "y" && response != "yes")
        {
            Console.WriteLine("Майнинг скасовано.");
            return;
        }
    }

    Console.WriteLine("Початок майнингу...");
    int beforeCount = blockchain.Chain.Count;

    var txToMine = new List<Transaction>(pending);
    blockchain.AddBlock(txToMine);

    if (blockchain.Chain.Count > beforeCount)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Блок успішно змайнено та додано до блокчейну!");
        Console.ResetColor();
        pending.Clear();
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Не вдалося додати блок. Можливо, транзакції не пройшли валідацію.");
        Console.ResetColor();
    }
}

void VerifyValidityMenu(BlockChainService blockchain)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("--- ПЕРЕВІРКА ВАЛІДНОСТІ ---");
    Console.ResetColor();

    string validationResult = blockchain.IsValid();
    if (validationResult == "true")
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Блокчейн валідний! Усі хеші та зв'язки коректні.");
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("УВАГА! Блокчейн НЕ валідний!");
        Console.WriteLine($"Деталі помилки: {validationResult}");
    }
    Console.ResetColor();
}

void AnalyticsMenu(BlockchainExplorer explorer)
{
    bool backToMain = false;
    while (!backToMain)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine();
        Console.WriteLine("==================================================");
        Console.WriteLine("               АНАЛІТИКА БЛОКЧЕЙНУ                ");
        Console.WriteLine("==================================================");
        Console.ResetColor();
        Console.WriteLine(" [1] Загальний обсяг усіх переказів");
        Console.WriteLine(" [2] Знайти транзакцію з найбільшою сумою");
        Console.WriteLine(" [3] Переглянути історію адреси");
        Console.WriteLine(" [4] Знайти транзакцію та блок за ID (Find Location)");
        Console.WriteLine(" [0] Повернутися до головного меню");
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("==================================================");
        Console.ResetColor();
        Console.Write("Виберіть опцію аналітики: ");

        string? choice = Console.ReadLine();
        Console.WriteLine();

        switch (choice)
        {
            case "1":
                decimal totalVolume = explorer.GetTotalVolume();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Загальний обсяг усіх переказів: {totalVolume} BTC");
                Console.ResetColor();
                break;

            case "2":
                var largestTx = explorer.GetLargestTransaction();
                if (largestTx != null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Транзакція з найбільшою сумою:");
                    Console.WriteLine($"   - {largestTx}");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("У блокчейні ще немає транзакцій.");
                }
                Console.ResetColor();
                break;

            case "3":
                Console.Write("Введіть адресу для пошуку історії: ");
                string? address = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(address))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Помилка: адреса не може бути порожньою.");
                    Console.ResetColor();
                    break;
                }
                var history = explorer.GetAddressHistory(address);
                if (history.Count > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Знайдено {history.Count} транзакцій для адреси '{address}':");
                    Console.ResetColor();
                    foreach (var tx in history)
                    {
                        Console.WriteLine($"   - {tx}");
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine($"Транзакцій для адреси '{address}' не знайдено.");
                    Console.ResetColor();
                }
                break;

            case "4":
                Console.Write("Введіть ID транзакції для пошуку: ");
                string? txId = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(txId))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Помилка: ID транзакції не може бути порожнім.");
                    Console.ResetColor();
                    break;
                }
                var (foundBlock, foundTx) = explorer.FindTransactionLocation(txId);
                if (foundBlock != null && foundTx != null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Транзакцію знайдено!");
                    Console.ResetColor();
                    Console.WriteLine($"   Блок: #{foundBlock.Index}");
                    Console.WriteLine($"   Хеш блоку: {foundBlock.Hash}");
                    Console.WriteLine($"   Транзакція: {foundTx}");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Транзакцію з таким ID не знайдено.");
                    Console.ResetColor();
                }
                break;

            case "0":
                backToMain = true;
                break;

            default:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Некоректна опція. Спробуйте ще раз.");
                Console.ResetColor();
                break;
        }
    }
}