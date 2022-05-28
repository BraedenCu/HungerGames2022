using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Diagnostics;
using Arena;
using DongUtility;
using System.Collections.Generic;
using System.IO;
using VisualizerBaseClasses;
using System.Reflection;
using System.Windows;

namespace ArenaVisualizer
{
    public class ArenaCoreInterface : HwndHost, IArenaDisplay
    {
        internal const int
                    WsChild = 0x40000000,
                    WsVisible = 0x10000000,
                    LbsNotify = 0x00000001,
                    HostId = 0x00000002,
                    ListboxId = 0x00000001,
                    WsVscroll = 0x00200000,
                    WsBorder = 0x00800000;

        public int HostHeight { get; set; }
        public int HostWidth { get; set; }
        private IntPtr hwndHost;

        public double ArenaHeight { get; set; }
        public double ArenaWidth { get; set; }

        private int currentMaxLayer = -1;

        public ArenaCoreInterface()
        { }

        public ArenaCoreInterface(double windowWidth, double windowHeight, double arenaWidth, double arenaHeight)
        {
            SetWindowDimensions(windowWidth, windowHeight, arenaWidth, arenaHeight);
        }

        public void AfterStartup(ArenaEngine arena)
        {
            var allGI = arena.Registry.GetAllGraphicInfo();
            for (int i = 0; i < allGI.Count; ++i)
            {
                AddToRegistry(arena.Registry, allGI[i], i);
            }

            //initialSet.ProcessAll(this);
            RedrawDX();
        }

        private void AddToRegistry(Registry registry, GraphicInfo gi, int index)
        {
            var scaledSizes = ConvertTo1Max(gi.XSize, gi.YSize);
            string fullPath = registry.ImageDirectory + gi.Filename;

           if (!File.Exists(fullPath))
             throw new FileNotFoundException("File " + gi.Filename + " not found!");

            AddToRegistryDX(fullPath, scaledSizes.X, scaledSizes.Y, index);
            addedGraphicsCodes.Add(index);
        }

        private readonly List<int> addedGraphicsCodes = new List<int>();

        private Vector2D ConvertTo1Max(double xSize, double ySize)
        {
            return new Vector2D(xSize / ArenaWidth, ySize / ArenaHeight);
        }

        private Vector2D ConvertTo1Max(Vector2D original)
        {
            return ConvertTo1Max(original.X, original.Y);
        }

        public IntPtr HwndListBox { get; private set; }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            HwndListBox = IntPtr.Zero;
            hwndHost = IntPtr.Zero;

            string windowName = "internalWindow";
            RegisterWindow(windowName);

            /*
             * On 4k screens you have to have UI scaling in order for things to not look super tiny.
             * Unfortunately it seems that the internal win32 apps don't get automatically scaled while the wpf does so we have to
             * manually scale here.
             */

            var source = PresentationSource.FromVisual(this);
            var xScale = source.CompositionTarget.TransformToDevice.M11;
            var yScale = source.CompositionTarget.TransformToDevice.M22;

            hwndHost = CreateWindowEx(0, "static", "",
                WsChild | WsVisible,
                0, 0,
                (int)(HostHeight), (int)(HostWidth),
                hwndParent.Handle,
                (IntPtr)HostId,
                IntPtr.Zero,
                0);

            HwndListBox = MakeWindow(windowName,
                WsChild | WsVisible | LbsNotify | WsBorder,
                (int)(HostHeight * xScale),
                (int)(HostWidth * yScale),
                hwndHost, xScale, yScale);

            return new HandleRef(this, hwndHost);
        }

        protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            handled = false;
            return IntPtr.Zero;
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            DestroyWindow(hwnd.Handle);
        }

        private const string dllName = @"..\..\..\..\ArenaVisualizer\ArenaCore.dll";

        [DllImport(dllName, EntryPoint = "RegisterWindow", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        internal static extern bool RegisterWindow(string ClassName);

        [DllImport(dllName, EntryPoint = "CheckRegistration", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        internal static extern bool CheckRegistration(string ClassName);

        [DllImport(dllName, EntryPoint = "MakeWindow", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        internal static extern IntPtr MakeWindow(string ClassName, int style, int height, int width, IntPtr parent,
            double widthFactor, double heightFactor);


        [DllImport("user32.dll", EntryPoint = "CreateWindowEx", CharSet = CharSet.Unicode)]
        internal static extern IntPtr CreateWindowEx(int dwExStyle,
            string lpszClassName,
            string lpszWindowName,
            int style,
            int x, int y,
            int width, int height,
            IntPtr hwndParent,
            IntPtr hMenu,
            IntPtr hInst,
            [MarshalAs(UnmanagedType.IUnknown)] object pvParam);

        [DllImport("user32.dll", EntryPoint = "DestroyWindow", CharSet = CharSet.Unicode)]
        private static extern bool DestroyWindow(IntPtr hwnd);

        [DllImport(dllName, EntryPoint = "AddToRegistry", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern void AddToRegistryDX([MarshalAs(UnmanagedType.LPWStr), In] string filename, double width,
            double height, int index);

        [DllImport(dllName, EntryPoint = "AddVisualLayer",
            CallingConvention = CallingConvention.Cdecl)]
        private static extern void AddVisualLayerDX();

        [DllImport(dllName, EntryPoint = "AddObject",
            CallingConvention = CallingConvention.Cdecl)]
        private static extern void AddObjectDX(int layer, int graphicIndex, int index,
            double x, double y);

        [DllImport(dllName, EntryPoint = "MoveObject",
            CallingConvention = CallingConvention.Cdecl)]
        private static extern void MoveObjectDX(int layer, int index,
            double x, double y);

        [DllImport(dllName, EntryPoint = "RemoveObject",
            CallingConvention = CallingConvention.Cdecl)]
        private static extern void RemoveObjectDX(int layer, int index);

        [DllImport(dllName, EntryPoint = "ChangeGraphic",
            CallingConvention = CallingConvention.Cdecl)]
        private static extern void ChangeGraphicObjectDX(int layer, int index,
            int newGraphicInstance);

        [DllImport(dllName, EntryPoint = "ResizeDisplay",
            CallingConvention = CallingConvention.Cdecl)]
        private static extern void ResizeDisplayDX(int newX, int newY);

        [DllImport(dllName, EntryPoint = "Redraw", CallingConvention = CallingConvention.Cdecl)]
        private static extern void RedrawDX();

        [DllImport(dllName, EntryPoint = "Zoom", CallingConvention = CallingConvention.Cdecl)]
        private static extern void ZoomDX(double xScale, double yScale, double xCenter, double yCenter);

        public void AddObject(Registry registry, int layer, int graphicCode, int objCode, Vector2D coord)
        {
            while (currentMaxLayer < layer)
            {
                AddVisualLayerDX();
                ++currentMaxLayer;
            }

            coord = ConvertTo1Max(coord);

            if (!addedGraphicsCodes.Contains(graphicCode))
            {
                AddToRegistry(registry, registry.GetInfo(graphicCode), graphicCode);
            }

            AddObjectDX(layer, graphicCode, objCode, coord.X, coord.Y);
        }

        public void MoveObject(int layer, int objCode, Vector2D newCoord)
        {
            newCoord = ConvertTo1Max(newCoord);
            MoveObjectDX(layer, objCode, newCoord.X, newCoord.Y);
        }

        public void RemoveObject(int layer, int objCode)
        {
            RemoveObjectDX(layer, objCode);
        }

        public void ChangeObjectGraphic(int layer, int objCode, int graphicCode)
        {
            ChangeGraphicObjectDX(layer, objCode, graphicCode);
        }

        public void ScaleDisplay(int newWidth, int newHeight)
        {
            ResizeDisplayDX(newWidth, newHeight);
        }

        public void Redraw()
        {
            RedrawDX();
        }

        public void Zoom(double xScale, double yScale, double xCenter, double yCenter)
        {
            ZoomDX(xScale, yScale, xCenter, yCenter);
        }

        public void SetWindowDimensions(double windowWidth, double windowHeight, double arenaWidth, double arenaHeight)
        {
            HostHeight = (int)windowHeight;
            HostWidth = (int)windowWidth;
            ArenaWidth = arenaWidth;
            ArenaHeight = arenaHeight;
        }
    }
}
