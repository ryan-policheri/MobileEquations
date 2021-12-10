import os

import cv2 as cv
import matplotlib.pyplot as plt
import numpy as np
from sklearn.utils import shuffle
from tensorflow import keras


def generate_neural_network():
    images, labels = load_dataset()
    images = normalize_pixels(images)

    data_generator = keras.preprocessing.image.ImageDataGenerator(
        featurewise_center=False,
        samplewise_center=False,
        featurewise_std_normalization=False,
        samplewise_std_normalization=False,
        zca_whitening=False,
        rotation_range=10,
        zoom_range = 0.1,
        width_shift_range=0.1,
        height_shift_range=0.1,
        horizontal_flip=False,
        vertical_flip=False
    )

    train_images = images[:int(len(images) * 0.8)]
    test_images = images[int(len(images) * 0.8):]

    train_labels = labels[:int(len(labels) * 0.8)]
    test_labels = labels[int(len(labels) * 0.8):]

    train_data = data_generator.flow(train_images, train_labels)
    test_data = data_generator.flow(test_images, test_labels)

    model = generate_model()
    #model.fit(train_images, train_labels, epochs=10, validation_data=(test_images, test_labels))
    model.fit_generator(train_data, epochs=5, validation_data=test_data)
    model.save("model")
    

def load_dataset():
    custom_images = list()
    custom_labels = list()
    for directory in os.listdir("data"):
        label_file = open("data/{}/label.csv".format(directory))
        label = np.asarray(label_file.readline().strip().split(","))
        label = np.asarray([int(value) for value in label])
        for file in os.listdir("data/{}".format(directory)):
            if file != "label.csv":
                image = cv.imread("data/{}/{}".format(directory, file), 0)
                image = np.reshape(image, (28, 28, 1))
                custom_images.append(image)
                custom_labels.append(label)
                
    custom_images = np.asarray(custom_images)
    custom_labels = np.asarray(custom_labels)

    custom_images, custom_labels = shuffle(custom_images, custom_labels)

    (mnist_train_images, mnist__train_labels), (mnist_test_images, mnist_test_labels) = keras.datasets.mnist.load_data(path="mnist.npz")

    mnist_train_images = np.reshape(mnist_train_images, (mnist_train_images.shape[0], 28, 28, 1))
    mnist_test_images = np.reshape(mnist_test_images, (mnist_test_images.shape[0], 28, 28, 1))

    mnist__train_labels = keras.utils.to_categorical(mnist__train_labels)
    mnist__train_labels = np.pad(mnist__train_labels, (0, 5))
    mnist_test_labels = keras.utils.to_categorical(mnist_test_labels)
    mnist_test_labels = np.pad(mnist_test_labels, (0, 5))

    images = np.concatenate((custom_images, mnist_train_images, mnist_test_images), axis=0)
    labels = np.concatenate((custom_labels, mnist__train_labels[:60000], mnist_test_labels[:10000]), axis=0)

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