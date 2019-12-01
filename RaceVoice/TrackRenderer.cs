using RaceVoice.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
#if (!APP)
using System.Windows.Forms;
#else
using SkiaSharp;
using SkiaSharp.Views.Forms;
#endif


namespace RaceVoice
{
    public class TrackRenderer
    {
        public TrackSegment SelectedSegment { get; private set; }
        public PointF CenterOffset { get; set; }
        public float Zoom { get; set; }
        public float Rotation { get; set; }

        private const string SEGMENT_COLOR_REGEX = @"\((\d+) (\d+) (\d+)\)";
        private const string SEGMENT_DATA_REGEX = @"\<([0-9a-fA-F]{4})\>";
        private const string SEGMENT_FORMAT_STRING = "{0}({1} {2} {3})";

        private const double DEG2RAD = 0.017453292;
        private const double RAD2DEG = 57.29578049;

        private bool _mouseDown;
        private bool _hasStartResizeGrab;
        private bool _hasEndResizeGrab;

        private readonly PointF[] _normalisedTrack;
        private readonly RectangleF _normalisedBounds;

        private PointF[] _transformedTrack;
        private PointF[] _clusteredTrack;
        private List<PointF[]> _transformedSegments = null;
        private readonly TrackModel _model;

        private int _closestMousePoint;

        private int _selectedSegmentMaxBounds;
        private int _selectedSegmentMinBounds;

        private TrackRendererSettings _settings;

#if (!APP)
        private Image _chequeredFlagImage;
        private static StringFormat _splitStringFormat;
        private static StringFormat _segmentStringFormat;

#else

#endif
        static TrackRenderer()
        {
#if (!APP)
            _splitStringFormat = new StringFormat();
            _splitStringFormat.LineAlignment = StringAlignment.Center;

            _segmentStringFormat = new StringFormat();
            _segmentStringFormat.LineAlignment = StringAlignment.Center;
            _segmentStringFormat.Alignment = StringAlignment.Center;
#endif
        }

        public TrackRenderer(TrackModel trackModel, TrackRendererSettings settings)
        {
            if (trackModel == null)
            {
                throw new ArgumentNullException("trackModel");
            }

            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            _settings = settings;

            _model = trackModel;
            _normalisedTrack = GetNormalisedTrackPoints(trackModel, out _normalisedBounds);
            _transformedTrack = new PointF[_normalisedTrack.Length];
            _clusteredTrack = new PointF[_normalisedTrack.Length / _settings.ClusterSize];

#if !APP
            _chequeredFlagImage = Image.FromFile(_settings.ChequeredFlagImage);
#endif
            Zoom = 1f;
        }

        private static PointD LatLngToXY(double lat, double lng)
        {
            lat = lat * DEG2RAD;
            lng = lng * DEG2RAD;

            //https://en.wikipedia.org/wiki/Web_Mercator_projection#Formulas
            var x = lng + Math.PI;

            var y = lat / 2;
            y += Math.PI / 4;
            y = Math.Tan(y);
            y = Math.Log(y);
            y = Math.PI - y;

            return new PointD() { X = x, Y = y };
        }

        private static PointD XYToLatLng(double x, double y)
        {
            var lat = Math.PI - y;
            lat = Math.Exp(lat);
            lat = Math.Atan(lat);
            lat -= Math.PI / 4;
            lat *= 2;

            var lng = x - Math.PI;

            return new PointD(lat * RAD2DEG , lng * RAD2DEG);
        }

        private static double LatLngDistance(double latA, double lngA, double latB, double lngB)
        {
            const double R = 6371e3; // metres
            var phi1 = latA * DEG2RAD;
            var phi2 = latB * DEG2RAD;
            var dphi = phi2 - phi1;
            var dlng = (lngB - lngA) * DEG2RAD;

            var a = Math.Sin(dphi / 2) * Math.Sin(dphi / 2) +
                    Math.Cos(phi1) * Math.Cos(phi2) *
                    Math.Sin(dlng / 2) * Math.Sin(dlng / 2);

            var c = 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));

