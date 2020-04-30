using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;

namespace tomogram_visualizer {
    class View {
        public void SetupView(int width, int height) {
            GL.ShadeModel(ShadingModel.Smooth);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, Bin.X, 0, Bin.Y, -1, 1);
            GL.Viewport(0, 0, width, height);
        }

        int TFmin = 0;
        int TFwidth = 2000;

        public int TransferFunctionMin {
            get {
                return TFmin;
            }
            set {
                TFmin = value;
            }
        }
        public int TransferFunctionWidth {
            get {
                return TFwidth;
            }
            set {
                TFwidth = value;
            }
        }

        private Color TransferFunction(short value) {
            int min = TFmin;
            int max = TFmin + TFwidth;
            int newVal = Clamp((value - min) * 255 / (max - min), 0, 255);
            return Color.FromArgb(255, newVal, newVal, newVal);
        }

        private int Clamp(int value, int min, int max) {
            if (value < min) {
                return min;
            }
            if (value > max) {
                return max;
            }
            return value;
        }

        public void DrawQuads(int LayerN) {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Begin(BeginMode.Quads);
            for(int x = 0; x<Bin.X - 1; x++) {
                for(int y = 0; y < Bin.Y - 1; y++) {
                    short value;
                    //1 вершина
                    value = Bin.array[x + y * Bin.X + LayerN * Bin.X * Bin.Y];
                    GL.Color3(TransferFunction(value));
                    GL.Vertex2(x, y);
                    //2 вершина
                    value = Bin.array[x + (y + 1) * Bin.X + LayerN * Bin.X * Bin.Y];
                    GL.Color3(TransferFunction(value));
                    GL.Vertex2(x, y + 1);
                    //3 вершина
                    value = Bin.array[x + 1 + (y + 1) * Bin.X + LayerN * Bin.X * Bin.Y];
                    GL.Color3(TransferFunction(value));
                    GL.Vertex2(x + 1, y + 1);
                    //4 вершина
                    value = Bin.array[x + 1 + y * Bin.X + LayerN * Bin.X * Bin.Y];
                    GL.Color3(TransferFunction(value));
                    GL.Vertex2(x + 1, y);
                }
            }
            GL.End();
        }

        Bitmap textureImage;
        int VBOtexture;

        public void Load2DTexture() {
            GL.BindTexture(TextureTarget.Texture2D, VBOtexture);
            BitmapData data = textureImage.LockBits(
                new Rectangle(0, 0, textureImage.Width, textureImage.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb
                );
            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgba,
                data.Width,
                data.Height,
                0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte,
                data.Scan0
                );

            textureImage.UnlockBits(data);

            GL.TexParameter(
                TextureTarget.Texture2D,
                TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Linear
                );
            GL.TexParameter(
                TextureTarget.Texture2D,
                TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Linear
                );

            ErrorCode Er = GL.GetError();
            string str = Er.ToString();
        }
        public void generateTextureImage(int layerN) {
            textureImage = new Bitmap(Bin.X, Bin.Y);
            for(int i = 0; i < Bin.X; i++) {
                for(int j = 0; j < Bin.Y; j++) {
                    int pixelN = i + j * Bin.X + layerN * Bin.X * Bin.Y;
                    textureImage.SetPixel(i, j, TransferFunction(Bin.array[pixelN]));
                }
            }
        }
        public void DrawTexture() {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, VBOtexture);

            GL.Begin(BeginMode.Quads);
            GL.Color3(Color.White);
            GL.TexCoord2(0f, 0f);
            GL.Vertex2(0, 0);
            GL.TexCoord2(0f, 1f);
            GL.Vertex2(0, Bin.Y);
            GL.TexCoord2(1f, 1f);
            GL.Vertex2(Bin.X, Bin.Y);
            GL.TexCoord2(1f, 0);
            GL.Vertex2(Bin.X, 0);
            GL.End();

            GL.Disable(EnableCap.Texture2D);
        }
        public void DrawQuadStrip(int LayerN) {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Begin(BeginMode.QuadStrip);
            for (int x = 0, y = 0; x < Bin.X - 1; x++) {
                short value;
                value = Bin.array[x + y * Bin.X + LayerN * Bin.X * Bin.Y];
                GL.Color3(TransferFunction(value));
                GL.Vertex2(x, y);
                value = Bin.array[x + 1 + y * Bin.X + LayerN * Bin.X * Bin.Y];
                GL.Color3(TransferFunction(value));
                GL.Vertex2(x + 1, y);
                for (y = 0; y < Bin.Y - 1; y++) {
                    value = Bin.array[x + (y + 1) * Bin.X + LayerN * Bin.X * Bin.Y];
                    GL.Color3(TransferFunction(value));
                    GL.Vertex2(x, y + 1);
                    //3 вершина
                    value = Bin.array[x + 1 + (y + 1) * Bin.X + LayerN * Bin.X * Bin.Y];
                    GL.Color3(TransferFunction(value));
                    GL.Vertex2(x + 1, y + 1);
                    //4 вершина
                   
                }
            }
            GL.End();
        }
    }
}
