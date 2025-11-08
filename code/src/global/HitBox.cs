using Godot;

// 添加 [GlobalClass] 属性
[GlobalClass]
public partial class HitBox : Area2D
{
    [Signal]
    public delegate void HitEventHandler(HurtBox hurtBox);

    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
    }

    public void OnAreaEntered(Area2D area)
    {
        if (area is HurtBox hurtBox)
        {
            EmitSignal(SignalName.Hit, hurtBox);
            // 打印 hitbox 的owner 和 hurtbox 的 owner 名称以便调试
            GD.Print("HitBox Owner: " + this.GetOwner().Name + ", HurtBox Owner: " + hurtBox.GetOwner().Name);
            hurtBox.EmitSignal(HurtBox.SignalName.Hurt, this);
        }
    }


}