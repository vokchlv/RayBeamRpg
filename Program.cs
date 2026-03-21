using System.Numerics;
using Raylib_cs;

namespace IdleGame;

internal static class Program
{

    private const int Width = 1200;
    private const int Height = 450;
    private const int TargetFps = 60;
    
    public static void Main()
    {
        Raylib.InitWindow(Width, Height, "Hello World");
        Raylib.SetTargetFPS(TargetFps);
        Vector2 position = new(350.0f, 280.0f);
        Player p = new();
        p.InitPlayer(position, TargetFps);

        while (!Raylib.WindowShouldClose())
        {
            
            p.PlayerMovement();

            // Draw
            
            Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.RayWhite);
                p.DrawPlayer();
            Raylib.EndDrawing();
        }

    }
}
