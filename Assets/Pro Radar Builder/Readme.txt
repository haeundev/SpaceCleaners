To create a Radar system . go to Tools > Pro Radar Builder.

Click on the type of radar system you wish to create.

At this point your radar code is fully functional but you will need to assign blips or a Minimap if you choose. To enable Minimaps, click on the Minimap button at the top of the Editor Window.

To assign blips, go the blips tab and increate the "Number of blip types" to the amount of blips you want.

Click the white round power button to make the blip active so that it displays in game.

Open the blip foldout and  at the bottom of the foldout you will see " Create Blip as(choose type) with tag ( the tag of the object in scene that you wish to reack) On Layer (the layer on which the blip is to be displayed) "

If you are not using the Unity Ui 2D system, then you will see a Render Camera gameobject created also, this is for rendering your 2D Radar on screen and for Rending the 3D Radar on screen if you are using the 3D Radar in screen space.

Render camera must only render your radar/ minimap/ blips (radar related objects). All other camera should ignore the layer that the Render camera renders. in URP you would use Camera Stacking.

in all the above setting you will assign your blip sprite, prefab or materials depending on what type of blip you choose at " Create Blip as(choose type)"

Turn on Show Help messages in the Designs area to show the explanation for all settings you see. all UI function not explained here will be explained in the UI.


Email address to contact me is in the Radar builder editor.
it is  daimangou@gmail.com
