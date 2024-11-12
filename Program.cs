// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Transactions;
class Register
{
    //Register Logic
    public void RegisterUser(Dictionary<string,string> users)
    {
        Console.Write("Enter your username: ");
        string username = Console.ReadLine();
        
        if (users.ContainsKey(username))
        {
            Console.WriteLine("Username already exists,Please try another one");
            return;
        }
        
            
        Console.Write("Enter your password: ");
        string password = Console.ReadLine();
        users[username] = password;
        Console.WriteLine("Registration SuccessFull");
        
    }

}


class Login
{
    //Login Logic
    public void LoginUser(Dictionary<string,string> users, Dictionary<string, List<Account>> Accounts, Dictionary<int, List<Transaction>> transactionHistory)
    {
        Console.Write("Enter your username: ");
        string username = Console.ReadLine();

        Console.Write("Enter your password: ");
        string password = Console.ReadLine();

        // Check if the username exists and the password matching or not
        if (users.ContainsKey(username) && users[username] == password)
        {
            Console.WriteLine("Login successful! Welcome, " + username + ".");
            NavigateToAccount(username, Accounts,transactionHistory);
            return;
        }
        else
        {
            Console.WriteLine("Login failed. Invalid username or password.");

        }

    }
    private void NavigateToAccount(string username, Dictionary<string, List<Account>> Accounts, Dictionary<int, List<Transaction>> transactionHistory)
    {
        while (true)
        {
            Console.WriteLine("Select any option");
            Console.WriteLine("1.Create a new account");
            Console.WriteLine("2.View Account details");
            Console.WriteLine("3. Log Out");
            string op = Console.ReadLine();
            switch (op)
            {
                case "1":
                    OpenAccount(username, Accounts, transactionHistory);
                    break;
                case "2":
                    ViewAccountDetails view=new ViewAccountDetails();
                    view.ViewAccount(username, Accounts,transactionHistory);
                    break;
                case "3":
                    Console.WriteLine("Logging out...");
                    return;
                default:
                    Console.WriteLine("Invalid option.Please try again...");
                    break;
            }
        }
    }
   
