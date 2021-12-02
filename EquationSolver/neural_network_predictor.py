import os

import matplotlib.pyplot as plt
import numpy as np
import skimage.filters
import skimage.io
import skimage.transform
import skimage.util
from tensorflow import keras


def main():
    IMAGES_DIRECTORY = "..\\Images"

    images = list()
    image_names = list()

    for file_name in os.listdir(IMAGES_DIRECTORY):
        image_path = "{}\\{}".format(IMAGES_DIRECTORY, file_name)
        image = skimage.io.imread(image_path, as_gray=True)
        image = process_image(image)
        
        images.append(image)
        image_names.append(file_name)

    images = np.asarray(images)
    model = keras.models.load_model("model")
    predictions = model.predict(images)

    for i in range(len(predictions)):
        prediction = predictions[i]
        prediction = np.rint(prediction)

        for j in range(len(prediction)):
            if prediction[j] == 1:
                pass
                print("Image: {}, Prediction: {}".format(image_names[i], j))


def process_image(image):
    image = skimage.util.invert(image)
    image = skimage.transform.resize(image, (28, 28))
    image = image.reshape(28, 28, 1)
    image = image.astype("float32")

    return image


def show(image):
    plt.imshow(image)
    plt.gray()
    plt.show()


if __name__ == "__main__":
    main()
