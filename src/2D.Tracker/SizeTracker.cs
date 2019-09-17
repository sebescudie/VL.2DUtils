using System;
using System.Collections.Generic;
using Xenko.Core.Mathematics;
using System.Linq;

namespace _2D.Tracker
{
    public class SizeTracker
    {
        private List<Tracker> trackerList = new List<Tracker>();
        private List<RectangleF> rects;
        private int trackCount = 0;

        private int initialTimer { get; set; }
        private int maxLifetime { get; set; }
        private float smoothing { get; set; }
        private float searchRadius { get; set; }
        private bool searchRadiusInPercent { get; set; }
        private float fitnessThreshold { get; set; }

        // Constructor
        public SizeTracker(int initialTimer = 0, int maxLifetime = 200, float smoothing = .9f, float searchRadius = 1f, bool searchRadiusInPercent = true, float fitnessThreshold = .3f)
        {
            this.initialTimer = initialTimer;
            this.maxLifetime = maxLifetime;
            this.smoothing = smoothing;
            this.searchRadius = searchRadius;
            this.searchRadiusInPercent = searchRadiusInPercent;
            this.fitnessThreshold = fitnessThreshold;
        }

        // Public

        public List<Tracker> Track(List<RectangleF> input)
        {
            rects = new List<RectangleF>();

            // Add all input rectangles
            foreach (var rect in input)
            {
                rects.Add(rect);
            }

            // Calculate tracking stuff
            PerformTracking(rects, this.initialTimer, this.maxLifetime);

            foreach (Tracker t in trackerList)
            {
                t.Run(this.smoothing, this.searchRadius, this.searchRadiusInPercent);
            }

            return trackerList;
        }

        // Private

        private void PerformTracking(List<RectangleF> rects, int timer, int maxLifetime)
        {
            // Scenario 1 - Trackerlist is empty
            if (trackerList.Count == 0)
            {
                Console.WriteLine("Trackerlist is empty, adding all input rects in");
                for (int i = 0; i < rects.Count; i++)
                {
                    trackerList.Add(new Tracker(rects[i], trackCount, this.initialTimer, this.maxLifetime));
                    trackCount++;
                }
            }

            // Scenario 2 - We have fewer tracker objects than Rectangles from the input
            else if (trackerList.Count < rects.Count)
            {
                Console.WriteLine(String.Format("Fewer Tracker objects than input Rectangles: {0} Trackers, {1} Rectangles.", trackerList.Count, rects.Count));
                bool[] used = new bool[rects.Count];

                // Match existing Tracker objects with a Rectangle
                foreach (Tracker tracker in trackerList.ToList())
                {
                    double fitnessRecord = 50000;
                    int index = -1;

                    for (int i = 0; i < rects.Count; i++)
                    {
                        double fitness = CheckSimilarity(rects[i], tracker.rectangle);
                        double distance = CalculateDistance(rects[i].Center, tracker.rectangle.Center);
                        if (fitness < fitnessRecord && !used[i] && distance < tracker.searchRadius && fitness < fitnessThreshold)
                        {
                            fitnessRecord = fitness;
                            index = i;
                        }

                        // Update tracker object location
                        if (index != -1)
                        {
                            used[index] = true;
                            tracker.Update(rects[index]);
                        }
                    }

                    // Add any unused rect
                    for (int i = 0; i < rects.Count; i++)
                    {
                        if (!used[i])
                        {
                            trackerList.Add(new Tracker(rects[i], trackCount, timer, maxLifetime));
                            trackCount++;
                        }
                    }
                }
            }

            // Scenario 3 - We have more tracker objects than rectangles from the plugin
            else
            {
                // All tracker objects start as available
                foreach (Tracker tracker in trackerList)
                {
                    tracker.isAvailable = true;
                }

                // If there is no match, we have to add new Trackers in this scenario
                bool[] used = new bool[rects.Count];

                // Match rectangles with a Tracker object
                for (int i = 0; i < rects.Count; i++)
                {
                    // Find Tracker object closest to Rect[i] & set available to false
                    double fitnessRecord = 50000;
                    int index = -1;
                    for (int j = 0; j < trackerList.Count; j++)
                    {
                        Tracker f = trackerList[j];
                        double fitness = CheckSimilarity(rects[i], f.rectangle);
                        double distance = CalculateDistance(rects[i].Center, f.rectangle.Center);
                        if (fitness < fitnessRecord && f.isAvailable && distance < f.searchRadius && fitness < fitnessThreshold)
                        {
                            fitnessRecord = fitness;
                            index = j;
                        }
                    }

                    // Update tracker object location
                    if (index == -1)
                    {
                        used[i] = false;
                    }
                    else
                    {
                        Tracker t = trackerList[index];
                        t.isAvailable = false;
                        t.Update(rects[i]);
                        used[i] = true;
                    }
                }

                // Add any unused rect (if no match was found, as in sc. 2)
                for (int i = 0; i < rects.Count; i++)
                {
                    if (!used[i])
                    {
                        trackerList.Add(new Tracker(rects[i], trackCount, timer, maxLifetime));
                        trackCount++;
                    }
                }

                // Start to kill any left over tracker
                foreach (Tracker tracker in trackerList)
                {
                    if (tracker.isAvailable)
                    {
                        tracker.CountDown();
                        if (tracker.isDead())
                        {
                            tracker.isDelete = true;
                        }
                    }
                }

                // Delete any tracker that should be deleted
                for (int i = trackerList.Count - 1; i >= 0; i--)
                {
                    Tracker t = trackerList[i];
                    if (t.isDelete)
                    {
                        trackerList.RemoveAt(i);
                    }
                }
            }
        }

        private static double CalculateDistance(Vector2 location1, Vector2 location2)
        {
            return (location1 - location2).Length();
        }

        private double CheckSimilarity(RectangleF rect1, RectangleF rect2)
        {
            return (new Vector2(rect1.Width, rect1.Height) - new Vector2(rect2.Width, rect2.Height)).Length();
        }
    }
}