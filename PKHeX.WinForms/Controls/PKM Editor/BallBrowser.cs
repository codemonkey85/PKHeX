using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using PKHeX.Core;
using PKHeX.Drawing;

namespace PKHeX.WinForms
{
    public partial class BallBrowser : Form
    {
        public BallBrowser() => InitializeComponent();

        public int BallChoice { get; private set; } = -1;

        public void LoadBalls(Ball[] poss, ICollection<Ball> legal, IReadOnlyList<ComboItem> names)
        {
            for (int i = 0; i < poss.Length; i++)
            {
                PictureBox? pb = GetBallView(poss[i], legal, names);
                flp.Controls.Add(pb);
                const int width = 5; // balls wide
                if (i % width == width - 1)
                    flp.SetFlowBreak(pb, true);
            }
        }

        public void LoadBalls(PKM pkm)
        {
            Ball[]? legal = BallApplicator.GetLegalBalls(pkm).ToArray();
            Ball[]? poss = ((Ball[])Enum.GetValues(typeof(Ball))).Skip(1)
                .TakeWhile(z => (int)z <= pkm.MaxBallID).ToArray();
            IReadOnlyList<ComboItem>? names = GameInfo.BallDataSource;
            LoadBalls(poss, legal, names);
        }

        private PictureBox GetBallView(Ball b, ICollection<Ball> legal, IReadOnlyList<ComboItem> names)
        {
            System.Drawing.Image? img = SpriteUtil.GetBallSprite((int)b);
            PictureBox? pb = new PictureBox
            {
                Size = img.Size,
                Image = img,
                BackgroundImage = legal.Contains(b) ? SpriteUtil.Spriter.Set : SpriteUtil.Spriter.Delete,
                BackgroundImageLayout = ImageLayout.Tile
            };
            pb.MouseEnter += (_, __) => Text = names.First(z => z.Value == (int)b).Text;
            pb.Click += (_, __) => SelectBall(b);
            return pb;
        }

        private void SelectBall(Ball b)
        {
            BallChoice = (int)b;
            Close();
        }
    }
}