            return R * c;
        }

        /// <summary>
        /// Add in extra GPS points if distance between two points is greater than a specified distance (in meters)
        /// </summary>
        public static void SmoothTrack(TrackModel model, double maximumDistanceBetweenPoints)
        {
            for (int i = 1; i <= model.Waypoints.Count; i++)
            {
                if (model.IsStraightTrack && i == model.Waypoints.Count)
                {
                    //We don't want to measure distance between the last and first points of a
                    //track that isn't a closed loop
                    break;
                }

                var p1 = model.Waypoints[i%model.Waypoints.Count];
                var p2 = model.Waypoints[i - 1];

                //Distance (meters) between two gps points
                var dist = LatLngDistance(p1.Latitude, p1.Longitude, p2.Latitude, p2.Longitude);

                if (dist > maximumDistanceBetweenPoints)
                {
                    //Convert to cartesian co-ordinates
                    var xy1 = LatLngToXY(p1.Latitude, p1.Longitude);
                    var xy2 = LatLngToXY(p2.Latitude, p2.Longitude);

                    //Measure cartesian distance
                    var dxy = new PointD(xy1.X - xy2.X, xy1.Y - xy2.Y);
                    var len = Math.Sqrt(dxy.X * dxy.X + dxy.Y * dxy.Y);

                    //Calculate number of points we need to insert
                    int neededPoints = (int)(dist / maximumDistanceBetweenPoints);
                    var md = len / neededPoints;

                    neededPoints -= 1;

                    //Calculate the length that each new line should be
                    var sxy = new PointD((dxy.X / len) * md, (dxy.Y / len) * md);

                    for (int n = 0; n < neededPoints; n++)
                    {
                        //Fill in the new points
                        //E.G. Where i == 2,
                        //   [1-------------------2---3] becomes
                        //   [1---2---3---4---5---6---7]
                        var np = new PointD(xy2.X + (sxy.X * (n + 1)), xy2.Y + (sxy.Y * (n + 1)));
                        var npll = XYToLatLng(np.X, np.Y);

                        model.Waypoints.Insert(i + n, new Waypoint { Latitude = npll.X, Longitude = npll.Y });
                    }

                    //If we added points, we need to fix all of the segment, split and flag references
                    //as the indexes they point to are no longer correct

                    //Fix segments
                    foreach (var seg in model.Segments)
                    {
                        if (seg.StartIndex >= i)
                        {
                            seg.StartIndex += neededPoints;
                        }

                        if (seg.EndIndex >= i)
                        {
                            seg.EndIndex += neededPoints;
                        }
                    }

                    //Fix splits
                    foreach (var split in model.Splits)
                    {
                        if (split.Index >= i)
                        {
                            split.Index += neededPoints;
                        }
                    }

                    //Fix flag position
                    if (model.FlagPosition >= i)
                    {
                        model.FlagPosition += neededPoints;

                    }

                    //Jump past all the new points we just added as we already know 
                    //for sure that they meet the distance requirement
                    i += neededPoints;
                }
            }

            // now find the gps position of the start point on the track
            model.StartLinePosition = model.Waypoints[model.FlagPosition];

        }
#if APP
        private static void DrawGradientCurve(PointF[] points, TrackColor startColor, TrackColor endColor, SkiaSharp.SKCanvas g, float thickness)
#else
        private static void DrawGradientCurve(PointF[] points, TrackColor startColor, TrackColor endColor, Graphics g, float thickness)
