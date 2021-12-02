def normalize_pixels(pixels):
    normalized_pixels = pixels.astype("float32")
    normalized_pixels /= 255.0

    return normalized_pixels