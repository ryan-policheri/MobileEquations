from skimage.io import imread
from skimage.filters import threshold_local


def image_preprocessor(image_path):
    image = imread(image_path, as_gray=True)
    threshold = threshold_local(image, 101, offset=0.02)
    image_threshold = image > threshold

    return image_threshold