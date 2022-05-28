#define PARALLEL
#define NOEXCEPTIONS
#define GRID

using Arena.GraphicTurns;
using DongUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisualizerBaseClasses;

namespace Arena
{
    abstract public class ArenaEngine : IEngine<IArenaDisplay, GraphicTurn>
    {
        static public Random Random => ThreadSafeRandom.Random(); 

        // This is in game coordinates
        public double Width { get; }
        public double Height { get; }

        public Registry Registry { get; } = new Registry();

        public List<BackgroundObject> BackgroundObjects { get; } = new List<BackgroundObject>();
        public List<ArenaObject> StationaryObjects { get; } = new List<ArenaObject>();
        public List<MovingObject> UpdatingObjects { get; } = new List<MovingObject>();
        public IEnumerable<ArenaObject> AllInteractingObjects
        {
            get
            {
                foreach (var obj in StationaryObjects)
                {
                    yield return obj;
                }
                foreach (var obj in UpdatingObjects)
                {
                    yield return obj;
                }
            }
        }
        public IEnumerable<ArenaObject> AllObjects
        {
            get
            {
                foreach (var obj in AllInteractingObjects)
                {
                    yield return obj;
                }
                foreach (var obj in BackgroundObjects)
                {
                    yield return obj;
                }
            }
        }

        public IEnumerable<ArenaObject> GetObjects(string name)
        {
            foreach (var obj in AllObjects)
            {
                if (obj.Name == name)
                    yield return obj;
            }
        }

        public int CountObjects(string name)
        {
            return GetObjects(name).Count();
        }

        public IEnumerable<ArenaObject> GetNearbyObjects(Vector2D position, double radius)
        {
            return GetNearbyObjects<ArenaObject>(position, radius);
        }

        public IEnumerable<T> GetNearbyObjects<T>(Vector2D position, double radius) where T : ArenaObject
        {
#if GRID
            return grid.GetNearby<T>(position, radius);
#else
            return GetObjectsOfType<T>();
#endif
        }
#if GRID
        private ArenaGrid grid;
#endif
        private List<ArenaObject> toBeAdded = new List<ArenaObject>();
        private List<ArenaObject> toBeRemoved = new List<ArenaObject>();

        public double Time { get; set; } = 0;

        private bool endGame = false;
        public bool IsPaused { get; set; } = false;

        public GraphicTurnSet TurnSet { get; private set; } = new GraphicTurnSet();

        public TurnStatistics Statistics { get; } = new TurnStatistics();

        public bool IsOccupied(Vector2D location, ArenaObject whoWantsToKnow = null)
        {
            double radius = whoWantsToKnow == null ? double.Epsilon 
                : Math.Max(whoWantsToKnow.Size.Width, whoWantsToKnow.Size.Height);
            foreach (var obj in GetNearbyObjects(location, radius))
            {
                if (obj != whoWantsToKnow && obj.Occupies(location, whoWantsToKnow))
                    return true;
            }
            return false;
        }

