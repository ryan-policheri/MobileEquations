import skimage.io
import skimage.transform
import skimage.util


def image_preprocessor(image_path):
    print("Preprocessing image \"{}\"...".format(image_path))
    
    image = "preprocessed image"
    
    return image

    image = skimage.io.imread(image_path, as_gray=True)
    image = process_image(image)


def process_image(image):
    image = skimage.util.invert(image)
    image = skimage.transform.resize(image, (28, 28))
    image = image.reshape(28, 28, 1)
    image = image.astype("float32")

    return image