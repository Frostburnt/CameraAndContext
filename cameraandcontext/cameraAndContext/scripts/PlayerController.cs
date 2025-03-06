using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class PlayerController : CharacterBody3D{
    [Export]
    public Curve InputRampCamera; //customizable input ramp. Modulates stick input, can make camera input propotionally faster, or linear with player input
    [Export]                      
    public Curve InputRampMovement;
    [Export]
    float InputAdjustPeriod; 
    [Export]
    Curve JumpC;
    [Export]
    Node3D helper;

    
    public Vector2 InputVector = new Vector2(0.0f,0.0f);
    public Vector2 OrbitVector = new Vector2(0.0f,0.0f);
    public float MovementMagnitude = 0.0f;
    public float OrbitAmount = 0.0f;
    string jump = "Jump";
    bool CanJump = false;
    float JumpTimer = 0;
    bool isJumping = false;
    float fallTime;
    float LastValidAngle;
    public override void _Ready()
    {
        LastValidAngle = helper.GlobalRotation.Y;
        
    }
    //bool IsNotJumping;
    	//private void OnArea3dAreaEntered(Node3D body){
            
		//GD.Print("bye");
	private  void Fortnite(){
        
    }
    public override void _Process(double delta){
        //this will likely still be pretty basic
        JumpTimer += (float)delta;
        PollInput();


        Velocity = new Vector3 (InputVector.X, Velocity.Y/10, InputVector.Y) * 10;           //an acceleration based approach would be nicer
        
        this.Velocity = this.Velocity.Rotated(Vector3.Up, LastValidAngle); 
       

        Vector2 xzVelocity = new Vector2(InputVector.X, InputVector.Y);
        if(OrbitAmount > 0 || xzVelocity.Length()== 0){
            LastValidAngle = helper.GlobalRotation.Y;
        }
        if(xzVelocity.Length()!= 0){
            LookAt(Position + new Vector3 (Velocity.X, 0, Velocity.Z) * 10);       //just 1:1 the model faces where we're heading
        }
       
        
        HandleJumpInput();
        FallChecker();
        MoveAndSlide();
        //Velocity = Velocity - new Vector3 (0, Velocity.Y, 0);

    
    void FallChecker(){
			if(!this.IsOnFloor()){
				if (!isJumping){
				this.Velocity = new Vector3(this.Velocity.X, this.Velocity.Y - 8 *(float)delta, this.Velocity.Z);
				}
				fallTime = fallTime += (float)delta;
				if(fallTime > 5.0f){
					//this.Position = RespawnLocation;
					fallTime = 0.0f;
				}
			}
			else{
				fallTime = 0.0f;
				}
    }
    }
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        
    }
    public void PollInput(){
        float leftX = Input.GetJoyAxis(0, JoyAxis.LeftX);
        float leftY = Input.GetJoyAxis(0, JoyAxis.LeftY);

        MovementMagnitude = InputRampMovement.Sample(new Vector2(leftX, leftY).Length());
        InputVector = new Vector2(leftX, leftY).Normalized()*MovementMagnitude; //definitely un needed multiplication here, could have done this in another step
    

        float rightX = Input.GetJoyAxis(0, JoyAxis.RightX);
        float rightY = Input.GetJoyAxis(0, JoyAxis.RightY);
        OrbitAmount = InputRampCamera.Sample(new Vector2(rightX, rightY).Length());
        OrbitVector = new Vector2(rightX, rightY);
        


       
    }
    void HandleJumpInput(){
			if(Input.IsActionPressed("Jump") && fallTime < 0.15f && !isJumping){
				isJumping = true;
				JumpTimer = 0.0f;
			}

			if(isJumping){
			Velocity = new Vector3(this.Velocity.X, JumpC.Sample(JumpTimer) * 5, this.Velocity.Z);

			if(JumpTimer >= 1.0f){
				isJumping = false;
				JumpTimer = 0.0f;
				fallTime = 1.0f;
			}
		}
    }
    


}
