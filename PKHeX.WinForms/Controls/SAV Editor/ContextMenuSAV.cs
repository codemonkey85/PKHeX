using System;
using System.ComponentModel;
using System.Windows.Forms;
using PKHeX.Core;

using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms.Controls
{
    public partial class ContextMenuSAV : UserControl
    {
        public ContextMenuSAV() => InitializeComponent();

        public SaveDataEditor<PictureBox> Editor { private get; set; } = null!;
        public SlotChangeManager Manager { get; set; } = null!;

        public event LegalityRequest? RequestEditorLegality;
        public delegate void LegalityRequest(object sender, EventArgs e, PKM pkm);

        public void OmniClick(object sender, EventArgs e, Keys z)
        {
            switch (z)
            {
                case Keys.Control: ClickView(sender, e); break;
                case Keys.Shift: ClickSet(sender, e); break;
                case Keys.Alt: ClickDelete(sender, e); break;
                default:
                    return;
            }

            // restart hovering since the mouse event isn't fired
            Manager.MouseEnter(sender, e);
        }

        private void ClickView(object sender, EventArgs e)
        {
            SlotViewInfo<PictureBox>? info = GetSenderInfo(ref sender);
            if ((sender as PictureBox)?.Image == null)
            { System.Media.SystemSounds.Asterisk.Play(); return; }

            Manager.Hover.Stop();
            PKM? pkm = Editor.Slots.Get(info.Slot);
            Editor.PKMEditor.PopulateFields(pkm, false, true);
        }

        private void ClickSet(object sender, EventArgs e)
        {
            IPKMView? editor = Editor.PKMEditor;
            if (!editor.EditsComplete)
                return;
            PKM pk = editor.PreparePKM();

            SlotViewInfo<PictureBox>? info = GetSenderInfo(ref sender);
            SaveFile? sav = info.View.SAV;

            if (!CheckDest(info, sav, pk))
                return;

            System.Collections.Generic.IReadOnlyList<string>? errata = sav.IsPKMCompatible(pk);
            if (errata.Count > 0 && DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, string.Join(Environment.NewLine, errata), MsgContinue))
                return;

            Manager.Hover.Stop();
            Editor.Slots.Set(info.Slot, pk);
            Manager.SE.UpdateUndoRedo();
        }

        private void ClickDelete(object sender, EventArgs e)
        {
            SlotViewInfo<PictureBox>? info = GetSenderInfo(ref sender);
            if ((sender as PictureBox)?.Image == null)
            { System.Media.SystemSounds.Asterisk.Play(); return; }

            SaveFile? sav = info.View.SAV;
            PKM? pk = sav.BlankPKM;
            if (!CheckDest(info, sav, pk))
                return;

            Manager.Hover.Stop();
            Editor.Slots.Delete(info.Slot);
            Manager.SE.UpdateUndoRedo();
        }

        private static bool CheckDest(SlotViewInfo<PictureBox> info, SaveFile sav, PKM pk)
        {
            WriteBlockedMessage msg = info.Slot.CanWriteTo(sav, pk);
            if (msg == WriteBlockedMessage.None)
                return true;

            switch (msg)
            {
                case WriteBlockedMessage.InvalidPartyConfiguration:
                    WinFormsUtil.Alert(MsgSaveSlotEmpty);
                    break;
                case WriteBlockedMessage.IncompatibleFormat:
                    break;
                case WriteBlockedMessage.InvalidDestination:
                    WinFormsUtil.Alert(MsgSaveSlotLocked);
                    break;
                default:
                    throw new IndexOutOfRangeException(nameof(msg));
            }
            return false;
        }

        private void ClickShowLegality(object sender, EventArgs e)
        {
            SlotViewInfo<PictureBox>? info = GetSenderInfo(ref sender);
            SaveFile? sav = info.View.SAV;
            PKM? pk = info.Slot.Read(sav);
            RequestEditorLegality?.Invoke(sender, e, pk);
        }

        private void MenuOpening(object sender, CancelEventArgs e)
        {
            ToolStripItemCollection? items = ((ContextMenuStrip)sender).Items;

            object ctrl = ((ContextMenuStrip)sender).SourceControl;
            SlotViewInfo<PictureBox>? info = GetSenderInfo(ref ctrl);
            bool SlotFull = (ctrl as PictureBox)?.Image != null;
            bool Editable = info.Slot.CanWriteTo(info.View.SAV);
            bool legality = ModifierKeys == Keys.Control;
            ToggleItem(items, mnuSet, Editable);
            ToggleItem(items, mnuDelete, Editable && SlotFull);
            ToggleItem(items, mnuLegality, legality && SlotFull && RequestEditorLegality != null);
            ToggleItem(items, mnuView, SlotFull || !Editable, true);

            if (items.Count == 0)
                e.Cancel = true;
        }

        private static SlotViewInfo<PictureBox> GetSenderInfo(ref object sender)
        {
            PictureBox? pb = WinFormsUtil.GetUnderlyingControl<PictureBox>(sender);
            if (pb == null)
                throw new InvalidCastException("Unable to find PictureBox");
            ISlotViewer<PictureBox>? view = WinFormsUtil.FindFirstControlOfType<ISlotViewer<PictureBox>>(pb);
            if (view == null)
                throw new InvalidCastException("Unable to find View Parent");
            ISlotInfo? loc = view.GetSlotData(pb);
            sender = pb;
            return new SlotViewInfo<PictureBox>(loc, view);
        }

        private static void ToggleItem(ToolStripItemCollection items, ToolStripItem item, bool visible, bool first = false)
        {
            if (visible)
            {
                if (first)
                    items.Insert(0, item);
                else
                    items.Add(item);
            }
            else if (items.Contains(item))
            {
                items.Remove(item);
            }
        }
    }
}
