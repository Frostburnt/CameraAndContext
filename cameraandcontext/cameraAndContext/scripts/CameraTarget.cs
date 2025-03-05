using Godot;
using System;
using System.Security.AccessControl;

public partial class CameraTarget : Node3D{
[Export]
public Node3D PlayerTarget;
[Export]
public Node3D CurrentTarget;
[Export]
public Node3D Pivot;
[Export]
Node3D UDPivot;
[Export]
Curve InterpolationRate;
[Export]
public Node3D PCPivot;
[Export]
public float qOffset;
[Export]
public float iOffset;
[Export]
PlayerController player;
[Export]
    RayCast3D ray1, ray2, ray3, ray4, ray5;
[Export]
float ySens, xSens;
float yRot, xRot;
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }
    public override void _Process(double Delta){    //dynamic camera
        //this will be a look target for whisker raycast
        //aswell as just a target for the camera to look at
        //two states following player and hint. in an actual game this could be expanded more
        interpolatePosition();
        if(CurrentTarget == PlayerTarget){
            //following the player camera position will be relative to this, maybe interporlated on a curve
            //slerp angle towards character velocity vector pass this to camera 
            //add a max vertical clamp, maybe changed based on the normal vector under the players feet
            //maybe increase FOV based on distance?
            if(ray1.IsColliding() || ray2.IsColliding()){//camera obscurance avoidance left
                GD.Print("Left");
            }
            else if( ray4.IsColliding()|| ray5.IsColliding()) {//camera obscurance avoidance right
                GD.Print("Right");
            }
            
            else{
                

            }
            if(player.OrbitVector.Length() > 0.03f){
                Pivot.Rotate(Vector3.Up, player.OrbitVector.X * -1 * (float)Delta * xSens);
                
                float xRot = Math.Clamp(UDPivot.GlobalRotation.X + player.OrbitVector.Y * (float)Delta * ySens, -0.3f, 0.6f);
                UDPivot.Rotation = new Vector3(xRot, 0, 0);
                interpolatePosition();
            }
            else if(player.Velocity.Length()!= 0){//no input
                noOrbit();
            }
            else{
                unHide();
            }
        }
        
        
        void noOrbit(){
            //float oldX = Rotation.X; //want to stop it from 
            GlobalBasis = PCPivot.GlobalBasis;
            
            float qWeight = InterpolationRate.Sample(Pivot.Quaternion.AngleTo(Quaternion)) * qOffset* (float)Delta;//replace this.quaternion with a new one based on the velocity vector
            Pivot.Quaternion = Pivot.Quaternion.Slerp(Quaternion, qWeight);
            unHide();

           // Rotation = newVector3(oldX, Rotation.Y, Rotation.Z);
        }
        
        
        void interpolatePosition(float factor = 1.0f){
            Position = CurrentTarget.GlobalPosition;
            Vector3 a = Pivot.Position;
            Vector3 b = Position;
            Vector3 difference = a-b;
            float distance = difference.Length();
            //want to move to the position fast if it's a large distance
            float iWeight = InterpolationRate.Sample(distance ) * iOffset* (float)Delta;
            Pivot.Position = Pivot.Position.Lerp(Position, iWeight * factor);
        }


        //else{   //static / hint
                //camera over-ride such as for cutscenes
                //this will likely be distance based instead of triggers
                //will read the properties of the current target, fov location, etc and interpolate to them
                //potentially disable certain render layers to hide stuff, if for instance entering a shop and going into a top down view then hiding the roof    
            

        void unHide(){
            //assume we can only have one ray collision
            //ray 3 is middle ray, this is used for checking for camera collisions
            bool found = false;
            Variant Obstacle = new Variant(); 
            Vector3 ObjPosition;
            if(ray1.IsColliding()){
               Obstacle = ray1.GetCollider();
               found = true;
            }
            if(ray2.IsColliding()){
                Obstacle = ray2.GetCollider();
                found = true;
            }
            if(ray4.IsColliding()){
                Obstacle = ray4.GetCollider();
                found = true;
            }
            if(ray5.IsColliding()){
                Obstacle = ray5.GetCollider();
                found = true;
            }
            if(found){
                ObjPosition = ((StaticBody3D)Obstacle).GlobalPosition;
            }
            else{
                return;
            }
            Transform3D Temp = Pivot.Transform;
            Temp = Temp.LookingAt(ObjPosition);








            GD.Print(ObjPosition);
            //then we move camera based on relative position to that object
            // if(ray1.IsColliding()){//left big
            //     Transform3D Temp = Pivot.Transform;
            //     Temp = Temp.LookingAt(Temp.Basis.Column0);
            //     Pivot.Quaternion = Pivot.Quaternion.Slerp(Temp.Basis.GetRotationQuaternion(), (float)Delta * 1.0f);
            //    //Pivot.LookAt(Pivot.Basis.X);
            //     //Pivot.Rotate(Vector3.Up, 0.418879f * (float)Delta * 4);
            // }
            // else if(ray2.IsColliding()){//left small
            //     Transform3D Temp = Pivot.Transform;
            //     Temp = Temp.LookingAt(Temp.Basis.Column0);
            //     Pivot.Quaternion = Pivot.Quaternion.Slerp(Temp.Basis.GetRotationQuaternion(), (float)Delta * 0.7f);
            // }
            // else if(ray5.IsColliding()){//right big
            //     Transform3D Temp = Pivot.Transform;
            //     Temp = Temp.LookingAt(Temp.Basis.Column0 * -1);
            //     Pivot.Quaternion = Pivot.Quaternion.Slerp(Temp.Basis.GetRotationQuaternion(), (float)Delta * 0.7f);
            // }
            // else if(ray4.IsColliding()){
            //     Transform3D Temp = Pivot.Transform;
            //     Temp = Temp.LookingAt(Temp.Basis.Column0 * -1);
            //     Pivot.Quaternion = Pivot.Quaternion.Slerp(Temp.Basis.GetRotationQuaternion(), (float)Delta * 1.0f);
            // }
        }

        


    }

}






