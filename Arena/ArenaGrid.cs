using DongUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arena
{
    internal class ArenaGrid
    {
        private readonly ArenaGridCell[,] grid;

        private readonly int width;
        private readonly int height;

        private readonly double xdiv;
        private readonly double ydiv;

        public ArenaGrid(ArenaEngine arena, int xDivs, int yDivs)
        {
            width = xDivs;
            height = yDivs;

            grid = new ArenaGridCell[xDivs, yDivs];

            xdiv = arena.Width / xDivs;
            ydiv = arena.Height / yDivs;

            for (int ix = 0; ix < xDivs; ++ix)
                for (int iy = 0; iy < yDivs; ++iy)
                {
                    grid[ix, iy] = new ArenaGridCell();
                }
        }

        private IEnumerable<ArenaGridCell> LocateCells(ArenaObject obj)
        {
            return LocateCells(obj.Size);
        }

        private IEnumerable<ArenaGridCell> LocateCells(Rectangle rect)
        {
            var upperLeft = GetGridCell(rect.MinXMinY);
            var lowerRight = GetGridCell(rect.MaxXMaxY);

            int minX = upperLeft.X;
            int minY = upperLeft.Y;
            int maxX = lowerRight.X;
            int maxY = lowerRight.Y;

            for (int ix = minX; ix <= maxX; ++ix)
                for (int iy = minY; iy <= maxY; ++iy)
                {
                    yield return grid[ix, iy];
                }
        }

        private ArenaGridCell LocateCell(Vector2D position)
        {
            var coord = GetGridCell(position);

            return grid[coord.X, coord.Y];
        }

        private Coordinate2D GetGridCell(Vector2D position)
        {
            int xCoord = (int)(position.X / xdiv);
            int yCoord = (int)(position.Y / ydiv);
            xCoord = Math.Clamp(xCoord, 0, width - 1);
            yCoord = Math.Clamp(yCoord, 0, height - 1);
            return new Coordinate2D(xCoord, yCoord);
        }

        public void AddObject(ArenaObject obj)
        {
            foreach (var cell in LocateCells(obj))
            {
                cell.AddObject(obj);
            }
        }

        public void RemoveObject(ArenaObject obj)
        {
            foreach (var cell in LocateCells(obj))
            {
                cell.RemoveObject(obj);
            }
        }

        /// <summary>
        /// Moves an object - do this instead of updating Position directly!
        /// </summary>
        public void MoveObject(ArenaObject obj, Vector2D newPosition)
        {
            var oldCells = LocateCells(obj);
            var newRect = new Rectangle(newPosition, obj.Size.Width, obj.Size.Height);
            var newCells = LocateCells(newRect);

            // This seems easier
            foreach (var cell in oldCells)
            {
                cell.RemoveObject(obj);
            }
            foreach (var cell in newCells)
            {
                cell.AddObject(obj);
            }

            obj.Position = newPosition;
        }

        public IEnumerable<T> GetNearby<T>(Vector2D position, double radius) where T : ArenaObject
        {
            var lookRectangle = new Rectangle(position, radius * 2, radius * 2);

            var alreadyDone = new HashSet<ArenaObject>();

            var cells = LocateCells(lookRectangle);
            foreach (var cell in cells)
                foreach (var obj in cell.Objects)
                {
                    if (obj is T && !alreadyDone.Contains(obj))
                    {
                        alreadyDone.Add(obj);
                        yield return ((T)obj);
                    }
                }
        }
    }
}
