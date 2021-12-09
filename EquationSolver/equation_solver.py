import sys

from character_image_evaluator import character_image_evaluator
from character_image_extractor import character_image_extractor
from equation_constructor import equation_constructor
from image_preprocessor import image_preprocessor


def main():
    input_file = sys.argv[1]
    output_file = sys.argv[2]

    print("Solving equation located at \"{}\"".format(input_file))

    # Preprocess the image to remove any artifacts and noise
    preprocessed_image = image_preprocessor(input_file)
    # Extract each character from the preprocessed image
    character_images = character_image_extractor(preprocessed_image)
    # Determine what each of the characters is
    characters = character_image_evaluator(character_images)
    # Construct the equation
    equation = equation_constructor(characters)
    
    equation_latex = f"$${equation.replace('#'. '\\cdot')}$$"
    equation_eval = equation.replace('#'. '*')
    print("Equation: {}".format(equation))
    solution = eval(equation_eval)
    hardcodedJsonForTesting ="{ \"equation\": \"20 + 7 = \", \"solution\": \"8\",  \"solvedEquation\": \"20 + 7 = 8\", \"laTeX\": \"x = \\\\frac{-b \\\\pm \\\\sqrt{b^2-4ac}}{2a}\" }"
    with open(output_file, 'w') as file:
        file.write(hardcodedJsonForTesting)
    print("Equation is solved is located at \"{}\"".format(output_file))


if __name__ == "__main__":
    main()