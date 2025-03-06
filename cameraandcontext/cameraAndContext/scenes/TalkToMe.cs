using Godot;
using System;

public partial class TalkToMe : StaticBody3D{
    [Export]
    PlayerController Friend;
    [Export]
    CameraTarget CameraController;
    [Export]
    public float MaxDistance;
    [Export]
    Node3D ActualTarget;
    public float fov = 60;
    public float distance = 0;
    public override void _Process(double delta){
       
        distance =  (GlobalPosition - Friend.GlobalPosition).Length();
        if(distance < MaxDistance){
            CameraController.CurrentTarget = ActualTarget;
        }
        else if(distance > MaxDistance && distance < MaxDistance + 2){
            CameraController.CurrentTarget = CameraController.PlayerTarget;
        }
 GD.Print(distance);
    }



}
