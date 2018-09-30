// -----------------------------------------------------------------------
// <file>World.cs</file>
// <copyright>Grupa za Grafiku, Interakciju i Multimediju 2013.</copyright>
// <author>Srđan Mihić</author>
// <author>Aleksandar Josić</author>
// <summary>Klasa koja enkapsulira OpenGL programski kod.</summary>
// -----------------------------------------------------------------------
using System;
using Assimp;
using System.IO;
using System.Reflection;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Core;
using SharpGL;
using System.Windows;

using System.Drawing;
using System.Drawing.Imaging;
using SharpGL.SceneGraph.Cameras;
using System.Windows.Threading;

namespace AssimpSample
{
    /// <summary>
    ///  Klasa enkapsulira OpenGL kod i omogucava njegovo iscrtavanje i azuriranje.
    /// </summary>
    public class World : IDisposable
    {
        #region Atributi

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        private AssimpScene2 m_scene;

        private AssimpScene2 castle_scene;
        private AssimpScene2 arrow_scene;

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        private float m_xRotation = 0.0f;   //20.0f

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        private float m_yRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije sveta oko Z ose.
        /// </summary>
        private float m_zRotation = 0.0f;

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        private float m_sceneDistance = 250.0f;    //3000.0f

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height;


        private float arrow_translateX = 50.0f;
        private float arrow_translateY = 80.0f;
        private float arrow_translateZ = 0.0f;

        private float arrow_rotateX = 0.0f;
        private float arrow_rotateY = 0.0f;
        private float arrow_rotateZ = 90.0f;

        private float eyey = 100f;
        private float centery = 0f;

        private float wall_translateX = 65.0f;
        private float wall_rotateY = 0.0f;
        private float arrow_scale = 30.0f;


        Cube zid = null;

        /// <summary>
        ///	 Identifikatori tekstura za jednostavniji pristup teksturama
        /// </summary>
        private enum TextureObjects { Grass = 0, Mud, Fence, Castle };
        private readonly int m_textureCount = Enum.GetNames(typeof(TextureObjects)).Length;


        /// <summary>
        ///	 Identifikatori OpenGL tekstura
        /// </summary>
        private uint[] m_textures = null;

        /// <summary>
        ///	 Putanje do slika koje se koriste za teksture
        /// </summary>
        private string[] m_textureFiles = { "..//..//images//grass.jpg", "..//..//images//mud.jpg", "..//..//images//fence.jpg", 
                                            "..//..//images//castle.jpg" };

        // Parametri za animaciju
        private DispatcherTimer timer1;
        private DispatcherTimer timer2;
        private DispatcherTimer timer3;
        private DispatcherTimer timer4;

        private bool isRunningAnimation = false;
        private int arrowCount = 0;

        #endregion Atributi

        #region Properties

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        public AssimpScene2 Scene
        {
            get { return m_scene; }
            set { m_scene = value; }
        }

        public AssimpScene2 CastleScene
        {
            get { return castle_scene; }
            set { castle_scene = value; }
        }

