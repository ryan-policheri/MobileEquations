import matplotlib.pyplot as plt
from tensorflow import keras


def generate_neural_network():
    train_x, train_y, test_x, test_y = load_dataset()
    train_x, test_x = normalize_pixels(train_x, test_x)
    model = generate_model()
    model.fit(train_x, train_y, epochs=3)
    model.evaluate(test_x, test_y)
    model.save("model")
    

def load_dataset():
    (train_x, train_y), (test_x, test_y) = keras.datasets.mnist.load_data(path="mnist.npz")
    
    train_x = train_x.reshape((train_x.shape[0], 28, 28, 1))
    test_x = test_x.reshape((test_x.shape[0], 28, 28, 1))

    train_y = keras.utils.to_categorical(train_y)
    test_y = keras.utils.to_categorical(test_y)

    return train_x, train_y, test_x, test_y


def normalize_pixels(train_x, test_x):
    train_x_normalized = train_x.astype("float32")
    test_x_normalized = test_x.astype("float32")

    train_x_normalized /= 255.0
    test_x_normalized /= 255.0

    return train_x_normalized, test_x_normalized


def generate_model():
    model = keras.Sequential()
    
    model.add(keras.layers.Conv2D(32, (3, 3), activation="relu", kernel_initializer="he_uniform", input_shape=(28, 28, 1)))
    model.add(keras.layers.MaxPool2D((2, 2)))
    model.add(keras.layers.Flatten())
    model.add(keras.layers.Dense(100, activation="relu", kernel_initializer="he_uniform"))
    model.add(keras.layers.Dense(10, activation="softmax"))

    optimizer = keras.optimizers.SGD(learning_rate=0.01, momentum=0.9)

    model.compile(optimizer=optimizer, loss="categorical_crossentropy", metrics=["accuracy"])

    return model


def show(image):
    plt.imshow(image)
    plt.gray()
    plt.show()


if __name__ == "__main__":
    generate_neural_network()