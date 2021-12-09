from skimage.measure import find_contours
from skimage.io import imread
import skimage
import matplotlib.pyplot as plt
from skimage.color import rgb2gray

from skimage.filters import threshold_local
import numpy as np

import sys


import cv2 as cv
import numpy as np


def image_preprocessor(image_path):
    print("Preprocessing image...")

    image = cv.imread(image_path, 0)
    blur = cv.medianBlur(image, 5)

    # Black: 0, White: 255
    median = np.median(blur)
    maximum = np.max(blur)
    minimum = np.min(blur)

    text_color = "white" if abs(median - maximum) > abs(median - minimum) else "black"

    if text_color == "white":
        blur = np.bitwise_not(blur)
        
    threshold = cv.adaptiveThreshold(blur, 255, cv.ADAPTIVE_THRESH_GAUSSIAN_C, cv.THRESH_BINARY, 501, 15)
    threshold = np.bitwise_not(threshold)

    return threshold
    
def add_channels(arr, channels):
    print(arr.shape)
    return np.concatenate((arr, np.array([channels]*arr.shape[0])[:,None]),axis=1)
#orig_img = imread('bench (1).jpg', as_gray=True)
image_threshold = image_preprocessor('IMG_0342.jpg')

gray_img = image_threshold
plt.imshow(gray_img,interpolation='nearest', cmap=plt.cm.gray)

contours = find_contours(gray_img, 0.7)

fig, ax = plt.subplots()
ax.imshow(gray_img, interpolation='nearest', cmap=plt.cm.gray)

for n, contour in enumerate(contours):
    ax.plot(contours[n][:, 1], contours[n][:, 0], linewidth=2)

plt.show()

from skimage.draw import polygon_perimeter

bounding_boxes = []

for contour in contours:
    Xmin = np.min(contour[:,0])
    Xmax = np.max(contour[:,0])
    Ymin = np.min(contour[:,1])
    Ymax = np.max(contour[:,1])
    
    bounding_boxes.append([Xmin, Xmax, Ymin, Ymax])


with_boxes  = np.copy(gray_img)
   
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
    

        
# remove any box that would be contained within another
# there are issues with 8 and 9 where the inner circles would be their own contour
bounding_boxes = list(filter(lambda x: not (is_contained(bounding_boxes, x)), bounding_boxes))

# remove any tiny boxes that result from noise
# this will avoid issues if there is a small speck close to the center and will avoid unneccessary work from specks being sent to the model
bounding_boxes = list(filter(lambda x: not (is_small(with_boxes, x, 0.001)), bounding_boxes))
    
for box in bounding_boxes:
    #[Xmin, Xmax, Ymin, Ymax]
    r = [box[0],box[1],box[1],box[0], box[0]]
    c = [box[3],box[3],box[2],box[2], box[3]]
    rr, cc = polygon_perimeter(r, c, with_boxes.shape)
    with_boxes[rr, cc] = 1 #set color white

plt.imshow(with_boxes, interpolation='nearest', cmap=plt.cm.gray)
plt.show()

sorted_boxes = sorted(bounding_boxes, key = lambda x: x[2])

def search_same_line(boxes, box):
    line_box = []
    x_min, x_max, y_min, y_max = box
    for potential_box in boxes:
        pot_x_min, pot_x_max, pot_y_min, pot_y_max = potential_box
        if (pot_x_min + pot_x_max)/2 > x_min and  x_max > (pot_x_min + pot_x_max)/2:
            line_box.append(potential_box)
    return line_box
 

from skimage.transform import rescale, resize, downscale_local_mean
from math import floor, ceil


x,y = gray_img.shape
center_box = sorted(sorted_boxes, key = lambda z: (abs(x//2-(z[0]+z[1])//2) + abs(y//2 - (z[2]+z[3])//2)))[0]
print(center_box)



for box in search_same_line(sorted_boxes, center_box):
    #[Xmin, Xmax, Ymin, Ymax]
    r = [box[0],box[1],box[1],box[0], box[0]]
    c = [box[3],box[3],box[2],box[2], box[3]]
    print(box)
    box = list(map(round, box))
    focus_shape = gray_img[box[0]:box[1], box[2]:box[3]]
    rr, cc = polygon_perimeter(r, c, with_boxes.shape)
    plt.imshow(focus_shape, interpolation='nearest', cmap=plt.cm.gray)
    plt.show()
    x,y = focus_shape.shape
    x_y_diff = x-y
    scale = (1/max(x,y))*24
    image_rescaled = rescale(focus_shape, scale, anti_aliasing=False)
    
    x,y = image_rescaled.shape
    x_y_diff = x-y
    x_pad = (floor(-min(0, x_y_diff)/2) +2,ceil(-min(0, x_y_diff)/2) +2)
    y_pad = (floor(max(0, x_y_diff)/2) +2,ceil(max(0, x_y_diff)/2) +2)
    print(scale,x_pad,y_pad, image_rescaled.shape)
    image_rescaled = np.pad(image_rescaled, (x_pad, y_pad), 'constant', constant_values=(0,0))
    print(image_rescaled.shape)
    plt.imshow(image_rescaled, interpolation='nearest', cmap=plt.cm.gray)
    plt.show()