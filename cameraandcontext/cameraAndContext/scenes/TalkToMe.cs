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
    public Node3D ActualTarget;
    public float fov = 40f;
    public float distance = 0;
    public override void _Process(double delta){
       
        distance =  (GlobalPosition - Friend.GlobalPosition).Length();
        if(distance < MaxDistance){
            
            CameraController.CurrentTarget = ActualTarget;
            ActualTarget.GlobalPosition = GlobalPosition.Lerp(Friend.GlobalPosition, Math.Clamp(distance/MaxDistance,0f,1f));
            //CameraController.ActualCamera.Fov =  CameraController.ActualCamera.Fov  + ( fov - CameraController.ActualCamera.Fov ) * (float)delta;
            CameraController.TargetInfo = this;
        }
        else if(distance > MaxDistance && distance < MaxDistance + 1){
            CameraController.CurrentTarget = CameraController.PlayerTarget;
        }
 //GD.Print(distance);
    }



}
