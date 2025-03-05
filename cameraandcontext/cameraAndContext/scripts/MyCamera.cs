using Godot;
using System;

public partial class MyCamera : Camera3D{
    [Export]
    CameraTarget MyTarget;
    
    public override void _Process(double delta)
    {
        base._Process(delta);
        if(MyTarget.CurrentTarget == MyTarget.PlayerTarget){//
            //strictly copy rotation of target, as that will have our desired camera angle
            //that is where the interpolation and calculations will happen
            //move the camera Z- local to move closer Z+ backwards
            

        }

        else{
            
        }






    }
}
