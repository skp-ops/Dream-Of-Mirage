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
    public virtual float Speed { get; set; }

    // Health properties
    [Export]
    public int MaxHP { get; set; }

    private int _curHP;
    [Export]
    public int CurHP
    {
        get => _curHP;
        set => _curHP = Mathf.Clamp(value, 0, MaxHP);
    }

    public readonly float gravity = ConstVar.GRAVITY;
    private Node2D graphic;
    private HurtBox hurtBox;

    public enum Direction { Left = -1, Right = 1 }

    public override void _Ready()
    {
        // Initialize current HP
        CurHP = MaxHP;

        graphic = GetNode<Node2D>(EnemyNodeName.GRAPHIC);
        try
        {
            Assert.IsNoneNode<Node2D>(graphic);
        }
        catch (NullReferenceException ex)
        {
            Logger.LogError("Node is null: " + ex.Message);
        }

        // Find and bind HurtBox (search recursively in children)
        hurtBox = FindChildByType<HurtBox>(this);
        if (hurtBox != null)
        {
            hurtBox.Hurt += OnHurt;
        }
        else
        {
            Logger.LogWarning($"HurtBox not found for {Name}");
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

    protected virtual void OnHurt(HitBox hitBox)
    {
        return;
    }

    public void TakeDamage(int damage)
    {
        CurHP -= damage;
        if (CurHP <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        return;
    }

    // Hook invoked whenever direction changes (editor or runtime). Subclasses can override.
    protected virtual void OnDirectionChanged()
    {
        graphic.Scale = new Vector2((int)Dir * -1, 1);
    }

    // Optional explicit accessors if you prefer method style
    public Direction GetDirection() => Dir;
    public void SetDirection(Direction d) => Dir = d;

    // Helper method to find child node by type recursively
    protected T FindChildByType<T>(Node parent) where T : class
    {
        foreach (Node child in parent.GetChildren())
        {
            if (child is T typedChild)
            {
                return typedChild;
            }
            var result = FindChildByType<T>(child);
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }
}
