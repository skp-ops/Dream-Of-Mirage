using Godot;

public static class ConstVar
{
    public static readonly float GRAVITY = (float)ProjectSettings.GetSetting("physics/2d/default_gravity");
}

public static class PlayerNodeName
{
    public static readonly string ROOT = "Player";
    public static readonly string GRAPHIC = "Graphic";
    public static readonly string SPRITE2D = "Graphic/Sprite2D";
    public static readonly string HAND_CHECK = "Graphic/HandCheck";
    public static readonly string FOOT_CHECK = "Graphic/FootCheck";
    public static readonly string ANIMATION = "AnimationPlayer";
    public static readonly string CAMERA = "Camera2D";
}

public static class StateName
{
    // FSM Node Names
    public static readonly string IDLE = "Idle";
    public static readonly string RUN = "Run";
    public static readonly string JUMP = "Jump";
    public static readonly string FALL = "Fall";
    public static readonly string ONWALL = "OnWall";
}

public static class AnimationName
{
    // Player Animation Names
    public static readonly string IDLE = "idle";
    public static readonly string RUN = "run";
    public static readonly string JUMP = "jump";
    public static readonly string FALL = "fall";
    public static readonly string ONWALL = "onwall";
}

public static class TscnNodeName
{
    public static readonly string FOREST1_PHYSICS_LAYER = "PhysicsLayer";
}