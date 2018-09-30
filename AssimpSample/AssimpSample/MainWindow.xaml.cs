using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using SharpGL.SceneGraph;
using SharpGL;
using Microsoft.Win32;
using System.ComponentModel;


namespace AssimpSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Atributi

        /// <summary>
        ///	 Instanca OpenGL "sveta" - klase koja je zaduzena za iscrtavanje koriscenjem OpenGL-a.
        /// </summary>
        World m_world = null;

        private String castle_name;
        private String castle_path;
        
        private String arrow_name;
        private String arrow_path;


        #endregion Atributi

        #region Konstruktori

        public MainWindow()
        {
            // Inicijalizacija komponenti
            InitializeComponent();

            // Kreiranje OpenGL sveta
            try
            {
                //m_world = new World(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\Castle"), "Castle.obj", (int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);

                castle_name = "3d-model.obj";
                castle_path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\Castle2");

                arrow_name = "Arrow.obj";
                arrow_path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\Arrow");

                m_world = new World(castle_path, castle_name, arrow_path, arrow_name, (int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);

            }
            catch (Exception e)
            {
                MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta. Poruka greške: " + e.Message, "Poruka", MessageBoxButton.OK);
                this.Close();
            }
        }

        #endregion Konstruktori

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            m_world.Draw(args.OpenGL);
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            m_world.Initialize(args.OpenGL);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            m_world.Resize(args.OpenGL, (int)openGLControl.Width, (int)openGLControl.Height);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F4: this.Close(); break;  //F10
                
                case Key.I:     //W
                    if (m_world.AnimationRunning == false && m_world.RotationX > -20)   //-20 da se ne vidi donja strana
                    {
                        m_world.RotationX -= 5.0f;
                    }   break;

                case Key.K:     //S
                    if (m_world.AnimationRunning == false && m_world.RotationX < 50)
                    {
                        m_world.RotationX += 5.0f;
                    }   break;
                
                case Key.J:     //A
                    if (m_world.AnimationRunning == false)
                    {
                        m_world.RotationY -= 5.0f;
                    }   break;   

                case Key.L:     //D
                    if (m_world.AnimationRunning == false)
                    {
                        m_world.RotationY += 5.0f;
                    }   break;   

                case Key.Add:
                    if (m_world.AnimationRunning == false)
                    {
                        m_world.SceneDistance -= 5.0f;
                    }   break;   //700 //50

                case Key.Subtract:
                    if (m_world.AnimationRunning == false)
                    {
                        m_world.SceneDistance += 5.0f;
                    }   break;  //700 /50

                case Key.V:                 //pokretanje animacija
                    if (m_world.AnimationRunning == false)
                    {
                        resetParameters();
                        m_world.ResetParameters();  //resetujem parametre ukoliko je doslo do nekih transformacija
                        m_world.KretanjeKaZamku(sender, e);

                        transSlider.IsEnabled = false;
                        rotSlider.IsEnabled = false;
                        skalSlider.IsEnabled = false;
                    }   break;    

                case Key.C:                 //zaustavljanje animacije
                    m_world.AnimationStop();
                    resetParameters(); 
                    break;     
                
                case Key.R:
                    if (m_world.AnimationRunning == false)
                    {
                        resetParameters();
                    }   break;

                case Key.F2:
                    OpenFileDialog opfModel = new OpenFileDialog();
                    bool result = (bool) opfModel.ShowDialog();
                    if (result)
                    {
                        try
                        {
                            World newWorld = new World(Directory.GetParent(opfModel.FileName).ToString(), Path.GetFileName(opfModel.FileName), (int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);
                            m_world.Dispose();
                            m_world = newWorld;
                            m_world.Initialize(openGLControl.OpenGL);
                        }
                        catch (Exception exp)
                        {
                            MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta:\n" + exp.Message, "GRESKA", MessageBoxButton.OK );
                        }
                    }
                    break;
            }
        }

        #region PropertyChangedNotifier
        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        /*
        private void Animacija_Click(object sender, RoutedEventArgs e)
        {
            m_world.ResetParameters();
            m_world.KretanjeKaZamku(sender, e);
        }
        */

        //translacija lijevog zida po x osi
        private void translacijaZida_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (m_world != null && m_world.AnimationRunning == false)
            {
                m_world.WallTranslateX = -float.Parse(translacijaZida.Text);
            }
        }

        //rotacija desnog zida
        private void rotacijaZida_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (m_world != null && m_world.AnimationRunning == false)
            {
                m_world.WallRorateY = float.Parse(rotacijaZida.Text);
            }
        }

        private void skalStrijele_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (m_world != null && m_world.AnimationRunning == false)
            {
                m_world.ScaleArrow = float.Parse(skalStrijele.Text);
            }
        }

        //resetuje wpf kontrole
        private void resetParameters()
        {
            transSlider.Value = -65.0f;
            rotSlider.Value = 0.0f;
            skalSlider.Value = 30.0f;

            transSlider.IsEnabled = true;
            rotSlider.IsEnabled = true;
            skalSlider.IsEnabled = true;
           

            //transSlider.Value = -m_world.WallTranslateX;
        }
    }
}
