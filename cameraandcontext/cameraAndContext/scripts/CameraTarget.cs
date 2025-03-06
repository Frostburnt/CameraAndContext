using Godot;
using System;
using System.Runtime;
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
[Export]
Node3D MaxCamera;
[Export]
public Camera3D ActualCamera;
float yRot, xRot;
[Export]
TalkToMe TargetInfo;
[Export]
Node3D LastCameraPos;
public float DialogueFov = 10f;
private float timer = 0.0f;

    public override void _Ready(){
        
    }


    public override void _Process(double Delta){    //dynamic camera

        
        //this will be a look target for whisker raycast
        //aswell as just a target for the camera to look at
        //two states following player and hint. in an actual game this could be expanded more
        
        if(CurrentTarget == PlayerTarget){
            //Position = CurrentTarget.Position;
            GD.Print(Position);
            timer = Math.Clamp(timer -=(float)Delta * 0.5f, 0, 1);
            //following the player camera position will be relative to this, maybe interporlated on a curve
            //slerp angle towards character velocity vector pass this to camera 
            //add a max vertical clamp, maybe changed based on the normal vector under the players feet
            //maybe increase FOV based on distance?
            // if(ray1.IsColliding() || ray2.IsColliding()){//camera obscurance avoidance left
            //     GD.Print("Left");
            // }
            // else if( ray4.IsColliding()|| ray5.IsColliding()) {//camera obscurance avoidance right
            //     GD.Print("Right");
            // }
            
            // else{
            interpolatePosition();

            // }+
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
                obstacleAvoidance();
            }
            avoidCameraClipping();
            
        }
        else{
            Position = CurrentTarget.Position;
            timer = Math.Clamp(timer +=(float)Delta * 0.1f, 0, 1);
            avoidCameraClipping(false);
            interpolatePositionWeightBased(timer);
            UDPivot.Rotation = new Vector3 (TargetInfo.ActualTarget.Rotation.X, UDPivot.Rotation.Y, UDPivot.Rotation.Z);
            // float intAmount = Math.Clamp((TargetInfo.MaxDistance - TargetInfo.distance), 0, 1);
            // ActualCamera.GlobalPosition = LastCameraPos.GlobalPosition.Slerp(CurrentTarget.GlobalPosition, intAmount);
            // ActualCamera.GlobalRotation = LastCameraPos.GlobalRotation.Slerp(CurrentTarget.GlobalRotation, intAmount);
            //Pivot.Transform = Pivot.Transform.InterpolateWith();


            //Pivot.GlobalRotation = Pivot.GlobalRotation.Slerp(CurrentTarget.GlobalRotation, (float)Delta);
            //avoidCameraClipping();
            //ActualCamera.Fov =  ActualCamera.Fov + (DialogueFov - ActualCamera.Fov) * (float)Delta * 5;
            
            //ActualCamera.Fov = fov;
            
        }
        
        
        void noOrbit(){
            //float oldX = Rotation.X; //want to stop it from 
            GlobalBasis = PCPivot.GlobalBasis;
            
            float qWeight = InterpolationRate.Sample(Pivot.Quaternion.AngleTo(Quaternion)) * qOffset* (float)Delta;//replace this.quaternion with a new one based on the velocity vector
            Pivot.Quaternion = Pivot.Quaternion.Slerp(Quaternion, qWeight);
            obstacleAvoidance();

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
        void interpolatePositionWeightBased(float factor){
            Position = CurrentTarget.GlobalPosition;
            Vector3 a = Pivot.Position;
            Vector3 b = Position;
            GD.Print(Position);
            
            //want to move to the position fast if it's a large distance
           
            Pivot.Position = Pivot.Position.Lerp(Position,factor);

        }
        

        //else{   //static / hint
                //camera over-ride such as for cutscenes
                //this will likely be distance based instead of triggers
                //will read the properties of the current target, fov location, etc and interpolate to them
                //potentially disable certain render layers to hide stuff, if for instance entering a shop and going into a top down view then hiding the roof    
        void avoidCameraClipping(bool changeFOV = true){
            //based on ray 3, looking back at the camera.. we want to adjust our cameras distance to the pivot
            ActualCamera.Position = MaxCamera.Position;
            float colDist = -20.5f;
            if(ray3.IsColliding()){
            colDist = (ray3.GetCollisionPoint() - Pivot.GlobalPosition).Length() + ray3.TargetPosition.Y;
           
            ActualCamera.TranslateObjectLocal(new Vector3(0,0,colDist));
            //GD.Print(colDist);
            }
            if(changeFOV){
            colDist = Math.Abs(colDist)/ 20.5f;      //set it relative to 1 to change fov based on
            float fov = 80f + ( 55f - 80f) * colDist;
            GD.Print(colDist);
            ActualCamera.Fov = fov;
            }
            //lerp based on distance?



            





        }    

        void obstacleAvoidance(){
            //assume we can only have one ray collision
            //ray 3 is middle ray, this is used for checking for camera collisions
            bool found = false;
            Vector3 Obstacle = new Vector3(0,0,0);
            Transform3D Temp = Pivot.Transform;
            float mult = 1;
            
            //sweep from left to right... might change this to get the closest obstacle instead of the 5
            if(ray1.IsColliding()){
            Obstacle = ray1.GetCollisionPoint();
            found = true;
            }
            if(ray2.IsColliding()){
            Obstacle = ray2.GetCollisionPoint();
            found = true;
            mult = 2;
            }
            if(ray4.IsColliding()){
                Obstacle = ray4.GetCollisionPoint();
            found = true;
            mult = 2;
            }
            if(ray5.IsColliding()){
            found = true;
            Obstacle = ray5.GetCollisionPoint();
            }
            if(found){
                Temp = Temp.LookingAt(Obstacle);
                Pivot.Quaternion = Pivot.Quaternion.Slerp(Temp.Basis.GetRotationQuaternion(), (float)Delta * 0.3f * mult);
                Pivot.Rotation = new (0, Pivot.Rotation.Y, 0);          //pivot should only rotate on the Y axis
            }
        }
    }

}