   //open Account Fo the user
    private void OpenAccount(string username,Dictionary<string, List<Account>> Accounts,Dictionary<int,List<Transaction>> transactionHistory)
    {
        Console.WriteLine("opening Account for "+username);
        Console.Write("Enter the Account Holder Name: ");
        string accName=Console.ReadLine();
        Console.Write("Enter the type of Account(savings/checking): ");
        string accType=Console.ReadLine();
        Console.Write("Enter the deposite Amount: ");
        if(decimal.TryParse(Console.ReadLine(), out decimal depoAmount) && depoAmount >= 0)
    {
            Account newAccount = new Account(accName, accType, depoAmount,  transactionHistory);

            // Check if the user already has accounts, if not, create a new list
            if (!Accounts.ContainsKey(username))
            {
                Accounts[username] = new List<Account>();
            }
            
            // Add the new account to the user's list of accounts
            Accounts[username].Add(newAccount);
            Console.WriteLine("Account successfully created and added to the list.");
        }
    else
        {
            Console.WriteLine("Invalid deposit amount. Please try again.");
        }


    }
    
}
// View Account Details
class ViewAccountDetails
{
    public void ViewAccount(string username, Dictionary<string, List<Account>> Accounts, Dictionary<int, List<Transaction>> transactionHistory)
    {
        Console.WriteLine("Account details for " + username + ":");
        bool found = false;
        List<Account> userAccounts = null;
        if (Accounts.ContainsKey(username))
        {
            userAccounts = Accounts[username];
            foreach (var account in Accounts[username])
            {
                Console.WriteLine($"Account Number :{account.AccountNumber},Account Holder: {account.AccHolder}, Current balance: {account.DepoAmount:C},Account Type:{account.AccType}");
                found = true;
            }
        }
        if (!found)
        {
            Console.WriteLine("No accounts found.");
            return;
        }
        //options contains in the viewdetails menu
        while (true) { 
            Console.WriteLine("select an Option");
            Console.WriteLine("1.Withdrawal");
            Console.WriteLine("2.Deposit");
            Console.WriteLine("3.Show transaction History");
            Console.WriteLine("4.Balance check");
            Console.WriteLine("5.Add Montly Interest");
            Console.WriteLine("6.Exit");
            string op=Console.ReadLine();
            switch (op)
            {
                case "1":
                    //withdraw here
                    Console.WriteLine("Enter Account Number for Withdrawal:");
                    int accNumForWithdrawal = int.Parse(Console.ReadLine());
                    var withdrawAcc=userAccounts.FirstOrDefault(acc=>acc.AccountNumber==accNumForWithdrawal);
                    if (withdrawAcc != null)
                    {
                        Console.Write("Enter withdrawal amount: ");
                        decimal withdrawalAmount = decimal.Parse(Console.ReadLine());
                        withdrawAcc.Withdraw(withdrawalAmount);
                    }
                    else
                    {
                        Console.WriteLine("Account not found.");
                    }
                    break;
                    
                case "2":
                    // Here you can deposite
                    Console.Write("Enter Account Number for Deposit:");
                    int accNumForDeposit = int.Parse(Console.ReadLine());
                    var depositAccount = userAccounts.FirstOrDefault(acc => acc.AccountNumber == accNumForDeposit);
                    if (depositAccount != null)
                    {
                        Console.Write("Enter deposit amount: ");
                        decimal depositAmount = decimal.Parse(Console.ReadLine());
                        depositAccount.Deposit(depositAmount);
                    }
                    else
                    {
                        Console.WriteLine("Account not found.");
                    }
                    break;
                case "3":
                    // Transaction History
                    bool find = false;
                    Console.Write("Enter Account Number: ");
                    int accNum = int.Parse(Console.ReadLine());
                    var transac = userAccounts.FirstOrDefault(acc => acc.AccountNumber == accNum);
                    if (transac != null)
                    {
                        foreach (var t in transactionHistory[accNum])
                        {
                            Console.WriteLine($"Account Number :{t.AccountNumber}, Current balance: {t.Balance:C},Account Type:{t.AccType},Transaction type:{t. transacType}");
                            find = true;
                        }
                    }
                    if (!find)
                    {
                        Console.WriteLine("No transaction History.");
                        return;
                    }
                    break;
                case "4":
                    // Account Balance
                    bool flag = false;
                    Console.Write("Enter Account Number: ");
                    int acc = int.Parse(Console.ReadLine());
                    var accNu= userAccounts.FirstOrDefault(account => account.AccountNumber == acc);
                    if (accNu != null) {
                        Console.WriteLine($"Your Balance for the account{accNu.DepoAmount}");
                     }
                    break;
                case "5":
                    // Calculating the monthly interest
                    Console.Write("Enter Account Number: ");
                    int accNumForInterest = int.Parse(Console.ReadLine());
                    var interestAccount = userAccounts.FirstOrDefault(acc => acc.AccountNumber == accNumForInterest);
                    if (interestAccount != null && interestAccount.AccType.ToLower() == "savings")
                    {
                        interestAccount.AddMonthlyInterest();
                    }
                    else
                    {
                        Console.WriteLine("Account not found or not a savings account.");
                    }
                
                    break;
                case "6": Console.WriteLine("Exit");
                        return;
                default:
                    return;
            }   
        }

    }
    
}

public class Transaction
{
    public static int TransactionId = 10;
    public int AccountNumber { get; set; }
    public DateOnly date { get; set; }
    public string AccType { get; set; }
    public decimal Balance { get; set; }
    public string transacType { get; set; }
    public Transaction(int accountNumber,string accType, decimal balance,string TransacType)
    {
        AccountNumber = accountNumber;
        date = date;
        AccType = accType;
        Balance = balance;
        TransactionId = generateId();
        transacType= TransacType;
        date=date;
    }
    public int  generateId()
    {
       return  ++TransactionId ;
    }
    
    public override string ToString()
    {
        return $"Transaction Id: {TransactionId},Account Number:{AccountNumber},Date:{date},Account type:{AccType},Balance:{Balance}"; 
    }
    public void AddTransaction(Transaction t,Dictionary<int, List<Transaction>> transactionHistory,string transacType)
    {
        if (!transactionHistory.ContainsKey(AccountNumber))
        {
            transactionHistory[AccountNumber] = new List<Transaction>();
        }
        transactionHistory[AccountNumber].Add(t);

    }
}

public class Account
{
    private static int accountCounter = 2000;
    private Dictionary<int, List<Transaction>> transactionHistory; // Field to hold transaction history

    public string AccHolder { get; set; }
    public string AccType { get; set; }
    public decimal DepoAmount { get; set; }
    public int AccountNumber { get; private set; }
    private DateTime lastInterestDate;
    private const decimal InterestRate = 0.02m;
    public Account(string accHolder, string accType, decimal depoAmount, Dictionary<int, List<Transaction>> transactionHistory)
    {
        AccHolder = accHolder;
        AccType = accType;
        DepoAmount = depoAmount;
        AccountNumber = GenerateAccountNumber();
        lastInterestDate = DateTime.MinValue;
        this.transactionHistory = transactionHistory;

        // Initialize transaction history for this account if not already present
        if (!transactionHistory.ContainsKey(AccountNumber))
        {
            transactionHistory[AccountNumber] = new List<Transaction>();
        }
    }

