from argparse import ArgumentParser

import numpy as np
import sklearn.model_selection
from tensorflow import keras


def main():
    parser = ArgumentParser(description="Generate neural network trained on MNIST dataset")
    parser.add_argument("--benchmark", action="store_true", help="Benchmark the neural network using k-fold cross-validation")
    parser.add_argument("--folds", type=int, default=5, metavar="", help="Set the number of folds for k-fold cross-validation")
    args = parser.parse_args()

    x, y = load_dataset()
    x = normalize_pixels(x)

    model = generate_model()

    if args.benchmark:
        scores = evaluate_model(model, x, y, args.folds)
        print("Accuracy: mean=%.3f std=%.3f, n=%d" % (np.mean(scores) * 100, np.std(scores) * 100, len(scores)))
    else:
        model.fit(x, y, epochs=5)
        model.save("model")


def load_dataset():
    (train_x, train_y), (test_x, test_y) = keras.datasets.mnist.load_data()
    
    train_x = train_x.reshape((train_x.shape[0], 28, 28, 1))
    test_x = test_x.reshape((test_x.shape[0], 28, 28, 1))

    train_y = keras.utils.to_categorical(train_y)
    test_y = keras.utils.to_categorical(test_y)

    x = np.concatenate((train_x, test_x))
    y = np.concatenate((train_y, test_y))

    return x, y


def normalize_pixels(train):
    train_normalized = train.astype("float32")
    train_normalized /= 255.0

    return train_normalized


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


def evaluate_model(model, x, y, number_of_folds):
    scores = list()
    kfold = sklearn.model_selection.KFold(number_of_folds, shuffle=True, random_state=1)
    for train_ix, test_ix in kfold.split(x):
        train_x, train_y, test_x, test_y = x[train_ix], y[train_ix], x[test_ix], y[test_ix]
        model.fit(train_x, train_y, epochs=10, batch_size=32, validation_data=(test_x, test_y), verbose=0)
        accuracy = model.evaluate(test_x, test_y, verbose=0)[1]
        scores.append(accuracy)
    return scores


if __name__ == "__main__":
    main()
