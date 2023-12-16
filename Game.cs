namespace TicTacToe
{
    class Game(bool aiOpponent = false, bool aiTurn = false)
    {
        public enum GameState
        {
            Empty,
            X,
            O
        }

        public GameState[,] GameBoard { get; private set; } = new GameState[3, 3];
        public List<Point> WinnerCells { get; private set; } = new(3);
        public bool Winner { get; private set; }
        public bool CurrentPlayer { get; private set; }
        public bool Tie { get; private set; }
        public bool AIOpponent { get; private set; } = aiOpponent;
        public bool AITurn { get; private set; } = aiTurn;

        public void MakeAMove(Point p)
        {
            if (GameBoard[p.X, p.Y] is not GameState.Empty)
                return;

            GameBoard[p.X, p.Y] = CurrentPlayer ? GameState.O : GameState.X;
            Winner = CheckWinner(GameState.X) || CheckWinner(GameState.O);
            Tie = IsBoardFull(GameBoard) && !Winner;
            CurrentPlayer = !CurrentPlayer;
            AITurn = !AITurn;
        }

        public void AIMove()
        {
            GameState playerState, opponentState;
            (playerState, opponentState) = CurrentPlayer ? (GameState.O, GameState.X) : (GameState.X, GameState.O);
            Point bestMove = GetBestMove(GameBoard, playerState, opponentState);
            GameBoard[bestMove.X, bestMove.Y] = playerState;
            Winner = CheckWinner(GameState.X) || CheckWinner(GameState.O);
            Tie = IsBoardFull(GameBoard) && !Winner;
            CurrentPlayer = !CurrentPlayer;
            AITurn = !AITurn;
        }

        bool CheckWinner(GameState state)
        {
            int size = GameBoard.GetLength(0);

            for (int i = 0; i < size; i++)
            {
                List<Point> row = new(3);
                List<Point> col = new(3);

                for (int j = 0; j < size; j++)
                {
                    if (GameBoard[i, j] == state)
                        row.Add(new Point(i, j));
                    if (GameBoard[j, i] == state)
                        col.Add(new Point(j, i));
                }

                if (row.Count == size)
                {
                    WinnerCells = row;
                    return true;
                }
                else if (col.Count == size)
                {
                    WinnerCells = col;
                    return true;
                }
            }

            List<Point> principal = new(3);
            List<Point> secondary = new(3);

            for (int i = 0; i < size; i++)
            {
                if (GameBoard[i, i] == state)
                    principal.Add(new Point(i, i));
                if (GameBoard[i, size - i - 1] == state)
                    secondary.Add(new Point(i, size - i - 1));

                if (principal.Count == size)
                {
                    WinnerCells = principal;
                    return true;
                }
                else if (secondary.Count == size)
                {
                    WinnerCells = secondary;
                    return true;
                }
            }

            return false;
        }

        Point GetBestMove(GameState[,] board, GameState player, GameState opponent)
        {
            int bestScore = int.MinValue;
            Point bestMove = new();
            int alpha = int.MinValue;
            int beta = int.MaxValue;

            for (int i = 0; i < board.GetLongLength(0); i++)
            {
                for (int j = 0; j < board.GetLongLength(1); j++)
                {
                    if (board[i, j] == GameState.Empty)
                    {
                        board[i, j] = player;
                        int score = MiniMax(board, 0, false, player, opponent, alpha, beta);
                        board[i, j] = GameState.Empty;

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

        int MiniMax(GameState[,] board, int depth, bool isMaximizing, GameState player, GameState opponent, int alpha, int beta)
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
                        if (board[i, j] == GameState.Empty)
                        {
                            board[i, j] = player;
                            int score = MiniMax(board, depth + 1, false, player, opponent, alpha, beta);
                            board[i, j] = GameState.Empty;
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
                        if (board[i, j] == GameState.Empty)
                        {
                            board[i, j] = opponent;
                            int score = MiniMax(board, depth + 1, true, player, opponent, alpha, beta);
                            board[i, j] = GameState.Empty;
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

        static bool IsBoardFull(GameState[,] gameBoard)
        {
            foreach (GameState item in gameBoard)
                if (item is GameState.Empty)
                    return false;

            return true;
        }
    }
}