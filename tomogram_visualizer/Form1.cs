using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace tomogram_visualizer {
    public enum TypeVisualisation {
        Quads,
        Texture,
        QuadStrip
    }
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        TypeVisualisation typeV = TypeVisualisation.Quads;
        Bin reader;
        View view;
        bool loaded;
        int curLayer;
        int FrameCount;
        DateTime NextFPSUpdate = DateTime.Now.AddSeconds(1);
        private void displayFPS() {
            if(DateTime.Now >= NextFPSUpdate) {
                this.Text = String.Format("CT Visualizer (fps = {0})", FrameCount);
                NextFPSUpdate = DateTime.Now.AddSeconds(1);
                FrameCount = 0;
            }
            FrameCount++;
        }

        private void Form1_Load(object sender, EventArgs e) {
            reader = new Bin();
            view = new View();
            loaded = false;
            Application.Idle += Application_Idle;
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();
            if(dialog.ShowDialog() == DialogResult.OK) {
                string str = dialog.FileName;
                reader.readBIN(str);
                trackBar1.Maximum = Bin.Z - 1;
                view.SetupView(glControl1.Width, glControl1.Height);
                loaded = true;
                glControl1.Invalidate();
            }
        }

        bool needReloaded = true;
        private void glControl1_Paint(object sender, PaintEventArgs e) {
            if (loaded) {
                if(typeV == TypeVisualisation.Quads) {
                    view.DrawQuads(curLayer);
                }
                else if(typeV == TypeVisualisation.QuadStrip) {
                    view.DrawQuadStrip(curLayer);
                }
                else {
                    if (needReloaded) {
                        view.generateTextureImage(curLayer);
                        view.Load2DTexture();
                        needReloaded = false;
                    }
                    view.DrawTexture();
                }
                glControl1.SwapBuffers();
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e) {
            curLayer = trackBar1.Value;
            needReloaded = true;
        }

        private void Application_Idle(object sender, EventArgs e) {
            while (glControl1.IsIdle) {
                displayFPS();
                glControl1.Invalidate();
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e) {
            typeV = TypeVisualisation.Quads;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e) {
            typeV = TypeVisualisation.Texture;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e) {
            typeV = TypeVisualisation.QuadStrip;
        }

        private void trackBar2_Scroll(object sender, EventArgs e) {
            view.TransferFunctionMin = trackBar2.Value;
            needReloaded = true;
        }

        private void trackBar3_Scroll(object sender, EventArgs e) {
            view.TransferFunctionWidth = trackBar3.Value;
            needReloaded = true;
        }
    }
}
