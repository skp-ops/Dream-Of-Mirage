using Godot;
using System;

// Base class for all monsters
public partial class Monster : CharacterBody2D
{
    // Backing field for exported direction
    private Direction _dir = Direction.Left;

    // Exported property with get/set so we can react to changes in editor or at runtime
    [Export]
    public Direction Dir
    {
        get => _dir;
        set
        {
            if (_dir == value) return;
            _dir = value;
            OnDirectionChanged();
        }
    }
    // [Export]
    public virtual float speed { get; set; }

    public int curHP;
    public int maxHP;
    // public float speed;
    public float acceleration;
    public readonly float gravity = ConstVar.GRAVITY;
    private Node2D graphic;

    public enum Direction { Left = -1, Right = 1 }

    public override void _Ready()
    {
        maxHP = 100;
        curHP = maxHP;
        speed = 50.0f;
        acceleration = speed / 0.1f;
        graphic = GetNode<Node2D>(EnemyNodeName.GRAPHIC);
        try
        {
            Assert.IsNoneNode<Node2D>(graphic);
        }
        catch (NullReferenceException ex)
        {
            Logger.LogError("Node is null: " + ex.Message);
        }
        // Ensure initial facing logic is applied
        OnDirectionChanged();

    }

    public override void _Process(double delta)
    {
    }

    public override void _PhysicsProcess(double delta)
    {
        // Basic movement logic (can be overridden by subclasses)
        MoveAndSlide();
    }

    // Method to apply damage to the monster
    public void ApplyDamage(int damage)
    {
        curHP -= damage;
        if (curHP <= 0)
        {
            Die();
        }
    }

    // Method to handle monster death
    private void Die()
    {
        QueueFree(); // Remove the monster from the scene
    }

    // Hook invoked whenever direction changes (editor or runtime). Subclasses can override.
    protected virtual void OnDirectionChanged()
    {
        graphic.Scale = new Vector2((int)Dir * -1, 1);
    }

    // Optional explicit accessors if you prefer method style
    public Direction GetDirection() => Dir;
    public void SetDirection(Direction d) => Dir = d;
}
