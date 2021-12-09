import numpy as np
from tensorflow import keras


def character_image_evaluator(character_images):
    print("Evaluating character images...")

    model = keras.models.load_model("model")

    character_images = np.asarray(character_images)
    character_images = np.reshape(character_images, (character_images.shape[0], 28, 28, 1))
    
    predictions = model.predict(character_images)
    predictions = np.argmax(predictions, 1)

    return predictions