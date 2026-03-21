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
    private int _tempSaveFrame;
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

    public void PlayerMovement()
    {
        // TODO: Movementmatrix an DeltaTime anpassen (jetzt noch kein Bock lol)
        
        _movementMatrix = Vector2.Zero;
        _speed = 2;
        int currentAnimSpeed = 8;

        if (Raylib.IsKeyDown(KeyboardKey.LeftShift))
        {
            _speed = 6;
            currentAnimSpeed = 12;
        }

        if (!_isAttacking)
        {
            if (Raylib.IsKeyDown(KeyboardKey.A)) _movementMatrix.X -= _speed;
            if (Raylib.IsKeyDown(KeyboardKey.D)) _movementMatrix.X += _speed;
            if (Raylib.IsKeyDown(KeyboardKey.W)) _movementMatrix.Y -= _speed;
            if (Raylib.IsKeyDown(KeyboardKey.S)) _movementMatrix.Y += _speed;
            if (Raylib.IsKeyPressed(KeyboardKey.J)) _isAttacking = true;
        }

        
        if (_movementMatrix.X != 0 && _movementMatrix.Y != 0)
        {
            _movementMatrix.X = (float) (_movementMatrix.X / Math.Sqrt(2));
            _movementMatrix.Y = (float) (_movementMatrix.Y / Math.Sqrt(2));
        }

        _isWalking = _movementMatrix.ToScalar() != 0;
        if (_movementMatrix.X != 0) _frameRec.Width = Math.Sign(_movementMatrix.X) * Math.Abs(_frameRec.Width);
        
        _position.X += _movementMatrix.X;
        _position.Y += _movementMatrix.Y;

        // Check next Animation
        Animation nextAnim;
        switch (_isAttacking)
        {
            // Animation-Wechsel
            case false:
            {
                nextAnim = _isWalking ? _runAnim : _idleAnim;
                break;
            }
            case true:
                nextAnim = _fightAnim;
                if (_attackFrameCounter == -1)
                {
                    _attackFrameCounter = 0;
                    _tempSaveFrame = _currentFrame;
                    _currentFrame = _attackFrameCounter;
                    InitAttack((int)_frameRec.Width);
                }
                break;
        }

        if (nextAnim.Texture.Id != _currentAnim.Texture.Id) 
        {
            _currentAnim = nextAnim;
            _currentFrame = 0;
            _framesCounter = 0;
            
            _frameRec.Width = Math.Sign(_frameRec.Width) * ((float)_currentAnim.Texture.Width / _currentAnim.Frames);
            _frameRec.Height = _currentAnim.Texture.Height;
        }

        if (_movementMatrix.X != 0) 
            _frameRec.Width = Math.Sign(_movementMatrix.X) * Math.Abs(_frameRec.Width);
        
        _position += _movementMatrix;
        
        // Updater
        _framesCounter++;
        if (_framesCounter < _targetFps / _currentAnim.Speed) return;
        _framesCounter = 0;
        _currentFrame++;
        if (_attackFrameCounter != -1) _attackFrameCounter++;

        Console.WriteLine(_currentFrame);
        
        if (_currentFrame >= _currentAnim.Frames) _currentFrame = 0;

        _frameRec.X = _currentFrame * Math.Abs(_frameRec.Width);
        if (_attackFrameCounter != 4) return;
        _isAttacking = false;
        _attackFrameCounter = -1;
        _currentFrame = _tempSaveFrame;
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