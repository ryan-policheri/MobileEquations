import sys

from utilities import show

import cv2 as cv
import numpy as np


def image_preprocessor(image_path):
    print("Preprocessing image...")

    image = cv.imread(image_path, 0)
    resized_image = resize_image(image)
    blurred_image = cv.medianBlur(resized_image, 3)

    # Black: 0, White: 255
    median = np.median(blurred_image)
    maximum = np.max(blurred_image)
    minimum = np.min(blurred_image)

    text_color = "white" if abs(median - maximum) > abs(median - minimum) else "black"

    if text_color == "white":
        blurred_image = np.bitwise_not(blurred_image)
        
    threshold = cv.adaptiveThreshold(blurred_image, 255, cv.ADAPTIVE_THRESH_GAUSSIAN_C, cv.THRESH_BINARY, 51, 30)
    threshold = np.bitwise_not(threshold)

    return threshold


def resize_image(image):
    height, width = image.shape[0], image.shape[1]

    target_height = 500
    scaling_factor = target_height / height

    adjusted_height = int(scaling_factor * height)
    adjusted_width = int(scaling_factor * width)

    resized_image = cv.resize(image, (adjusted_width, adjusted_height))

    return resized_image


if __name__ == "__main__":
    output = image_preprocessor(sys.argv[1])
    show(output)