import sys

from character_image_evaluator import character_image_evaluator
from character_image_extractor import character_image_extractor
from equation_constructor import equation_constructor
from image_preprocessor import image_preprocessor
import json

def main():
    input_file = sys.argv[1]
    output_file = sys.argv[2]

    print("Solving equation located at \"{}\"".format(input_file))

    try:
        # Preprocess the image to remove any artifacts and noise
        preprocessed_image = image_preprocessor(input_file)
        # Extract each character from the preprocessed image
        character_images = character_image_extractor(preprocessed_image)
        # Determine what each of the characters is
        characters = character_image_evaluator(character_images)
        # Construct the equation
        equation = equation_constructor(characters)
        
        equation_latex = equation.replace('#', '\\cdot')
        equation_eval = equation.replace('#', '*')
        print("Equation: {}".format(equation))
        solution = eval(equation_eval.replace(' ', ''))
        equation_latex = f"$${equation_latex} = {solution}$$"
        solved_equation = f"{equation_eval} = {solution}"
        hardcodedJsonForTesting =json.dumps({"equation": equation_eval, 
                        "solution": f"{solution}", 
                        "solvedEquation": solved_equation,
                        "laTeX": equation_latex})
    except Exception as e:
        error_text = f"{input_file}: Error {e}"
        hardcodedJsonForTesting =json.dumps({"equation": error_text, 
                        "solution": error_text, 
                        "solvedEquation": error_text,
                        "laTeX": error_text})
    with open(output_file, 'w') as file:
        file.write(hardcodedJsonForTesting)
    print("Equation is solved is located at \"{}\"".format(output_file))


if __name__ == "__main__":
    main()