        public bool IsOccupied(Rectangle area, ArenaObject whoWantsToKnow = null)
        {
            double radius = Math.Max(area.Width, area.Height);
            foreach (var obj in GetNearbyObjects(area.Center, radius))
            {
                if (whoWantsToKnow != null && whoWantsToKnow.Code == obj.Code)
                    continue;
                if (!obj.IsPassable(whoWantsToKnow) && obj.Size.Overlaps(area))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsValidLocation(Vector2D point)
        {
            return !IsOccupied(point) && TestPoint(point);
        }

        public bool IsValidLocation(Rectangle area)
        {
            return !IsOccupied(area) && TestRectangle(area);
        }

        public ArenaEngine(double width, double height, string backgroundFilename, int xDivs = 10, int yDivs = 10)
        {
            Width = width;
            Height = height;
#if GRID
            grid = new ArenaGrid(this, xDivs, yDivs);
#endif

            BackgroundGraphicsCode = Registry.AddEntry(new GraphicInfo(backgroundFilename, width, height));
            AddObject(new BackgroundObject(BackgroundGraphicsCode), new Vector2D(Width / 2, Height / 2));

            Initialize();
        }

        public int BackgroundGraphicsCode { get; }

        abstract public void Initialize();

        public CommandSet<IArenaDisplay> Initialization()
        {
            var graphics = new GraphicTurnSet();

            foreach (var org in AllObjects)
            {
                graphics.AddCommand(new AddObject(org));
            }

            return graphics;
        }

        public bool Continue => !endGame;

        public void AddObjectRandom<T>() where T : ArenaObject, new()
        {
            T newObj = new T();
            AddObjectRandom(newObj);
        }

        public void AddObjectRandom<T>(int num) where T : ArenaObject, new()
        {
            for (int i = 0; i < num; ++i)
            {
                AddObjectRandom<T>();
            }
        }

        public void AddObjectRandom(ArenaObject obj)
        {
            double x = Random.NextDouble(0, Width);
            double y = Random.NextDouble(0, Height);

            AddObject(obj, new Vector2D(x, y));
        }

        public void AddObjectDelay(ArenaObject obj)
        {
            lock (toBeAdded)
            {
                toBeAdded.Add(obj);
            }
        }

        public void RemoveObjectDelay(ArenaObject obj)
        {
            lock (toBeRemoved)
            {
                toBeRemoved.Add(obj);
            }
        }

        public void AddObject(ArenaObject obj, Vector2D location)
        {
            if (obj == null)
                return;

            obj.Position = location;
            obj.Arena = this;
            if (obj is MovingObject @object)
            {
                UpdatingObjects.Add(@object);
            }
            else
            {
                if (obj is BackgroundObject)
                {
                    BackgroundObjects.Add((BackgroundObject)obj);
                }
                else
                {
                    StationaryObjects.Add(obj);
                }
            }
#if GRID
            grid.AddObject(obj);
#endif
            TurnSet.AddCommand(new AddObject(obj));
        }

        public IEnumerable<T> GetObjectsOfType<T>() where T : ArenaObject
        {
            foreach (var obj in AllObjects)
                if (obj is T)
                    yield return (T)obj;
        }

        public void RemoveObject(ArenaObject obj)
        {
            if (obj == null) return;

            if (obj is MovingObject)
                UpdatingObjects.Remove((MovingObject)obj);
            else if (obj is BackgroundObject)
                BackgroundObjects.Remove((BackgroundObject)obj);
            else
                StationaryObjects.Remove(obj);
#if GRID
            grid.RemoveObject(obj);
#endif
            TurnSet.AddCommand(new RemoveObject(obj));
        }

        public bool MoveObject(ArenaObject obj, Vector2D target)
        {
            var newRect = new Rectangle(target, obj.Size.Width, obj.Size.Height);
            var rect = !TestRectangle(newRect);
            var occupied = IsOccupied(newRect, obj);
            if (obj == null || rect || occupied)
            {
                return false;
            }
            else
            {
#if GRID
                grid.MoveObject(obj, target);
#else
                obj.Position = target;
#endif

                TurnSet.AddCommand(new MoveObject(obj, target));
                return true;
            }
        }

        public void EndGame()
        {
            endGame = true;
        }

        public CommandSet<IArenaDisplay> Tick(double newTime)
        {
            Time = newTime;
            TurnSet = new GraphicTurnSet();
            Statistics.ClearStatistics();

            BeginningOfTurn();

            UpdatingObjects.Shuffle(Random);

            DoTurn();

            EndOfTurn();

            CleanUp();

            if (Done())
            {
                endGame = true;
            }

            return TurnSet;
        }

        private void BeginningOfTurn()
        {
#if PARALLEL
            Parallel.ForEach(UpdatingObjects, (MovingObject obj) => DoWithExceptions(() => obj.BeginningOfTurn()));
#else
            foreach (var obj in UpdatingObjects)
            {
                DoWithExceptions(() => obj.BeginningOfTurn());
            }
#endif
            UserDefinedBeginningOfTurn();
        }

        protected virtual void UserDefinedBeginningOfTurn() { }
        protected virtual void UserDefinedEndOfTurn() { }

        private void DoTurn()
        {
#if PARALLEL
            Parallel.ForEach(UpdatingObjects, (MovingObject obj) =>
            {
                DoWithExceptions(() => obj.ChooseAction());
            });
#else
            foreach (var obj in UpdatingObjects)
            {
                //Console.WriteLine("updating object " + obj.Name + " " + obj.Position);
                DoWithExceptions(() => obj.ChooseAction());
            }
#endif
            foreach (var obj in UpdatingObjects)
            {
                DoWithExceptions(() => obj.ExecuteAction());
            }
        }

        private void EndOfTurn()
        {

#if PARALLEL
            Parallel.ForEach(UpdatingObjects, (MovingObject obj) => DoWithExceptions(() => obj.EndOfTurn()));
#else
            foreach (var obj in UpdatingObjects)
            {
                DoWithExceptions(() => obj.EndOfTurn());
            }
#endif
            UserDefinedEndOfTurn();
        }

        private void CleanUp()
        {
            foreach (var obj in toBeRemoved)
            {
                if (obj != null)
                    DoWithExceptions(() => RemoveObject(obj));
            }
            toBeRemoved.Clear();

            foreach (var obj in toBeAdded)
            {
                if (obj != null)
                    DoWithExceptions(() => AddObject(obj, obj.Position));
            }
            toBeAdded.Clear();
        }

        private delegate void GenericFunction();
        private void DoWithExceptions(GenericFunction func)
        {
#if NOEXCEPTIONS
            try
            {
#endif
            func();
#if NOEXCEPTIONS
            }
            catch (Exception)
            { }
#endif
        }

        virtual protected bool Done()
        {
            return false;
        }

        public bool TestPoint(Vector2D coord)
        {
            return coord.X >= 0 && coord.X <= Width && coord.Y >= 0 && coord.Y <= Height;
        }

        public bool TestRectangle(Rectangle rect)
        {
            return TestPoint(rect.MaxXMaxY) && TestPoint(rect.MaxXMinY) && TestPoint(rect.MinXMaxY) && TestPoint(rect.MinXMinY);
        }
    }
}
