import sys

from utilities import show

from character_image_extractor import character_image_extractor
from image_preprocessor import image_preprocessor
import time
import numpy

import matplotlib.pyplot as plt

import cv2


def main():
    input_file = sys.argv[1]
    output_dir = sys.argv[2]

    # Preprocess the image to remove any artifacts and noise
    preprocessed_image = image_preprocessor(input_file)
    # Extract each character from the preprocessed image
    character_images = character_image_extractor(preprocessed_image, full=True)
    counter = 0
    print(len(character_images))
    for image in character_images:
        image[:] *= 255
        cv2.imwrite(f"{output_dir}\\{time.time()}{counter}.jpg", image)
        counter += 1

if __name__ == "__main__":
    main()