#endif
        {
            for (int i = 0; i < points.Length - 1; i++)
            {
                var p1 = points[i];
                var p2 = points[i + 1];
                if (p1 == p2)
                {
                    continue;
                }
#if APP

                SKPaint green = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    Color = SKColors.LawnGreen,
                    StrokeWidth = 10
                };
                SKPoint sp1 = new SKPoint();
                SKPoint sp2 = new SKPoint();

                sp1.X = p1.X;
                sp1.Y = p1.Y;
                sp2.X = p2.X;
                sp2.Y = p2.Y;
                g.DrawLine(sp1,sp2, green);
#else
                var st = (float)i / points.Length;
                var sc = TrackColor.Lerp(startColor, endColor, st);

                var et = (float)(i + 1) / points.Length;
                var ec = TrackColor.Lerp(startColor, endColor, et);

                using (var b = new LinearGradientBrush(p1, p2, sc.ToDrawingColor(), ec.ToDrawingColor()))
                using (var p = new Pen(b, thickness))
                {
                    g.DrawLine(p, p1, p2);
                }
#endif
            }
        }

       

        private void UpdateSelectedSegmentBounds()
        {
            var leftSegment = _model.Segments
                .Where(s => s.EndIndex < SelectedSegment.StartIndex)
                .OrderBy(s => Math.Abs(s.EndIndex - SelectedSegment.StartIndex))
                .FirstOrDefault();

            _selectedSegmentMinBounds = leftSegment != null ? leftSegment.EndIndex + 1 : 0;

            var rightSegment = _model.Segments
                .Where(s => s.StartIndex > SelectedSegment.EndIndex)
                .OrderBy(s => Math.Abs(s.EndIndex - SelectedSegment.StartIndex))
                .FirstOrDefault();

            _selectedSegmentMaxBounds = rightSegment != null ? rightSegment.StartIndex - 1 : _model.Waypoints.Count - 1;
        }

        public void SelectSegment(TrackSegment segment)
        {
            if (segment == null)
            {
                SelectedSegment = null;
                return;
            }

            int idx = _model.Segments.IndexOf(segment);
            if (idx < 0)
            {
                SelectedSegment = null;
                return;
            }

            SelectedSegment = segment;
            UpdateSelectedSegmentBounds();
        }

        private int FindClosestPointOnTrack(PointF pt)
        {
            int closest = 0;
            double dist = double.MaxValue;
            for (int i = 0; i < _clusteredTrack.Length; i++)
            {
                var p = _clusteredTrack[i];
                var d = Distance(p, pt);
                if (d < dist)
                {
                    closest = i * _settings.ClusterSize;
                    dist = d;
                }
            }

            return closest;
        }

#if (!APP)

         public bool HandleClick(object sender, EventArgs e)
        {
            var me = e as MouseEventArgs;
            PointF mouse = new PointF(me.X, me.Y);
            if (me != null)
            {
                int closest = FindClosestPointOnTrack(mouse);

                bool found = false;
                foreach (var seg in _model.Segments)
                {
                    if (closest >= seg.StartIndex && closest <= seg.EndIndex)
                    {
                        found = true;
                        SelectedSegment = seg;
                        break;
                    }
                }

                if (found)
                {
                    UpdateSelectedSegmentBounds();
                }
                else
                {
                    SelectedSegment = null;
                }
            }

            return true;
        }


        public bool HandleMouseDown(object sender, MouseEventArgs e)
        {
            _mouseDown = true;

            if (SelectedSegment != null)
            {
                var m = new PointF(e.X, e.Y);

                var idx = _model.Segments.IndexOf(SelectedSegment);
                if (idx < 0) return true; // this can be out of range sometimes
                if (idx > _transformedSegments.Count) return true;

                var p1 = _transformedSegments[idx].First();
                var p2 = _transformedSegments[idx].Last();

                _hasStartResizeGrab = Distance(p1, m) <= _settings.SegmentResizeHandleSize * Zoom;
                _hasEndResizeGrab = Distance(p2, m) <= _settings.SegmentResizeHandleSize * Zoom && !_hasStartResizeGrab;
            }

            return true;
        }

        public bool HandleMouseUp(object sender, MouseEventArgs e)
        {
            _mouseDown = false;
            _hasStartResizeGrab = false;
            _hasEndResizeGrab = false;
            return true;
        }

        public bool HandleMouseMove(object sender, MouseEventArgs e)
        {
            bool needsRerender = false;

            var m = new PointF(e.X, e.Y);
            var newClosest = FindClosestPointOnTrack(m);
            needsRerender = newClosest != _closestMousePoint;

            _closestMousePoint = newClosest;

            if (_mouseDown && SelectedSegment != null)
            {
                if (_hasStartResizeGrab)
                {
                    if (_closestMousePoint != SelectedSegment.StartIndex &&
                        _closestMousePoint >= _selectedSegmentMinBounds &&
                        _closestMousePoint <= _selectedSegmentMaxBounds &&
                        _closestMousePoint < SelectedSegment.EndIndex - (_settings.ClusterSize * 3))
                    {
                        SelectedSegment.StartIndex = _closestMousePoint;
                        _transformedSegments = null;
                        return true;
                    }
                }

                if (_hasEndResizeGrab)
                {
                    if (_closestMousePoint != SelectedSegment.EndIndex &&
                        _closestMousePoint >= _selectedSegmentMinBounds &&
                        _closestMousePoint <= _selectedSegmentMaxBounds &&
                        _closestMousePoint > SelectedSegment.StartIndex)
                    {
                        SelectedSegment.EndIndex = _closestMousePoint;
                        _transformedSegments = null;
                        return true;
                    }
                }
            }

            return needsRerender;
        }

