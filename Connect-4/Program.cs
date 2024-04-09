using System;

public class Player
{
    public string Name { get; set; }
    public char Token { get; set; }

    public Player(string name, char token)
    {
        Name = name;
        Token = token;
    }

    public virtual int MakeMove()
    {
        Console.WriteLine($"{Name}'s turn ({Token}): ");
        return int.Parse(Console.ReadLine());
    }
}

public class HumanPlayer : Player
{
    public HumanPlayer(string name, char token) : base(name, token) { }

    public override int MakeMove()
    {
        int column;
        do
        {
            Console.Write($"{Name}'s turn ({Token}): ");
        } while (!int.TryParse(Console.ReadLine(), out column) || column < 1 || column > 7);

        return column;
    }
}

public class ComputerPlayer : Player
{
    public ComputerPlayer(string name, char token) : base(name, token) { }

    public override int MakeMove()
    {
        // Example: Random move for computer player
        Random rand = new Random();
        return rand.Next(1, 8); // Random column between 1 and 7
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
        Console.WriteLine(" 1 2 3 4 5 6 7");
        for (int i = 0; i < 6; i++)
        {
            Console.Write("|");
            for (int j = 0; j < 7; j++)
            {
                Console.Write($"{grid[i, j]}|");
            }
            Console.WriteLine();
        }
    }

    public bool IsColumnFull(int column)
    {
        return grid[0, column - 1] != ' ';
    }

    public bool IsGameOver()
    {
        // Check for a win
        if (CheckForWin('X'))
        {
            Console.WriteLine("Player 1 wins!");
            return true;
        }
        else if (CheckForWin('O'))
        {
            Console.WriteLine("Player 2 wins!");
            return true;
        }
        // Check for a draw
        else if (IsBoardFull())
        {
            Console.WriteLine("It's a draw!");
            return true;
        }
        return false;
    }

    private bool CheckForWin(char token)
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
                break;
            }
        }
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
            int column = currentPlayer.MakeMove();

            if (board.IsColumnFull(column))
            {
                Console.WriteLine("Column is full. Choose another column.");
                continue;
            }

            board.PlaceToken(currentPlayer.Token, column);
            currentPlayer = (currentPlayer == player1) ? player2 : player1;
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Player player1 = new HumanPlayer("Player 1", 'X');
        Player player2 = new HumanPlayer("Player 2", 'O');
        Board board = new Board();
        GameController gameController = new GameController(player1, player2, board);

        gameController.StartGame();
    }
}
