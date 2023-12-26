# Dialogue System
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/07a9391f2e08430c86e6fde5e45464ff)](https://app.codacy.com?utm_source=gh&utm_medium=referral&utm_content=&utm_campaign=Badge_grade)

Dialogue system by Marcus Aas Jensen. 

This dialogue system is a versatile tool integrated to Unity's engine to build story based interactions in a video game.

# Demo

https://github.com/marcusaasjensen/dialogue-system/assets/88457743/e2b22499-79bd-46cd-b5c0-79d5cdd33868


# Features
What's possible to create with this dialogue system:
- Create branching dialogues with dialogue choices using the integrated Dialogue Graph Editor.
- Dialogue path save system.
- Have different characters in a dialogue with each the possibility to change sprite, voice (sound, pitch), reactions, and emotion.
- Text typer with control over typing pace in play mode.
- Character speaker to control current character's "voice" (one sound repeated) with typer synchronization and default pace.
- Add multiple "command" **tags** to different text of the dialogue to have more control over storytelling.
- Workflow based on scriptable objects (save & load).

> [!IMPORTANT]
> Software and packages:
> 
> - **Unity** 2022.3.0f1
> 
> - **TextMeshPro** 3.0.6 (dialogue UI's text)

# Dialogue Graph Editor

<img width="1039" alt="branching" src="https://github.com/marcusaasjensen/dialogue-system/assets/88457743/ac28e200-bd49-40a4-91d3-9c62c258ffa7">

Open the Dialogue Graph Editor window in the top menu *"Graph > Dialogue Graph Editor"*.

You will see a blueprint with a menu at the top. The only existing node is the *"START"* node which is the node that directs where the dialogue should start.

## Simple Nodes
A simple node is a visual representation of a dialogue composed of a list of messages. It can start from other nodes and you can connect it to a single other dialogue node.

<img width="155" alt="simple_node" src="https://github.com/marcusaasjensen/dialogue-system/assets/88457743/fa763645-dc3e-480a-89f5-327536516219">

Create a simple node in the top menu with the button *"Create Simple Node"*. A simple node will appear on the blueprint. This node is composed of a disconnected input port, an empty dialogue, and a disconnected output port.

You can connect the START node to the simple node by dragging your mouse click from the start node port named *"Next"* to the simple node's input port. This indicates the dialogue will start the simple node's dialogue at the very beginning of the narration.
Now, you know how you can build a chain of nodes by creating and connecting outputs to inputs. Also, it is possible to close the dialogue UI and reopen it at the next interaction using the option at the bottom of the simple node.

<img width="211" alt="connected_simple_node" src="https://github.com/marcusaasjensen/dialogue-system/assets/88457743/bfd96002-299b-468f-ab53-4b5a859f2062">

## Multiple Choice Nodes
A multiple choice node is a simple node with multiple possible outputs. Each outputs has a text and must be a path to another node. Basically, when the player enters a multiple choice node, he will have to choose between the different text options. 

<img width="264" alt="mutliple_choice_node" src="https://github.com/marcusaasjensen/dialogue-system/assets/88457743/640af5d2-931b-4de8-8675-6339854c3413">


Create a multiple choice node in the top menu with the button *"Create Multiple Choice Node"*. A multiple choice node will appear on the blueprint. It is similar to the simple node.
You can add an option using the *"New Choice"* button. Then, an option appears inside the node. Now you can edit the text and connect its port to other nodes. You can add up to 5 choices (otherwise it is too much). Also, you can remove it using the *"Remove"* button next to the option.

At the end of the dialogue, it will automatically show the options you added. However, an option must be connected to another node, otherwise, it will be removed on save.
In case the player goes back to the same node, it is possible to disable already chosen options at the bottom of the node. 

## Dialogue Editor
Edit the dialogue of a specific node by clicking on the *"Edit Dialogue"* button of the node. This will open a new window presenting an empty list of messages. 

<img width="691" alt="messages" src="https://github.com/marcusaasjensen/dialogue-system/assets/88457743/2087467c-32af-451d-8539-57638e405cb0">

Add a new message to the list. 
Then you can write the character's name, the message to show, add the command tags (see how to use them in the **Command tags** section) with the option of showing or not the character (*"Hide Character"*). 
Make sure to write correctly the character's name or the dialogue system will not know which character is speaking.
If the message list is empty, it will close the dialogue at that node or show the possible choices for the case of a multiple choice node.

You can close the dialogue messages window to save.

## Load and Save
To save your narrative, you must write the name of your file in the top menu. Then, you can save by clicking on the *"Save data"* button. All of your narratives will be saved in the path *"Assets/Resources/Dialogue/Narratives/"*.
You can load an existing narrative from the resource path by writing the file's name and clicking on the *"Load Data"* button in the top menu.

## Test the narrative
Before testing your narrative, open your narrative scriptable object's inspector and list all the characters present in your narrative.

<img width="332" alt="narrative" src="https://github.com/marcusaasjensen/dialogue-system/assets/88457743/e31a0192-8ffc-40d6-b7a8-2a1318f3366d">

The *"Path To Checkpoint"* property is the path that will be created when the player makes choices in the narrative. You can use its value or remove it to reset the narrative.

*"Is Narrative End Reached"* is also an option you must reset if the player has reached the end of the narrative and that you want to reset it for testing.

*"Start From Previous Narrative Path"* is an option when the player closed the dialogue and reopens it to start from where he left off.

Do not hesitate to use the scene templates I have provided in the project.
Every time you save your narrative, you will need to reference it to a GameObject's component inheriting from the DialogueMonoBehaviour class. 
You can use DialogueStarter GameObject to start immediately the dialogue or the Character GameObject (InteractableDialogue) where you need to interact with the character (Interactable) to start. 
You must test in **Play Mode**.

# Command tags
> [!CAUTION] 
> Make sure to fully respect the tag notations and examples here. Otherwise, you may encounter unwanted behaviours.

## Pause tag ```<p:pause>```

Pause the text typer for a certain time.

Replace the ```pause``` text by one of the following pause types.
```
tiny: 0.1s
short: 0.25s
normal: 0.666s
long: 1s
read: 2s
```

```"Wait... <p:normal> You're kidding right?"```

## Speed tag ```<sp:speed>```

Changes the typer text's speed, otherwise the speed is the default typer pace.

Replace the ```speed``` text by a real number.

The lower the value, the faster the typer.

```"PLEASE <sp:0,1>I BEG YOU!"```

## Animation tags ```<anim:animation>...</anim>```

Animates the text in between by the provided animation and the default text animator speed, amount and sync values.

Here is a Wobble animation example...

![wobble](https://github.com/marcusaasjensen/dialogue-system/assets/88457743/375c0046-09a1-4894-8386-77bcc93a4a47)

Replace the ```animation``` text by one of the following animation types.
```
None
Shake
Wobble
Wave
PingPong
Flicker
Spiral
Jitter
```
> [!WARNING] 
> Make sure the AnimatorText component has a direct reference to the TextMeshPro text showing the character's message.

> [!TIP] 
> It is possible to use the TextAnimator component to test your animation values on the default text message in **Editor Mode**.
> Make sure to disable animation testing before entering Play Mode.
> 
> <img width="322" alt="animator" src="https://github.com/marcusaasjensen/dialogue-system/assets/88457743/943e0382-f9e8-4916-8129-d717465feb08">

When you have decided the values to use in your dialogue, you can add other parameters in your tag:

 ```<anim:animation a:amount s:speed>...</anim>```
 
  ```speed``` is the speed of the animation (real number).
  
  ```amount``` is how much the animation is exagerated (real number).
 
 ```<anim:animation a:amount s:speed sync:synchronize>...</anim>```

 ```synchronize``` is the animation synchronization of all letters in between (true or false).
 
```"<anim:wobble s:1 a:2.5 sync:false>I'm feelin' dizzy right now...</anim>"```

## Emotion tag ```<em:emotion>```

Changes the "emotion" of the current speaking character. The emotion must be defined in the Character's Scriptable Object in the list of character's states. If not defined, it will automatically use the default character's state that must be defined too.

<img width="332" alt="default_character" src="https://github.com/marcusaasjensen/dialogue-system/assets/88457743/82cc96bd-3be3-4937-9427-34c984ef9795">

> [!WARNING]  
> Make sure to create the character scriptable object, define it's properties (unless it is optional), and reference it in the narrative scriptable object (character's list).

> Make sure the default character scriptable object is well defined and is referenced in the NarrativeController component.
>

Replace the ```emotion``` text by one of the following emotion types.
```
Default
Hidden
Happy
Neutral
Angry
Scared
Fearful
Sad
Disgusted
Annoyed
Surprised
Curious
Warm
Evil
```

When the tag is used, the character will have the associated character face sprite, reaction sound (plays one time), and speaking pitch.

```"<em:evil>I am the danger."```

## Value tag ```<val:value>```

Replaces the tag's position by a stored value named ```value```. The variable must be defined in the Scriptable Singleton "DialogueVariableData.asset". If not defined, the value will be ```"X"```. The variables can be of type *int*, *float* or *string*. Boolean variables are not included.

> [!WARNING] 
> Make sure to have only one DialogueVariableData Scriptable Singleton in the directory "Assets/Resources/Dialogue/".
>

You can manipulate these variables using C# script.

```cs
public class ExampleSceneScript : MonoBehaviour
{
  private void Awake()
  {
    DialogueVariableData.Instance.RemoveAllDialogueVariables();
    DialogueVariableData.Instance.AddDialogueVariable("cost_coffee", 1.5f);
    DialogueVariableData.Instance.AddDialogueVariable("cost_bread", 1);
    DialogueVariableData.Instance.AddDialogueVariable("character", "John");
    DialogueVariableData.Instance.AddDialogueVariable("tmp", 0);
    DialogueVariableData.Instance.RemoveDialogueVariable("tmp");
    DialogueVariableData.Instance.ListAllDialogueVariables();
  }
}
```


```"This coffee costs exactly <val:cost_coffee>$."```

Resulting DialogueVariableData.asset values:

<img width="331" alt="dialogue_variables" src="https://github.com/marcusaasjensen/dialogue-system/assets/88457743/4b8f7fb4-a8d8-4a9f-973a-e170ed2f787a">

> [!TIP] 
> It is possible to use this tag inside text options
>
> ```"Pay <val:cost_coffee>$."```

## Audio tags
> [!WARNING] 
> Make sure to have only one DialogueAudioData Scriptable Singleton in the directory "Assets/Resources/Dialogue/".
> 
> Make sure the AudioPlayer GameObject is in the scene with the AudioPlayer script attached. Each audio source property must have a reference (both music and sound).
>

<img width="332" alt="dialogue_audio" src="https://github.com/marcusaasjensen/dialogue-system/assets/88457743/8cbea8fa-ef95-4576-9b89-07dc0567ebfb">

### SFX tag ```<sfx:sound>```

Plays one shot the sound named ```sound``` when the text typer arrives at tag's position. The Sound effect must be defined with a name in the Scriptable Singleton "DialogueAudioData.asset" (sound effect list).

```"So I was saying...<sfx:fart> Woops, I farted."```

### Start music tag ```<music:score>```

Plays in loop the music named ```score``` when the text typer arrives at tag's position. The music must be defined with a name in the Scriptable Singleton "DialogueAudioData.asset" (music list).

```"<music:emotional>Life is like a box of chocolate..."```

### Stop music tag ```</music>```

Stops any current looping music.

```"*music playing*...</music>It's not your fault."```

## Other tags
Of course, it is possible to use the default tags to customize your TextMeshPro text like colored, italic, bold, rotated etc.

