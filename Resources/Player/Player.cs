using Godot;
using System;

public partial class Player : CharacterBody3D
{
    [ExportGroup("Required Nodes")]
    [Export] private AnimationPlayer animationPlayerNode;
    [Export] private AnimatedSprite3D animationSprite3DNode; 
    [Export] private Sprite3D sprite3DNode;
     
    private Vector2 direction = new();

    public override void _Ready()
    {
        AnimationFunction("Idle");
    }

    public void AnimationFunction(String AnimationLabel)// Create a new animation
    {
        Boolean FoundAnim = false;
        SpriteFrames spriteFrames = animationSprite3DNode.SpriteFrames;
        // Example: Print all animation names for debuging purposes... 
        foreach (string animName in spriteFrames.GetAnimationNames())
        {
            GD.Print("Animation: " + animName);
            if (animName == AnimationLabel)
            {
                FoundAnim = true;
                // Verifing the labeled sprite frame exists.
            }
        }
        if (FoundAnim == true)
        {
            // Example: Get number of frames in "walk" animation
            int frameCount = spriteFrames.GetFrameCount(AnimationLabel);
            GD.Print(AnimationLabel + " has " + frameCount + " frames.");
            Texture2D[] frames = new Texture2D[frameCount];

            // Add a new animation to the animation player.
            Animation myAnim = new Animation();
            // Set the time length for the whole animation
            myAnim.Length = 1.0f;
            myAnim.LoopMode = Animation.LoopModeEnum.Linear;
            // I dunno what linear does yet, but it's the same as the default for the editor
            // set the frame time base don the animation lenght above, and the number of sprite frames that are being added to the animation.
            float frameTime = myAnim.Length / frameCount;

            // Add track to the animation player (just like the editor)
            int trackIdx = myAnim.AddTrack(Animation.TrackType.Value);
            // Set the track to use textures (just like the editor)
            myAnim.TrackSetPath(trackIdx, sprite3DNode.GetPath() + ":texture");

            // bulid the animations key frames from the selected sprite frames
            for (int i = 0; i < frameCount; i++)
            {
                Texture2D FrameAnim = spriteFrames.GetFrameTexture(AnimationLabel, i);
                int trackIndex = myAnim.GetTrackCount() - 1;
                frames[i] = FrameAnim;
                myAnim.TrackInsertKey(trackIndex, i * frameTime, frames[i]);
            }

            // Create and assign a new AnimationLibrary
            var animLibrary = new AnimationLibrary();
            animLibrary.AddAnimation(AnimationLabel, myAnim);

            // Register the AnimationLibrary under the AnimationPlayer
            animationPlayerNode.AddAnimationLibrary(AnimationLabel, animLibrary);

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
        AnimationFunction("Walk");
    }


}
