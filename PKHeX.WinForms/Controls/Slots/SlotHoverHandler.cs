using System;
using System.Drawing;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing;

namespace PKHeX.WinForms.Controls
{
    /// <summary>
    /// Handles Hovering operations for an editor, where only one (1) slot can be animated at a given time when hovering over it.
    /// </summary>
    public sealed class SlotHoverHandler : IDisposable
    {
        public DrawConfig Draw { private get; set; } = new();
        public bool GlowHover { private get; set; } = true;

        public static readonly CryPlayer CryPlayer = new();
        public static readonly SummaryPreviewer Preview = new();
        private static Bitmap Hover => SpriteUtil.Spriter.Hover;

        private readonly BitmapAnimator HoverWorker = new();

        private PictureBox? Slot;
        private SlotTrackerImage? LastSlot;

        public void Start(PictureBox pb, SlotTrackerImage lastSlot)
        {
            ISlotViewer<PictureBox>? view = WinFormsUtil.FindFirstControlOfType<ISlotViewer<PictureBox>>(pb);
            if (view == null)
                throw new InvalidCastException(nameof(view));
            ISlotInfo? data = view.GetSlotData(pb);
            PKM? pk = data.Read(view.SAV);
            Slot = pb;
            LastSlot = lastSlot;

            Image? orig = LastSlot.OriginalBackground = pb.BackgroundImage;

            Bitmap bg;
            if (GlowHover)
            {
                HoverWorker.Stop();
                Bitmap? hover = Hover;
                SpriteUtil.GetSpriteGlow(pk, Draw.GlowInitial.B, Draw.GlowInitial.G, Draw.GlowInitial.R, out byte[]? glowdata, out Image? GlowBase);
                bg = ImageUtil.LayerImage(GlowBase, hover, 0, 0);
                HoverWorker.GlowToColor = Draw.GlowFinal;
                HoverWorker.GlowFromColor = Draw.GlowInitial;
                HoverWorker.Start(pb, GlowBase, glowdata, orig, hover);
            }
            else
            {
                bg = Hover;
            }

            if (orig != null)
                bg = ImageUtil.LayerImage(orig, bg, 0, 0);
            pb.BackgroundImage = LastSlot.CurrentBackground = bg;

            Preview.Show(pb, pk);
        }

        public void Stop()
        {
            if (Slot != null)
            {
                if (HoverWorker.Enabled)
                    HoverWorker.Stop();
                else
                    Slot.BackgroundImage = LastSlot?.OriginalBackground;
                Slot = null;
                LastSlot = null;
            }
            Preview.Clear();
        }

        public void Dispose()
        {
            HoverWorker.Dispose();
            Slot = null;
            Draw.Dispose();
        }
    }
}