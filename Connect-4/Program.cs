using System;

namespace ConnectFour
{
    // Represents a player in the game
    public class Player
    {
        public string Name { get; }
        public char Token { get; }
        public ConsoleColor TokenColor { get; }

        // Constructor to initialize player properties
        public Player(string name, char token, ConsoleColor tokenColor)
        {
            Name = name;
            Token = token;
            TokenColor = tokenColor;
        }

        // Method for making a move on the board
        public virtual int MakeMove(Board board)
        {
            Console.WriteLine($"{Name}'s turn ({Token}): ");
            // Read the column number from the console input and parse it to an integer
            return int.Parse(Console.ReadLine()!);
        }
    }

    // Represents a human player
    public class HumanPlayer : Player
    {
        // Constructor to initialize human player properties
        public HumanPlayer(string name, char token, ConsoleColor tokenColor) : base(name, token, tokenColor) { }

        // Override the MakeMove method to handle human player's move
        public override int MakeMove(Board board)
        {
            int column;
            do
            {
                Console.Write($"{Name}'s turn ({Token}): ");
                // Read the column number from the console input and parse it to an integer
                if (!int.TryParse(Console.ReadLine(), out column) || column < 1 || column > 7)
                {
                    Console.WriteLine("Invalid input. Please enter a number between 1 and 7.");
                    continue;
                }
            } while (column < 1 || column > 7 || board.IsColumnFull(column));

            return column;
        }
    }

    // Represents a computer player
    public class ComputerPlayer : Player
    {
        // Constructor to initialize computer player properties
        public ComputerPlayer(string name, char token, ConsoleColor tokenColor) : base(name, token, tokenColor) { }

        // Override the MakeMove method to handle computer player's move
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

    // Represents the game board
    public class Board
    {
        private readonly char[,] grid;

        // Constructor to initialize the board
        public Board()
        {
            grid = new char[6, 7];
            InitializeBoard();
        }

        // Method to initialize the board with empty spaces
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

        // Method to display the board on the console
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

        // Method to get token color based on token value
        private static ConsoleColor GetTokenColor(char token)
        {
            return token switch
            {
                'X' => ConsoleColor.Red,
                'O' => ConsoleColor.Blue,
                _ => ConsoleColor.White,
            };
        }

        // Method to check if a column is full
        public bool IsColumnFull(int column)
        {
            return grid[0, column - 1] != ' ';
        }

        // Method to check if the game is over (win or draw)
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

        // Method to check for a win based on token
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

        // Method to check if the board is full
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

        // Method to place a token in a column
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

        // Method to copy the grid from another board
        public void CopyGrid(Board originalBoard)
        {
            Array.Copy(originalBoard.grid, grid, originalBoard.grid.Length);
        }
    }

    // Represents the game controller
    public class GameController
    {
        private static Player? player1; // Change Player to Player? to make it nullable
        private static Player? player2; // Change Player to Player? to make it nullable

        // Method to start the game
        public static void StartGame()
        {
            Console.WriteLine("Welcome to Connect 4!");

            do
            {
                InitializePlayers();
                Board board = new Board();
                Player currentPlayer = player1!; // Use ! to indicate that player1 is not null

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

                        if (board.CheckForWin(player1!.Token)) // Use ! to indicate that player1 is not null
                        {
                            Console.WriteLine("It's a Connect 4!");
                            Console.WriteLine($"{player1!.Name} wins!"); // Use ! to indicate that player1 is not null
                        }
                        else if (board.CheckForWin(player2!.Token)) // Use ! to indicate that player2 is not null
                        {
                            Console.WriteLine("It's a Connect 4!");
                            Console.WriteLine($"{player2!.Name} wins!"); // Use ! to indicate that player2 is not null
                        }
                        else
                        {
                            Console.WriteLine("It's a draw!");
                        }

                        if (!PlayAgain())
                            return; // If players don't want to play again, exit the method
                        else
                            break; // Restart the game
                    }

                    currentPlayer = (currentPlayer == player1) ? player2! : player1!; // Use ! to indicate that player1 and player2 are not null
                }
            } while (true); // Loop indefinitely until players decide not to play again
        }

        // Method to initialize players
        private static void InitializePlayers()
        {
            // Player 1
            Console.WriteLine("Choose the type of Player 1: (1) Human or (2) AI");
            int player1Choice = GetValidPlayerChoice();

            if (player1Choice == 1)
            {
                Console.WriteLine("Enter name for Player 1:");
                string player1Name = Console.ReadLine()!;
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
            int player2Choice = GetValidPlayerChoice();

            if (player2Choice == 1)
            {
                Console.WriteLine("Enter name for Player 2:");
                string player2Name = Console.ReadLine()!;
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
        }

        // Method to ask if players want to play again
        private static bool PlayAgain()
        {
            Console.WriteLine("Do you want to play again? (yes/no)");
            string input = Console.ReadLine()!.ToLower();

            while (input != "yes" && input != "no")
            {
                Console.WriteLine("Invalid input. Please enter 'yes' or 'no'.");
                input = Console.ReadLine()!.ToLower();
            }

            return input == "yes";
        }

        // Method to get a valid player choice
        private static int GetValidPlayerChoice()
        {
            int choice;
            while (!int.TryParse(Console.ReadLine(), out choice) || (choice != 1 && choice != 2))
            {
                Console.WriteLine("Invalid input. Please enter 1 or 2.");
            }
            return choice;
        }
    }

    // Main program class
    class Program
    {
        static void Main(string[] args)
        {
            GameController.StartGame(); // Start the game
        }
    }
}
