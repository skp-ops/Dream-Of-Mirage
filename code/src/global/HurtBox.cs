using Godot;

// 添加 [GlobalClass] 属性
[GlobalClass]
public partial class HurtBox : Area2D
{
    [Signal]
    public delegate void HurtEventHandler(HitBox hitBox);

    public override void _Ready()
    {
    }


}