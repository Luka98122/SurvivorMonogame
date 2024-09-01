using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using SharpDX.Direct2D1;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using SharpDX.MediaFoundation;
using Accessibility;
using Microsoft.Xna.Framework.Content;
using SharpDX.DXGI;
//using SharpDX.Direct2D1;

namespace SurvivorMonogame
{

    public class Bullet
    {
        public int x;
        public int y;
        public int dx;
        public int dy;
        public int size = 10;
        public int dmg = 25;
        public int speed = 3;
        public int timeAlive = 0;
        Texture2D tex;
        public Bullet(int x, int y, int xd, int yd, Texture2D te, int size=10, int dmg = 25)
        {

            this.x = x;
            this.y = y;
            dx = xd;
            dy = yd;
            this.size = size;
            this.dmg = dmg;
            this.tex = te;
        }
        public void Update()
        {
            x += dx*speed;
            y += dy*speed;
            timeAlive++;

        }
        public void Draw(SpriteBatch _spb, int cx, int cy)
        {
            _spb.Draw(tex,new Rectangle(x-cx,y-cy,size,size),Color.AliceBlue);
        }
    }
    public class Weapon
    {
        public int damage;
        public int cooldown;
        public int tillnext;
        public Texture2D bulletTex;
        public List<Bullet> bullets = new List<Bullet> { };
        public Weapon(int dmg, int cd, Texture2D bulTex)
        {
            damage = dmg;
            cooldown = cd;
            bulletTex = bulTex;
        }
        public void Update(int x, int y, int psize)
        {
            tillnext -= 1;
            if (tillnext <= 0)
            {
                bullets.Add(new Bullet(x + psize / 2 - 5, y+ psize / 2 - 5, 0, -1, bulletTex));
                bullets.Add(new Bullet(x + psize / 2 - 5, y+ psize / 2 - 5, 1, 0, bulletTex));
                bullets.Add(new Bullet(x + psize / 2 - 5, y+ psize / 2 - 5, 0, 1, bulletTex));
                bullets.Add(new Bullet(x + psize / 2 - 5, y+ psize / 2 - 5, -1, 0, bulletTex));
                tillnext = cooldown;
            }
            int i = 0;
            int cnter = 0;
            while (cnter < bullets.Count)
            {
                bullets[i].Update(); 
                if (bullets[i].timeAlive > 300) {
                    bullets.RemoveAt(i);
                    i--;
                }
                
                i++;
                cnter++;
            }
        }

        public void Draw(SpriteBatch _spb, int cx, int cy)
        {
            for (int i = 0; i < bullets.Count; i++)
            {
                
                bullets[i].Draw(_spb,cx,cy);
            }
        }
    }

    public class Map
    {
        public Dictionary<int, Texture2D> bindings = new Dictionary<int, Texture2D>{};
        public List<List<int>> map = new List<List<int>> { };
        public int w;
        public int h;
        public int size = 30;
        
        public Map(int w, int h, bool random=true)
        {
            this.w = w;
            this.h = h;
            Random RNG = new Random();
            if (random)
            {
                for (int i = 0; i < w; i++)
                {
                    map.Add(new List<int> { });
                    for (int j = 0; j < h; j++)
                    {
                        if (RNG.Next(0, 15) == 0){
                            map[i].Add(1);
                        }
                        else
                        {
                            map[i].Add(0);
                        }
                    }
                }
            }
        } 
        public void Draw(SpriteBatch _spb, int cx, int cy)
        {
            for (int i = 0; i < w; i++)
            {
                
                for (int j = 0; j < h; j++)
                {
                    _spb.Draw(bindings[map[i][j]],new Rectangle(j*size-cx,i*size-cy,size,size),Color.AliceBlue);
                }
            }
        }
    }
    public class Enemy
    {
        public int x;
        public int y;
        public int baseSpeed = 1;
        public int size = 30;
        public int health = 100;
        public Texture2D rectTexture;

        public Enemy(int x, int y,Texture2D tex)
        {
            this.rectTexture = tex;
            this.x = x;
            this.y = y; 
        }

