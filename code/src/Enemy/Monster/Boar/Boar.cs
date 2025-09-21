using Godot;
using System;

public partial class Boar : Monster
{
    public override void _Ready()
    {
        base._Ready();
        speed = 10.0f;
    }
}

