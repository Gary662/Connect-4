using System;

public class Player
{
    public string Name { get; }
    public char Token { get; }
    public ConsoleColor TokenColor { get; }

    public Player(string name, char token, ConsoleColor tokenColor)
    {
        Name = name;
        Token = token;
        TokenColor = tokenColor;
    }

    public virtual int MakeMove(Board board)
    {
        Console.WriteLine($"{Name}'s turn ({Token}): ");
        return int.Parse(Console.ReadLine());
    }
}

public class HumanPlayer : Player
{
    public HumanPlayer(string name, char token, ConsoleColor tokenColor) : base(name, token, tokenColor) { }

    public override int MakeMove(Board board)
    {
        int column;
        do
        {
            Console.Write($"{Name}'s turn ({Token}): ");
            if (!int.TryParse(Console.ReadLine(), out column) || column < 1 || column > 7)
            {
                Console.WriteLine("Invalid input. Please enter a number between 1 and 7.");
                continue;
            }
        } while (column < 1 || column > 7 || board.IsColumnFull(column));

        return column;
    }
}

public class ComputerPlayer : Player
{
    public ComputerPlayer(string name, char token, ConsoleColor tokenColor) : base(name, token, tokenColor) { }

    public override int MakeMove(Board board)
    {
        // Check if AI can win in the next move
        for (int column = 1; column <= 7; column++)
        {
            Board testBoard = new Board();
            testBoard.CopyGrid(board);
            if (!testBoard.IsColumnFull(column))
            {
                testBoard.PlaceToken(Token, column);
                if (testBoard.CheckForWin(Token))
                {
                    return column;
                }
            }
        }

        // Check if opponent can win in the next move, and block them
        char opponentToken = Token == 'X' ? 'O' : 'X';
        for (int column = 1; column <= 7; column++)
        {
            Board testBoard = new Board();
            testBoard.CopyGrid(board);
            if (!testBoard.IsColumnFull(column))
            {
                testBoard.PlaceToken(opponentToken, column);
                if (testBoard.CheckForWin(opponentToken))
                {
                    return column;
                }
            }
        }

        // Otherwise, prioritize center and sides
        int[] preferredColumns = { 4, 3, 5, 2, 6, 1, 7 };
        foreach (int column in preferredColumns)
        {
            if (!board.IsColumnFull(column))
            {
                return column;
            }
        }

        // If all else fails, make a random move
        Random rand = new Random();
        int randomColumn;
        do
        {
            randomColumn = rand.Next(1, 8);
        } while (board.IsColumnFull(randomColumn));

        return randomColumn;
    }
}

public class Board
{
    private readonly char[,] grid;

    public Board()
    {
        grid = new char[6, 7];
        InitializeBoard();
    }