        public void Update(Map m, int px, int py)
        {
            int y_tile = y / m.size;
            int x_tile = x / m.size;


            int ny = y;
            int nx = x;
            int oy = y;
            int ox = x;

            bool flag = true;

            if (py < y)
            {
                ny = y - baseSpeed;
            }
            if (py > y)
            {
                ny = y + baseSpeed;
            }


            if (ny != y)
            {
                for (int i = y_tile - 1; i < y_tile + 2; i++)
                {
                    if (i < 0 | i > m.h)
                        continue;
                    for (int j = x_tile - 1; j < x_tile + 2; j++)
                    {
                        if (j < 0 | j > m.w)
                            continue;
                        Rectangle tempRect = new Rectangle(j * m.size, i * m.size, m.size, m.size);
                        if (m.map[i][j] == 1 && tempRect.Intersects(new Rectangle(x, ny, size, size)))
                        {
                            flag = false;
                        }
                    }
                }
            }
            if (flag)
            {
                y = ny;
            }

            if (px < x)
            {
                nx = x - baseSpeed;
            }
            if (px > x)
            {
                nx = x + baseSpeed;
            }
            flag = true;
            if (nx != x)
            {
                for (int i = y_tile - 1; i < y_tile + 2; i++)
                {
                    for (int j = x_tile - 1; j < x_tile + 2; j++)
                    {
                        Rectangle tempRect = new Rectangle(j * m.size, i * m.size, m.size, m.size);
                        if (m.map[i][j] == 1 && tempRect.Intersects(new Rectangle(nx, oy, 30, 30)))
                        {
                            flag = false;
                        }
                    }
                }
            }
            if (flag)
            {
                x = nx;
            }
        }
        public void Draw(SpriteBatch _spb,int cx, int cy)
        {
            _spb.Draw(rectTexture, new Rectangle(x-cx, y-cy, size, size), Color.AliceBlue);
        }
    }
    public class Player
    {
        public int x = 400;
        public int y = 400;
        public int cx;
        public int cy;
        public int baseSpeed = 2;
        public int size = 30;
        public Texture2D rectTexture;
        public Texture2D bulletTex;
        public List<Weapon> weapons = new List<Weapon> { };

         
        public Player(Texture2D recTex, Texture2D bulletTex)
        {
            rectTexture = recTex;
            weapons.Add(new Weapon(25, 15, bulletTex));
        }

        public void Update(Map m)
        {
            for (int i = 0; i < weapons.Count; i++)
            {
                weapons[i].Update(x,y,size);
            }
            int y_tile = y / m.size;
            int x_tile = x / m.size;


            int ny = y;
            int nx = x;
            int oy = y;
            int ox = x;
            int ncx = cx;
            int ncy = cy;

            bool flag = true;

            if (Inputs.importantKeys["w"])
            {
                ny -= baseSpeed;
                ncy -= baseSpeed;
            }
            if (Inputs.importantKeys["s"])
            {
                ny += baseSpeed;
                ncy += baseSpeed;
            }
            if (ny != y)
            {
                for (int i = y_tile - 1; i < y_tile + 2; i++)
                {
                    for (int j = x_tile - 1; j < x_tile + 2; j++)
                    {
                        Rectangle tempRect = new Rectangle(j * m.size, i * m.size, m.size, m.size);
                        if (m.map[i][j] == 1 && tempRect.Intersects(new Rectangle(x, ny, size, size)))
                        {
                            flag = false;
                        }
                    }
                }
            }
            if (flag)
            {
                y = ny;
                cy = ncy;
            }
            if (Inputs.importantKeys["a"])
            {
                nx -= baseSpeed;
                ncx -= baseSpeed;
            }
            if (Inputs.importantKeys["d"])
            {
                nx += baseSpeed;
                ncx += baseSpeed;
            }
            flag = true;
            if (nx != x)
            {
                for (int i = y_tile - 1; i < y_tile + 2; i++)
                {
                    for (int j = x_tile - 1; j < x_tile + 2; j++)
                    {
                        Rectangle tempRect = new Rectangle(j * m.size, i * m.size, m.size, m.size);
                        if (m.map[i][j] == 1 && tempRect.Intersects(new Rectangle(nx,ny,30,30)))
                        {
                            flag = false;
                        }
                    }
                }
            }
            if (flag)
            {
                x = nx;
                cx = ncx;
            }
        }

        public void Draw(SpriteBatch _spb)
        {
            for (int i = 0; i < weapons.Count; i++)
            {
                weapons[i].Draw(_spb, cx, cy);
            }
            _spb.Draw(rectTexture, new Rectangle(x-cx, y-cy, size, size), Color.White);
        }
    }
    public static class Inputs
    {
        public static bool LeftClicked = false;

        private static MouseState ms = new MouseState(), oms;

        public static KeyboardState keyboardState;
        public static Keys[] keys;
        public static Dictionary<string, bool> importantKeys = new Dictionary<string,bool> {};

        public static void init()
        {
            importantKeys.Add("w", false);
            importantKeys.Add("a", false);
            importantKeys.Add("s", false);
            importantKeys.Add("d", false);

        }