#endif

        private static double Distance(PointF a, PointF b)
        {
            return Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));
        }

#if APP
        public void Render(SkiaSharp.SKCanvas g, SKImageInfo info)
#else
        public void Render(Bitmap renderTarget)
#endif
        {
            if (_transformedSegments == null)
            {
                _transformedSegments = new List<PointF[]>(_model.Segments.Count);
                foreach (var segment in _model.Segments)
                {
                    var size = (int)Math.Ceiling((float)(segment.EndIndex - segment.StartIndex) / _settings.ClusterSize)+1;
                    size = Math.Max(size, 3);
                    _transformedSegments.Add(new PointF[size]);
                }
            }


#if !APP
            using (var g = Graphics.FromImage(renderTarget))
#endif
            {
#if !APP
                using (var b = new SolidBrush(_settings.BackgroundColor))
                {
                    g.FillRectangle(b, 0, 0, renderTarget.Width, renderTarget.Height);
                }

                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.PixelOffsetMode = PixelOffsetMode.Half;
#endif
                RectangleF rotatedBounds = default(RectangleF);
                Rotate(_normalisedTrack, _transformedTrack, Rotation, out rotatedBounds);

#if !APP
                float scale = Math.Min(renderTarget.Width / rotatedBounds.Width, renderTarget.Height / rotatedBounds.Height) * Zoom;

                var centerX = renderTarget.Width / 2;
                var centerY = renderTarget.Height / 2;
#else
                float scale = Math.Min(info.Width,info.Height) * Zoom;

                var centerX = info.Width / 2;
                var centerY = info.Height / 2;
#endif
                var offset = new PointF()
                {
                    X = -(rotatedBounds.Left * scale) + centerX - (rotatedBounds.Width * scale / 2),
                    Y = -(rotatedBounds.Top * scale) + centerY - (rotatedBounds.Height * scale / 2)
                };

                offset.X += CenterOffset.X;
                offset.Y += CenterOffset.Y;

                ScaleAndTranslate(_transformedTrack, _transformedTrack, scale, offset);

                for (int i = 0; i < _clusteredTrack.Length; i++)
                {
                    _clusteredTrack[i] = _transformedTrack[i * _settings.ClusterSize];
                }

                for (int i = 0; i < _model.Segments.Count; i++)
                {
                    var segment = _model.Segments[i];
                    var segmentSize = segment.EndIndex - segment.StartIndex;
                    int cs = _settings.ClusterSize;
                    if (segmentSize < _settings.ClusterSize * 3)
                    {
                        cs = segmentSize / 3;
                    }
                    for (int j = 0; j < _transformedSegments[i].Length; j++)
                    {
                        //_transformedSegments[i][j] = _transformedTrack[segment.StartIndex + (j * cs)];
                        int clusteredIndex = (segment.StartIndex / _settings.ClusterSize) + j;
                        if (clusteredIndex < _clusteredTrack.Length)
                        {
                            _transformedSegments[i][j] = _clusteredTrack[clusteredIndex];
                        }
                    }
                }
#if APP
                int k = 0;

                SKPaint black = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    Color = SKColors.LightGray,
                    StrokeWidth = 2
                };



                while (k<_clusteredTrack.Count()-1)
                {
                    SKPoint p0 = new SKPoint();
                    SKPoint p1 = new SKPoint();

                    p0.X = _clusteredTrack[k].X;
                    p0.Y = _clusteredTrack[k].Y;
                    k++;

                    p1.X = _clusteredTrack[k].X;
                    p1.Y = _clusteredTrack[k].Y;

                    g.DrawLine(p0,p1,black);

                }

                for (int i = 0; i < _model.Segments.Count; i++)
                {
                    var segment = _model.Segments[i];
                    var segmentPoints = _transformedSegments[i];
                    var startColor = TrackColor.Gray;
                    var endColor = TrackColor.Gray;
                    if (segment.DataBits!=0)
                    {
                        DrawGradientCurve(segmentPoints, startColor, endColor, g, _settings.SegmentThickness * Zoom);
                    }

                }
#else
                using (var b = new SolidBrush(_settings.TrackColor))
                using (var p = new Pen(b, _settings.TrackThickness * Zoom))
                {
                    if (_model.IsStraightTrack)
                    {
                        if (_settings.UseCurveRendering)
                        {
                            g.DrawCurve(p, _clusteredTrack);
                        }
                        else
                        {
                            g.DrawLines(p, _clusteredTrack);
                        }
                    }
                    else
                    {
                        if (_settings.UseCurveRendering)
                        {
                            g.DrawClosedCurve(p, _clusteredTrack);
                        }
                        else
                        {
                            g.DrawLines(p, _clusteredTrack);
                            g.DrawLine(p, _clusteredTrack.Last(), _clusteredTrack.First());
                        }
                    }
                }

                var flagDirection = new PointF(_transformedTrack[_model.FlagPosition+1].X - _transformedTrack[_model.FlagPosition].X, _transformedTrack[_model.FlagPosition+1].Y - _transformedTrack[_model.FlagPosition].Y);
                var flagRotation = Math.Atan2(flagDirection.Y, flagDirection.X);
                g.TranslateTransform(_transformedTrack[_model.FlagPosition].X, _transformedTrack[_model.FlagPosition].Y);
                g.RotateTransform((float)((flagRotation + (Math.PI / 2)) * RAD2DEG));
                g.DrawImage(_chequeredFlagImage, (-_chequeredFlagImage.Width * Zoom / 2) + (_settings.TrackThickness * Zoom / 2), -_chequeredFlagImage.Height * Zoom / 2, _chequeredFlagImage.Width * Zoom, _chequeredFlagImage.Height * Zoom);
                g.ResetTransform();

                for (int i = 0; i < _model.Segments.Count; i++)
                {
                    var segment = _model.Segments[i];
                    var segmentPoints = _transformedSegments[i];
                    var startColor = TrackColor.Gray;
                    var endColor = TrackColor.Gray;
                    if (!segment.Hidden)
                    {
                        startColor = SelectedSegment == segment ? _settings.SelectedSegmentColor.ToTrackColor() : segment.StartColor;
                        endColor = SelectedSegment == segment ? _settings.SelectedSegmentColor.ToTrackColor() : segment.EndColor;
                    }
                    
                        DrawGradientCurve(segmentPoints, startColor, endColor, g, _settings.SegmentThickness * Zoom);

                        var labelPosition = segmentPoints[segmentPoints.Length / 2];
                        using (var b = new SolidBrush(_settings.SegmentLabelColor))
                        {
                            g.DrawString(segment.Name, _settings.SegmentFont, b, labelPosition, _segmentStringFormat);
                        }

                        if (SelectedSegment == segment)
                        {
                            using (var b = new SolidBrush(_settings.SegmentResizeHandleColor))
                            {
                                g.FillEllipse(b, new RectangleF(segmentPoints.First().X - (_settings.SegmentResizeHandleSize * Zoom / 2), segmentPoints.First().Y - (_settings.SegmentResizeHandleSize * Zoom / 2), _settings.SegmentResizeHandleSize * Zoom, _settings.SegmentResizeHandleSize * Zoom));
                                g.FillEllipse(b, new RectangleF(segmentPoints.Last().X - (_settings.SegmentResizeHandleSize * Zoom / 2), segmentPoints.Last().Y - (_settings.SegmentResizeHandleSize * Zoom / 2), _settings.SegmentResizeHandleSize * Zoom, _settings.SegmentResizeHandleSize * Zoom));

                            }
                        }
                    
                }

              
                    for (int i = 0; i < _model.Splits.Count; i++)
                    {
                        var split = _model.Splits[i];
                        var b = new SolidBrush(_settings.SplitIndicatorColor);
                        var p = new Pen(b, _settings.SplitIndicatorThickness * Zoom);
                        if (split.Hidden)
                        {
                            b = new SolidBrush(_settings.InactiveColor);
                            p = new Pen(b, _settings.SplitIndicatorThickness * Zoom);
                        }
                        

                        var pos = _transformedTrack[split.Index];
                        pos.X -= (_settings.SplitIndicatorSize / 2) * Zoom;
                        pos.Y -= (_settings.SplitIndicatorSize / 2) * Zoom;

                        g.DrawEllipse(p, new RectangleF(pos, new SizeF(_settings.SplitIndicatorSize * Zoom, _settings.SplitIndicatorSize * Zoom)));

                        g.DrawString(split.Text, _settings.SplitFont, b, new PointF(_transformedTrack[split.Index].X + (_settings.SplitIndicatorSize * Zoom), _transformedTrack[split.Index].Y), _splitStringFormat);

                }

                using (var b = new SolidBrush(_settings.MouseIndicatorColor))
                {
                    var highlightPos = _transformedTrack[_closestMousePoint];
                    g.FillEllipse(b, highlightPos.X - (_settings.MouseIndicatorSize * Zoom / 2), highlightPos.Y - (_settings.MouseIndicatorSize * Zoom / 2), _settings.MouseIndicatorSize * Zoom, _settings.MouseIndicatorSize * Zoom);
                }

                if (_settings.ShowGpsPoints)
                {
                    int x = 0;
                    using (var b = new SolidBrush(_settings.GpsPointColor))
                    {
                        foreach (var pt in _clusteredTrack)
                        {
                            g.FillEllipse(b, pt.X - _settings.GpsPointSize * Zoom * 0.5f, pt.Y - _settings.GpsPointSize * Zoom * 0.5f, _settings.GpsPointSize * Zoom, _settings.GpsPointSize * Zoom);
                            x++;
                        }
                    }
                }
#endif
                }
        }

        private static PointF Rotate(PointF p, float rad)
        {
            var sin = (float)Math.Sin(rad);
            var cos = (float)Math.Cos(rad);

            return new PointF()
            {
                X = (cos * p.X) - (sin * p.Y),
                Y = (sin * p.X) + (cos * p.Y)
            };
        }

        private static void ScaleAndTranslate(PointF[] src, PointF[] dst, float scale, PointF translate)
        {
            Parallel.For(0, src.Length, (i) =>
            {
                dst[i] = new PointF
                {
                    X = (src[i].X * scale) + translate.X,
                    Y = (src[i].Y * scale) + translate.Y
                };
            });
        }

        public bool CanWeMakeASegmentHere()
        {
            bool valid = true;
            var start = Math.Max(_closestMousePoint - _settings.ClusterSize * 4, 0);
            var end = Math.Min(_closestMousePoint + _settings.ClusterSize * 4, _model.Waypoints.Count - 1);

            for (int i = 0; i < _model.Segments.Count; i++)
            {
                var segment = _model.Segments[i];
                if (start>=segment.StartIndex)
                {
                    // ok so we know the new segment starts at least at or beyond an existing segment...
                    if (end<= segment.EndIndex)
                    {
                        // and the end of an existing segment falls within the end of the new segment...
                        valid = false;
                    }
                }

                if (start<=segment.StartIndex)
                {
                    // ok so the new segment starts before an existing segment
                    if (end>=segment.EndIndex)
                    {
                        // and the end of the existing segment falls after the end of the new segment....
                        valid = false;
                    }
                }

                if (start>=segment.StartIndex && end<=segment.EndIndex)
                {
                    // new segment is completely within a current segment!
                    valid = false;

                }

                if (valid == false) break;
            }

                return valid;
        }

        public TrackModel CreateSegmentAtHighlightedPosition(string name, TrackColor startColor, TrackColor endColor)
        {
            if (_model.Segments.Count >= globals.MAX_SEGMENTS)
            {
                throw new InvalidOperationException("Attempted to create too many segments.");
            }

            var start = Math.Max(_closestMousePoint - _settings.ClusterSize * 4, 0);
            var end = Math.Min(_closestMousePoint + _settings.ClusterSize * 4, _model.Waypoints.Count - 1);

            _model.Segments.Add(new TrackSegment()
            {
                Name = name,
                StartColor = startColor,
                EndColor = endColor,
                StartIndex = start,
                EndIndex = end
            });

            _transformedSegments = null;

            return _model;
        }

        public TrackModel DeleteSegment(TrackSegment segment)
        {
            _model.Segments.Remove(segment);
            _transformedSegments = null;
            return _model;
        }

        public TrackModel DeleteSplit(TrackSplit split)
        {
            _model.Splits.Remove(split);
            return _model;
        }

        public TrackModel CreateSplitAtHighlightedPosition(string text)
        {
            if (_model.Splits.Count >= globals.MAX_SPLITS)
            {
                throw new InvalidOperationException("Tried to create too many splits.");
            }

            _model.Splits.Add(new TrackSplit()
            {
                Text = text,
                Index = _closestMousePoint
            });

            return _model;
        }

        private static void Rotate(PointF[] src, PointF[] dst, float rotation, out RectangleF bounds)
        {
            float left = float.MaxValue;
            float right = float.MinValue;
            float top = float.MaxValue;
            float bottom = float.MinValue;

            for (int i = 0; i < src.Length; i++)
            {
                var p = Rotate(src[i], rotation);

                if (p.X > right)
                {
                    right = p.X;
                }

                if (p.X < left)
                {
                    left = p.X;
                }

                if (p.Y > bottom)
                {
                    bottom = p.Y;
                }

                if (p.Y < top)
                {
                    top = p.Y;
                }

                dst[i] = p;
            }

            bounds = new RectangleF(left, top, right - left, bottom - top);
        }

        private static PointF[] GetNormalisedTrackPoints(TrackModel track, out RectangleF bounds)
        {
            PointF[] points = new PointF[track.Waypoints.Count];
            float tx = 0;
            float ty = 0;

            double dist = 0;
            for (int i = 0; i < track.Waypoints.Count; i++)
            {
                var wp = track.Waypoints[i];
                var xy = LatLngToXY(wp.Latitude, wp.Longitude).ToPointF();

                tx += xy.X;
                ty += xy.Y;

                points[i] = xy;
            }

            List<PointF> cleanedPoints = new List<PointF>();
            cleanedPoints.Add(points[0]);
            for (int i = 1; i < points.Length; i++)
            {
                if (Math.Sqrt(Math.Pow(points[i].X - cleanedPoints.Last().X, 2) + Math.Pow(points[i].Y - cleanedPoints.Last().Y, 2)) >= dist)
                {
                    cleanedPoints.Add(points[i]);
                }
            }

            points = cleanedPoints.ToArray();

            tx /= track.Waypoints.Count;
            ty /= track.Waypoints.Count;

            var avg = new PointF(tx, ty);

            float left = float.MaxValue;
            float right = float.MinValue;
            float top = float.MaxValue;
            float bottom = float.MinValue;
            for (int i = 0; i < points.Length; i++)
            {
                var p = new PointF(points[i].X - avg.X, points[i].Y - avg.Y);
                points[i] = p;

                if (p.X < left)
                {
                    left = p.X;
                }
                if (p.X > right)
                {
                    right = p.X;
                }
                if (p.Y > bottom)
                {
                    bottom = p.Y;
                }
                if (p.Y < top)
                {
                    top = p.Y;
                }
            }

            float w = right - left;
            float h = bottom - top;
            float longEdge = w > h ? w : h;

            float scaleFactor = 1f / longEdge;

            for (int i = 0; i < points.Length; i++)
            {
                points[i] = new PointF(points[i].X * scaleFactor, points[i].Y * scaleFactor);
            }

            bounds = new RectangleF(left, top, w, h);

            return points;
        }

        public static void SaveFile(string filepath, TrackModel model)
        {
            Dictionary<int, List<string>> controlWords = new Dictionary<int, List<string>>();
            foreach (var split in model.Splits)
            {
                List<string> l = null;
                if (!controlWords.TryGetValue(split.Index, out l))
                {
                    l = new List<string>();
                    controlWords[split.Index] = l;
                }

                l.Add("SPLIT:" + split.Text);
            }

            foreach (var segment in model.Segments)
            {
                List<string> l = null;
                if (!controlWords.TryGetValue(segment.StartIndex, out l))
                {
                    l = new List<string>();
                    controlWords[segment.StartIndex] = l;
                }

                l.Add(string.Format(SEGMENT_FORMAT_STRING, "START", segment.StartColor.R, segment.StartColor.G, segment.StartColor.B) + ":" + segment.Name);

                if (!controlWords.TryGetValue(segment.EndIndex, out l))
                {
                    l = new List<string>();
                    controlWords[segment.EndIndex] = l;
                }

                l.Add(string.Format(SEGMENT_FORMAT_STRING, "STOP", segment.EndColor.R, segment.EndColor.G, segment.EndColor.B));
            }

            List<string> csv = new List<string>(model.Waypoints.Count);
            // add these in during save
            csv.Add("#latitude,longitude,control");
            csv.Add("samplerate," + model.SampleRate);
            for (int i = 0; i < model.Waypoints.Count; i++)
            {
                var wp = model.Waypoints[i];
                var controlWordsJoined = "0";

                if (controlWords.ContainsKey(i))
                {
                    controlWordsJoined = string.Join("|", controlWords[i]);
                }

                csv.Add(string.Join(",", wp.Latitude, wp.Longitude, controlWordsJoined));
            }

            File.WriteAllLines(filepath, csv);
        }

        public static TrackModel LoadFile(string filepath)
        {
            var lines = File.ReadAllLines(filepath);
            var model = new TrackModel();
            TrackSegment openSegment = null;
            foreach (var line in lines.Skip(1))
            {
                if (line.ToUpper().Contains("SAMPLERATE"))
                {
                    try
                    {
                        var sampleRateSplit = line.Split(',');
                        model.SampleRate = int.Parse(sampleRateSplit[1]);
                    }
                    catch
                    {
                    }
                    continue;
                }

                if (line.StartsWith("#"))
                {
                    continue;
                }

                if (line.ToUpper().Contains("STRAIGHT_TRACK"))
                {
                    model.IsStraightTrack = true;
                    continue;
                }

                var split = line.Split(',');
                var waypoint = new Waypoint()
                {
                    Latitude = double.Parse(split[0]),
                    Longitude = double.Parse(split[1]),
                };

                if (split.Length >= 3)
                {
                    var controlWords = split[2].Split('|');
                    foreach (var controlWord in controlWords)
                    {
                        if (controlWord == "0")
                        {
                            continue;
                        }

                        if (controlWord.ToUpper().StartsWith("SPLIT"))
                        {
                            var splitSplit = controlWord.Split(':');
                            var s = new TrackSplit()
                            {
                                Index = model.Waypoints.Count,
                                Text = splitSplit[1]
                            };
                            model.Splits.Add(s);
                        }

                        if (controlWord.ToUpper() == "FLAG_POSITION")
                        {
                            model.FlagPosition = model.Waypoints.Count;
                        }

                        if (controlWord.ToUpper().StartsWith("START"))
                        {
                            var segmentSplit = controlWord.Split(':');
                            var colorMatch = Regex.Match(segmentSplit[0], SEGMENT_COLOR_REGEX);
                            var startColor = TrackColor.LightBlue;
                            if (colorMatch.Success)
                            {
                                startColor = new TrackColor()
                                {
                                    R = byte.Parse(colorMatch.Groups[1].Value),
                                    G = byte.Parse(colorMatch.Groups[2].Value),
                                    B = byte.Parse(colorMatch.Groups[3].Value)
                                };
                            }

                            var dataMatch = Regex.Match(segmentSplit[0], SEGMENT_DATA_REGEX);
                            UInt16 dataBits = 0;
                            if (dataMatch.Success)
                            {
                                dataBits = Convert.ToUInt16(dataMatch.Groups[1].Value, 16);
                            }

                            openSegment = new TrackSegment()
                            {
                                Name = segmentSplit[1],
                                StartIndex = model.Waypoints.Count,
                                StartColor = startColor,
                                EndColor = TrackColor.LightBlue,
                                DataBits = dataBits
                            };
                        }

                        if (controlWord.ToUpper().StartsWith("STOP") && openSegment != null)
                        {
                            var endColor = TrackColor.LightBlue;
                            var colorMatch = Regex.Match(controlWord, SEGMENT_COLOR_REGEX);
                            if (colorMatch.Success)
                            {
                                endColor = new TrackColor()
                                {
                                    R = byte.Parse(colorMatch.Groups[1].Value),
                                    G = byte.Parse(colorMatch.Groups[2].Value),
                                    B = byte.Parse(colorMatch.Groups[3].Value)
                                };
                            }

                            openSegment.EndIndex = model.Waypoints.Count;
                            openSegment.EndColor = endColor;
                            model.Segments.Add(openSegment);
                            openSegment = null;
                        }
                    }
                }

                model.Waypoints.Add(waypoint);
            }

            return model;
        }
    }
}