using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RaceVoice
{
    public class TrackModelParser
    {
        private const string SEGMENT_COLOR_REGEX = @"\((\d+) (\d+) (\d+)\)";
        private const string SEGMENT_DATA_REGEX = @"\<([0-9a-fA-F]{4})\>";
        private const string SEGMENT_FORMAT_STRING = "{0}({1} {2} {3})";

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
    }
}