    private void InitializeBoard()
    {
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                grid[i, j] = ' ';
            }
        }
    }

    public void DisplayBoard()
    {
        Console.Clear(); // Clear console before displaying the board
        Console.WriteLine(" 1 2 3 4 5 6 7");
        for (int i = 0; i < 6; i++)
        {
            Console.Write("|");
            for (int j = 0; j < 7; j++)
            {
                Console.ForegroundColor = GetTokenColor(grid[i, j]);
                Console.Write($"{grid[i, j]}|");
                Console.ResetColor();
            }
            Console.WriteLine();
        }
    }

    private ConsoleColor GetTokenColor(char token)
    {
        return token switch
        {
            'X' => ConsoleColor.Red,
            'O' => ConsoleColor.Blue,
            _ => ConsoleColor.White,
        };
    }

    public bool IsColumnFull(int column)
    {
        return grid[0, column - 1] != ' ';
    }

    public bool IsGameOver()
    {
        // Check for a win
        if (CheckForWin('X') || CheckForWin('O'))
        {
            return true;
        }
        // Check for a draw
        else if (IsBoardFull())
        {
            return true;
        }
        return false;
    }

    public bool CheckForWin(char token)
    {
        // Check horizontally
        for (int row = 0; row < 6; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                if (grid[row, col] == token &&
                    grid[row, col + 1] == token &&
                    grid[row, col + 2] == token &&
                    grid[row, col + 3] == token)
                {
                    return true;
                }
            }
        }
        // Check vertically
        for (int col = 0; col < 7; col++)
        {
            for (int row = 0; row < 3; row++)
            {
                if (grid[row, col] == token &&
                    grid[row + 1, col] == token &&
                    grid[row + 2, col] == token &&
                    grid[row + 3, col] == token)
                {
                    return true;
                }
            }
        }
        // Check diagonally (from bottom left to top right)
        for (int row = 3; row < 6; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                if (grid[row, col] == token &&
                    grid[row - 1, col + 1] == token &&
                    grid[row - 2, col + 2] == token &&
                    grid[row - 3, col + 3] == token)
                {
                    return true;
                }
            }
        }
        // Check diagonally (from top left to bottom right)
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                if (grid[row, col] == token &&
                    grid[row + 1, col + 1] == token &&
                    grid[row + 2, col + 2] == token &&
                    grid[row + 3, col + 3] == token)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsBoardFull()
    {
        for (int row = 0; row < 6; row++)
        {
            for (int col = 0; col < 7; col++)
            {
                if (grid[row, col] == ' ')
                {
                    return false; // If there's an empty space, the board is not full
                }
            }
        }
        return true; // If no empty spaces are found, the board is full
    }

    public void PlaceToken(char token, int column)
    {
        for (int i = 5; i >= 0; i--)
        {
            if (grid[i, column - 1] == ' ')
            {
                grid[i, column - 1] = token;
                return; // Return after placing the token to avoid further checks
            }
        }
        Console.WriteLine("Column is full. Choose another column.");
    }

    public void CopyGrid(Board originalBoard)
    {
        Array.Copy(originalBoard.grid, grid, originalBoard.grid.Length);
    }
}

public class GameController
{
    private readonly Player player1;
    private readonly Player player2;
    private readonly Board board;

    public GameController(Player player1, Player player2, Board board)
    {
        this.player1 = player1;
        this.player2 = player2;
        this.board = board;
    }

    public void StartGame()
    {
        Player currentPlayer = player1;

        while (!board.IsGameOver())
        {
            board.DisplayBoard();
            int column = currentPlayer.MakeMove(board);

            if (board.IsColumnFull(column))
            {
                Console.WriteLine("Column is full. Choose another column.");
                continue;
            }

            board.PlaceToken(currentPlayer.Token, column);

            // Check for a win after each move
            if (board.IsGameOver())
            {
                // Display the final state of the board
                board.DisplayBoard();

                if (board.CheckForWin(player1.Token))
                {
                    Console.WriteLine($"{player1.Name} wins!");
                }
                else if (board.CheckForWin(player2.Token))
                {
                    Console.WriteLine($"{player2.Name} wins!");
                }
                else
                {
                    Console.WriteLine("It's a draw!");
                }
                break;
            }

            currentPlayer = (currentPlayer == player1) ? player2 : player1;
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Welcome to Connect 4!");

        // Player 1
        Console.WriteLine("Choose the type of Player 1: (1) Human or (2) AI");
        int player1Choice = int.Parse(Console.ReadLine());
        Player player1;

        if (player1Choice == 1)
        {
            Console.WriteLine("Enter name for Player 1:");
            string player1Name = Console.ReadLine();
            player1 = new HumanPlayer(player1Name, 'X', ConsoleColor.Red);
        }
        else if (player1Choice == 2)
        {
            player1 = new ComputerPlayer("Player 1 (AI)", 'X', ConsoleColor.Red);
        }
        else
        {
            throw new ArgumentException("Invalid player type selected for Player 1.");
        }

        // Player 2
        Console.WriteLine("Choose the type of Player 2: (1) Human or (2) AI");
        int player2Choice = int.Parse(Console.ReadLine());
        Player player2;

        if (player2Choice == 1)
        {
            Console.WriteLine("Enter name for Player 2:");
            string player2Name = Console.ReadLine();
            player2 = new HumanPlayer(player2Name, 'O', ConsoleColor.Blue);
        }
        else if (player2Choice == 2)
        {
            player2 = new ComputerPlayer("Player 2 (AI)", 'O', ConsoleColor.Blue);
        }
        else
        {
            throw new ArgumentException("Invalid player type selected for Player 2.");
        }

        Board board = new Board();
        GameController gameController = new GameController(player1, player2, board);

        gameController.StartGame();
    }
}
