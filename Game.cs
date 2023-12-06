namespace TicTacToe
{
    class Game(bool aiOpponent = false, bool aiTurn = false)
    {
        public enum GameBoard
        {
            Empty,
            X,
            O
        }

        public GameBoard[,] gameBoard = new GameBoard[3, 3];
        public List<Point> winnerCells = new(3);
        public bool Winner { get; set; }
        public bool CurrentPlayer { get; set; }
        public bool Tie { get; set; }
        public bool AIOpponent { get; set; } = aiOpponent;
        public bool AITurn { get; set; } = aiTurn;

        public void MakeAMove(Point p)
        {
            if (gameBoard[p.X, p.Y] is not GameBoard.Empty)
                return;

            gameBoard[p.X, p.Y] = CurrentPlayer ? GameBoard.O : GameBoard.X;
            Winner = CheckWinner(GameBoard.X) || CheckWinner(GameBoard.O);
            Tie = IsBoardFull(gameBoard) && !Winner;
            CurrentPlayer = !CurrentPlayer;
            AITurn = !AITurn;
        }

        public void AIMove()
        {
            GameBoard playerState, opponentState;
            (playerState, opponentState) = CurrentPlayer ? (GameBoard.O, GameBoard.X) : (GameBoard.X, GameBoard.O);
            Point bestMove = GetBestMove(gameBoard, playerState, opponentState);
            gameBoard[bestMove.X, bestMove.Y] = playerState;
            Winner = CheckWinner(GameBoard.X) || CheckWinner(GameBoard.O);
            Tie = IsBoardFull(gameBoard) && !Winner;
            CurrentPlayer = !CurrentPlayer;
            AITurn = !AITurn;
        }

        bool CheckWinner(GameBoard state)
        {
            int size = gameBoard.GetLength(0);

            for (int i = 0; i < size; i++)
            {
                List<Point> row = new(3);
                List<Point> col = new(3);

                for (int j = 0; j < size; j++)
                {
                    if (gameBoard[i, j] == state)
                        row.Add(new Point(i, j));
                    if (gameBoard[j, i] == state)
                        col.Add(new Point(j, i));
                }

                if (row.Count == size)
                {
                    winnerCells = row;
                    return true;
                }
                else if (col.Count == size)
                {
                    winnerCells = col;
                    return true;
                }
            }

            List<Point> principal = new(3);
            List<Point> secondary = new(3);

            for (int i = 0; i < size; i++)
            {
                if (gameBoard[i, i] == state)
                    principal.Add(new Point(i, i));
                if (gameBoard[i, size - i - 1] == state)
                    secondary.Add(new Point(i, size - i - 1));

                if (principal.Count == size)
                {
                    winnerCells = principal;
                    return true;
                }
                else if (secondary.Count == size)
                {
                    winnerCells = secondary;
                    return true;
                }
            }

            return false;
        }

        Point GetBestMove(GameBoard[,] board, GameBoard player, GameBoard opponent)
        {
            int bestScore = int.MinValue;
            Point bestMove = new();
            int alpha = int.MinValue;
            int beta = int.MaxValue;

            for (int i = 0; i < board.GetLongLength(0); i++)
            {
                for (int j = 0; j < board.GetLongLength(1); j++)
                {
                    if (board[i, j] == GameBoard.Empty)
                    {
                        board[i, j] = player;
                        int score = MiniMax(board, 0, false, player, opponent, alpha, beta);
                        board[i, j] = GameBoard.Empty;

                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestMove = new Point(i, j);
                        }

                        alpha = Math.Max(alpha, bestScore);
                        if (beta <= alpha)
                            break;
                    }
                }
            }

            return bestMove;
        }

        int MiniMax(GameBoard[,] board, int depth, bool isMaximizing, GameBoard player, GameBoard opponent, int alpha, int beta)
        {
            if (CheckWinner(player))
                return 10 - depth;
            else if (CheckWinner(opponent))
                return depth - 10;
            else if (IsBoardFull(board))
                return 0;

            if (isMaximizing)
            {
                int bestScore = int.MinValue;

                for (int i = 0; i < board.GetLongLength(0); i++)
                {
                    for (int j = 0; j < board.GetLongLength(1); j++)
                    {
                        if (board[i, j] == GameBoard.Empty)
                        {
                            board[i, j] = player;
                            int score = MiniMax(board, depth + 1, false, player, opponent, alpha, beta);
                            board[i, j] = GameBoard.Empty;
                            bestScore = Math.Max(score, bestScore);

                            alpha = Math.Max(alpha, bestScore);
                            if (beta <= alpha)
                                break;
                        }
                    }
                }

                return bestScore;
            }
            else
            {
                int bestScore = int.MaxValue;

                for (int i = 0; i < board.GetLongLength(0); i++)
                {
                    for (int j = 0; j < board.GetLongLength(1); j++)
                    {
                        if (board[i, j] == GameBoard.Empty)
                        {
                            board[i, j] = opponent;
                            int score = MiniMax(board, depth + 1, true, player, opponent, alpha, beta);
                            board[i, j] = GameBoard.Empty;
                            bestScore = Math.Min(score, bestScore);

                            beta = Math.Min(beta, bestScore);
                            if (beta <= alpha)
                                break;
                        }
                    }
                }

                return bestScore;
            }
        }

        static bool IsBoardFull(GameBoard[,] gameBoard)
        {
            foreach (GameBoard item in gameBoard)
                if (item is GameBoard.Empty)
                    return false;

            return true;
        }
    }
}