namespace TicTacToe
{
    public partial class TicTacToeForm : Form
    {
        readonly Dictionary<byte, Image> images = new Dictionary<byte, Image>
        {
            [1] = Image.FromFile(@"../../../images/x.png"),
            [2] = Image.FromFile(@"../../../images/o.png"),
            [3] = Image.FromFile(@"../../../images/pve.png"),
            [4] = Image.FromFile(@"../../../images/pvp.png"),
            [5] = Image.FromFile(@"../../../images/restart.png")
        };

        readonly PictureBox[,] gameBoardCells = new PictureBox[3, 3];
        readonly PictureBox pvePb = new();
        readonly RadioButton pvpRb = new();
        readonly RadioButton xRb = new();
        readonly RadioButton oRb = new();
        readonly PictureBox restartPb = new();
        readonly PictureBox nextMovePb = new();
        readonly Size pbSize = new(150, 150);
        Game? game = null;

        public TicTacToeForm()
        {
            BuildMainForm();
            NewGame();
            InitializeComponent();

            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = Icon.ExtractAssociatedIcon(@"../../../images/icon.ico");
            MinimizeBox = MaximizeBox = false;
            BackColor = Color.LightSkyBlue;
            Size = new Size(pbSize.Width * 4 + pbSize.Width / 2, pbSize.Height * 3 + pbSize.Height / 3 + 5);
            Text = "Tic-Tac-Toe";
        }

        void BuildMainForm()
        {
            for (byte i = 0; i < gameBoardCells.GetLength(0); i++)
            {
                for (byte j = 0; j < gameBoardCells.GetLength(1); j++)
                {
                    PictureBox p = new PictureBox();
                    p.Location = new Point(p.Location.X + (i * pbSize.Width) + 5, p.Location.Y + (j * pbSize.Height) + 5);
                    p.Size = pbSize;
                    p.BorderStyle = BorderStyle.Fixed3D;
                    p.Tag = new Point(i, j);
                    p.Cursor = Cursors.Hand;
                    p.SizeMode = PictureBoxSizeMode.StretchImage;

                    gameBoardCells[i, j] = p;
                    Controls.Add(gameBoardCells[i, j]);
                    gameBoardCells[i, j].Click += GameBoardCell_Click;
                }
            }

            pvePb.Location = new Point(pvePb.Location.X + pbSize.Width * 3 + 50, pvePb.Location.Y + 15);
            xRb.Location = new Point(xRb.Location.X + pbSize.Width * 3 + 70, xRb.Location.Y + 90);
            oRb.Location = new Point(oRb.Location.X + pbSize.Width * 3 + 70, oRb.Location.Y + 140);
            pvpRb.Location = new Point(pvpRb.Location.X + pbSize.Width * 3 + 50, pvpRb.Location.Y + 170);
            nextMovePb.Location = new Point(nextMovePb.Location.X + pbSize.Width + 330, nextMovePb.Location.Y + 350);
            restartPb.Location = new Point(restartPb.Location.X + pbSize.Width + 420, restartPb.Location.Y + 350);

            restartPb.SizeMode =
            nextMovePb.SizeMode = PictureBoxSizeMode.StretchImage;
            pvePb.SizeMode = PictureBoxSizeMode.Zoom;

            xRb.BackgroundImageLayout =
            oRb.BackgroundImageLayout =
            pvpRb.BackgroundImageLayout = ImageLayout.Zoom;

            xRb.BackgroundImage = images[1];
            oRb.BackgroundImage = images[2];
            pvpRb.BackgroundImage = images[4];
            restartPb.Image = images[5];
            pvePb.Image = images[3];

            pvePb.Size = new Size(70, 70);
            nextMovePb.Size =
            restartPb.Size = new Size(60, 60);

            pvpRb.Height = 70;
            pvePb.Width =
            pvpRb.Width = 100;

            xRb.Height =
            oRb.Height = 30;
            xRb.Width =
            oRb.Width = 70;

            xRb.Checked = true;

            xRb.CheckedChanged += new EventHandler(StartNewGame);
            oRb.CheckedChanged += new EventHandler(StartNewGame);
            pvpRb.CheckedChanged += new EventHandler(StartNewGame);
            restartPb.Click += new EventHandler(StartNewGame);

            Controls.Add(pvePb);
            Controls.Add(pvpRb);
            Controls.Add(xRb);
            Controls.Add(oRb);
            Controls.Add(restartPb);
            Controls.Add(nextMovePb);
        }

        private async void GameBoardCell_Click(object sender, EventArgs e)
        {
            game.MakeAMove((Point)(sender as Control).Tag);

            if (game.AIOpponent && game.AITurn && !game.Tie)
                game.AIMove();

            BuildGameBoard(game);

            if (game.Winner || game.Tie)
            {
                Action GameOver = (game.Winner, game.Tie) switch
                {
                    (true, _) => () => game.winnerCells.ForEach(cell => gameBoardCells[cell.X, cell.Y].BackColor = Color.Chartreuse),
                    _ => () =>
                    {
                        foreach (PictureBox item in gameBoardCells)
                            item.BackColor = Color.Plum;
                    }
                };

                GameOver();

                foreach (PictureBox item in gameBoardCells)
                    item.Enabled = false;

                await Task.Delay(1000);

                NewGame();
            }

            BuildGameBoard(game);
        }

        void BuildGameBoard(Game game)
        {
            nextMovePb.Image = game.CurrentPlayer ? images[2] : images[1];

            for (int i = 0; i < gameBoardCells.GetLength(0); i++)
            {
                for (int j = 0; j < gameBoardCells.GetLength(1); j++)
                {
                    switch (game.gameBoard[i, j])
                    {
                        case Game.GameBoard.Empty:
                            gameBoardCells[i, j].Image = null;
                            break;
                        case Game.GameBoard.X:
                            gameBoardCells[i, j].Image = images[1];
                            gameBoardCells[i, j].Cursor = Cursors.No;
                            break;
                        case Game.GameBoard.O:
                            gameBoardCells[i, j].Image = images[2];
                            gameBoardCells[i, j].Cursor = Cursors.No;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        void NewGame()
        {
            ClearGameBoard();

            game = (xRb.Checked, oRb.Checked, pvpRb.Checked) switch
            {
                (true, false, false) => new Game(true, false),
                (false, true, false) => new Game(true, true),
                _ => new Game()
            };
            if (game.AITurn)
                game.AIMove();

            BuildGameBoard(game);
        }

        void ClearGameBoard()
        {
            foreach (PictureBox item in gameBoardCells)
            {
                item.Enabled = true;
                item.BackColor = Color.LightSkyBlue;
                item.Image = null;
                item.Cursor = Cursors.Hand;
            }
        }

        private void StartNewGame(object? sender, EventArgs e) => NewGame();
    }
}