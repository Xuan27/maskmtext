using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Colors;
using System;

[assembly: CommandClass(typeof(MaskMText.Commands))]

namespace MaskMText
{
    public class Commands
    {
        [CommandMethod("BBMASK")]
        public void MaskTextCommand()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            try
            {
                // Show options
                PromptKeywordOptions pko = new PromptKeywordOptions("\nSelect action");
                pko.Keywords.Add("Mask");
                pko.Keywords.Add("Settings");
                pko.Keywords.Default = "Mask";
                pko.AllowNone = true;

                PromptResult pkr = ed.GetKeywords(pko);

                if (pkr.Status == PromptStatus.Cancel)
                    return;

                if (pkr.Status == PromptStatus.None || pkr.StringResult == "Mask")
                {
                    ApplyMasks(doc, db, ed);
                }
                else if (pkr.StringResult == "Settings")
                {
                    ShowSettings(ed);
                }
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage($"\nError: {ex.Message}");
            }
        }

        private void ApplyMasks(Document doc, Database db, Editor ed)
        {
            // Create selection filter for MText and MultiLeader
            TypedValue[] filterList = new TypedValue[]
            {
                new TypedValue((int)DxfCode.Operator, "<OR"),
                new TypedValue((int)DxfCode.Start, "MTEXT"),
                new TypedValue((int)DxfCode.Start, "MULTILEADER"),
                new TypedValue((int)DxfCode.Operator, "OR>")
            };

            SelectionFilter filter = new SelectionFilter(filterList);

            // Prompt for selection
            PromptSelectionOptions pso = new PromptSelectionOptions();
            pso.MessageForAdding = "\nSelect MText or MultiLeader objects to mask:";

            PromptSelectionResult psr = ed.GetSelection(pso, filter);

            if (psr.Status != PromptStatus.OK)
            {
                ed.WriteMessage("\nNo objects selected.");
                return;
            }

            int maskedCount = 0;
            int skippedCount = 0;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                foreach (SelectedObject selObj in psr.Value)
                {
                    if (selObj == null)
                        continue;

                    DBObject obj = tr.GetObject(selObj.ObjectId, OpenMode.ForWrite);

                    if (obj is MText mtext)
                    {
                        // Apply mask to MText
                        mtext.BackgroundFill = true;
                        mtext.UseBackgroundColor = true;

                        // Apply settings
                        mtext.BackgroundScaleFactor = MaskSettings.MaskOffset;
                        mtext.BackgroundFillColor = MaskSettings.MaskColor;

                        maskedCount++;
                    }
                    else if (obj is MLeader mleader)
                    {
                        // Apply mask to MultiLeader text
                        MLeaderStyle mleaderStyle = tr.GetObject(mleader.MLeaderStyle, OpenMode.ForRead) as MLeaderStyle;

                        if (mleaderStyle != null && mleaderStyle.ContentType == ContentType.MTextContent)
                        {
                            // Enable text frame and apply mask settings to the MText content
                            mleader.EnableFrameText = true;

                            // Get the MText content and apply background mask
                            MText leaderText = mleader.MText;
                            if (leaderText != null)
                            {
                                leaderText.BackgroundFill = true;
                                leaderText.UseBackgroundColor = true;
                                leaderText.BackgroundScaleFactor = MaskSettings.MaskOffset;
                                leaderText.BackgroundFillColor = MaskSettings.MaskColor;

                                // Update the MLeader with modified MText
                                mleader.MText = leaderText;
                            }

                            maskedCount++;
                        }
                        else
                        {
                            skippedCount++;
                        }
                    }
                }

                tr.Commit();
            }

            ed.WriteMessage($"\n{maskedCount} object(s) masked successfully.");
            if (skippedCount > 0)
                ed.WriteMessage($"\n{skippedCount} object(s) skipped (no text content).");
        }

        private void ShowSettings(Editor ed)
        {
            ed.WriteMessage("\n--- BB Mask Settings ---");
            ed.WriteMessage($"\nCurrent Mask Offset: {MaskSettings.MaskOffset}");
            ed.WriteMessage($"\nCurrent Mask Color: Index {MaskSettings.MaskColor.ColorIndex}");
            ed.WriteMessage("\n");

            // Mask offset setting
            PromptDoubleOptions pdo = new PromptDoubleOptions($"\nEnter mask offset factor [Current: {MaskSettings.MaskOffset}]");
            pdo.AllowNegative = false;
            pdo.AllowZero = false;
            pdo.DefaultValue = MaskSettings.MaskOffset;
            pdo.UseDefaultValue = true;

            PromptDoubleResult pdr = ed.GetDouble(pdo);
            if (pdr.Status == PromptStatus.OK)
            {
                MaskSettings.MaskOffset = pdr.Value;
                ed.WriteMessage($"\nMask offset set to: {MaskSettings.MaskOffset}");
            }

            // Mask color setting
            PromptIntegerOptions pio = new PromptIntegerOptions($"\nEnter mask color index (0-256) [Current: {MaskSettings.MaskColor.ColorIndex}]");
            pio.AllowNegative = false;
            pio.LowerLimit = 0;
            pio.UpperLimit = 256;
            pio.DefaultValue = (int)MaskSettings.MaskColor.ColorIndex;
            pio.UseDefaultValue = true;

            PromptIntegerResult pir = ed.GetInteger(pio);
            if (pir.Status == PromptStatus.OK)
            {
                MaskSettings.MaskColor = Color.FromColorIndex(ColorMethod.ByAci, (short)pir.Value);
                ed.WriteMessage($"\nMask color set to: Index {pir.Value}");
            }

            ed.WriteMessage("\nSettings updated successfully!");
        }
    }
}
