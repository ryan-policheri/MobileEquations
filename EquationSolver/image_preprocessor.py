import sys

from utilities.image.display import show

import cv2 as cv
import numpy as np


def image_preprocessor(image_path):
    image = cv.imread(image_path, 0)
    blur = cv.medianBlur(image, 5)

    # Black: 0, White: 255
    median = np.median(blur)
    maximum = np.max(blur)
    minimum = np.min(blur)

    text_color = "white" if abs(median - maximum) > abs(median - minimum) else "black"

    if text_color == "white":
        blur = np.bitwise_not(blur)
        
    threshold = cv.adaptiveThreshold(blur, 255, cv.ADAPTIVE_THRESH_MEAN_C, cv.THRESH_BINARY, 31, 5)
        
    return threshold


if __name__ == "__main__":
    output = image_preprocessor(sys.argv[1])
    show(output)