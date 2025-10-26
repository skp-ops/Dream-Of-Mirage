using Godot;

public static class ConstVar
{
    public static readonly float GRAVITY = (float)ProjectSettings.GetSetting("physics/2d/default_gravity") * 0.8f;
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

public static class EnemyNodeName
{
    public static readonly string ROOT = "Enemy";
    public static readonly string GRAPHIC = "Graphic";
    public static readonly string SPRITE2D = "Graphic/Sprite2D";
    public static readonly string ANIMATION = "AnimationPlayer";
    public static readonly string WALL_CHECK = "Graphic/WallCheck";
    public static readonly string FLOOR_CHECK = "Graphic/FloorCheck";
    public static readonly string PLAYER_CHECK = "Graphic/PlayerCheck";
}

public static class StateName
{
    // FSM Node Names
    public static readonly string IDLE = "Idle";
    public static readonly string RUN = "Run";
    public static readonly string WALK = "Walk";
    public static readonly string JUMP = "Jump";
    public static readonly string FALL = "Fall";
    public static readonly string ONWALL = "OnWall";
    public static readonly string ATTACK = "Attack";
    public static readonly string HIT = "Hit";
    public static readonly string ATTACK_1 = "Attack_1";
    public static readonly string ATTACK_2 = "Attack_2";
    public static readonly string ATTACK_3 = "Attack_3";
}

public static class AnimationName
{
    /*
    common animation names, the actual names should match the ones in AnimationPlayer nodes
    */
    public static readonly string IDLE = "idle";
    public static readonly string RUN = "run";
    public static readonly string WALK = "walk";
    public static readonly string JUMP = "jump";
    public static readonly string FALL = "fall";
    public static readonly string ONWALL = "onwall";
    public static readonly string ATTACK = "attack";
    public static readonly string HIT = "hit";
}

public static class AnimationSpecialName
{
    /*
    special animation names for specific enemies or players
    OWNER_ANIMATION
    */
    public static readonly string PLAYER_ATTACK_1 = "player_attack_1";
    public static readonly string PLAYER_ATTACK_2 = "player_attack_2";
    public static readonly string PLAYER_ATTACK_3 = "player_attack_3";
}

public static class TscnNodeName
{
    public static readonly string FOREST1_PHYSICS_LAYER = "PhysicsLayer";
}