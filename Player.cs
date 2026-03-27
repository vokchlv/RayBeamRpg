using System.Numerics;
using Raylib_cs;

namespace IdleGame;

public class Player
{
    
    private struct Animation(Texture2D texture, int frames, int speed)
    {
        public Texture2D Texture { get; } = texture;
        public int Frames { get; } = frames;
        public int Speed { get; set; } = speed;
    }
    
    private int _currentFrame;
    private int _framesCounter;
    private int _attackFrameCounter = -1;
    
    private static bool _isWalking;
    private static bool _isAttacking;
    private Vector2 _position;
    private Rectangle _frameRec;
    private int _targetFps;

    // Animationen
    private Animation _runAnim;
    private Animation _idleAnim;
    private Animation _fightAnim;
    private Animation _currentAnim;
    
    
    // Movement
    private Vector2 _movementMatrix;
    private float _speed;
    
    public void InitPlayer(Vector2 position, int targetFps)
    {
        _position = position;
        _targetFps = targetFps;

        _runAnim = new Animation(
            Raylib.LoadTexture(Util.BuildPath("Sprites", "Warrior_Run.png")),
            6, 8);
        _idleAnim = new Animation(
            Raylib.LoadTexture(Util.BuildPath("Sprites", "Warrior_Idle.png")),
            8, 12);
        _fightAnim = new Animation(
            Raylib.LoadTexture(Util.BuildPath("Sprites", "Warrior_Attack1.png")),
            4, 8);

        _currentAnim = _idleAnim;
        
        _frameRec = new Rectangle(0.0f, 0.0f, 
            (float)_currentAnim.Texture.Width / _currentAnim.Frames,
            _currentAnim.Texture.Height);
    }

    public void PlayerAnimationHandler()
    {
        // TODO: Movementmatrix an DeltaTime anpassen (jetzt noch kein Bock lol)
        
        _movementMatrix = Vector2.Zero;
        _speed = Raylib.IsKeyDown(KeyboardKey.LeftShift) ? 6 : 2;

        if (!_isAttacking)
        {
            if (Raylib.IsKeyDown(KeyboardKey.A)) _movementMatrix.X -= _speed;
            if (Raylib.IsKeyDown(KeyboardKey.D)) _movementMatrix.X += _speed;
            if (Raylib.IsKeyDown(KeyboardKey.W)) _movementMatrix.Y -= _speed;
            if (Raylib.IsKeyDown(KeyboardKey.S)) _movementMatrix.Y += _speed;
            if (Raylib.IsKeyPressed(KeyboardKey.J)) _isAttacking = true;
        }

        // Normalisierung für diagonal
        if (_movementMatrix.X != 0 && _movementMatrix.Y != 0)
        {
            _movementMatrix = Vector2.Normalize(_movementMatrix) * _speed;
        }

        _isWalking = _movementMatrix.Length() > 0;
        _position += _movementMatrix;

        // Check next Animation
        Animation nextAnim = _isAttacking ? _fightAnim : (_isWalking ? _runAnim : _idleAnim);

        if (nextAnim.Texture.Id != _currentAnim.Texture.Id) 
        {
            _currentAnim = nextAnim;
            _currentFrame = 0;
            _framesCounter = 0;
            if (_isAttacking) _attackFrameCounter = 0;
        }

        // Blickrichtung
        float frameWidth = (float)_currentAnim.Texture.Width / _currentAnim.Frames;
        if (_movementMatrix.X < 0) _frameRec.Width = -frameWidth;
        else if (_movementMatrix.X > 0) _frameRec.Width = frameWidth;

        _frameRec.Height = _currentAnim.Texture.Height;
        
        // Frame-Counter Logik
        _framesCounter++;
        if (_framesCounter >= _targetFps / _currentAnim.Speed)
        {
            _framesCounter = 0;
            _currentFrame++;

            if (_isAttacking)
            {
                _attackFrameCounter++;
                if (_attackFrameCounter >= _currentAnim.Frames)
                {
                    _isAttacking = false;
                    _attackFrameCounter = -1;
                    _currentFrame = 0;
                }
            }
            else if (_currentFrame >= _currentAnim.Frames)
            {
                _currentFrame = 0;
            }
        }

        _frameRec.X = _currentFrame * Math.Abs(_frameRec.Width);
    }

    public void InitAttack(int direction)
    {
        //Console.WriteLine(direction);
    }

    public void DrawPlayer()
    {
        Raylib.DrawText(_framesCounter.ToString(), 10, 10, 20, Color.Black);
        Raylib.DrawTextureRec(_currentAnim.Texture, _frameRec, _position, Color.White);
    }
    
}