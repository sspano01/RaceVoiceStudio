using RaceVoice;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackImageGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (var f in Directory.GetFiles(@"C:\code\RaceVoiceStudio\RaceVoice\bin\Release\Tracks", "*.csv"))
            {
                var fn = Path.GetFileNameWithoutExtension(f);
                var tmd = TrackMetadata.Load(@"C:\code\RaceVoiceStudio\RaceVoice\bin\Release\Tracks\" + fn + ".json");
                var rendererSettings = new TrackRendererSettings(tmd.ClusterSize)
                {
                    BackgroundColor = Color.White,
                    DefaultSegmentColor = Color.CornflowerBlue,
                    MouseIndicatorColor = Color.Orange,
                    MouseIndicatorSize = 10,
                    SegmentFont = new Font("Consolas", 16, FontStyle.Bold, GraphicsUnit.Pixel),
                    SegmentLabelColor = Color.Black,
                    SegmentResizeHandleColor = Color.DarkGreen,
                    SegmentResizeHandleSize = 16,
                    SelectedSegmentColor = Color.LawnGreen,
                    SegmentThickness = 10,
                    SplitFont = new Font("Consolas", 16, GraphicsUnit.Pixel),
                    SplitIndicatorColor = Color.Blue,
                    InactiveColor = Color.Gray,
                    SplitIndicatorSize = 20,
                    SplitIndicatorThickness = 4,
                    TrackColor = Color.Black,
                    TrackThickness = 4,
                    ChequeredFlagImage = @"C:\code\RaceVoiceStudio\RaceVoice\bin\Release\flag.png",

                    UseCurveRendering = tmd.UseCurveRendering,
                    ShowGpsPoints = false,
                    GpsPointSize = 3,
                    GpsPointColor = Color.Yellow
                };

                var outFile = fn + ".png";
                if (!File.Exists(outFile))
                {
                    var md = TrackModelParser.LoadFile(f);
                    TrackRenderer.SmoothTrack(md, 10);
                    var tr = new TrackRenderer(md, rendererSettings);
                    using (var bmp = new Bitmap(1024, 1024))
                    {
                        tr.Render(bmp);
                        bmp.Save(outFile);
                    }
                }
            }
        }
    }
}
