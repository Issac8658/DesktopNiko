using Godot;
using System;
using System.Linq;

public partial class Line2DAnimator : Line2D
{
    [Export]
    public Texture2D[] Sprites;
    [Export]
    public double WaitTime = 0.5;

    private int _currentFrame = 0;
    private double _currentTime = 0.0;

    public override void _Process(double delta)
    {
        _currentTime += delta;
        if (_currentTime >= WaitTime)
        {
            _currentFrame++;
            _currentTime = 0;
        }
        if (_currentFrame > Sprites.Length - 1)
        {
            _currentFrame = 0;
        }
        Texture = Sprites[_currentFrame];
    }
}
