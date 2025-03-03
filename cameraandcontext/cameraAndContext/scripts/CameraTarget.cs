using Godot;
using System;

public partial class CameraTarget : Node3D{
[Export]
public Node3D PlayerTarget;
[Export]
public Node3D CurrentTarget;
[Export]
public Node3D Pivot;
[Export]
Curve InterpolationRate;
[Export]
public float qOffset;
[Export]
public float iOffset;
[Export]
PlayerController player;

    public override void _Process(double Delta){    //dynamic camera
        //this will be a look target for whisker raycast
        //aswell as just a target for the camera to look at
        //two states following player and hint. in an actual game this could be expanded more
        
        if(CurrentTarget == PlayerTarget){
            //following the player camera position will be relative to this, maybe interporlated on a curve
            //slerp angle towards character velocity vector pass this to camera 
            //add a max vertical clamp, maybe changed based on the normal vector under the players feet
            //maybe increase FOV based on distance?
            Vector3 a = Pivot.Position;
            Vector3 b = Position;
            Vector3 difference = a-b;
            float distance = difference.Length();
            //want to move to the position fast if it's a large distance
            float iWeight = InterpolationRate.Sample(distance ) * iOffset* (float)Delta;
            Pivot.Position = Pivot.Position.Lerp(Position, iWeight);
            //if no input
            if(player.OrbitVector.Length() < 0.001f){
                float qWeight = InterpolationRate.Sample(Pivot.Quaternion.AngleTo(Quaternion)) * qOffset* (float)Delta;//replace this.quaternion with a new one based on the velocity vector
                Pivot.Quaternion = Pivot.Quaternion.Slerp(Quaternion, qWeight);
            }
        }

        else{   //static / hint
                //camera over-ride such as for cutscenes
                //this will likely be distance based instead of triggers
                //will read the properties of the current target, fov location, etc and interpolate to them
                //potentially disable certain render layers to hide stuff, if for instance entering a shop and going into a top down view then hiding the roof    
            



        }





}


}



