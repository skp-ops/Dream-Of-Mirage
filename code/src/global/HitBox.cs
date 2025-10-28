using Godot;

// 添加 [GlobalClass] 属性
[GlobalClass]
public partial class HitBox : Area2D
{
    // 可以在这里定义属性、方法和信号等
    [Export]
    public int Health { get; set; } = 100;

    public override void _Ready()
    {
        GD.Print("HitBox is ready!");
    }

    // ... 其他代码 ...
}