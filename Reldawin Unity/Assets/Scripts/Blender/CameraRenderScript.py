import bpy
from math import radians
from os.path import join

S = bpy.context.scene

renderFolder = "C:/Users/Retri/Documents/GitHub/Renders/Running/"

camParent = bpy.data.objects['Circle']

startFrame = 1 # replace with your start frame
endFrame  = 1 # replace with your end frame
numAngles = 8
rotAngle  = 360 / numAngles

for i in range(numAngles):
    # Set camera angle via parent
    angle = i * rotAngle
    camParent.rotation_euler.z = radians( angle )

    # Render animation
    for f in range(startFrame,endFrame + 1):
        S.frame_set( f ) # Set frame

        frmNum   = str( f-startFrame ).zfill(3) # Formats 5 --> 005
        fileName = "angle_{a}_frame_{f}".format( a = angle, f = frmNum)
        fileName += S.render.file_extension
        bpy.context.scene.render.filepath = join( renderFolder, fileName )

        bpy.ops.render.render(write_still = True)