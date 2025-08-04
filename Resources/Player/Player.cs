using Godot;
using System;

public partial class Player : CharacterBody3D
{
    [ExportGroup("Required Nodes")]
    [Export] private AnimationPlayer animationPlayerNode;
    [Export] private AnimatedSprite3D animationSprite3DNode; 
    [Export] private Sprite3D sprite3DNode;
     
    private Vector2 direction = new();
    
    private String last_known_direction ="left";
    private int runBALOnce = 0;

    public override void _Ready()
    {
        if (runBALOnce < 1)
        {
            BuildAnimationLibrary();
            runBALOnce = runBALOnce + 1;
        }
        AnimationFunction("Idle_Left");
    }

    public void BuildAnimationLibrary()// Create a new animation
    {
         SpriteFrames spriteFrames = animationSprite3DNode.SpriteFrames;
        // Example: Print all animation names for debuging purposes... 
        foreach (string animName in spriteFrames.GetAnimationNames())
        {
            GD.Print("Building Animation Library: " + animName);

            // Example: Get number of frames in "walk" animation
            int frameCount = spriteFrames.GetFrameCount(animName);
            GD.Print(animName + " has " + frameCount + " frames.");
            Texture2D[] frames = new Texture2D[frameCount];

            // Add a new animation to the animation player.
            Animation myAnim = new Animation();        
            GD.Print("Created new animation [ " + animName  + " ]");
            // Set the time length for the whole animation
            myAnim.Length = 1.0f;
            myAnim.LoopMode = Animation.LoopModeEnum.Linear;
            // I dunno what linear does yet, but it's the same as the default for the editor
            // set the frame time base don the animation lenght above, and the number of sprite frames that are being added to the animation.
            float frameTime = myAnim.Length / frameCount;

            // Add track to the animation player (just like the editor)
            int trackIdx = myAnim.AddTrack(Animation.TrackType.Value);            
            GD.Print("AddTrack [ " + animName  + " ]");

            // Set the track to use textures (just like the editor)
            myAnim.TrackSetPath(trackIdx, sprite3DNode.GetPath() + ":texture");        
            GD.Print("TrackSetPath [ " + animName  + " ]");

            // bulid the animations key frames from the selected sprite frames
            for (int i = 0; i < frameCount; i++)
            {
                Texture2D FrameAnim = spriteFrames.GetFrameTexture(animName, i);
                int trackIndex = myAnim.GetTrackCount() - 1;
                frames[i] = FrameAnim;
                myAnim.TrackInsertKey(trackIndex, i * frameTime, frames[i]);
            }            
            GD.Print("Added key frames to track. [ " + animName  + " ]");

            // Create and assign a new AnimationLibrary
            var animLibrary = new AnimationLibrary();
            animLibrary.AddAnimation(animName, myAnim);
            GD.Print("AddAnimation [ " + animName  + " ]");
            // Register the AnimationLibrary under the AnimationPlayer
            animationPlayerNode.AddAnimationLibrary(animName, animLibrary);
            GD.Print("AddAnimationLibrary [ " + animName  + " ] ");
            // animation is ready to play at a later time.            
            GD.Print("Animation Library is ready for [ " + animName  + " ] ");
            GD.Print(" **************************************************** ");
        }         

    }


    public void AnimationFunction(String AnimationLabel)// Create a new animation
    {
        Boolean FoundAnim = false;
        SpriteFrames spriteFrames = animationSprite3DNode.SpriteFrames;
        // Example: Print all animation names for debuging purposes... 
        foreach (string animName in spriteFrames.GetAnimationNames())
        { 
            if (animName == AnimationLabel)
            {
                FoundAnim = true;
                // Verifing the labeled sprite frame exists. 
            }
        }
        if (FoundAnim == true)
        {
            // Play it
            animationPlayerNode.Play(AnimationLabel + "/" + AnimationLabel);
        }
        else
        {
            GD.Print("ERROR: The animation [" + AnimationLabel + "] does not exist in sprite frames.");
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        Velocity = new(direction.X, 0, direction.Y);
        Velocity *= 5;         
        MoveAndSlide();
    } 
    public override void _Input(InputEvent @event)
    {
        direction = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBackward");
        if (direction > Vector2.Zero)
        {
            AnimationFunction("Walk_Right");
            last_known_direction = "right";
        }
        else if (direction < Vector2.Zero)
        {
            AnimationFunction("Walk_Left");
            last_known_direction = "left";
        }
        else
        {
            if (last_known_direction == "right")
            {
                AnimationFunction("Idle_Right");
            }
            else
            { 
                AnimationFunction("Idle_Left");
            }
        }
    }


}
