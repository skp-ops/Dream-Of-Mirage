using Godot;

// 添加 [GlobalClass] 属性
[GlobalClass]
public partial class HurtBox : Area2D
{
    // 可以在这里定义属性、方法和信号等
    [Export]
    public int Damage { get; set; } = 10;

    public override void _Ready()
    {
        GD.Print("HurtBox is ready!");
    }

    // ... 其他代码 ...
}