        public static void Update()
        {
            oms = ms;
            ms = Mouse.GetState();
            if (ms.LeftButton != ButtonState.Pressed && oms.LeftButton == ButtonState.Pressed)
            {
                LeftClicked = true;
            }
            keyboardState = Keyboard.GetState();
            keys = keyboardState.GetPressedKeys();

            foreach (string key in importantKeys.Keys)
            {
                importantKeys[key] = false;
            }

            if (keys.Length > 0)
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    var keyValue = keys[i].ToString();
                    if ("WASD".Contains(keyValue))
                    {
                        importantKeys[keyValue.ToLower()] = true;
                    }
                }
            }
        }

        public static bool Hovering(Rectangle r)
        {
            return r.Contains(new Vector2(ms.X, ms.Y));
        }
    }
    public class Button
    {
        public Rectangle rect;
        public Texture2D rectTexture;
        public Color rectColor;
        public Color textColor;
        public string text;
        public Button(Rectangle rect, string text, Texture2D rectTexture, Color rectColor, Color textColor)
        {
            this.rect = rect;
            this.text = text;
            this.rectTexture = rectTexture;
            this.rectColor = rectColor;
            this.textColor = textColor;
          
        }
        public void Draw(SpriteBatch _spb)
        {
            
            _spb.Draw(this.rectTexture, this.rect, this.rectColor);
        }

    }
    public class SurvivorMonogame : Game
    {
        public Random RNG = new Random();
        public string screen = "main";
        public GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;
        private Texture2D startRect;
        private Button startButton;
        public List<Button> buttons = new List<Button>();
        public Player player;
        public List<Enemy> enemies = new List<Enemy> { };
        public Map map1;

        public SurvivorMonogame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        public Vector2 FindSpawn(Map m1)
        {
            int x = -1;
            int y = -1;
            while (true)
            {
                x = RNG.Next(1, m1.w - 1);
                y = RNG.Next(1, m1.h - 1);
                if (m1.map[y][x] == 0)
                {
                    return new Vector2(x, y);
                }
            }
        }

        protected override void Initialize()
        {
            Inputs.init();



            startRect = new Texture2D(GraphicsDevice, 1, 1);
            startRect.SetData(new Color[] { Color.DarkGray });

            Texture2D grass = new Texture2D(GraphicsDevice, 1, 1);
            grass.SetData(new Color[] { Color.LawnGreen });

            Texture2D red = new Texture2D(GraphicsDevice, 1, 1);
            red.SetData(new Color[] { Color.Red });


            Texture2D playerRect = new Texture2D(GraphicsDevice, 1, 1); 
            playerRect.SetData(new Color[] { Color.ForestGreen });

            map1 = new Map(100, 100);
            map1.bindings[0] = grass;
            map1.bindings[1] = startRect;

            player = new Player(playerRect,red);
            Vector2 pPos = FindSpawn(map1);
            player.x = Convert.ToInt32(pPos.X) * map1.size;// + map1.size / 2;
            player.cx = Convert.ToInt32(pPos.X) * map1.size-250;
            player.y = Convert.ToInt32(pPos.Y) * map1.size;// + map1.size / 2;
            player.cy = Convert.ToInt32(pPos.Y) * map1.size-250;
            player.bulletTex = red;
            for (int i = 0; i < 3; i++)
            {
                Vector2 enemyPos = FindSpawn(map1);

                enemies.Add(new Enemy(Convert.ToInt32(enemyPos.X)*map1.size, Convert.ToInt32(enemyPos.Y)*map1.size, red));
            }
            
            

            startButton = new Button(new Rectangle(100,100,100,50), "Start", startRect, Color.DarkGray, Color.DarkGoldenrod);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            buttons.Add(startButton);


        }

        protected override void Update(GameTime gameTime)
        {
            Inputs.Update();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (screen == "main")
            {
                for (int i = 0; i < buttons.Count; i++)
                {
                    if (Inputs.Hovering(buttons[i].rect) && Inputs.LeftClicked)
                    {
                        string txt = buttons[i].text;
                        if (txt == "Start")
                        {
                            screen = "game";
                        }
                    }
                }
            }

            if (screen == "game")
            {
                player.Update(map1);
                for (int i = 0; i < enemies.Count; i++)
                {
                    enemies[i].Update(map1, player.x, player.y);
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            if (screen == "main")
            {
                GraphicsDevice.Clear(Color.CornflowerBlue);
                for (int i = 0; i < buttons.Count; i++)
                {
                    buttons[i].Draw(_spriteBatch);
                }   
            }

            if (screen == "game")
            {
                GraphicsDevice.Clear(Color.Beige);
                map1.Draw(_spriteBatch, player.cx, player.cy);
                player.Draw(_spriteBatch);

                for (int i = 0; i < enemies.Count; i++)
                {
                    enemies[i].Draw(_spriteBatch, player.cx, player.cy);
                }
            
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
