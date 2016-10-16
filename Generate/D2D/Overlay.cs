﻿using SharpDX;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using Texture2D = SharpDX.Direct3D11.Texture2D;
using System;

namespace Generate.D2D
{
    using SharpDX.Direct2D1;
    class Overlay : IDisposable
    {
        private TextFormat TextFormat;
        public DeviceContext Context2D;
        private SolidColorBrush Brush;
        private static BitmapProperties1 Properties = new BitmapProperties1(new PixelFormat(Format.R8G8B8A8_UNorm, AlphaMode.Premultiplied), 96, 96, BitmapOptions.Target | BitmapOptions.CannotDraw);

        public Overlay(SharpDX.Direct3D11.Device Device3D, Texture2D BackBuffer)
        {
            using (var FontFactory = new SharpDX.DirectWrite.Factory())
            {
                TextFormat = new TextFormat(FontFactory, "Segoe UI", 16.0f);
            }

            using (var Device2D = new Device(Device3D.QueryInterface<SharpDX.DXGI.Device>()))
            {
                Context2D = new DeviceContext(Device2D, DeviceContextOptions.EnableMultithreadedOptimizations);
            }

            Context2D.PrimitiveBlend = PrimitiveBlend.SourceOver;
            Brush = new SolidColorBrush(Context2D, Color.White);

            Context2D.Target = new Bitmap1(Context2D, BackBuffer.QueryInterface<Surface>());
        }

        public void Start()
        {
            Context2D.BeginDraw();
        }

        public void Draw(string Text, int X, int Y, int Width, int Height)
        {
            Context2D.DrawText(Text, TextFormat, new RectangleF(X, Y, Width, Height), Brush);
        }

        public void DrawCrosshair()
        {
            Context2D.DrawLine(new SharpDX.Mathematics.Interop.RawVector2(960, 530), new SharpDX.Mathematics.Interop.RawVector2(960, 550), Brush);
            Context2D.DrawLine(new SharpDX.Mathematics.Interop.RawVector2(950, 540), new SharpDX.Mathematics.Interop.RawVector2(970, 540), Brush);
        }

        public void End()
        {
            Context2D.EndDraw();
        }

        public void Dispose()
        {
            Utilities.Dispose(ref Brush);
            Utilities.Dispose(ref TextFormat);
            Context2D?.Target?.Dispose();
            Utilities.Dispose(ref Context2D);
        }
    }
}