        public AssimpScene2 ArrowScene
        {
            get { return arrow_scene; }
            set { arrow_scene = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        public float RotationX
        {
            get { return m_xRotation; }
            set { m_xRotation = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        public float RotationY
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko Z ose.
        /// </summary>
        public float RotationZ
        {
            get { return m_zRotation; }
            set { m_zRotation = value; }
        }

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        public float SceneDistance
        {
            get { return m_sceneDistance; }
            set { m_sceneDistance = value; }
        }

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        /// <summary>
        ///	 Translacija lijevog zida po X koordinati.
        /// </summary>
        public float WallTranslateX
        {
            get { return wall_translateX; }
            set { wall_translateX = value; }
        }
        
        /// <summary>
        ///	 Rotacija desnog zida po Y osi.
        /// </summary>
        public float WallRorateY
        {
            get { return wall_rotateY; }
            set { wall_rotateY = value; }
        }

        /// <summary>
        ///	 Skaliranje strijele
        /// </summary>
        public float ScaleArrow
        {
            get { return arrow_scale; }
            set { arrow_scale = value; }
        }

        /// <summary>
        ///	 Skaliranje strijele
        /// </summary>
        public bool AnimationRunning
        {
            get { return isRunningAnimation; }
            set { isRunningAnimation = value; }
        }
        

        #endregion Properties

        #region Konstruktori

        /// <summary>
        ///  Konstruktor klase World.
        /// </summary>
        public World(String scenePath, String sceneFileName, int width, int height, OpenGL gl)
        {
            this.m_scene = new AssimpScene2(scenePath, sceneFileName, gl);
            this.m_width = width;
            this.m_height = height;
        }

        //konstruktor za 2 scene
        public World(String castlePath, String castleFileName, String arrowPath, String arrowFileName, int width, int height, OpenGL gl)
        {
            this.castle_scene = new AssimpScene2(castlePath, castleFileName, gl);
            this.arrow_scene = new AssimpScene2(arrowPath, arrowFileName, gl);
            
            this.m_width = width;
            this.m_height = height;

            try
            {
                this.zid = new Cube();
            }
            catch 
            {
                MessageBox.Show("Neuspjesno kreirana instanca klase Cube", "GRESKA", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            m_textures = new uint[m_textureCount];

        }

        /// <summary>
        ///  Destruktor klase World.
        /// </summary>
        ~World()
        {
            this.Dispose(false);
        }

        #endregion Konstruktori

        #region Metode

        /// <summary>
        ///  Korisnicka inicijalizacija i podesavanje OpenGL parametara.
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Color(1f, 0f, 0f);
            // Model sencenja na flat (konstantno)
            gl.ShadeModel(OpenGL.GL_FLAT);
            gl.Enable(OpenGL.GL_DEPTH_TEST);    
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.FrontFace(OpenGL.GL_CW);

            //m_scene.LoadScene();
            //m_scene.Initialize();
            castle_scene.LoadScene();
            castle_scene.Initialize();
            arrow_scene.LoadScene();
            arrow_scene.Initialize();

            //SetupLighting(gl);

            LoadTexture(gl);

            InitializeAnimation();
        }

        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;
            gl.Viewport(0, 0, m_width, m_height);
            gl.MatrixMode(OpenGL.GL_PROJECTION);      // selektuj Projection Matrix
            gl.LoadIdentity();
            gl.Perspective(50f, (double)width / height, 1f, 20000f);    //fov = 50, near = 1, far = 20 000
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();                // resetuj ModelView Matrix
        }

        /// <summary>
        /// Podesavanje osvjetljenja
        /// https://www.opengl.org/discussion_boards/showthread.php/132502-Color-tables 
        /// </summary>
        private void SetupLighting(OpenGL gl)
        {   
            gl.ClearColor(0.30f, 0.30f, 1.0f, 1.0f);      //boja svijeta
            gl.Color(1.0f, 1.0f, 1.0f);
            gl.Enable(OpenGL.GL_LIGHTING);

            gl.Enable(OpenGL.GL_COLOR_MATERIAL);    //color tracking - color
            // Podesi na koje parametre materijala se odnose pozivi glColor funkcije
            gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE);

            //1. tackasti svjetlosni izvor svjetlo zute boje pozicionirangore lijevo u odnosu na centar
            float[] ambLight0 = { 1.0f, 1.0f, 0.0f, 1.0f };
            float[] difLight0 = { 1.0f, 1.0f, 0.0f, 1.0f };
            float[] spcLight0 = { 0.0f, 1.0f, 0.0f, 1.0f };
            float[] lightPos0 = { 1000.0f, 200.0f, 0.0f, 1.0f };    //izgleda da treba + iako je na negativnom djelu?!

            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180.0f);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, lightPos0);

            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, ambLight0);
            //gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, difLight0);
            //gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, spcLight0);
            
            gl.Enable(OpenGL.GL_LIGHT0);


            //2. reflektorski svjetlosni izvor (cut-off = 45) bjele boje iznad zamka a moze i iznad auta cak :P
            float[] ambLight1 = { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] difLight1 = { 0.6f, 0.6f, 0.6f, 1.0f };
            float[] spcLight1 = { 0.95f, 0.95f, 0.95f, 1.0f };
            float[] lightPos1 = { 0.0f, 20.0f, 0.0f, 1.0f };

            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, ambLight1);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_CUTOFF, 45.0f);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, lightPos1);

            //gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, ambLight1);
            //gl.LightModel(OpenGL.GL_POSITION, lightPos1);
            //gl.LightModel(OpenGL.GL_SPOT_CUTOFF, 45.0f);

            gl.Enable(OpenGL.GL_LIGHT1);
        }

        /// <summary>
        /// Ucitavanje tekstura
        /// </summary>
        public void LoadTexture(OpenGL gl)
        {
            // Teksture se primenjuju sa parametrom decal
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_DECAL);  //nacin spajanja teksture sa materijalo

            // Ucitaj slike i kreiraj teksture
            gl.GenTextures(m_textureCount, m_textures);
            for (int i = 0; i < m_textureCount; ++i)
            {
                // Pridruzi teksturu odgovarajucem identifikatoru
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[i]);
                // Ucitaj sliku i podesi parametre teksture
                Bitmap image = new Bitmap(m_textureFiles[i]);
                // rotiramo sliku zbog koordinantog sistema opengl-a
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                // RGBA format (dozvoljena providnost slike tj. alfa kanal)
                BitmapData imageData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                                      System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                gl.Build2DMipmaps(OpenGL.GL_TEXTURE_2D, (int)OpenGL.GL_RGBA8, image.Width, image.Height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);		// Linear Filtering
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);		// Linear Filtering

                image.UnlockBits(imageData);
                image.Dispose();
            }
        }

        #region Animacija

        /// <summary>
        /// Definisanje tajmera animacije
        /// </summary>
        public void InitializeAnimation()
        {
            timer1 = new DispatcherTimer();
            timer1.Interval = TimeSpan.FromMilliseconds(20);  //20
            timer1.Tick += new EventHandler(KretanjeKaZamku);

            timer2 = new DispatcherTimer();
            timer2.Interval = TimeSpan.FromMilliseconds(20);    //20
            timer2.Tick += new EventHandler(PticijaPerspektiva);

            timer3 = new DispatcherTimer();
            timer3.Interval = TimeSpan.FromMilliseconds(20);    //20
            timer3.Tick += new EventHandler(OkretanjeKaVratima);

            timer4 = new DispatcherTimer();
            timer4.Interval = TimeSpan.FromMilliseconds(20);     //20
            timer4.Tick += new EventHandler(IspaljivanjeStrijela);

        }

        //kamera prvo po stazi krece da se priblizava zamku
        public void KretanjeKaZamku(object sender, EventArgs e)
        {
            timer1.Start();

            eyey = 3f;
            m_sceneDistance--;

            if (m_sceneDistance == 0f)   //provjerava da li dosao do odredjene tacke
            {
                timer1.Stop();
                timer2.Start();
            }

            isRunningAnimation = true;  //signalizira da je animacija pokrenuta kako bi onemogucio ostale tastere
        }

        //podizanje u pticiju perspektivu
        public void PticijaPerspektiva(object sender, EventArgs e)
        {
            eyey++;

            if (eyey > 200f)
            {
                timer2.Stop();
                timer3.Start();
            }
        }

        //rotacija ka vratima
        public void OkretanjeKaVratima(object sender, EventArgs e)
        {
            m_yRotation++;

            if (m_yRotation == 180f)
            {
                timer3.Stop();

                arrow_translateX = 0.0f;
                arrow_translateY = 0.0f;
                arrow_translateY = 30.0f;
                arrow_rotateZ = -230f;

                timer4.Start();
            }
        }

        public void IspaljivanjeStrijela(object sender, EventArgs e)
        {
            m_xRotation = -60.0f;     //stimam ugao gledanja iz pticije perpsektive

            arrow_translateX++;
            arrow_translateY++;

            if (arrow_translateY > 80)
            {
                arrow_translateX = 0f;
                arrow_translateY = 0f;
                arrowCount++;
            }

            if (arrowCount == 10)
            {
                timer4.Stop();
                isRunningAnimation = false;
                ResetParameters();
                AnimationStop();
            }
        }

        public void AnimationStop()
        {
            timer1.Stop();
            timer2.Stop();
            timer3.Stop();
            timer4.Stop();
            
            ResetParameters();
            isRunningAnimation = false;

            InitializeAnimation();
        }

        public void ResetParameters()
        {
            m_sceneDistance = 250f;
            m_xRotation = 0.0f;
            m_yRotation = 0.0f;
           
            eyey = 100f;
            centery = 0f;

            wall_translateX = 65.0f;
            wall_rotateY = 0.0f;
            arrow_scale = 30.0f;

            arrow_translateX = 50.0f;
            arrow_translateY = 80.0f;
            arrow_translateZ = 0.0f;

            arrow_rotateX = 0.0f;
            arrow_rotateY = 0.0f;
            arrow_rotateZ = 90.0f;

            arrowCount = 0;
        }

        #endregion Animacija

        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE); 
            gl.LoadIdentity();

            gl.LookAt(0f, eyey, m_sceneDistance,   
                      0f, centery, -10f, 
                      0f, 1f, 0f);
            
            SetupLighting(gl); //mora ovim redoslijedom da bi bio izvor svjetlosti stacionaran!!!
            
            gl.PushMatrix(); 
            //gl.Translate(0.0f, 0.0f, -m_sceneDistance);   //0.0f za z osu
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);

            gl.Viewport(0, 0, m_width, m_height);

            //zamak
            gl.PushMatrix();
            //gl.Color(0f, 0f, 1f);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Castle]);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);   //nacin spajanja teksture sa materijalom na modulate
            gl.Scale(0.1f, 0.1f, 0.1f);
            castle_scene.Draw();
            gl.PopMatrix();

            //strijela
            gl.PushMatrix();
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Castle]);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            gl.Translate(arrow_translateX, arrow_translateY, arrow_translateZ);
            gl.Rotate(arrow_rotateX, arrow_rotateY, arrow_rotateZ);
            gl.Scale(arrow_scale, arrow_scale, arrow_scale);
            //gl.Scale(30f, 30f, 30f);
            arrow_scene.Draw();
            gl.PopMatrix();

            //podloga
            gl.PushMatrix();
            //gl.Color(0.3f, 50.4f, 0.3f);
            gl.Translate(0f, -0.1f, -20f);
            gl.Disable(OpenGL.GL_CULL_FACE);    //iskljucio da bi mi se vidjela podloga
            gl.Enable(OpenGL.GL_NORMALIZE);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Grass]);

            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            gl.MatrixMode(OpenGL.GL_TEXTURE);    
            gl.LoadIdentity();
            //gl.Scale(5, 5, 5);      //skaliranje teksture
            gl.MatrixMode(OpenGL.GL_MODELVIEW);

            gl.Begin(OpenGL.GL_QUADS);
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(-90f, 0f, 100f);
            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(-90f, 0f, -30f);
            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(90f, 0f, -30f);
            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(90f, 0f, 100f);
            gl.End();
            gl.PopMatrix();
            
            //staza
            gl.PushMatrix();
            //gl.Color(0.36f, 0.25f, 0.20f);
            gl.Translate(0f, 0f, 0f);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Mud]);
            gl.Enable(OpenGL.GL_NORMALIZE);
            gl.Begin(OpenGL.GL_QUADS);      //crtano u x i z ravni
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(-5f, 0f, 80f);
            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(-5f, 0f, 0f);
            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(5f, 0f, 0f);
            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(5f, 0f, 80f);
            gl.End();
            gl.PopMatrix();

            //lijevi zid
            gl.PushMatrix();
            //gl.Color(0.85f, 0.53f, 0.10f);
            gl.Translate(-wall_translateX, 15.0f, 0f);
            //gl.Translate(-65.0f, 15.0f, 0f);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Fence]);
            gl.Scale(3.0f, 15.0f, 40.0f);   //kolika je visina zida, toliko sam stavio da bude i translacija   
            zid.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            //desni zid
            gl.PushMatrix();
            //gl.Color(0.85f, 0.53f, 0.10f);
            gl.Translate(65.0f, 15.0f, 0f);
            gl.Rotate(0.0f, wall_rotateY, 0.0f);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Fence]);
            gl.Scale(3.0f, 15.0f, 40.0f);   //kolika je visina zida, toliko sam stavio da bude i translacija   
            zid.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            //text
            DrawText1(gl);

            gl.PopMatrix();

            // Oznaci kraj iscrtavanja
            gl.Flush();
        }

        private void DrawText1(OpenGL gl)
        {
            /*
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PushMatrix();
            gl.LoadIdentity();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.PushMatrix();
            gl.LoadIdentity();

            */

            //gl.PushMatrix();

            gl.Viewport(m_width / 2, 0, m_width / 2, m_height / 2);

            gl.DrawText(10, 120, 0.5f, 1f, 1f, "Arials", 14, "");
            gl.DrawText(1150, 100, 0.5f, 1f, 1f, "Arial", 14, "Predmet: Racunarska grafika");
            gl.DrawText(1150, 100, 0.5f, 1f, 1f, "Arial", 14, "_______________________");
            gl.DrawText(1150, 80, 0.5f, 1f, 1f, "Arial", 14, "Sk.god: 2017/18");
            gl.DrawText(1150, 80, 0.5f, 1f, 1f, "Arial", 14, "_____________");
            gl.DrawText(1150, 60, 0.5f, 1f, 1f, "Arial", 14, "Ime: Danilo");
            gl.DrawText(1150, 60, 0.5f, 1f, 1f, "Arial", 14, "_________");
            gl.DrawText(1150, 40, 0.5f, 1f, 1f, "Arial", 14, "Prezme: Jevtovic");
            gl.DrawText(1150, 40, 0.5f, 1f, 1f, "Arial", 14, "_____________");
            gl.DrawText(1150, 20, 0.5f, 1f, 1f, "Arial", 14, "Sifra zad: 20.1");
            gl.DrawText(1150, 20, 0.5f, 1f, 1f, "Arial", 14, "___________");

            //gl.PopMatrix();

            /*
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.PopMatrix();
            */
        }
         
        /// <summary>
        ///  Iscrtavanje SharpGL primitive grida.
        /// </summary>
        private void DrawGrid(OpenGL gl)
        {
            gl.PushMatrix(); // pamti kord sistem
            Grid grid = new Grid();
            gl.Translate(0f, 1f, 0f);
            gl.Rotate(90f, 0f, 0f);
            gl.Scale(7f, 7f, 7f);
            grid.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Design);
            gl.PopMatrix(); //vrati se na mesto odakle smo krenuli
        }


        /// <summary>
        ///  Implementacija IDisposable interfejsa.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //m_scene.Dispose();
                castle_scene.Dispose();
                arrow_scene.Dispose();
                timer1.Stop();
                timer2.Stop();
                timer3.Stop();

                //gl.DeleteTextures(m_textureCount, m_textures);
            }
        }

        #endregion Metode

        #region IDisposable metode

        /// <summary>
        ///  Dispose metoda.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }



        #endregion IDisposable metode
    }
}
