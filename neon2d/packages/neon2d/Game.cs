﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Timers;
using System.Threading;

namespace neon2d
{

    public class Game
    {

        public Window window;
        public Scene scene;
        public Window defaultwindow = new Window(800, 600, "neon2d");
        public int fps = 30;
        public int tick = 0;

        public System.Timers.Timer refreshtimer;

        public Action update;

        public bool windowOpened = false;
        public bool threadRunning = false;
        public bool shouldRender = false;

        public Game(Window mainwindow, Scene mainscene, Action onUpdate, int framerate = 30)
        {
            window = mainwindow;
            scene = mainscene;
            fps = framerate;
            
            window.gamewindow.Focus();
            window.gamewindow.Paint += new System.Windows.Forms.PaintEventHandler(this.window_Paint);

            window.gamewindow.FormClosed += Gamewindow_FormClosed;

            refreshtimer = new System.Timers.Timer(1000 / framerate);
            refreshtimer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            refreshtimer.Enabled = true;

            update = onUpdate;

        }

        private void Gamewindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

        public void window_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            
            for(int i = 0; i <= 999998; i++)
            {
                if(scene.spritelist[i] != null)
                {
                    g.DrawImage(scene.spritelist[i].currentframe, new Rectangle(scene.spritelist[i].spritex, scene.spritelist[i].spritey, scene.spritelist[i].spritewidth, scene.spritelist[i].spriteheight));
                }
                if(scene.proplist[i] != null)
                {
                    g.DrawImage(scene.proplist[i].propsource, new Rectangle(scene.proplist[i].propx, scene.proplist[i].propy, scene.proplist[i].propwidth, scene.proplist[i].propheight));
                }
                if(scene.textlist[i] != null)
                {
                    g.DrawString(scene.textlist[i].textcontent, scene.textlist[i].stringfont, scene.textlist[i].stringbrush, (float)scene.textlist[i].textx, (float)scene.textlist[i].texty);
                }
            }
        }

        public void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //clear draw buffer
            scene.cleanRenderBuffer();
            //refill draw buffer
            
            update();
            //render draw buffer
            shouldRender = true;
            
        }

        public void runGame()
        {
            Thread windowthread = new Thread(windowRender);
            threadRunning = true;
            windowthread.Start();
        }

        public void windowRender()
        {
            if(!windowOpened)
            {
                window.gamewindow.Show();
                window.gamewindow.Refresh();
                window.gamewindow.SuspendLayout();
                window.gamewindow.ResumeLayout();
            }
            while (threadRunning)
            {
                Application.DoEvents();
                if(shouldRender)
                {
                    window.gamewindow.Refresh();
                    shouldRender = false;
                }
            }
        }

    }
}