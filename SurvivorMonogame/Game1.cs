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
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
    
//using SharpDX.Direct2D1;

namespace SurvivorMonogame
{

    public class Bullet
    {
        public int x;
        public int y;
        public double dx;
        public double dy;
        public int size = 10;
        public int dmg = 25;
        public int speed = 3;
        public int timeAlive = 0;
        
        Texture2D tex;
        public Bullet(int x, int y, double xd, double yd, Texture2D te, int size=10, int dmg = 25)
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
            try
            {
                x += Convert.ToInt32(dx * speed);
                y += Convert.ToInt32(dy * speed);
            } catch
            {
                return;
            }
            
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

        public static (double dx, double dy) CalculateDirection((double x, double y) start, (double x, double y) target)
        {
            double diffX = target.x - start.x;
            double diffY = target.y - start.y;

            double len = Math.Sqrt(diffX * diffX + diffY * diffY);

            double dx = diffX / len;
            double dy = diffY / len;

            return (dx, dy);
        }

        public Weapon(int dmg, int cd, Texture2D bulTex)
        {
            damage = dmg;
            cooldown = cd;
            bulletTex = bulTex;
        }
        public void Update(int x, int y, int psize, int ex, int ey)
        {
            tillnext -= 1;

            var start = (Convert.ToDouble(x), Convert.ToDouble(y));
            var end = (Convert.ToDouble(ex), Convert.ToDouble(ey));


            if (tillnext <= 0)
            {
                var dat = CalculateDirection(start, end);
                if (dat.dx == double.NaN | dat.dy == double.NaN)
                {

                }
                else
                {
                    bullets.Add(new Bullet(x + psize / 2 - 5, y + psize / 2 - 5, dat.dx*2, dat.dy*2, bulletTex));

                    tillnext = cooldown;
                }
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

    public class TilePattern
    {
        public List<List<int>> pattern = new List<List<int>>() { };
        public string id = "";
        public TilePattern(List<List<int>> pattern, string id)
        {
            this.pattern = pattern;
            this.id = id;
        }

        public bool equal(TilePattern other) {

            for (int y1 = -1; y1 < 2; y1++)
            {
                for (int x1 = -1; x1 < 2; x1++)
                {
                    if (pattern[y1+1][x1+1] == 2)
                    {
                        continue;
                    }
                    if (other.pattern[y1+1][x1+1] != pattern[y1+1][x1+1])
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }

    public class Map
    {
        public Dictionary<int, Texture2D> bindings = new Dictionary<int, Texture2D>{};
        public List<List<int>> map = new List<List<int>> { };
        public int w;
        public int h;
        //public int size = 30;
        public int sw = 49;
        public int sh = 99;
        public Texture2D terrain;
        public int tsize = 141;
        public Dictionary<string, Texture2D> tiles;
        public int percentage;
        public List<TilePattern> patterns = new List<TilePattern> { };

        public List<List<string>> tileToDraw = new List<List<string>> { };
        
        public float getPercentage()
        {
            float blocks = 0;
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    if (map[i][j] >= 1)
                    {
                        blocks += 1;
                    }
                }
            }
            return blocks * 100 / (w * h);
            
        }

        public void addNewClusterBlock(int x, int y, int depth, Random rng)
        {
            if (y < 0 | y>=h | x <0 | x >= w)
            {
                return;
            }
            if (depth > 20)
            {
                return;
            }
            if (map[y][x] == 0)
            {
                map[y][x] = 100;
                return;
            }
            else
            {
                int dir = rng.Next(0, 4);
                int newX = x;
                int newY = y;
                if (dir == 0)
                {
                    newY--;
                }
                if (dir == 1)
                {
                    newY++;
                }
                if (dir == 2)
                {
                    newX--;
                }
                if (dir == 3)
                {
                    newX++;
                }
                addNewClusterBlock(newX, newY, depth + 1, rng);  
            }
        }

        public string whichTile(int x, int y)
        {
            if (true == false)
            {
                TilePattern ttile = new TilePattern(new List<List<int>> {
                new List<int> {},
                new List<int> {},
                new List<int> {}
            }, "null");
                for (int y1 = -1; y1 < 2; y1++)
                {
                    for (int x1 = -1; x1 < 2; x1++)
                    {
                        if (map[y + y1][x + x1] == 0)
                        {
                            ttile.pattern[y1 + 1].Add(0);
                        }
                        else
                        {
                            ttile.pattern[y1 + 1].Add(1);
                        }
                    }
                }

                TilePattern ttile2 = new TilePattern(new List<List<int>> {
                new List<int> {0,1,0},
                new List<int> {0,1,0},
                new List<int> {0,1,0}
            }, "null");

                foreach (TilePattern tp in patterns)
                {
                    if (tp.equal(ttile) == true)
                    {
                        return tp.id;
                    }
                }
            }
            else
            {

                // Top row
                if (map[y][x - 1] == 0 && map[y - 1][x] == 0 && map[y][x + 1] != 0 && map[y + 1][x] != 0) // Top left corner
                {
                    return "-1,-1";
                }
                if (map[y][x + 1] == 0 && map[y - 1][x] == 0 && map[y][x - 1] != 0 && map[y + 1][x] != 0) // Top right corner
                {
                    return "1,-1";
                }
                if (map[y][x + 1] != 0 && map[y - 1][x] == 0 && map[y][x - 1] != 0 && map[y + 1][x] != 0) // top center
                {
                    return "0,-1";
                }


                // Center row
                if (map[y][x + 1] != 0 && map[y - 1][x] != 0 && map[y][x - 1] == 0 && map[y + 1][x] != 0) // left center
                {
                    return "-1,0";
                }
                if (map[y][x - 1] != 0 && map[y][x + 1] != 0 && map[y - 1][x] != 0 && map[y + 1][x] != 0) // Center
                {
                    return "0,0";
                }
                if (map[y][x + 1] == 0 && map[y - 1][x] != 0 && map[y][x - 1] != 0 && map[y + 1][x] != 0) // right center
                {
                    return "1,0";
                }

                // Bottom row
                if (map[y][x - 1] == 0 && map[y - 1][x] != 0 && map[y][x + 1] != 0 && map[y + 1][x] == 0) // Bot left corner
                {
                    return "-1,1";
                }
                if (map[y][x + 1] == 0 && map[y - 1][x] != 0 && map[y][x - 1] != 0 && map[y + 1][x] == 0) // Bot right corner
                {
                    return "1,1";
                }
                if (map[y][x + 1] != 0 && map[y - 1][x] != 0 && map[y][x - 1] != 0 && map[y + 1][x] == 0) // bot center
                {
                    return "0,1";
                }

                if (map[y][x + 1] == 0 && map[y - 1][x] != 0 && map[y][x - 1] != 0 && map[y + 1][x] != 0 && map[y + 1][x + 1] != 0) // At end of wall right corner
                {
                    return "1,-1";
                }

                return "solo";
                return "Error: Tile-picking logic did not pick any tile. Function: Map.whichTile";
            }
        }

        public Map(int w, int h, bool random=true, string randomType="clump")
        {
            this.w = w;
            this.h = h;
            Random RNG = new Random();

            
            if (random && randomType == "basic")
            {
                for (int i = 0; i < w; i++)
                {
                    map.Add(new List<int> { });
                    for (int j = 0; j < h; j++)
                    {
                        if (RNG.Next(0, 15) == 0){
                            map[i].Add(100);
                        }
                        else
                        {
                            map[i].Add(0);
                        }
                    }
                }
                return;
            }

            for (int i = 0; i < w; i++)
            {
                map.Add(new List<int> { });
                for (int j = 0; j < h; j++)
                {
                    map[i].Add(0);
                }
            }
            if (random && randomType == "clump")
            {
                percentage = RNG.Next(35, 55);
                while (true)
                {
                    if (getPercentage() >= percentage)
                    {
                        break;
                    }
                    int cluster_limit = RNG.Next(15, 40);

                    int thisX = RNG.Next(0, w);
                    int thisY = RNG.Next(0, w);


                    for (int i = 0; i < cluster_limit; i++)
                    {
                        addNewClusterBlock(thisX, thisY, 0, RNG);
                    }
                }
            }
            for (int i = 0; i < w; i++)
            {
                tileToDraw.Add(new List<string> { });
                for (int j = 0; j < h; j++)
                {
                    if (i >= 1 && i <= w - 2 && j >= 1 && j <= h - 2)
                    {

                        string tile = whichTile(j, i);
                        tileToDraw[i].Add(tile);

                    }
                }
            }
        } 
        public void Draw(SpriteBatch _spb, int cx, int cy)
        {
            for (int i = -5; i < sw*w / tsize + 5; i++)
            {
                for (int j = -5; j < sh*h / tsize + 1; j++)
                {
                    _spb.Draw(terrain, new Rectangle(i * tsize, j * tsize, tsize, tsize), Color.White);
                }
            }

            for (int i = 0; i < w; i++)
            {
                
                
                
                
                
                for (int j = 0; j < h; j++)
                {
                    if (map[i][j] >= 1 && i>=1 && i<=w-3 && j>=1 && j<=h-3)
                    {
                        string tile = whichTile(j, i);
                        //string tile = tileToDraw[i][j];
                        if (tile == "None")
                        _spb.Draw(bindings[1], new Rectangle(j * sw - cx, i * sh - cy, sw, sh), Color.AliceBlue);
                        else
                        {
                            _spb.Draw(tiles[tile], new Rectangle(j * sw - cx, i * sh - cy, sw, sh), Color.AliceBlue);
                        }
                    }
                    else
                    {
                        //_spb.Draw(bindings[0], new Rectangle(j * sw - cx, i * sh - cy, sw, sh), Color.AliceBlue);
                    }
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

        public bool AreRectanglesTouching(Rectangle r1, Rectangle r2)
        {

            if (r1.X + r1.Width == r2.X)
                return true;
            if (r1.Y + r1.Height == r2.Y)
                return true;
            if (r1.X == r2.X + r2.Width)
                return true;
            if (r1.Y == r2.Y + r2.Height)
                return true;

            return false;
        }

        public void Update(Map m, int px, int py)
        {
            int y_tile = y / m.sh;
            int x_tile = x / m.sw;


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
                        Rectangle tempRect = new Rectangle(j * m.sw, i * m.sh, m.sw, m.sh);
                        if (m.map[i][j] >= 1 && (tempRect.Intersects(new Rectangle(x, ny, size, size)))) //| AreRectanglesTouching(tempRect, new Rectangle(x, ny, size, size))))
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
                    if (i < 0 | i > m.h)
                        continue;
                    for (int j = x_tile - 1; j < x_tile + 2; j++)
                    {
                        if (j < 0 | j > m.w)
                            continue;
                        Rectangle tempRect = new Rectangle(j * m.sw, i * m.sh, m.sw, m.sh);
                        if (m.map[i][j] >= 1 && (tempRect.Intersects(new Rectangle(nx, oy, size, size))))// | AreRectanglesTouching(tempRect, new Rectangle(nx, oy, size, size))))
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
        public int character = 0;
        public string currentAnimation = "idle";
        public int currentFrame = 0;
        public int pickaxeDmg = 1;
        public Texture2D spriteSheet;
        public Texture2D invSpriteSheet;

        public Dictionary<String, List<Rectangle>> spriteRects;
        public Dictionary<String, List<Rectangle>> invSpriteRects;
        public string facing = "r";
        public int lastFrameMoving = 60;
        public int tillIdle = 16;

        public Dictionary<string, int> animationTimes = new Dictionary<string, int> { };
        public Dictionary<string, int> animationFrames = new Dictionary<string, int> { };
        public List<string> animationNames = new List<string> { "idle", "run", "sword_idle", "sword_run", "sword_jump", "sword_falling", "sword_use_item", "sword_attack", "take_hit", "death" };
        public Dictionary<string, Texture2D> loadedAnimations = new Dictionary<string, Texture2D> {};



        public Player(Texture2D recTex, Texture2D bulletTex, Dictionary<String,List<Rectangle>> rects, Dictionary<String, List<Rectangle>> INVrects)
        {
            rectTexture = recTex;
            weapons.Add(new Weapon(25, 12000, bulletTex));
            animationTimes["char_0!death"] = 150;
            animationTimes["char_0!sword_idle"] = 45;
            animationTimes["char_0!run"] = 30;
            animationTimes["char_0!sword_run"] = 30;


            spriteRects = rects;
            invSpriteRects = INVrects;
            
        }

        public void Update(Map m, int ex, int ey)
        {
            lastFrameMoving += 1;
            
            if (lastFrameMoving <= tillIdle)
            {
                currentAnimation = "sword_run";
            }
            else
            {
                currentAnimation = "sword_idle";
            }
            if (lastFrameMoving >= 900)
            {
                // currentAnimation = "death";
            }
            currentFrame += 1;
            currentFrame = currentFrame % animationTimes[$"char_0!{currentAnimation}"];
            for (int i = 0; i < weapons.Count; i++)
            {
                weapons[i].Update(x,y,size, ex, ey);
            }
            int y_tile = y / m.sh;
            int x_tile = x / m.sw;


            int ny = y;
            int nx = x;
            int oy = y;
            int ox = x;
            int ncx = cx;
            int ncy = cy;

            bool flag = false;

            if (Inputs.importantKeys["w"])
            {
                flag = true;
                ny -= baseSpeed;
                ncy -= baseSpeed;
                lastFrameMoving = 0;
            }
            if (Inputs.importantKeys["s"])
            {
                ny += baseSpeed;
                ncy += baseSpeed;
                flag = true;
                lastFrameMoving = 0;
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
                        // m.map[i][j] -= 3;
                        Rectangle tempRect = new Rectangle(j * m.sw, i * m.sh, m.sw, m.sh);
                        if (m.map[i][j] >= 1 && tempRect.Intersects(new Rectangle(ox, ny, size, size)))
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
            flag = false;
            if (Inputs.importantKeys["a"])
            {
                flag = true;
                nx -= baseSpeed;
                ncx -= baseSpeed;
                lastFrameMoving = 0;
                facing = "l";
            }
            if (Inputs.importantKeys["d"])
            {
                flag = true;
                nx += baseSpeed;
                ncx += baseSpeed;
                lastFrameMoving = 0;
                facing = "r";
            }
            
            if (nx != x)
            {
                for (int i = y_tile - 1; i < y_tile + 2; i++)
                {
                    if (i < 0 | i > m.h)
                        continue;
                    for (int j = x_tile - 1; j < x_tile + 2; j++)
                    {
                        if (j < 0 | j > m.w)
                            continue;
                        Rectangle tempRect = new Rectangle(j * m.sw, i * m.sh, m.sw, m.sh);
                        if (m.map[i][j] >= 1 && tempRect.Intersects(new Rectangle(nx,oy,30,30)))
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

            int img = Convert.ToInt32((Convert.ToDecimal(currentFrame) / Convert.ToDecimal(animationTimes[$"char_0!{currentAnimation}"])) * spriteRects[currentAnimation].Count -1);
            if (img < 0)
                img = 0;
            if (facing == "r")
            {
                _spb.Draw(spriteSheet, new Vector2(x - cx - 55, y - cy - 95), spriteRects[currentAnimation][img], Color.White);
            }
            else
            {
                _spb.Draw(invSpriteSheet, new Vector2(x - cx - 55, y - cy - 95), invSpriteRects[currentAnimation][img], Color.White);
            }
            //_spb.Draw(rectTexture, new Rectangle(x-cx, y-cy, size, size), Color.White);
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
        public Texture2D sel;
        public Texture2D nsel;
        public Color rectColor;
        public Color textColor;
        public string text;
        public string mode;
        public Button(Rectangle rect, string text, Texture2D sel, Texture2D nsel, Color rectColor, Color textColor)
        {
            this.rect = rect;
            this.text = text;
            this.sel = sel;
            this.nsel = nsel;
            this.rectColor = rectColor;
            this.textColor = textColor;
            this.mode = "nsel";
          
        }
        public void Draw(SpriteBatch _spb)
        {
            if (mode=="sel")
            _spb.Draw(this.sel, this.rect, this.rectColor);
            if (mode == "nsel")
            _spb.Draw(this.nsel, this.rect, this.rectColor);

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
        public Texture2D main_background;
        public Texture2D game_text;
        public Texture2D terrain;

        public int W = 1920;
        public int H = 1080;


        float alpha;
        float fadeSpeed;
        bool isFadingIn;


        public SurvivorMonogame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            alpha = 0f;
            fadeSpeed = 0.02f; // Adjust fade speed as needed
            isFadingIn = true;
        }

        public int findNearestEnemy(int x, int y)
        {
            int bestDist = int.MaxValue;
            int bestIndex = -1;
            for (int i = 0; i < enemies.Count; i++)
            {
                int dist = Math.Abs(x - enemies[i].x) + Math.Abs(y - enemies[i].y);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestIndex = i;
                }
            }
            return bestIndex;
        }

        public Vector2 FindSpawn(Map m1)
        {
            int x = -1;
            int y = -1;
            while (true)
            {
                x = RNG.Next(1, m1.w/4 - 1);
                y = RNG.Next(1, m1.h/4 - 1);
                if (m1.map[y][x] == 0)
                {
                    return new Vector2(x, y);
                }
            }
        }

        public Dictionary<string, List<Rectangle>> LoadSpriteSheet()
        {
            Dictionary<String, List<Rectangle>> sprites = new Dictionary<string, List<Rectangle>> { };
            List<string> names = new List<string> { "idle", "run","sword_idle","sword_run","sword_jump","sword_falling","sword_use_item","sword_attack","take_hit","death", "death", "death" };
            List<int> lenghts = new List<int> {6,8,6,8,3,5,10,10,5,12,12,6};
            
            for (int i = 0; i < names.Count; i++)
            {
                sprites[names[i]] = new List<Rectangle> { };
            }
            for (int y = 0; y < names.Count; y++)
            {
                for (int x = 0; x < lenghts[y]; x++)
                {
                    sprites[names[y]].Add(new Rectangle(x * 128, y * 128, 128, 128));
                }
            }

            return sprites;
        }

        public Dictionary<string, List<Rectangle>> LoadInvertedSpriteSheet()
        {
            Dictionary<String, List<Rectangle>> sprites = new Dictionary<string, List<Rectangle>> { };
            List<string> names = new List<string> { "idle", "run", "sword_idle", "sword_run", "sword_jump", "sword_falling", "sword_use_item", "sword_attack", "take_hit", "death", "death", "death" };
            List<int> lenghts = new List<int> { 6, 8, 6, 8, 3, 5, 10, 10, 5, 12, 12, 6 };

            for (int i = 0; i < names.Count; i++)
            {
                sprites[names[i]] = new List<Rectangle> { };
            }
            for (int y = 0; y < names.Count; y++)
            {
                for (int x = 1; x < lenghts[y]+1; x++)
                {
                    sprites[names[y]].Add(new Rectangle((12-x) * 128-1, y * 128, 128, 128));
                }
            }

            return sprites;
        }

        public static List<TilePattern> loadPatterns()
        {
            List<TilePattern> patterns = new List<TilePattern> { };

            List<List<int>> template = new List<List<int>>()
            {
                new List<int>{0,0,0},
                new List<int>{0,1,0},
                new List<int>{0,0,0}

            };

            List<List<int>> p;
            TilePattern tp;
            
            p = new List<List<int>>()
            {
                new List<int>{0,0,0},
                new List<int>{0,1,0},
                new List<int>{0,0,0}

            };
            tp = new TilePattern(p, "solo");
            patterns.Add(tp);
            p = new List<List<int>>()
            {
                new List<int>{2,0,2},
                new List<int>{1,1,1},
                new List<int>{2,0,2}

            };
            tp = new TilePattern(p, "single_horizontal");
            patterns.Add(tp);
            p = new List<List<int>>()
            {
                new List<int>{2,1,2},
                new List<int>{0,1,0},
                new List<int>{2,1,2}

            };
            tp = new TilePattern(p, "single_vertical");
            patterns.Add(tp);
            p = new List<List<int>>()
            {
                new List<int>{0,0,2},
                new List<int>{0,1,1},
                new List<int>{2,1,2}

            };
            tp = new TilePattern(p, "-1,-1");
            patterns.Add(tp);

            p = new List<List<int>>()
            {
                new List<int>{2,0,0},
                new List<int>{1,1,0},
                new List<int>{2,1,2}

            };
            tp = new TilePattern(p, "1,-1");
            patterns.Add(tp);
            p = new List<List<int>>()
            {
                new List<int>{2,0,2},
                new List<int>{1,1,1},
                new List<int>{2,1,2}

            };
            tp = new TilePattern(p, "0,-1");
            patterns.Add(tp);
            p = new List<List<int>>()
            {
                new List<int>{2,1,2},
                new List<int>{1,1,1},
                new List<int>{2,0,2}

            };
            tp = new TilePattern(p, "0,1");
            patterns.Add(tp);
            p = new List<List<int>>()
            {
                new List<int>{2,1,2},
                new List<int>{1,1,0},
                new List<int>{2,1,2}

            };
            tp = new TilePattern(p, "1,0");
            patterns.Add(tp);
            p = new List<List<int>>()
            {
                new List<int>{2,1,2},
                new List<int>{0,1,1},
                new List<int>{2,1,2}

            };
            tp = new TilePattern(p, "-1,0");
            patterns.Add(tp);
            p = new List<List<int>>()
            {
                new List<int>{2,1,2},
                new List<int>{0,1,1},
                new List<int>{0,0,2}

            };
            tp = new TilePattern(p, "-1,1");
            patterns.Add(tp);
            p = new List<List<int>>()
            {
                new List<int>{2,1,2},
                new List<int>{1,1,2},
                new List<int>{2,0,0}

            };
            tp = new TilePattern(p, "1,1");
            patterns.Add(tp);

            p = new List<List<int>>()
            {
                new List<int>{2,0,2},
                new List<int>{0,1,0},
                new List<int>{2,1,2}

            };
            tp = new TilePattern(p, "vertical_end_up");
            patterns.Add(tp);

            p = new List<List<int>>()
            {
                new List<int>{2,1,2},
                new List<int>{0,1,0},
                new List<int>{2,0,2}

            };
            tp = new TilePattern(p, "vertical_end_down");
            patterns.Add(tp);

            p = new List<List<int>>()
            {
                new List<int>{2,0,2},
                new List<int>{1,1,0},
                new List<int>{2,0,2}

            };
            tp = new TilePattern(p, "horizontal_end_right");
            patterns.Add(tp);

            p = new List<List<int>>()
            {
                new List<int>{2,0,2},
                new List<int>{0,1,1},
                new List<int>{2,0,2}

            };
            tp = new TilePattern(p, "horizontal_end_left");
            patterns.Add(tp);


            return patterns;
        }

        public Dictionary<string, Texture2D> buttonTexs = new Dictionary<string, Texture2D> { };
        protected override void Initialize()
        {
            Inputs.init();
            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = W;
            _graphics.PreferredBackBufferHeight = H;
            _graphics.ApplyChanges();


            Texture2D grass = new Texture2D(GraphicsDevice, 1, 1);
            grass.SetData(new Color[] { Color.LawnGreen });

            Texture2D red = new Texture2D(GraphicsDevice, 1, 1);
            red.SetData(new Color[] { Color.Red });


            Texture2D playerRect = new Texture2D(GraphicsDevice, 1, 1);
            playerRect.SetData(new Color[] { Color.ForestGreen });

            map1 = new Map(100, 100);
            List<TilePattern> tilePatterns = loadPatterns();
            map1.patterns = tilePatterns;
            map1.bindings[0] = grass;
            map1.bindings[1] = startRect;

            Dictionary<String, List<Rectangle>> pSprites = LoadSpriteSheet();
            Dictionary<String, List<Rectangle>> InvPSprites = LoadInvertedSpriteSheet();

            
            









            player = new Player(playerRect, red, pSprites, InvPSprites);
            Vector2 pPos = FindSpawn(map1);
            player.x = Convert.ToInt32(pPos.X) * map1.sw;// + map1.size / 2;
            player.cx = Convert.ToInt32(pPos.X) * map1.sw - 250;
            player.y = Convert.ToInt32(pPos.Y) * map1.sh;// + map1.size / 2;
            player.cy = Convert.ToInt32(pPos.Y) * map1.sh - 250;
            player.bulletTex = red;
            Texture2D spritesheet = Content.Load<Texture2D>("Jotem spritesheet");
            Texture2D invspritesheet = Content.Load<Texture2D>("JotemInverted");

            player.spriteSheet = spritesheet;
            player.invSpriteSheet = invspritesheet;

            Dictionary<string, Texture2D> m_tiles = new Dictionary<string, Texture2D> { };
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    m_tiles[$"{i},{j}"] = Content.Load<Texture2D>($"{i},{j}");

                }
            }
            List<string> special_cave = new List<string> {"single_horizontal","single_vertical","solo","black","horizontal_end_left","horizontal_end_right","vertical_end_up","vertical_end_down"};
            for (int i = 0; i < special_cave.Count; i++)
            {
                m_tiles[special_cave[i]] = Content.Load<Texture2D>(special_cave[i]);    
            }



            map1.tiles = m_tiles;

            for (int i = 0; i < 15; i++)
            {
                Vector2 enemyPos = FindSpawn(map1);

                enemies.Add(new Enemy(Convert.ToInt32(enemyPos.X)*map1.sw, Convert.ToInt32(enemyPos.Y)*map1.sh, red));
            }
            List<string> names = new List<string> { "play", "quit", "shop" };
            foreach (string i in names)
            {
                buttonTexs[$"sel_{i}"] = Content.Load<Texture2D>($"sel_{i}");
                buttonTexs[$"nsel_{i}"] = Content.Load<Texture2D>($"nsel_{i}");


            }



            Button playButton = new Button(new Rectangle(800,400,200,100), "Play", buttonTexs["sel_play"], buttonTexs["nsel_play"], Color.DarkGray, Color.DarkGoldenrod);

            Button shopButton = new Button(new Rectangle(800, 550, 200, 100), "Shop", buttonTexs["sel_shop"], buttonTexs["nsel_shop"], Color.DarkGray, Color.DarkGoldenrod);

            Button quitButton = new Button(new Rectangle(800, 700, 200, 100), "Quit", buttonTexs["sel_quit"], buttonTexs["nsel_quit"], Color.DarkGray, Color.DarkGoldenrod);


            buttons.Add(playButton);
            buttons.Add(shopButton);
            buttons.Add(quitButton);


            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            main_background = Content.Load<Texture2D>("CaveBasic");
            game_text = Content.Load<Texture2D>("TheDepths");
            terrain = Content.Load<Texture2D>("terrain");
            map1.terrain = terrain;
            
            


        }
        public double y_glide = 0;
        protected override void Update(GameTime gameTime)
        {
            Inputs.Update();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (screen == "main")
            {
                for (int i = 0; i < buttons.Count; i++)
                {
                    if (Inputs.Hovering(buttons[i].rect))
                    {
                        buttons[i].mode = "sel";
                        if (Inputs.LeftClicked)
                        {
                            string txt = buttons[i].text;
                            if (txt == "Play")
                            {
                                screen = "game";
                            }
                        }
                    }
                    else
                    {
                        buttons[i].mode = "nsel";
                    }
                }
                if (isFadingIn)
                {
                    alpha += fadeSpeed;
                    if (alpha >= 1f)
                    {
                        alpha = 1f;
                        isFadingIn = false; // Stop fading in once fully visible
                    }
                }
                y_glide += 0.2;
            }

            if (screen == "game")
            {
                int enemyToTarget = findNearestEnemy(player.x, player.y);
                player.Update(map1, enemies[enemyToTarget].x, enemies[enemyToTarget].y);
                for (int i = 0; i < enemies.Count; i++)
                {
                    enemies[i].Update(map1, player.x, player.y);
                }

                foreach (Weapon weapon in player.weapons)
                {
                    int rmved = 0;
                    for (int i = 0; i < weapon.bullets.Count-rmved; i++)
                    {

                        for (int j = 0; j < enemies.Count; j++)
                        {
                            if (new Rectangle(enemies[j].x, enemies[j].y, enemies[j].size, enemies[j].size).Contains(new Rectangle(weapon.bullets[i].x, weapon.bullets[i].y, weapon.bullets[i].size, weapon.bullets[i].size)))
                            {
                                enemies[j].health -= weapon.bullets[i].dmg;
                            }
                            if (enemies[j].health <= 0)
                            {
                                Vector2 enemyPos = FindSpawn(map1);

                                enemies.Add(new Enemy(Convert.ToInt32(enemyPos.X) * map1.sw, Convert.ToInt32(enemyPos.Y) * map1.sh, enemies[j].rectTexture));
                                enemies.RemoveAt(j);

                            }
                        }

                        int mx = weapon.bullets[i].x / map1.sw;
                        int my = weapon.bullets[i].y / map1.sh;
                        if (mx < 0 | mx > map1.w | my < 0 | my > map1.h)
                        {

                        }
                        else
                        {
                            if (map1.map[my][mx] > 0)
                            {
                                map1.map[my][mx] -= weapon.bullets[i].dmg;
                                weapon.bullets.RemoveAt(i);
                                i--;
                                rmved += 1; 
                            }
                        }

                    }
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
                _spriteBatch.Draw(main_background, new Rectangle(0, 0, W, H), Color.White);
                _spriteBatch.Draw(game_text, new Rectangle(W / 2 - game_text.Width, H/2-game_text.Height*4-Math.Min(Convert.ToInt32(y_glide),30), game_text.Width*2, game_text.Height*2), Color.White * alpha);
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
                    //enemies[i].Draw(_spriteBatch, player.cx, player.cy);
                }
            
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
