﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Alife.AlifeMaps;

namespace Alife.Agents
{
    /**
     * <summary>
     * Tunneling agent that "explodes", creating rooms.
     * <author>1upD</author>
     * </summary>
     */
    [Serializable]
    public class Roomer : BaseAgent
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public int X;
        public int Y;
        public int Z;
        private bool _completed;
        private int _x1;
        private int _x2;
        private int _y1;
        private int _y2;
        public int Height;
        public string Style;
        public int MaxWidth;
        public bool MustDeploy;

        // Default constructor required by serializer
        public Roomer()
        {
            this._completed = false;
            this.X = 0;
            this.Y = 0;
            this.Z = 0;

            Random random = new Random();

            this.MaxWidth = 8;

            this._x1 = random.Next(1, this.MaxWidth);
            this._x2 = random.Next(-1 * this.MaxWidth, -1);
            this._y1 = random.Next(1, this.MaxWidth);
            this._y2 = random.Next(-1 * this.MaxWidth, -1);

            this.Style = "default";
            this.Height = 2;

            this.MustDeploy = true;
        }

        /**
         * <summary>
         * Constructor with a lot of parameters
         * <author>1upD</author>
         * </summary>
         */
        public Roomer(int x = 0, int y = 0, int z = 0, string style = "", int height = 2, int maxWidth = 8, bool mustDeploy = true)
        {
            this._completed = false;
            this.X = x;
            this.Y = y;
            this.Z = z;

            Random random = new Random();

            this.MaxWidth = maxWidth;

            this._x1 = random.Next(1, this.MaxWidth);
            this._x2 = random.Next(-1 * this.MaxWidth, -1);
            this._y1 = random.Next(1, this.MaxWidth);
            this._y2 = random.Next(-1 * this.MaxWidth, -1);

            this.Style = style;
            this.Height = height;

            this.MustDeploy = mustDeploy;

            log.Debug(string.Format("Roomer spawned at {0}, {1}, {2}.", this.X, this.Y, this.Z));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        /**
         * <summary>
         * Returns one turn remaining if the roomer hasn't stepped yet, otherwise returns 0.
         * <author>1upD</author>
         * </summary>
         */
        public override int GetTurnsLeft()
        {
            return this._completed ? 0 : 1;
        }

        /**
         * <summary>
         * Step function for roomer. Checks if less than half of the spaces are occupied, and if not, creates a room.
         * <author>1upD</author>
         * </summary>
         */
        public override void Step(AlifeMap map)
        {
            try
            {
                // Mark to discard
                this._completed = true;

                // Local variables
                int spacesOccupied = 0;
                int total = 0;
                List<Tuple<int, int, int>> locationsToMark = new List<Tuple<int, int, int>>();

                for (int z = this.Z; z < this.Z + this.Height; z++)
                {
                    for (int x = this.X + this._x2; x < this.X + this._x1; x++)
                    {
                        for (int y = this.Y + this._y2; y < this.Y + this._y1; y++)
                        {
                            locationsToMark.Add(new Tuple<int, int, int>(x, y, z));
                            bool isSpaceOccupied = map.GetLocation(x, y, z) != null && map.GetLocation(x, y, z) != "";
                            spacesOccupied += isSpaceOccupied ? 1 : 0;
                            total++;
                        }
                    }
                }

                if (spacesOccupied < total / 2 && MustDeploy)
                {
                    log.Debug(string.Format("Roomer deployed: {0} spaces occupied, {1} total.", spacesOccupied, total));
                    log.Debug(string.Format("Roomer deployed at x: {0} y: {1} z: {2}", this.X, this.Y, this.Z));
                    foreach (var tuple in locationsToMark)
                    {
                        map.MarkLocation(this.Style, tuple.Item1, tuple.Item2, tuple.Item3);
                    }
                } else
                {
                    log.Debug(string.Format("Roomer did not deploy. {0} spaces occupied, {1} total.", spacesOccupied, total));
                }

            }
            catch (Exception e)
            {
                log.Error("Error in Roomer Step function: ", e);
            }

        }


    }
}
