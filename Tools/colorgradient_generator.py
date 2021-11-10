from matplotlib.pyplot import get_cmap
import numpy as np


def get_colors(colormap, points):
    "Return an array with {points} points evenly distributed along a colormap"
    
    # initialize an empty array and get the color values from matplotlib
    colorselection = np.zeros((points, 3))
    colors = get_cmap(colormap).colors
    if points > len(colors):
        print(f"Colormap {colormap} is being sampled at {points} points "
              + f"but there are only {len(colors)} colors in the colormap")

    # sample the colormap a certain amount of times
    current_row = 0
    for i in np.linspace(0, len(colors), points - 1, endpoint=False, dtype=int):
        colorselection[current_row] = np.array(colors[i])
        current_row += 1
    
    # last sample point is the last color in the colormap
    colorselection[current_row] = np.array(colors[-1])
    return colorselection
    
    
def format_as_csharp(property_name, color_array):
    "Format a numpy array as a ColorGradient definition"
    
    output = f"public static ColorGradient {property_name} => new ColorGradient("
    
    # add a new UnityEngine.Color for every row in color_array
    for color in color_array:
        output += f"\n    new Color({color[0]:.6f}f, {color[1]:.6f}f, {color[2]:.6f}f),"
        
    return output[:-1] + "\n);" # remove last comma and close definition
    
    
def generate_colorgradient(colormap, points, property_name=None):
    """Generate code for a ColorGradient definition by linearly sampling
    a matplotlib colormap for a certain amount of points. If property_name 
    is None, the capitalized colormap name is used as the property name"""
    
    if property_name is None:
        property_name = colormap.capitalize()
    
    color_array = get_colors(colormap, points)
    return format_as_csharp(property_name, color_array)
    
    
if __name__ == "__main__":
    print(generate_colorgradient("viridis", 8))
    print(generate_colorgradient("cividis", 8))
    print(generate_colorgradient("plasma", 8))
