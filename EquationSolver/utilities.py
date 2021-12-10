import os

import cv2 as cv
import matplotlib.pyplot as plt
import numpy as np
from sklearn.utils import shuffle
from tensorflow import keras


def generate_neural_network():
    images, labels = load_dataset()
    images = normalize_pixels(images)

    train_images = images[:int(len(images) * 0.8)]
    test_images = images[int(len(images) * 0.8):]

    train_labels = labels[:int(len(labels) * 0.8)]
    test_labels = labels[int(len(labels) * 0.8):]

    model = generate_model()
    model.fit(train_images, train_labels, epochs=5, validation_data=(test_images, test_labels))
    model.save("model")
    

def load_dataset():
    images = list()
    labels = list()
    for directory in os.listdir("data"):
        label_file = open("data/{}/label.csv".format(directory))
        label = np.asarray(label_file.readline().strip().split(","))
        label = np.asarray([int(value) for value in label])
        for file in os.listdir("data/{}".format(directory)):
            if file != "label.csv":
                image = cv.imread("data/{}/{}".format(directory, file), 0)
                image = np.reshape(image, (28, 28, 1))
                images.append(image)
                labels.append(label)
                
    images = np.asarray(images)
    labels = np.asarray(labels)

    images, labels = shuffle(images, labels)

    return images, labels


def normalize_pixels(images):
    images_normalized = images.astype("float32")
    images_normalized /= 255.0

    return images_normalized


def generate_model():
    model = keras.Sequential()
    
    model.add(keras.layers.Conv2D(32, (3, 3), activation="relu", kernel_initializer="he_uniform", input_shape=(28, 28, 1)))
    model.add(keras.layers.MaxPool2D((2, 2)))
    model.add(keras.layers.Flatten())
    model.add(keras.layers.Dense(100, activation="relu", kernel_initializer="he_uniform"))
    model.add(keras.layers.Dense(15, activation="softmax"))

    optimizer = keras.optimizers.SGD(learning_rate=0.01, momentum=0.9)

    model.compile(optimizer=optimizer, loss="categorical_crossentropy", metrics=["accuracy"])

    return model


def show(image):
    plt.imshow(image)
    plt.gray()
    plt.show()


if __name__ == "__main__":
    generate_neural_network()