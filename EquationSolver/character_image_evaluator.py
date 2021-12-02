from tensorflow import keras


def character_image_evaluator(character_image):
    print("Evaluating character image {}...".format(character_image))
    return "char"

    model = keras.models.load_model("model")
    image = np.asarray(image)
    
    predictions = model.predict(image)
    predictions = predictions[1]

    prediction = np.argmax(predictions)
    print("Prediction: {}".format(prediction))