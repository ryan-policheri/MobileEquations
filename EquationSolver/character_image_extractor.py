from skimage.measure import find_contours
from skimage.io import imread
import skimage
import matplotlib.pyplot as plt
from skimage.color import rgb2gray

import numpy as np


from skimage.transform import rescale, resize, downscale_local_mean
from math import floor, ceil

def is_contained(boxes, box):
    
    x_min, x_max, y_min, y_max = box
    for potential_box in boxes:
        if potential_box == box:
            continue
        pot_x_min, pot_x_max, pot_y_min, pot_y_max = potential_box
        
        if pot_x_min < x_min and  x_max < pot_x_max and pot_y_min < y_min and  y_max < pot_y_max:
            return True
    return False
def is_small(image, box, margin):
    height, width = image.shape
    x_min, x_max, y_min, y_max = box
    box_height = x_max - x_min
    box_width = y_max - y_min
    return (box_height * box_width)/(height * width) < margin
    
def search_same_line(boxes, box):
    line_box = []
    x_min, x_max, y_min, y_max = box
    for potential_box in boxes:
        pot_x_min, pot_x_max, pot_y_min, pot_y_max = potential_box
        if (pot_x_min + pot_x_max)/2 > x_min and  x_max > (pot_x_min + pot_x_max)/2:
            line_box.append(potential_box)
    return line_box


def character_image_extractor(image):
    print("Extracting character images...")
    
    character_images = list()

    # get the edges of the text
    contours = find_contours(image, 0.7)
    bounding_boxes = []
    
    # find the bounding box of the text
    for contour in contours:
        Xmin = np.min(contour[:,0])
        Xmax = np.max(contour[:,0])
        Ymin = np.min(contour[:,1])
        Ymax = np.max(contour[:,1])
        
        bounding_boxes.append([Xmin, Xmax, Ymin, Ymax])
        
    # remove any box that would be contained within another
    # there are issues with 8 and 9 where the inner circles would be their own contour
    bounding_boxes = list(filter(lambda x: not (is_contained(bounding_boxes, x)), bounding_boxes))
    
    # remove any tiny boxes that result from noise
    # this will avoid issues if there is a small speck close to the center and will avoid unnecessary work from specks being sent to the model
    bounding_boxes = list(filter(lambda x: not (is_small(image, x, 0.01)), bounding_boxes))
    
    # order boxes left to right
    sorted_boxes = sorted(bounding_boxes, key = lambda x: x[2])

    x,y = image.shape
    
    # find the box that is closest to the center
    center_box = sorted(sorted_boxes, key = lambda z: (abs(x//2-(z[0]+z[1])//2) + abs(y//2 - (z[2]+z[3])//2)))[0]

    # find all boxes within the same line roughly
    # filters based on the center of a box being 
    # within the somewhere within the height range of the searching box
    for box in search_same_line(sorted_boxes, center_box):
        box = list(map(round, box))
        # extract the part of the image within the box
        focus_shape = image[box[0]:box[1], box[2]:box[3]]
        x,y = focus_shape.shape
        x_y_diff = x-y
        scale = (1/max(x,y))*24
        #scale to a 24*n or n*24 image depending on x or y being larger
        image_rescaled = rescale(focus_shape, scale, anti_aliasing=False)
        
        x,y = image_rescaled.shape
        x_y_diff = x-y
        x_pad = (floor(-min(0, x_y_diff)/2) +2,ceil(-min(0, x_y_diff)/2) +2)
        y_pad = (floor(max(0, x_y_diff)/2) +2,ceil(max(0, x_y_diff)/2) +2)
        
        # pad the image to become 28 by 28, rounding pixel is added to the right
        image_rescaled = np.pad(image_rescaled, (x_pad, y_pad), 'constant', constant_values=(0,0))
        character_images.append(image_rescaled)
    

    return character_images