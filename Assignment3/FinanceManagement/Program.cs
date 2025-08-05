using System;
using System.Collections.Generic;

public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Processing Bank Transfer of ${transaction.Amount} for {transaction.Category}");
    }
}

public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Processing Mobile Transfer of ${transaction.Amount} for {transaction.Category}");
    }
}

public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Processing Crypto Transfer of ${transaction.Amount} for {transaction.Category}");
    }
}

public class Account
{
    public string AccountNumber { get; }
    public decimal Balance { get; protected set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction)
    {
        Balance -= transaction.Amount;
    }

}

public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance) : base(accountNumber, initialBalance) { }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
            Console.WriteLine("Insufficient Funds");
        else
        {
            base.ApplyTransaction(transaction);
            Console.WriteLine($"Transaction Successful. Balance: ${Balance}");
        }
    }
}

public class FinanceApp
{
    private List<Transaction> _transaction = new List<Transaction>();

    public void Run()
    {
        var account = new SavingsAccount("11297349", 15000);

        Transaction t1 = new(1, DateTime.Now, 200, "Groceries");
        Transaction t2 = new(2, DateTime.Now, 100, "Utilities");
        Transaction t3 = new(3, DateTime.Now, 50, "Entertainment");

        var bankTransferProcessor = new BankTransferProcessor();
        var mobileMoneyProcessor = new MobileMoneyProcessor();
        var cryptoWalletProcessor = new CryptoWalletProcessor();

        bankTransferProcessor.Process(t1);
        mobileMoneyProcessor.Process(t2);
        cryptoWalletProcessor.Process(t3);

        account.ApplyTransaction(t1);
        account.ApplyTransaction(t2);
        account.ApplyTransaction(t3);
    }
}

class Program
{
    static void Main(string[] args)
    {
        var app = new FinanceApp();
        app.Run();
    }
}