    private int GenerateAccountNumber()
    {
        return ++accountCounter;
    }

    public override string ToString()
    {
        return $"Account Id:{accountCounter}, Account Holder: {AccHolder}, Type: {AccType}, Balance: {DepoAmount:C}";
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("Withdrawal amount must be greater than zero.");
            return;
        }

        if (amount > DepoAmount)
        {
            Console.WriteLine("Insufficient balance. Withdrawal denied.");
            return;
        }

        DepoAmount -= amount;
        Console.WriteLine($"Withdrawal of {amount:C} successful. New balance: {DepoAmount:C}");

        // Create and add the transaction
        Transaction t = new Transaction(AccountNumber, AccType, DepoAmount,"Withdraw");
        AddTransaction(t);
    }

    public void Deposit(decimal amount)

    {
      
        if (amount <= 0)
        {
            Console.WriteLine("Amount should be greater than zero.");
            return;
        }

        DepoAmount += amount;
        Console.WriteLine($"Deposit of {amount:C} successful. New balance: {DepoAmount:C}");

        // Create and add the transaction
        Transaction t = new Transaction(AccountNumber, AccType, DepoAmount,"Deposit");
        AddTransaction(t);
    }

    private void AddTransaction(Transaction transaction)
    {
        if (transactionHistory.ContainsKey(AccountNumber))
        {
            transactionHistory[AccountNumber].Add(transaction);
        }
        else
        {
            transactionHistory[AccountNumber] = new List<Transaction> { transaction };
        }
    }
    public void AddMonthlyInterest()
    {
        if(AccType=="savings") {
            if (lastInterestDate == DateTime.MinValue || (DateTime.Now - lastInterestDate).Days >= 30)
            {
                decimal interest=DepoAmount*InterestRate;
                DepoAmount += interest;
                lastInterestDate = DateTime.Now;
                Console.WriteLine($"Monthly interest of {interest:C} added. New balance: {DepoAmount:C}");
                Transaction interestTransaction = new Transaction(AccountNumber, AccType, DepoAmount, "Interest");
                AddTransaction(interestTransaction);
            }
            else
            {
                Console.WriteLine("Interest has already been added for this month.");
            }
        }
        else
        {
            Console.WriteLine("Interest calculation is only applicable for savings accounts.");
        }
    
}
}
// start of the application
class Program
{
    static Dictionary<string, string> users = new Dictionary<string, string>();
    static Dictionary<string, List<Account>> Accounts=new Dictionary<string, List<Account>>();
    static Dictionary<int, List<Transaction>> transactionHistory = new Dictionary<int, List<Transaction>>();
    
    public static void Main()
    { 
        users.Add("Nithisha", "nits");
        users.Add("Rizwana", "Rizzz");
        users.Add("Vignesh", "vig");
        List<Account> user1 = new List<Account>
        {
            new Account("Nithisha","savings",1000,transactionHistory),
            new Account("Nithisha","checking",100,transactionHistory),
            
        };
        Accounts.Add("Nithisha",user1);
        List<Account> user2 = new List<Account>
        {
            new Account("Rizwana","savings",2000,transactionHistory),
            
        };
        Accounts.Add("Rizwana", user2);
        List<Account> user3 = new List<Account>
        {
            new Account("Vignesh","checkings",10090,transactionHistory),
        };
        Accounts.Add("Vignesh",user3);
        //transactionHistory.Add(5, new List<Transaction>
        //{
        //    new Transaction(user2[0].AccountNumber, "checking", 1000, "Deposit")
        //});

        //transactionHistory.Add(4, new List<Transaction>
        //    {
        //    new Transaction(user1[0].AccountNumber, "savings", 500, "Deposit")
        //});
        List<Transaction> transac = new List<Transaction>
        {
            new Transaction(user1[0].AccountNumber,"checking",100,"withdraw")
        };
        transactionHistory.Add(10, transac);

        while (true)
        {
            Console.WriteLine("Welocome to Banking Application");
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Exit");
            Console.WriteLine("select an option");
            string op=Console.ReadLine();
            switch (op)
            {
                case "1":
                Register register =new Register();
                register.RegisterUser(users);  
                     break;
                case "2":
                Login l=new Login();
                l.LoginUser(users,Accounts,transactionHistory);
                    break;
                case "3":
                    Console.WriteLine("Thank you for using the Banking Application");
                    break;
                default:
                    Console.WriteLine("Invalid Option,Please try again");
                    break;

            }

        }



    }
}


